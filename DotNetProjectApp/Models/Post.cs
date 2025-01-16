using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProjectApp.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Caption { get; set; }
        public string ImagePath { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}