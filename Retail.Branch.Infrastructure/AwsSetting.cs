using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Infrastructure
{
   
    public class AwsSetting
    {
        public string tokenurl { get; set; }
        public string userservice { get; set; }
        public string accountsservice { get; set; }
        public string dbserver { get; set; }
        public string dbport { get; set; }
        public string dbuser { get; set; }
        public string dbname { get; set; }
        public string dbpassword { get; set; }
        public string ConnectionStrings__DefaultConnection { get; set; }
    }

}
