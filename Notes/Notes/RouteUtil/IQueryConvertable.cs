using System.Collections.Generic;

namespace Notes.RouteUtil
{
    public interface IQueryConvertable
    {
        public string ToQuery();
        public void FromQuery(IDictionary<string, string> query);
    }
}
