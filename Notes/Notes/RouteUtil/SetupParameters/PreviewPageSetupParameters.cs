using System;
using System.Collections.Generic;

namespace Notes.RouteUtil
{
    public class PreviewPageSetupParameters : IQueryConvertable
    {
        public PreviewPageSetupParameters()
        {

        }
        public PreviewPageSetupParameters(Guid folderId, PreviewPageMode mode)
        {
            FolderId = folderId;
            Mode = mode;
        }
        public Guid FolderId { get; private set; }
        public PreviewPageMode Mode { get; private set; }

        public void FromQuery(IDictionary<string, string> query)
        {
            if (query.TryGetValue(Routes.Query.FolderId, out string folderString))
            {
                FolderId = Guid.Parse(folderString);
            }
            if (query.TryGetValue(Routes.Query.Mode, out string modeString))
            {
                Mode = (PreviewPageMode)Enum.Parse(typeof(PreviewPageMode), modeString);
            }
        }

        public string ToQuery()
        {
            var b = new QueryBuilder();
            b.AddQuery(Routes.Query.FolderId, FolderId.ToString());
            b.AddQuery(Routes.Query.Mode, Mode.ToString());
            return b.Result;
        }
    }

    public enum PreviewPageMode
    {
        Markdown,
        Html,
    }

}
