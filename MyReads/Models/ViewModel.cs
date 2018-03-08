using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyReads.Models
{
    public class ViewModel
    {
        private int totalPages;
        private string mostCommonAuthor;
        string mostCommonGenre;

        public int Pages { get { return totalPages; } }
        public string Author { get { return mostCommonAuthor; } }
        public string Genre { get { return mostCommonGenre; } }
        public IEnumerable<Users> Users { get; set; }
        public IEnumerable<UserBooks> UserBooks { get; set; }
        public IEnumerable<Books> Books { get; set; }
        public Users User { get; set; }
        public UserBooks UserBook { get; set; }
        public Books Book { get; set; }

        public void SetStats(int pages, string author, string genre)
        {
            totalPages = pages;
            mostCommonAuthor = author;
            mostCommonGenre = genre;
        }
    }
}