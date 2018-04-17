using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LibraryMSAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Sakura.AspNetCore.Mvc;
using System.Net;

namespace LibraryMSAPI.Controllers
{
    public class BookState
    {
        public BookState(DateTime _d,int _b)
        {
            returnDate = _d;
            borrowState = _b;
        }
        public DateTime returnDate;
        public int borrowState;
    }
    public class SearchForm
    {
        public string name;
        public string press;
        public string author;
        public string type;
        public string fromyear;
        public string toyear;
        public string fromprice;
        public string toprice;
    }
    [Route("api/[controller]")]
    public class BooksController : Controller
    {
        public LibraryDbContext DbContext { get; }
       // public int booksAmount { get; set; }
        public BooksController(LibraryDbContext libraryDbContext)
        {
            DbContext = libraryDbContext;
    //        booksAmount = DbContext.Books.Count();
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
        public async Task<Book[]> GetbyPrice(decimal fromp,decimal top)
        {
            var books = DbContext.Books;
            var data = await (from book in DbContext.Books where book.price>=fromp&&book.price<=top select book).ToArrayAsync();
            return data;
        }
        [HttpPost("search")]
        public async Task<Book[]> SearchBook([FromBody] SearchForm searchForm)
        {
            var books = (from i in DbContext.Books
                         select i);
            if (searchForm.name != "")
                books = (from i in books
                        where i.name.Contains(searchForm.name)
                        select i);
            if (searchForm.press != "")
                books = (from j in books
                               where j.press.Contains(searchForm.press)
                               select j);
            if (searchForm.fromyear != "")
                books = (from k in books
                         where k.year >= int.Parse(searchForm.fromyear)
                         select k);
            if (searchForm.toyear != "")
                books = (from q in books
                         where q.year <= int.Parse(searchForm.toyear)
                         select q);
            if (searchForm.type != "")
                books = (from w in books
                         where w.type.Equals(searchForm.type)
                         select w);
            if (searchForm.fromprice != "")
                books = (from e in books
                         where e.price >= decimal.Parse(searchForm.fromprice)
                         select e);
            if (searchForm.toprice != "")
                books = (from r in books
                         where r.price <= decimal.Parse(searchForm.toprice)
                         select r);
            if (searchForm.author != "")
                books = (from t in books
                         where t.author.Contains(searchForm.author)
                         select t);
            var data = await books.ToArrayAsync();
            return data;
        }


        [Authorize]
        [HttpGet("state/{id}")]
        public async Task<BookState> GetBookState(int id)
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name) select i).FirstAsync();
            var cid = card.Id;
            var records = await (from k in DbContext.Records where id == k.bookId orderby k.return_date select k).ToArrayAsync();
            if(records.Length==0) return new BookState(DateTime.MinValue, 0);
            var d = records[0].return_date;
            var record = await (from j in DbContext.Records where cid == j.cardId && id == j.bookId select j).ToArrayAsync();
            if (record.Length == 0) return new BookState(d,0);
            if (record[0].accept == 1) return new BookState(d, 1);
            if (record[0].accept == 0) return new BookState(d, 0);
            return new BookState(d, 2);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddBook([FromBody] Book book)
        {
            var userName = User.Identity.Name;
            var card = await (from i in DbContext.Cards where userName.Equals(i.name)select i).FirstAsync();
            if (card.type.Equals("管理员"))
            {
                var books = DbContext.Books;
                await books.AddAsync(book);
                await DbContext.SaveChangesAsync();
                return Ok();
            }
            else
            {
                throw new ActionResultException(HttpStatusCode.BadRequest, "no right");
            }
          
        }


    }
}
