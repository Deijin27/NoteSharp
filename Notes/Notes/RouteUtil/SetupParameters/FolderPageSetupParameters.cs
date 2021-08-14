using System;
using System.Collections.Generic;

namespace Notes.RouteUtil
{
    public class FolderPageSetupParameters : IQueryConvertable
    {
        public Guid FolderId { get; set; }

        public void FromQuery(IDictionary<string, string> query)
        {
            if (query.TryGetValue(Routes.Query.FolderId, out string folderIdString))
            {
                FolderId = Guid.Parse(folderIdString);
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
