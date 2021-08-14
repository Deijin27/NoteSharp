using Notes.Models;
using Notes.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Mocks;
using Xunit;
using Notes.Services;
using Notes.RouteUtil;
using Notes.Data;

namespace Tests.ViewModelTests.FolderPageViewModelTests
{
    public class CommandTests
    {
        MockAppServiceProvider Services;
        FolderPageViewModel ViewModel;
        readonly Guid folderId;
        public CommandTests()
        {
            Services = new MockAppServiceProvider();
            ViewModel = new FolderPageViewModel(Services);
            folderId = Guid.NewGuid();
            Services.MockNoteDatabase.FolderTable[folderId] = new Folder();
            ViewModel.Setup(new FolderPageSetupParameters() { FolderId = folderId });
        }

        [Fact]
        public void RenameFolderCommand_RenamesAndSavesFolder()
        {
            Services.MockPopups.PromptPopupResult = (TwoOptionPopupResult.LeftButton, "new name");
            var paramFolder = new Folder() { Name = "original name"};

            ViewModel.RenameFolderCommand.Execute(new FolderContentItem(paramFolder));

            Assert.Single(Services.MockNoteDatabase.SaveFolderCalls);
            var called = Services.MockNoteDatabase.SaveFolderCalls.Dequeue();
            Assert.Same(paramFolder, called);
            Assert.Equal("new name", called.Name);
        }

        [Fact]
        public void AddFolderCommand_AddsNewFolderInCurrentFolder()
        {
            Services.MockPopups.PromptPopupResult = (TwoOptionPopupResult.LeftButton, "name of new folder");

            ViewModel.AddFolderCommand.Execute(null);

            Assert.Single(Services.MockNoteDatabase.SaveFolderCalls);
            var called = Services.MockNoteDatabase.SaveFolderCalls.Dequeue();
            Assert.Equal("name of new folder", called.Name);
            Assert.Equal(folderId, called.ParentID);
        }

        [Fact]
        public void DeleteFolderCommand_DeletesFolder()
        {
            Services.MockPopups.AlertPopupResult = AlertPopupResult.Dismiss;
            var paramFolder = new Folder();

            ViewModel.DeleteFolderCommand.Execute(new FolderContentItem(paramFolder));

            Assert.Single(Services.MockNoteDatabase.DeleteFolderAndAllContentsCalls);
            Assert.Same(paramFolder, Services.MockNoteDatabase.DeleteFolderAndAllContentsCalls.Dequeue());
        }

        [Fact]
        public void RenameNoteCommand_RenamesAndSavesNote()
        {
            Services.MockPopups.PromptPopupResult = (TwoOptionPopupResult.LeftButton, "new name");
            var paramNote = new Note() { Name = "original name" };

            ViewModel.RenameNoteCommand.Execute(new FolderContentItem(paramNote));

            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var called = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Same(paramNote, called);
            Assert.Equal("new name", called.Name);
        }

        [Fact]
        public void DeleteNoteCommand_DeletesNote()
        {
            Services.MockPopups.AlertPopupResult = AlertPopupResult.Dismiss;
            var paramNote = new Note();

            ViewModel.DeleteNoteCommand.Execute(new FolderContentItem(paramNote));

            Assert.Single(Services.MockNoteDatabase.DeleteNoteCalls);
            Assert.Same(paramNote, Services.MockNoteDatabase.DeleteNoteCalls.Dequeue());
        }

        [Fact]
        public void AddNoteCommand_NavigatesToNotePageWithNewNoteParameters()
        {
            ViewModel.AddNoteCommand.Execute(null);

            Assert.Single(Services.MockNavigation.GoToWithParametersCalls);
            var (vmType, setupParameters) = Services.MockNavigation.GoToWithParametersCalls.Dequeue();
            Assert.Equal(typeof(NotePageViewModel), vmType);
            Assert.IsType<NotePageSetupParameters>(setupParameters);
            var setup = (NotePageSetupParameters)setupParameters;
            Assert.Equal(folderId, setup.FolderId);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToggleQuickAccessNoteCommand_TogglesAndSaves(bool initialQuickAccess)
        {
            var paramNote = new Note() { IsQuickAccess = initialQuickAccess };

            ViewModel.ToggleQuickAccessNoteCommand.Execute(new FolderContentItem(paramNote));

            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var carriedParamNote = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Same(paramNote, carriedParamNote);
            Assert.NotEqual(initialQuickAccess, carriedParamNote.IsQuickAccess);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ToggleQuickAccessFolderCommand_TogglesAndSaves(bool initialQuickAccess)
        {
            var paramFolder = new Folder() { IsQuickAccess = initialQuickAccess };

            ViewModel.ToggleQuickAccessFolderCommand.Execute(new FolderContentItem(paramFolder));

            Assert.Single(Services.MockNoteDatabase.SaveFolderCalls);
            var carriedParamFolder = Services.MockNoteDatabase.SaveFolderCalls.Dequeue();
            Assert.Same(paramFolder, carriedParamFolder);
            Assert.NotEqual(initialQuickAccess, carriedParamFolder.IsQuickAccess);
        }

        [Fact]
        public void MoveFolderCommand_NavigatesToMovePageWithNoteSetup()
        {
            var id = Guid.NewGuid();
            var pid = Guid.NewGuid();
            var paramFolder = new Folder() { ID = id, ParentID = pid };

            ViewModel.MoveFolderCommand.Execute(new FolderContentItem(paramFolder));

            Assert.Single(Services.MockNavigation.GoToWithParametersCalls);
            var (vmType, setupParameters) = Services.MockNavigation.GoToWithParametersCalls.Dequeue();
            Assert.Equal(typeof(MovePageViewModel), vmType);
            Assert.IsType<MovePageSetupParameters>(setupParameters);
            var setup = (MovePageSetupParameters)setupParameters;
            Assert.Equal(MoveMode.Folder, setup.Mode);
            Assert.Equal(id, setup.ItemToMoveId);
            Assert.Equal(pid, setup.CurrentFolderId);
        }

        [Fact]
        public void MoveNoteCommand_NavigatesToMovePageWithNoteSetup()
        {
            var id = Guid.NewGuid();
            var pid = Guid.NewGuid();
            var paramNote = new Note() { ID = id, FolderID = pid };

            ViewModel.MoveNoteCommand.Execute(new FolderContentItem(paramNote));

            Assert.Single(Services.MockNavigation.GoToWithParametersCalls);
            var (vmType, setupParameters) = Services.MockNavigation.GoToWithParametersCalls.Dequeue();
            Assert.Equal(typeof(MovePageViewModel), vmType);
            Assert.IsType<MovePageSetupParameters>(setupParameters);
            var setup = (MovePageSetupParameters)setupParameters;
            Assert.Equal(MoveMode.Note, setup.Mode);
            Assert.Equal(id, setup.ItemToMoveId);
            Assert.Equal(pid, setup.CurrentFolderId);
        }

        [Fact]
        public void OrderByCommand_ChangesOrderBy()
        {
            Services.MockPreferences.SortingMode = SortingMode.DateCreated;
            Services.MockPopups.ListPopupResult = (ListPopupResult.ListItem, SortingMode.DateModified);

            ViewModel.OrderByCommand.Execute(null);

            Assert.Equal(SortingMode.DateModified, Services.MockPreferences.SortingMode); 
        }
    }
}
