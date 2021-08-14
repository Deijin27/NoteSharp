using System;
using System.Collections.Generic;

namespace Notes.RouteUtil
{
    public class RenderPageSetupParameters : IQueryConvertable
    {
        public RenderPageSetupParameters() { }

        public RenderPageSetupParameters(Guid folderId)
        {
            FolderId = folderId;
        }

        public Guid FolderId { get; set; }

        public void FromQuery(IDictionary<string, string> query)
        {
            if (query.TryGetValue(Routes.Query.FolderId, out string folderString))
            {
                FolderId = Guid.Parse(folderString);
            }
        }

        public string ToQuery()
        {
            var b = new QueryBuilder();
            b.AddQuery(Routes.Query.FolderId, FolderId.ToString());
            return b.Result;
        }
    }
}
