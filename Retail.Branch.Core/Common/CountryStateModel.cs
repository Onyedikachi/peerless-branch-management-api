using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Common
{
   

    public class CountryStateModel
    {
        public List<Country> countries { get; set; } = new();
    }

    public class Country
    {
        public string country { get; set; }
        public string[] states { get; set; }
    }

}
