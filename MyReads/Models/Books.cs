//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyReads.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Books
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Books()
        {
            this.UserBooks = new HashSet<UserBooks>();
        }
    
        public int Book_ID { get; set; }
        public string Book_Title { get; set; }
        public string Book_Author { get; set; }
        public Nullable<int> Book_Genre { get; set; }
        public string Book_Description { get; set; }
        public Nullable<int> Book_Pages { get; set; }
        public string Book_ImageLink { get; set; }
        public string Book_InfoLink { get; set; }
    
        public virtual Categories Categories { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserBooks> UserBooks { get; set; }
    }
}
