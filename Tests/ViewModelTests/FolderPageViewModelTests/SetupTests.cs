using System;
using System.Collections.Generic;
using Xunit;
using Tests.Mocks;
using Notes.ViewModels;
using Notes.Models;
using Notes.RouteUtil;

namespace Tests.ViewModelTests.FolderPageViewModelTests
{
    public class SetupTests
    {
        MockAppServiceProvider Services;
        FolderPageViewModel ViewModel;
        public SetupTests()
        {
            Services = new MockAppServiceProvider();
            ViewModel = new FolderPageViewModel(Services);
        }

        [Fact]
        public void SetupAsRoot_ShouldLoadRootFolderContents()
        {
            var rootNotes = new List<Note>
            {
                new Note() { ID = Guid.NewGuid(), FolderID = Guid.Empty },
                new Note() { ID = Guid.NewGuid(), FolderID = Guid.Empty }
            };
            var rootFolders = new List<Folder>
            {
                new Folder() { ID = Guid.NewGuid(), ParentID = Guid.Empty },
                new Folder() { ID = Guid.NewGuid(), ParentID = Guid.Empty }
            };
            Services.MockNoteDatabase.GetFoldersReturnLookup[Guid.Empty] = rootFolders;
            Services.MockNoteDatabase.GetNotesReturnLookup[Guid.Empty] = rootNotes;

            ViewModel.Setup(new FolderPageSetupParameters() { FolderId = Guid.Empty });

            Assert.NotNull(ViewModel.FolderContentItems);
            Assert.Equal(4, ViewModel.FolderContentItems.Count);
            foreach (var fci in ViewModel.FolderContentItems)
            {
                if (fci.Identifier == FolderContentItemIdentifier.File)
                {
                    Assert.Contains(fci.ContentNote, rootNotes);
                }
                else if (fci.Identifier == FolderContentItemIdentifier.Folder)
                {
                    Assert.Contains(fci.ContentFolder, rootFolders);
                }
            }
        }

        [Fact]
        public void SetupAsNonRoot_ShouldLoadNonRootFolderContents()
        {
            Guid fid = Guid.NewGuid();
            var rootNotes = new List<Note>
            {
                new Note() { ID = Guid.NewGuid(), FolderID = fid },
                new Note() { ID = Guid.NewGuid(), FolderID = fid },
                new Note() { ID = Guid.NewGuid(), FolderID = fid }
            };
            var rootFolders = new List<Folder>
            {
                new Folder() { ID = Guid.NewGuid(), ParentID = fid },
                new Folder() { ID = Guid.NewGuid(), ParentID = fid }
            };
            Services.MockNoteDatabase.GetFoldersReturnLookup[fid] = rootFolders;
            Services.MockNoteDatabase.GetNotesReturnLookup[fid] = rootNotes;
            Services.MockNoteDatabase.FolderTable[fid] = new Folder() { Name = "yountyufnt" };

            ViewModel.Setup(new FolderPageSetupParameters() { FolderId = fid });

            Assert.NotNull(ViewModel.FolderContentItems);
            Assert.Equal(5, ViewModel.FolderContentItems.Count);
            foreach (var fci in ViewModel.FolderContentItems)
            {
                if (fci.Identifier == FolderContentItemIdentifier.File)
                {
                    Assert.Contains(fci.ContentNote, rootNotes);
                }
                else if (fci.Identifier == FolderContentItemIdentifier.Folder)
                {
                    Assert.Contains(fci.ContentFolder, rootFolders);
                }
            }
        }

        [Fact]
        public void SetupAsNonRootFolder_ShouldSetTitleToFolderName()
        {
            Guid fid = Guid.NewGuid();
            var rootNotes = new List<Note>();
            var rootFolders = new List<Folder>();
            Services.MockNoteDatabase.GetFoldersReturnLookup[fid] = rootFolders;
            Services.MockNoteDatabase.GetNotesReturnLookup[fid] = rootNotes;
            Services.MockNoteDatabase.FolderTable[fid] = new Folder() { Name = "some title" };

            ViewModel.Setup(new FolderPageSetupParameters() { FolderId = fid });

            Assert.Equal("some title", ViewModel.PageTitle);
        }
    }
}
