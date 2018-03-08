//Variable to store search results
var jdata = {};
// takes user input to get books from google books api
function bookSearch() {
    // store user input
    var search = document.getElementById("searchInput").value;

    if (!$('#results').length && search != "") {
        console.log("Redirecting...")
        //Get search page
        $.ajax({
            type: "POST",
            url: "/Books/Search", // the URL of the controller action method
            data: { search: search }, // data
            success: function (result) {
                // do something with result
                console.log("Sucess");
                window.location = "/books/search";
            },
            error: function (req, status, error) {
                // do something with error   
                console.log("Error");
            }
        });
    }
    else if ($('#results').length) {
        // clear any previous data
        document.getElementById("results").innerHTML = "";
        // make a data request
        $.ajax({
            // url for database
            url: "https://www.googleapis.com/books/v1/volumes?q=" + search,
            dataType: "json",
            type: 'GET',
            // on success, do this
            success: function (data) {
                // display data being passed through
                console.log(data);
                // clear any previous data
                jdata = {};

                // loop through data in data.items
                for (var i = 0; i < data.items.length; i++) {
                    // store current books volume info
                    jdata[i] = data.items[i].volumeInfo;
                    // adds the book to the search result
                    searchBook(jdata[i], i);
                }
            }
        });
    }
}

//Adds a book object to view
function searchBook(book, searchId) {
    console.log(book);
    // create elements
    var bookItem = document.createElement('div');
    var wrapper = document.createElement('span');
    var controls = document.createElement('span');
    var addButton = document.createElement('button');
    var newImg = document.createElement('img');
    var bookInfo = document.createElement('span');
    var title = document.createElement('h6');
    var author = document.createElement('p');
    var moreInfo = document.createElement('a');

    // add classes to elements
    bookItem.className = 'col-xs-12 col-md-6 col-lg-4 list-group-item';
    wrapper.className = 'list-group-item-wrapper';
    moreInfo.className = 'btn btn-primary';
    controls.className = 'col-md-12';
    bookInfo.className = 'col-md-4';
    addButton.className = 'btn_add glyphicon iconBtn glyphicon-plus';

    // add text to tags
    title.innerText = book.title; //.substring(0, 20);
    moreInfo.innerText = 'Learn More';

    // add attributes
    bookItem.id = 'searchItem';
    controls.id = 'searchControls';
    moreInfo.href = book.infoLink;
    moreInfo.setAttribute('target', '_blank');
    bookInfo.id = 'bookInfo';
    addButton.id = searchId;

    // create image if one exists

    if (book.imageLinks) {
        if (book.imageLinks.smallThumbnail) {
            newImg.src = book.imageLinks.smallThumbnail;
        }
    } else {
        newImg.src = '../img/notebook.jpg';
    }
    // create author if one exists
    if (book.authors) {
        author.innerText = book.authors[0];
    } else {

        author.innerText = 'No author found';
    }
    // add tags to document
    controls.appendChild(addButton);
    bookInfo.appendChild(title);
    bookInfo.appendChild(author);
    bookInfo.appendChild(moreInfo);
    wrapper.appendChild(controls);
    wrapper.appendChild(newImg);
    wrapper.appendChild(bookInfo);
    bookItem.appendChild(wrapper);

    // add results to the screen
    var books = document.getElementById("results");
    books.appendChild(bookItem);
    // initialize page-scroll
    //$('#searchElement').click();
}

//Adds a book object to database
function addBook(i) {
    var book = {};
    var genre = "";
    console.log(jdata[i]);
    //Assign the desired values
    book.Book_Title = jdata[i].title;
    book.Book_Author = jdata[i].authors[0];
    book.Book_Description = jdata[i].description;
    book.Book_Pages = jdata[i].pageCount;
    if (jdata[i].categories) {
        genre = jdata[i].categories[0];
    } else {
        genre = "Uncathegorized";
    }
    book.Book_ImageLink = jdata[i].imageLinks.thumbnail;
    book.Book_InfoLink = jdata[i].infoLink;
    //Log the book beeing passed
    console.log(book);
    console.log(genre);

    //Post to database
    $.ajax({
        type: "POST",
        url: "/Books/AddBook", // the URL of the controller action method
        data: { bookData: JSON.stringify(book), bookGenre: genre }, // data
        dataType: "json",
        success: function (result) {
            // do something with result
            console.log("Sucess");
        },
        error: function (req, status, error) {
            // do something with error   
            console.log("Error");
        }
    });
}

// Activates bookSearch when enter is pressed through the search input
document.getElementById("searchInput").addEventListener("keydown", function (e) {

    // Enter is pressed
    if (e.keyCode === 13) {
        bookSearch();
    }
}, false);

// adds target searched book to the collection when button.btn_add is clicked
$(document).on('click', 'button.btn_add', function (event) {
    // store id for the clicked button
    var id = this.id;
    event.preventDefault();
    // animate parent element
    //window.animatelo.pulse(this.closest('div'));
    // change class on the clicked button
    this.className = 'iconBtn glyphicon glyphicon-ok';
    // adds the book to the collection
    addBook(id);
});

$(document).ready(function () {
    console.log("ready!");
    $('#searchBtn').click();
});