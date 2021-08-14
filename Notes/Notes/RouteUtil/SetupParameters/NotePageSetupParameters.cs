using System;
using System.Collections.Generic;

namespace Notes.RouteUtil
{
    public class NotePageSetupParameters : IQueryConvertable
    {
        public static NotePageSetupParameters NewNote(Guid folderId)
        {
            return new NotePageSetupParameters()
            {
                FolderId = folderId,
                NoteId = Guid.NewGuid()
            };
        }
        public static NotePageSetupParameters ExistingNote(Guid noteId)
        {
            return new NotePageSetupParameters()
            {
                NoteId = noteId,
            };
        }

        public string ToQuery()
        {
            var b = new QueryBuilder();
            b.AddQuery(Routes.Query.FolderId, FolderId.ToString());
            b.AddQuery(Routes.Query.NoteId, NoteId.ToString());
            return b.Result;
        }

        public void FromQuery(IDictionary<string, string> query)
        {
            if (query.TryGetValue(Routes.Query.FolderId, out string folderString))
            {
                FolderId = Guid.Parse(folderString);
            }
            if (query.TryGetValue(Routes.Query.NoteId, out string noteString))
            {
                NoteId = Guid.Parse(noteString);
            }
        }

        public Guid FolderId { get; set; }
        public Guid NoteId { get; set; }
    }
}
