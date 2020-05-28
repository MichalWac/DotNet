using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class User
    {
        public String ID { get; set; }
        public String UserName { get; set; }
        public List<Project> Projects { get; set; }
    }
}
