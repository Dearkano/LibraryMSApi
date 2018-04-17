using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryMSAPI.Data
{
    [Table("Book")]
    public class Book
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("press")]
        public string press { get; set; }

        [Column("author")]
        public string author { get; set; }

        [Column("year")]
        public int year { get; set; }

        [Column("stock")]
        public int stock { get; set; }

        [Column("total")]
        public int total { get; set; }

        [Column("type")]
        public string type { get; set; }

        [Column("price")]
        public decimal price { get; set; }
    }
}
