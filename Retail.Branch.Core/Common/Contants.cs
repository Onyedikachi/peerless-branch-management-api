namespace Retail.Branch.Core.Common
{
    public static class AppContants
    {
        public static readonly char PENDING_BRANCH_STATUS = 'P';
        public static readonly char DRAFT_BRANCH_STATUS = 'D';
        public static readonly char APPROVED_BRANCH_STATUS = 'A';
        public static readonly char REJECTED_BRANCH_STATUS = 'R';
        public static readonly char INACTIVE_BRANCH_STATUS = 'I';

        public static readonly (char, string)[] TEAM_STATUSES = new (char, string)[]
        {
            (PENDING_BRANCH_STATUS, "Pending"),
            (DRAFT_BRANCH_STATUS, "Draft"),
            (APPROVED_BRANCH_STATUS, "Active"),
            (REJECTED_BRANCH_STATUS, "Rejected"),
            (INACTIVE_BRANCH_STATUS, "Inactive")
        };

        public const char PENDING_REQUEST_STATUS = 'P';
        public const char APPROVED_REQUEST_STATUS = 'A';
        public const char REJECTED_REQUEST_STATUS = 'R';
        public const char DRAFT_REQUEST_STATUS = 'D';


        public static readonly (char, string)[] REQUEST_STATUSES = new (char, string)[]
        {
            (PENDING_REQUEST_STATUS, "Pending"),
            (APPROVED_REQUEST_STATUS, "Approved"),
            (REJECTED_REQUEST_STATUS, "Rejected"),
            (DRAFT_REQUEST_STATUS, "Draft"),
        };


        public const string BULK_CREATE_REQUEST_TYPE = "BULK_CREATE";
        public const string CREATE_REQUEST_TYPE = "CREATE";
        public const string DEACTIVATE_REQUEST_TYPE = "DEACTIVATE";
        public const string REACTIVATE_REQUEST_TYPE = "REACTIVATE";
        public const string CHANGE_REQUEST_TYPE = "CHANGE";

        public static readonly (string, string)[] REQUEST_TYPES = new (string, string)[]
        {
             (BULK_CREATE_REQUEST_TYPE, "Bulk Create"),
             (CREATE_REQUEST_TYPE, "Create"),
             (DEACTIVATE_REQUEST_TYPE, "Deactivate"),
             (REACTIVATE_REQUEST_TYPE, "Reactivate"),
             (CHANGE_REQUEST_TYPE, "Change"),
        };

        public static string LIST_BRANCH_FILTER_CREATED_BY_ME = "created_by_me";
        public static string LIST_BRANCH_FILTER_CREATED_BY_MY_TEAM = "created_by_my_team";

        public static readonly (string, string)[] LIST_TEAM_FILTER_CHOICES = new (string, string)[]
        {
             (LIST_BRANCH_FILTER_CREATED_BY_ME, "Created by me"),
             (LIST_BRANCH_FILTER_CREATED_BY_MY_TEAM, "Created by my team"),
        };
    }
}
