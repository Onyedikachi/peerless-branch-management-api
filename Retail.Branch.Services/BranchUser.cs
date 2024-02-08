using System.Security.Claims;

namespace Retail.Branch.Services
{

    public class BranchUser : ClaimsPrincipal
    {
        public BranchUser(ClaimsPrincipal principal) : base(principal)
        {

        }

        public string UserId
        {
            get
            {
                if (!(Identity is ClaimsIdentity identity))
                    return null;
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                return claim?.Value;
            }
         
        }

        public string FullName
        {
            get
            {
                if (!(Identity is ClaimsIdentity identity))
                    return null;
                var claim = Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                return claim?.Value;
            }
         
        }

        public string Is_Super
        {
            get
            {
                if (!(Identity is ClaimsIdentity identity))
                    return null;

                if(this.FullName.ToLower()== "Admin AppQuest".ToLower())
                {
                    return "False";
                }
                var claim = Claims.FirstOrDefault(c => c.Type == "is_super");
                return claim?.Value;
            }

        }

        public Guid BranchId
        {
            get {  return Guid.Empty; }
        }


    }
}
