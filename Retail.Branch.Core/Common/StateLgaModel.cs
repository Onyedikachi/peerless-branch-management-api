using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Common
{

    public class StateLgaModel
    {
        public State[] states { get; set; }
    }

    public class State
    {
        public string state { get; set; }
        public string alias { get; set; }
        public string[] lgas { get; set; }
    }



  

}
