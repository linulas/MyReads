$(document).on('ready', function () {

    $('.rating').on(
        'change', function () {
            console.log('Rating selected: ' + $(this).val());
            var bookID = $(this).attr('id');
            var rating = $(this).val();
            console.log("Rating Parameter: " + rating + " Book_ID Parameter: " + bookID);
            $.ajax({
                type: "POST",
                url: "/Books/RateBook", // the URL of the controller action method
                data: { rating: rating, bookID: bookID }, // data
                dataType: "json",
                success: function (result) {
                    // do something with result
                    console.log("Sucess: ", + result);
                },
                error: function (req, status, error) {
                    // do something with error   
                    console.log("Error");
                    console.log(req);
                    console.log(status);
                    console.log(error);
                }
            });
        });
});