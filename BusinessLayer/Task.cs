using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Task
    {
        public String Id { get; set; }
        public String ProjectId { get; set; }
        public String Assigne { get; set; }
        public int Hour { get; set; }
        
        public String Description { get; set; }
        [Required]
        public String Name { get; set; }
    }
}
