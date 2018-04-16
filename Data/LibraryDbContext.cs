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

        public virtual DbSet<Card> Cards { get; set; }

        public virtual DbSet<Manager> Managers { get; set; }

        public virtual DbSet<Record> Records { get; set; }
    }
}
