using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyEfCore.Entity
{
    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
