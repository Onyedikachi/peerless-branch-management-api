using System.Globalization;

namespace retail_teams_management_api.Exceptions
{

    public class PermissionException : Exception
    {
        public PermissionException() : base() { }

        public PermissionException(string message) : base(message) { }

        public PermissionException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}
