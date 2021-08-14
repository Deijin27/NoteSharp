using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.RouteUtil
{
    public class CssPageSetupParameters : IQueryConvertable
    {
        public CssPageSetupParameters() { }

        public CssPageSetupParameters(Guid cssId)
        {
            CssId = cssId;
        }

        public string ToQuery()
        {
            var b = new QueryBuilder();
            b.AddQuery(Routes.Query.Id, CssId.ToString());
            return b.Result;
        }

        public void FromQuery(IDictionary<string, string> query)
        {
            if (query.TryGetValue(Routes.Query.Id, out string cssString))
            {
                CssId = Guid.Parse(cssString);
            }
        }

        public Guid CssId { get; set; }
    }
    
}
