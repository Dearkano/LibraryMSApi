using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
namespace LibraryMSAPI.Data
{
    [Table("Record")]
    public class Record
    {
        public Record(int bid,int cid,DateTime bor, DateTime ret,int acceptid)
        {
            bookId = bid;
            cardId = cid;
            borrow_date = bor;
            return_date = ret;
            accept = acceptid;
        }
        public Record() { }
        [Column("Id")]
        public int Id { get; set; }

        [Column("cardId")]
        public int cardId { get; set; }

        [Column("bookId")]
        public int bookId { get; set; }
        [Column("borrow_date")]
        public DateTime borrow_date { get; set; }

        [Column("return_date")]
        public DateTime return_date { get; set; }

        [Column("manager_id")]
        public int manager_id { get; set; }

        [Column("accept")]
        public int accept { get; set; }
    }
}
