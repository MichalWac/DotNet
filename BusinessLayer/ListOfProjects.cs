using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BusinessLayer
{
    public class ListOfProjects
    {
        public List<SelectListItem> list { get; set; }
        
        public String userId { get; set; }

        public int count { get; set; }

        public String id { get; set; }
    }
}
