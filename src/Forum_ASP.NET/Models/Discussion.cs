using Microsoft.Data.Entity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Forum_ASP.NET.Models
{
    public class Discussion
    {
        //TODO: Delete First Comment ?
        public int DiscussionId { get; set; }
        public string DiscussionName { get; set; }
        public string FirstComment { get; set; }
        public string Author { get; set; }
        public string CreatingDate { get; set; }
        public string LastDate { get; set; }

        public List<Comment> Posts { get; set; }
    }
}