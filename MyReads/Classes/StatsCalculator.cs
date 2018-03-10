//Created by Linus Brännström 2017-12-17
//ITS Learning ID: ah9248


using MyReads.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReads.Classes
{
    /// <summary>
    /// Calculates statistics for the users reading habits. Includes total pages read,
    /// favourite genre and favourite author
    /// </summary>
    public class StatsCalculator
    {
        private int totalpages;
        private string mostCommonAuthor;
        private string mostCommonGenre;
        //Constructors-------------------------------------------------------------------------------------

        public StatsCalculator()
        {
            Books = new List<Books>();
            totalpages = 0;
            mostCommonAuthor = "none";
            mostCommonGenre = "none";
        }
        //Properties---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the number of books in the bookManager.booksList
        /// </summary>
        public int NumOfBooks
        {
            get
            {
                return Books.Count;
            }
        }

        /// <summary>
        /// Gets the favourite author
        /// </summary>
        public string FavouriteAuthor
        {
            get
            {
                CalculateAuthor();
                return mostCommonAuthor;
            }
        }

        /// <summary>
        /// Gets the Favourite genre
        /// </summary>
        public string FavouriteGenre
        {
            get
            {
                CalculateGenre();
                return mostCommonGenre;
            }
        }

        /// <summary>
        /// Gets the totalPages
        /// </summary>
        public int TotalPages
        {
            get
            {
                CalculatePages();
                return totalpages;
            }
        }

        /// <summary>
        /// Gets the BookManager instance
        /// </summary>
        public List<Books> Books { get; set; }

        //Methods------------------------------------------------------------------------------------------

        /// <summary>
        /// Calcualtes totalPages by adding 
        /// pages from every book in the booksList
        /// </summary>
        private void CalculatePages()
        {
            totalpages = 0;

            for(int i = 0; i < Books.Count; i++)
            {
                if (Books[i].Book_Pages.HasValue)
                {
                    int pages = Convert.ToInt32(Books[i].Book_Pages);
                    totalpages = totalpages + pages;
                }
            }
        }

        /// <summary>
        /// Calculates the favourite author
        /// </summary>
        private void CalculateAuthor()
        {
            try
            {
                string[] authors = new string[Books.Count];
                for(int i = 0; i < Books.Count; i++)
                {
                    authors[i] = Books[i].Book_Author;
                }

                var nameGroup = authors.GroupBy(x => x);
                var maxCount = nameGroup.Max(g => g.Count());
                var mostCommons = nameGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).ToArray();
                mostCommonAuthor = mostCommons[0];
            }
            catch
            {
                mostCommonAuthor = "none";
            }
        }

        /// <summary>
        /// Calculates the favourite genre
        /// </summary>
        private void CalculateGenre()
        {
            try
            {
                string[] genres = new string[Books.Count];
                for (int i = 0; i < Books.Count; i++)
                {
                    genres[i] = Books[i].Categories.Category_Genre;
                }

                var nameGroup = genres.GroupBy(x => x);
                var maxCount = nameGroup.Max(g => g.Count());
                var mostCommons = nameGroup.Where(x => x.Count() == maxCount).Select(x => x.Key).ToArray();
                mostCommonGenre = mostCommons[0];
            }
            catch
            {
                mostCommonGenre = "none";
            }
        }
    }
}
