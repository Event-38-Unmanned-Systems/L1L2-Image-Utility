using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Position_Adverage
{
    public class posline
    {

        public double latitude;//(deg) 
        public double longitude;//(deg) 
        public double height;//(m)   
        public int Q; //Q=1:fix,2:float,3:sbas,4:dgps,5:single,6:ppp
        public int ns;//number of satellites
        public double sdn;//(m)   
        public double sde;//(m)   
        public double sdu;//(m)  
        public double sdne;//(m)  
        public double sdeu;//(m)  
        public double sdun;//(m) 
        public double age;//(s)  
        public double ratio;
        public int sampleNumber;

    }
}
