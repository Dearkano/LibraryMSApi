using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using LibraryMSAPI.Data;

namespace LibraryMSAPI.Data
{
    public class LibraryDbContext:DbContext
    {
 
        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {

        }

        public  DbSet<Book> Books { get; set; }

        public  DbSet<Card> Cards { get; set; }

        public  DbSet<Record> Records { get; set; }

        public  DbSet<Authorization> Authorizations { get; set; }
    }
}
