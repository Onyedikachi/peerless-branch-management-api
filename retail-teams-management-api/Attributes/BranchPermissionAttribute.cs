using Microsoft.AspNetCore.Authorization;

namespace retail_teams_management_api.Attributes
{
    public class BranchPermissionAttribute:AuthorizeAttribute
    {
        public BranchPermissionAttribute(string permission):base(policy:permission)
        {
            
        }
    }
}
