﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.PermissionModule.Model
{   

    public class PermissionModel
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string submodule { get; set; }
    }

}
