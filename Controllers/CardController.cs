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

namespace LibraryMSAPI.Controllers
{
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


        [HttpPost("add")]
        public async Task<IActionResult> AddCard([FromBody]Card card)
        {
            try
            {
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
