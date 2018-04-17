using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryMSAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Net;
using Sakura.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryMSAPI.Controllers
{
    public class MyB
    {
        public MyB(Book _b, Record _d)
        {
            book = _b;
            record = _d;
        }
        public Book book;
        public Record record;
    }
    [Route("api/[controller]")]
    public class CardController : Controller
    {
        public LibraryDbContext DbContext { get; }
        public CardController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
        }

        [HttpGet("{id}")]
        public async Task<Card> GetCard(int id)
        {
            var books = DbContext.Books;
            var cards = DbContext.Cards;
            var data = await (from card in DbContext.Cards where card.Id == id select card).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no card");
            return data[0];
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<Card> GetMyCard()
        {
            // var token = Request.Headers["Authorization"];
            //var data1 = await (from i in DbContext.Authorizations where token.Equals(i.token) select i).FirstAsync();
            //  var userName = data1.name;
            var userName = User.Identity.Name;
            var books = DbContext.Books;
            var cards = DbContext.Cards;
            var data = await (from card in DbContext.Cards where card.name.Trim().Equals(userName) select card).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no card");
            return data[0];
        }

        [Authorize]
        [HttpGet("books/me")]
        public async Task<MyB[]> GetMyBooks()
        {
            var userName = User.Identity.Name;
            var books = DbContext.Books;
            var cards = DbContext.Cards;
         
            var records = DbContext.Records;
            var data = await (from card in cards where card.name.Equals(userName) select card).ToArrayAsync();

            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no card");
            var myCard =data[0];
            var cid = myCard.Id;
            var ds = await (from b in DbContext.Books
                            join r in DbContext.Records on b.Id equals r.bookId into rbs
                            from d in rbs
                            where d.cardId == cid&&d.accept!=0
                            select new { b, d }).ToArrayAsync();
            var bk = new List<MyB>();
            foreach(var i in ds)
            {
                bk.Add(new MyB(i.b, i.d));
            }
            return bk.ToArray();
        }
        [HttpGet("book/{id}")]
        public async Task<Book[]> GetBooksbyCard(int id)
        {
            var books = DbContext.Books;
            var cards = DbContext.Cards;
            var records = DbContext.Records;
            var data = await (from card in cards where id == card.Id select card).ToArrayAsync();
            if (data.Length == 0) throw new ActionResultException(HttpStatusCode.BadRequest, "no card");
            var bids = await (from record in records where record.cardId == id select record.bookId).ToArrayAsync();
            var bbooks = await (from book in books where bids.Contains(book.Id) select book).ToArrayAsync();
            return bbooks;
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddCard([FromBody]Card card)
        {
            try
            {
                var userName = User.Identity.Name;
                var myCard = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
                if (!myCard.type.Equals("管理员"))
                {
                    throw new ActionResultException(HttpStatusCode.BadRequest, "no right");
                }
                var cards = DbContext.Cards;
                await cards.AddAsync(card);
                await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                throw new ActionResultException(HttpStatusCode.BadRequest, "card message is not right");
            }
           
        }

        [HttpPost("delete/{id}")]
        public async Task<IActionResult> DeleteCard(int id)
        {
            try
            {
                var cards = DbContext.Cards;
                var data = await (from i in cards where i.Id == id select i).ToArrayAsync();
                var card = data[0];
                cards.Remove(card);
                await DbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                throw new ActionResultException(HttpStatusCode.BadRequest, "card id is not right");
            }

        }
    }
}
