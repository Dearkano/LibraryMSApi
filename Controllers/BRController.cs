using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryMSAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore.Mvc;
using System.Net;

namespace LibraryMSAPI.Controllers
{
    [Route("api/[controller]")]
    public class BRController:Controller
    {
        public LibraryDbContext DbContext { get; }
        public BRController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
        }

        [HttpPost("borrow/{bid}/{cid}/{mid}")]
        public async Task<string> BorrowBook(int bid,int cid,int mid)
        {
            var books = DbContext.Books;
            var records = DbContext.Records;
            //检查有没有库存
            var data = await (from book in DbContext.Books where book.Id.Equals(bid) select book).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no book");
            var thisbook = data[0];
            if (thisbook.stock == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no stock");

            thisbook.stock--;
            books.Update(thisbook);

            var now = DateTime.Now;
            var returnDate = now.AddDays(15);
            Record newRecord = new Record(bid,cid,mid,now,returnDate);
            records.Add(newRecord);

            await DbContext.SaveChangesAsync();
            return "ojbk";
        }


        [HttpPost("return/{bid}/{cid}/{mid}")]
        public async Task<string> ReturnBook(int bid, int cid, int mid)
        {
            var books = DbContext.Books;
            var records = DbContext.Records;
        
            var data = await (from record in records where cid==record.cardId&&bid==record.bookId select record).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no record");

            var thisRecord = data[0];

            var data1 = await (from book in DbContext.Books where book.Id.Equals(bid) select book).ToArrayAsync();
            var thisbook = data1[0];
            thisbook.stock++;
            books.Update(thisbook);


            await DbContext.SaveChangesAsync();
            if (thisRecord.return_date < DateTime.Now) return "overdue";
            return "ojbk";
        }
    }
}
