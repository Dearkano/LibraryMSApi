using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryMSAPI.Data
{
    [Table("Authorization")]
    public class Authorization
    {
        public Authorization() { }
        public Authorization(string _name,string _token)
        {
            name = _name;
            token = _token;
        }
        [Column("name")]
        public string name { get; set; }

        [Column("token")]
        public string token { get; set; }

        [Column("id")]
        public int id { get; set; }

    }
}
