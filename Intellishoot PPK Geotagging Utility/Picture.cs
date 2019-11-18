using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intellishoot_PPK_Geotagging_Utility
{
    class Picture
    {
        public double timetaken { get; set; }

        public string ImageName { get; set; }

        public double timeDist { get; set; }

        public int estIDX { get; set; }

        public int calcedIDX { get; set; }

        public bool isused { get; set; }
    }
}
