using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Branch.Core.Common
{
    public class BranchPermissions
    {
        public const string VIEW_ALL_TEAM_RECORDS = "VIEW_ALL_TEAM_RECORDS";
        public const string VIEW_ALL_TEAM_REQUESTS = "VIEW_ALL_TEAM_REQUESTS";
        public const string RE_OR_DE_ACTIVATE_TEAM = "RE_OR_DE_ACTIVATE_TEAM";
        public const string AUTHORIZE_CREATION_OR_MODIFICATION_REQUESTS = "AUTHORIZE_CREATION_OR_MODIFICATION_REQUESTS";
        public const string AUTHORIZE_REACTIVATION_OR_DEACTIVATION_REQUESTS = "AUTHORIZE_REACTIVATION_OR_DEACTIVATION_REQUESTS";
        public const string CREATE_BULK_TEAM = "CREATE_BULK_TEAM";
        public const string CREATE_SINGLE_TEAM = "CREATE_SINGLE_TEAM";
        public const string FAKE = "FAKE";

        public static  List<string> GetBranchPermissions()
        {
            var result = new List<string>();
            result.Add(VIEW_ALL_TEAM_RECORDS);
            result.Add(VIEW_ALL_TEAM_REQUESTS);
            result.Add(RE_OR_DE_ACTIVATE_TEAM);
            result.Add(AUTHORIZE_CREATION_OR_MODIFICATION_REQUESTS);
            result.Add(AUTHORIZE_REACTIVATION_OR_DEACTIVATION_REQUESTS);
            result.Add(CREATE_BULK_TEAM);   
            result.Add (CREATE_SINGLE_TEAM);
            result.Add(FAKE);   
            return result;
        }
    }
}
