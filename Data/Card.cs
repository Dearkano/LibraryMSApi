using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryMSAPI.Data
{
    [Table("Card")]
    public class Card
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("company")]
        public string company { get; set; }

        [Column("type")]
        public string type { get; set; }
    }
}
