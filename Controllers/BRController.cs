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
using Microsoft.AspNetCore.Authorization;

namespace LibraryMSAPI.Controllers
{
    public class CheckData
    {
        public CheckData(Record _r,Book _b,Card _c)
        {
            record = _r;
            book = _b;
            card = _c;
        }
        public Record record;
        public Book book;
        public Card card;
    }
    [Route("api/[controller]")]
    public class BRController:Controller
    {
        public LibraryDbContext DbContext { get; }
        public BRController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
        }

        [Authorize]
        [HttpGet("borrow/{bid}")]
        public async Task<string> BorrowBook(int bid)
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
            var cid = card.Id;
            var books = DbContext.Books;
            var records = DbContext.Records;
            //检查有没有库存
            var data = await (from book in DbContext.Books where book.Id.Equals(bid) select book).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no book");
            var thisbook = data[0];
            if (thisbook.stock == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no stock");
            var rec = await (from n in DbContext.Records where card.Id == n.cardId && bid == n.bookId&&n.accept==1 select n).ToArrayAsync();
            if(rec.Length!=0) throw new ActionResultException(HttpStatusCode.BadRequest, "has borrow");
            thisbook.stock--;
            books.Update(thisbook);

            var now = DateTime.Now;
            var returnDate = now.AddDays(15);
            Record newRecord = new Record(bid, cid, now, returnDate,2);
            records.Add(newRecord);

            await DbContext.SaveChangesAsync();
            return "ojbk";
        }

        [Authorize]
        [HttpGet("getcheck")]
        public async Task<CheckData[]> GetUnacceptedCheckOperation()
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
            if (!card.type.Equals("管理员")){
                throw new ActionResultException(HttpStatusCode.BadRequest, "no right");
            }
            var rec = await (from i in DbContext.Records where i.accept == 2 select i).ToArrayAsync();
            var ds= new CheckData[rec.Length];
            for (int r= 0;r < rec.Length;r++)
            {
                var book = await (from j in DbContext.Books where rec[r].bookId.Equals(j.Id) select j).FirstAsync();
                var card1 = await (from k in DbContext.Cards where k.Id == rec[r].cardId select k).FirstAsync();
                var d = new CheckData(rec[r], book,card1);
                ds[r] = d;
            }
                  
            return ds;
        }
        [Authorize]
        [HttpGet("check/{bid}/{cid}")]
        public async Task<string> CheckOperation(int bid,int cid)
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
            if (!card.type.Equals("管理员"))
            {
                throw new ActionResultException(HttpStatusCode.BadRequest, "no right");
            }
            var rec = await (from i in DbContext.Records where bid==i.bookId&&cid==i.cardId&&i.accept == 2 select i).FirstAsync();
            rec.accept = 1;
            DbContext.Records.Update(rec);
            await DbContext.SaveChangesAsync();
            return "ojbk";
        }


        [Authorize]
        [HttpGet("refuse/{bid}/{cid}")]
        public async Task<string> RefuseOperation(int bid, int cid)
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
            if (!card.type.Equals("管理员"))
            {
                throw new ActionResultException(HttpStatusCode.BadRequest, "no right");
            }
            var rec = await (from i in DbContext.Records where bid == i.bookId && cid == i.cardId && i.accept == 2 select i).FirstAsync();
            rec.accept = 0;
            DbContext.Records.Update(rec);
            await DbContext.SaveChangesAsync();
            return "ojbk";
        }
        [Authorize]
        [HttpGet("return/{bid}")]
        public async Task<string> ReturnBook(int bid)
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
            var cid = card.Id;
   
            var books = DbContext.Books;
            var records = DbContext.Records;
        
            var data = await (from record in records where cid==record.cardId&&bid==record.bookId select record).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no record");

            var thisRecord = data[0];

            var data1 = await (from book in DbContext.Books where book.Id.Equals(bid) select book).ToArrayAsync();
            var thisbook = data1[0];
            thisbook.stock++;
            books.Update(thisbook);
            var rec = await (from p in DbContext.Records where bid == p.bookId && cid == p.cardId select p).FirstAsync();
            rec.accept = 0;
            DbContext.Records.Update(rec);
            await DbContext.SaveChangesAsync();
            if (thisRecord.return_date < DateTime.Now) return "overdue";
            return "ojbk";
        }
    }
}
