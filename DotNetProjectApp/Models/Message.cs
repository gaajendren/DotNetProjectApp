using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetProjectApp.Models
{
    public class Message
    {

       
        public int Id { get; set; }       
        public int SenderUserID { get; set; }
  
        public int ReceiverUserID { get; set; }
       
        public string Content { get; set; }
        public User SenderUser { get; set; }
        public User ReceiverUser { get; set; }

        public DateTime SentAt { get; set; }
    }
}
