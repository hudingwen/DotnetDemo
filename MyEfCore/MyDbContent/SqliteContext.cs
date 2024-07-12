using Microsoft.EntityFrameworkCore;
using MyEfCore.Entity;

namespace MyEfCore.MyDbContent
{
    /// <summary>
    /// sqlite
    /// </summary>
    public class SqliteContext: DbContext
    {
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Post> Post { get; set; }
        public string DbPath { get; }

        public SqliteContext()
        {
            DbPath = System.IO.Path.Join(Directory.GetCurrentDirectory(), "SqliteContext.db");
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
    }
}
