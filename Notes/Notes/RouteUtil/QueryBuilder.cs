
namespace Notes.RouteUtil
{
    public class QueryBuilder
    {
        const string ResultDefault = "";
        public string Result { get; private set; } = ResultDefault;
        public void AddQuery(string key, string value)
        {
            string preceeder = Result == ResultDefault ? "?" : "&";
            Result += $"{preceeder}{key}={value}";
        }
    }
}
