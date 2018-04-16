using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryMSAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMSAPI.Controllers
{
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        public LibraryDbContext DbContext { get; }
        public int booksAmount { get; set; }
        public BooksController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
            booksAmount = DbContext.Books.Count();
        }
        // GET api/values
        [HttpGet("name/{name}")]
        public async Task<Book[]> GetbyName(string name)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.name.Equals(name)select book).ToArrayAsync();
            return data;
        }
        [HttpGet("press/{press}")]
        public async Task<Book[]> GetbyPress(string press)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.press.Equals(press) select book).ToArrayAsync();
            return data;
        }
        [HttpGet("author/{author}")]
        public async Task<Book[]> GetbyAuthor(string author)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.author.Equals(author) select book).ToArrayAsync();
            return data;
        }
        [HttpGet("year/{fromy}/{toy}")]
        public async Task<Book[]> GetbyYear(int fromy,int toy)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.year<=toy&&book.year>=fromy select book).ToArrayAsync();
            return data;
        }
        [HttpGet("stock/{stock}")]
        public async Task<Book[]> GetbyName(int stock)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.stock>=stock select book).ToArrayAsync();
            return data;
        }
        [HttpGet("type/{type}")]
        public async Task<Book[]> GetbyType(int type)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.type.Equals(type) select book).ToArrayAsync();
            return data;
        }
        [HttpGet("price/{fromp}/{top}")]
        public async Task<Book[]> GetbyPrice(double fromp,double top)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.price>=fromp&&book.price<=top select book).ToArrayAsync();
            return data;
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            var books = DbContext.Books;
            await books.AddAsync(book);
            await DbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
