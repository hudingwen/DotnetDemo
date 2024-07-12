using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace MyEfCore.Entity
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }

    }
}
