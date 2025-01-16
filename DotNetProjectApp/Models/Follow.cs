using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProjectApp.Models
{
    public class Follow
    {
        public int Id { get; set; }
        public int FollowerUserID { get; set; }
        public int FollowingUserID { get; set; }

        public User FollowerUser { get; set; }
        public User FollowingUser { get; set; }
    }
}
