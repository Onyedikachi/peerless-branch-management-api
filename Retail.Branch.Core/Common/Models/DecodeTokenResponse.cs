using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Common.Models
{
#nullable disable
    public class DecodeTokenResponse
    {
        public string id { get; set; }
        public object username { get; set; }
        public string email { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public object image { get; set; }
        public string phone { get; set; }
        public object external_reference { get; set; }
        public int failed_password_attempts { get; set; }
        public DateTime last_password_change_date { get; set; }
        public bool is_staff { get; set; }
        public bool is_active { get; set; }
        public bool is_superuser { get; set; }
        public DateTime prev_last_login { get; set; }
        public DateTime last_login { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public bool verified { get; set; }
        public bool is_admin { get; set; }
        public bool is_locked { get; set; }
        public bool force_change_password { get; set; }
        public object team { get; set; }
        public string status { get; set; }
        public object deactivation_reason { get; set; }
        public object group_id { get; set; }
        public string tenant { get; set; }
        public object initiator { get; set; }
        public object approved_by { get; set; }
        public object[] groups { get; set; }
        public object[] user_permissions { get; set; }
        public object[] roles { get; set; }
        public string tenant_id { get; set; }
        public object[] permissions { get; set; }
    }

}
