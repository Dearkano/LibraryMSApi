using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryMSAPI.Data
{
    [Table("Manager")]
    public class Manager
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("name")]
        public string name { get; set; }

        [Column("password")]
        public string password { get; set; }

        [Column("phone")]
        public string phone { get; set; }
    }
}
