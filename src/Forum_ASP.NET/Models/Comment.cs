using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Forum_ASP.NET.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int DiscussionId { get; set; }
        //[Required]
        public string Content { get; set; }
        public string CommentAuthor { get; set; }
        public string CommentDate { get; set; }

        [ForeignKey("DiscussionId")]
        public virtual Discussion Discussion { get; set; }
    }
}