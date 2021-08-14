using Notes.Models;
using Notes.ViewModels;
using System;
using System.Collections.Generic;

namespace Notes.RouteUtil
{
    public class MovePageSetupParameters : IQueryConvertable
    {
        public Guid ItemToMoveId { get; set; }
        public Guid CurrentFolderId { get; set; }
        public MoveMode Mode { get; set; }

        public void FromQuery(IDictionary<string, string> query)
        {
            if (query.TryGetValue(Routes.Query.Id, out string idString))
            {
                ItemToMoveId = Guid.Parse(idString);
            }
            if (query.TryGetValue(Routes.Query.FolderId, out string folderIdString))
            {
                CurrentFolderId = Guid.Parse(folderIdString);
            }
            if (query.TryGetValue(Routes.Query.Mode, out string modeString))
            {
                Mode = (MoveMode)Enum.Parse(typeof(MoveMode), modeString);
            }
        }

        public string ToQuery()
        {
            var b = new QueryBuilder();
            b.AddQuery(Routes.Query.Id, ItemToMoveId.ToString());
            b.AddQuery(Routes.Query.FolderId, CurrentFolderId.ToString());
            b.AddQuery(Routes.Query.Mode, Mode.ToString());
            return b.Result;
        }

        public MovePageSetupParameters() { }

        public MovePageSetupParameters(Note note)
        {
            Mode = MoveMode.Note;
            ItemToMoveId = note.ID;
            CurrentFolderId = note.FolderID;
        }

        public MovePageSetupParameters(Folder folder)
        {
            Mode = MoveMode.Folder;
            ItemToMoveId = folder.ID;
            CurrentFolderId = folder.ParentID;
        }
    }
}
