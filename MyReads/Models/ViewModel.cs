using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyReads.Models
{
    public class ViewModel
    {
        public int Pages { get; private set; }
        public string Author { get; private set; }
        public string Genre { get; private set; }
        public IEnumerable<Users> Users { get; set; }
        public IEnumerable<UserBooks> UserBooks { get; set; }
        public IEnumerable<Books> Books { get; set; }
        public Users User { get; set; }
        public UserBooks UserBook { get; set; }
        public Books Book { get; set; }

        public void SetStats(int pages, string author, string genre)
        {
            Pages = pages;
            Author = author;
            Genre = genre;
        }
    }
}