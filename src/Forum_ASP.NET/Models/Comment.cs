using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum_ASP.NET.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        public string Content { get; set; }
        public string CommentAuthor { get; set; }
        public string CommentDate { get; set; }

        public Discussion DiscussionId { get; set; }
    }
}