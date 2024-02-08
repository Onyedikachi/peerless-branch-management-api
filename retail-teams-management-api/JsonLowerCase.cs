using System.Text.Json;

namespace retail_teams_management_api
{
    public class JsonLowerCase: JsonNamingPolicy
    {
        public static JsonLowerCase Instance { get; } = new JsonLowerCase();
        public override string ConvertName(string name) =>
           name.ToLower();        
    }

}
