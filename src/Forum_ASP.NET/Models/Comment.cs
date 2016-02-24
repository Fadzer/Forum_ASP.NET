using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum_ASP.NET.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public string CommentAuthor { get; set; }

        public int Discussionname { get; set; }
        public Discussion Discussion { get; set; }
    }
}