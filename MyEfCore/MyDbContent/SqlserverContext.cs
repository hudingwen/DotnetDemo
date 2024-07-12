using Microsoft.EntityFrameworkCore;
using MyEfCore.Entity;

namespace MyEfCore.MyDbContent
{
    /// <summary>
    /// sqlserver
    /// </summary>
    public class SqlserverContext : DbContext
    {
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Comment> Comment { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(@"Data Source = 192.168.0.70;Initial Catalog = test;User Id = sa;Password = ERTYUIGFVBNrtu@#$.;Encrypt=False");
        }
    }
}
