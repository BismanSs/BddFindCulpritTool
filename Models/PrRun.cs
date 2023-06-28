using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BddFindCulpritTool.Models
{
    public class PrRun
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GitHash { get; set; }
        public string Link { get; set; }
        public int RunStatus { get; set; }

        public PrRun()
        {

        }

    }
}
