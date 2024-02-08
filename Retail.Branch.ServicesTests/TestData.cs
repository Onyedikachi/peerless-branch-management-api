using Retail.Branch.Services;
using Retail.Branch.Services.BranchModule.Models;
using Retail.Branch.Services.BranchRequestModule.Models;
using Retail.Branch.Services.LedgerModule.Model;
using Retail.Branch.Services.UsersModule.Models;
using System.Security.Claims;

namespace Retail.Branch.ServicesTests
{
    public static class TestData
    {
        public static Guid DefaultBranchId = Guid.Parse("18063564-c028-4543-b2df-e0b12dffb2ca");
      //  public static Guid AltBranchId = Guid.Parse("18063564-c028-4543-b2df-e0b12dffb2ca");
        public static Guid DefaultRequestId = Guid.Parse("18063564-c028-4543-b2df-e0b12dffb2ca");
        public static Guid PendingRequestId = Guid.Parse("18063565-c028-4543-b2df-e0b13dffb2ca");
        public static Guid PendingRequestId2 = Guid.Parse("18063565-c028-4543-b2df-e0b12dffb2cb");
        public static Guid PendingRequestId3 = Guid.Parse("18063565-c028-4543-b2df-e0b12dffb2cc");
        public static Guid PendingBulkRequestId = Guid.Parse("18063564-c028-4543-b2df-e0b13dffb2ce");
        public static Guid DraftBulkRequestId = Guid.Parse("18063564-c028-4553-b2df-e0b13dffb2cf");

        public static Guid PendingChangeRequestId = Guid.Parse("18073565-c028-4533-b2df-e0b12dffb2cf");

        public static BranchUser GetCurrentUser()
        {
            var claims = new List<Claim>()
                     {

                         new Claim(ClaimTypes.Name,"ojo"),
                         new Claim(ClaimTypes.Email,"ojunix@gmail.com"),
                         new Claim(ClaimTypes.NameIdentifier,Guid.Empty.ToString()),
                         new Claim(ClaimTypes.MobilePhone,"0805"),
                         new Claim("is_super","True")

                     };

            var claimsIdentity = new ClaimsIdentity
                (claims, "");
            var claimsPrincipal = new ClaimsPrincipal
                (claimsIdentity);


            return new BranchUser(claimsPrincipal);
        }

        public static BranchUser GetBasicUser()
        {
            var claims = new List<Claim>()
                     {

                         new Claim(ClaimTypes.Name,"ojo"),
                         new Claim(ClaimTypes.Email,"ojunix@gmail.com"),
                         new Claim(ClaimTypes.NameIdentifier,Guid.Empty.ToString()),
                         new Claim(ClaimTypes.MobilePhone,"0805"),
                         new Claim("is_super","False")

                     };

            var claimsIdentity = new ClaimsIdentity
                (claims, "");
            var claimsPrincipal = new ClaimsPrincipal
                (claimsIdentity);


            return new BranchUser(claimsPrincipal);
        }

        public static LedgerResponseModel LedgerData()
        {
            LedgerResponseModel model = new LedgerResponseModel();
            var result = new List<LedgerResult>();

            result.Add(new LedgerResult()
            {
                approved_by = "Admin",
                balance_flag = true,
                class_ledger_code = "101",
                created_at = DateTime.Now,
                currency = new Currency() { abbreviation = "NGN", id = "1", is_default = true, name = "Naira" },
                is_default = true,
                is_locked = true,
                description = "Default",
                id = "1",
                is_parent = true,
                ledger_code = "101",
                name = "Default",
                net_balance = 20000,
                parent_ledger_code = "101",
                post_no_credit = true,
                post_no_debit = true,
                sequential_ledger_code = "101",
                state = "Lagos",
                sub_ledger_sequencing_start = 10,
                sub_ledger_sequencing_stop = 10,
                sub_ledger_sequencing_type = "Easy",
                sub_ledger_sequencing_type_number = 10,
                transaction_count = 10,
                updated_at = DateTime.UtcNow,
                initiator = new Initiator() { email = "ojunix@hotmail.com", firstname = "ojorma", lastname = "odumah", id = "1" },
                group_id = "1",
                gl_class = new Gl_Class() { id = "1", code = "123", children_count = 0, credit_impact_on_balance = "132", debit_impact_on_balance = "300", is_configured = true, name = "Ledger" },
                sub_ledgers = new Sub_Ledgers[]
                            { new Sub_Ledgers() { approved_by="admin", balance_flag=true, class_ledger_code="23423",
                                created_at= DateTime.UtcNow, currency= new Currency1(){ abbreviation="NGN", id = "1", is_default=true, name="Naira" },
                                 description="Naira", gl_class=new Gl_Class1 { id = "1", code = "123", children_count = 0, credit_impact_on_balance = "132", debit_impact_on_balance = "300", is_configured = true, name = "Ledger" },
                                  group_id="1", id="1", identifier="sfsdf", is_default = true, is_locked = true, name = "1", is_parent=false,
                                   ledger_code = "1", net_balance = 3000, post_no_credit = true, post_no_debit = true,
                                    state="Open", sub_ledger_sequencing_start = "1", sub_ledger_sequencing_stop="0",
                                     transaction_count=1, updated_at= DateTime.UtcNow,   sub_ledger_sequencing_type_number="3",
                                      sub_ledger_sequencing_type="323"
                            } }

            });
            model.results = result;
            return model;
        }

        public static List<UserInfoModel> GetUsers()
        {
            var result = new List<UserInfoModel>();
            result.Add(new UserInfoModel()
            {

                created_at = DateTime.UtcNow,
                email = "ojunix@hotmail.com",
                firstname = "john",
                id = "1",
                is_active = true,
                lastname = "doe",
                last_login = DateTime.UtcNow,
                phone = "0805913",
                team = "",
                username = "ojunix",
                verified = true,
                image = "image",
                roles = new Role[] { new Role() { id = "1" } },


            });

            return result;
        }

        public static UserInfoModel GetUser()
        {
            var model = new UserInfoModel()
            {

                created_at = DateTime.UtcNow,
                email = "ojunix@hotmail.com",
                firstname = "john",
                id = "1",
                is_active = true,
                lastname = "doe",
                last_login = DateTime.UtcNow,
                phone = "0805913",
                team = "",
                username = "ojunix",
                verified = true,

            };
            return model;
        }

        public static  UploadDocument uploadDocData()
        {
            return new UploadDocument() { Base64 = "12323123", ext = "ext" };
        }

        public static EditRequestModel GetEditModel()
        {
            EditRequestModel model = new EditRequestModel();
            model.number = "234";
            model.streetname = "Edit Street";
            model.country = "Nigeria";
            model.city = "Asaba";
            model.description = "Description";
            model.lga = "Oshimili";
            model.name = "New Edited Name";
            model.State = "Lagos";
          

            return model;
        }
    }
}
