using Notes.Models;
using Notes.RouteUtil;
using Notes.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Mocks;
using Xunit;

namespace Tests.ViewModelTests.FolderPageViewModelTests
{
    public class ListViewTests
    {
        MockAppServiceProvider Services;
        FolderPageViewModel ViewModel;
        readonly Guid fid;
        Guid nestedFolderId;
        Guid nestedNoteId;
        public ListViewTests()
        {
            Services = new MockAppServiceProvider();
            ViewModel = new FolderPageViewModel(Services);

            fid = Guid.Parse("69348ae3-c0e5-4ea8-b3d8-c3ba404fdd83");
            nestedFolderId = Guid.Parse("763cdab7-43de-4d06-b5e6-aa0bf04fd0b3");
            nestedNoteId = Guid.Parse("069119f7-06e6-4fa2-93ee-f0e22ad305ca");
            var rootNotes = new List<Note>
            {
                new Note() { ID = Guid.NewGuid(), FolderID = fid },
                new Note() { ID = nestedNoteId, FolderID = fid },
                new Note() { ID = Guid.NewGuid(), FolderID = fid }
            };
            var rootFolders = new List<Folder>
            {
                new Folder() { ID = nestedFolderId, ParentID = fid },
                new Folder() { ID = Guid.NewGuid(), ParentID = fid }
            };
            Services.MockNoteDatabase.GetFoldersReturnLookup[fid] = rootFolders;
            Services.MockNoteDatabase.GetNotesReturnLookup[fid] = rootNotes;
            Services.MockNoteDatabase.FolderTable[fid] = new Folder() { Name = "yountyufnt" };

            ViewModel.Setup(new FolderPageSetupParameters() { FolderId = fid });
        }

        [Fact]
        public void OnFolderSelected_NavigatesToFolder()
        {
            ViewModel.ListViewSelectedItem = new FolderContentItem(new Folder() { ID = nestedFolderId, ParentID = fid });

            Assert.Single(Services.MockNavigation.GoToWithParametersCalls);
            var (vmType, setupParams) = Services.MockNavigation.GoToWithParametersCalls.Dequeue();
            Assert.Equal(typeof(FolderPageViewModel), vmType);
            Assert.IsType<FolderPageSetupParameters>(setupParams);
            var setup = (FolderPageSetupParameters)setupParams;
            Assert.Equal(nestedFolderId, setup.FolderId);
        }

        [Fact]
        public void OnNoteSelected_NavigatesToNote()
        {
            ViewModel.ListViewSelectedItem = new FolderContentItem(new Note() { ID = nestedNoteId, FolderID = fid });

            Assert.Single(Services.MockNavigation.GoToWithParametersCalls);
            var (vmType, setupParams) = Services.MockNavigation.GoToWithParametersCalls.Dequeue();
            Assert.Equal(typeof(NotePageViewModel), vmType);
            Assert.IsType<NotePageSetupParameters>(setupParams);
            var setup = (NotePageSetupParameters)setupParams;
            Assert.Equal(nestedNoteId, setup.NoteId);
        }
    }
}
