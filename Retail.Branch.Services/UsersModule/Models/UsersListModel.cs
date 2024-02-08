using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Services.UsersModule.Models
{


    public class UsersListModel
    {
        public Links links { get; set; }
        public int total { get; set; }
        public int total_pages { get; set; }
        public int current_page { get; set; }
        public int page_size { get; set; }
        public List<UserInfoModel> results { get; set; } = new();
        public int all { get; set; }
        public int active { get; set; }
        public int inactive { get; set; }
    }

    public class Links
    {
        public object next { get; set; }
        public object previous { get; set; }
    }

    public class UserInfoModel
    {
        public string id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public object username { get; set; }
        public string email { get; set; }
        public Role[] roles { get; set; }
        public object image { get; set; }
        public bool is_active { get; set; }
        public bool verified { get; set; }
        public DateTime? last_login { get; set; }
        public DateTime created_at { get; set; }
        public string phone { get; set; }
        public string team { get; set; }
    }

    public class Role
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool is_active { get; set; }
        public bool is_locked { get; set; }
        public string tenant { get; set; }
        public string created_by { get; set; }
        public UserInfoPermission[] permissions { get; set; }
    }

    public class UserInfoPermission
    {
        public string id { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string submodule { get; set; }
    }

}
