using Notes.Models;
using Notes.RouteUtil;
using Notes.ViewModels;
using System;
using Tests.Mocks;
using Xunit;

namespace Tests.ViewModelTests.NotePageViewModelTests
{
    /// <summary>
    /// Scenario: User opens existing note then, without changing anything, leaves the page by means other than closing such that the note is cached.
    /// Then the user re-opens the note page.
    /// </summary>
    public class CachedUneditedExistingNoteTests
    {
        MockAppServiceProvider Services;
        public CachedUneditedExistingNoteTests()
        {
            Services = new MockAppServiceProvider();
            ViewModel = new NotePageViewModel(Services);
            var id = Guid.NewGuid();
            Services.MockNoteDatabase.NoteTable[id] = new Note() { ID = id, FolderID = Guid.NewGuid(), Name = "name", Text = "text" };
            SetupParams = NotePageSetupParameters.ExistingNote(id);
            ViewModel.Setup(SetupParams);
        }
        NotePageSetupParameters SetupParams;
        NotePageViewModel ViewModel;

        [Fact]
        public void ImmediatelySaving_ShouldNotSave()
        {
            ViewModel.SaveCommand.Execute(null);
            Assert.Empty(Services.MockNoteDatabase.SaveNoteCalls);
        }

        [Fact]
        public void EditingNameThenSaving_ShouldSaveNoteWithName()
        {
            ViewModel.CurrentName = "World";
            ViewModel.SaveCommand.Execute(null);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("World", note.Name);
        }

        [Fact]
        public void EditingTextThenSaving_ShouldSaveNoteWithText()
        {
            ViewModel.CurrentText = "Hello";
            ViewModel.SaveCommand.Execute(null);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("Hello", note.Text);
        }

        [Fact]
        public void ImmediatelyClosing_ShouldDiscardAndClose()
        {
            ViewModel.CloseCommand.Execute(null);
            Assert.Empty(Services.MockNoteDatabase.SaveNoteCalls);
            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }

        [Fact]
        public void EditingThenClosingWithChoiceYes_ShouldSaveAndClose()
        {
            Services.MockPopups.TwoOptionPopupResult = Notes.Services.TwoOptionPopupResult.LeftButton;
            ViewModel.CurrentName = "hi";
            ViewModel.CloseCommand.Execute(null);

            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("hi", note.Name);

            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }

        [Fact]
        public void EditingThenClosingWithChoiceNo_ShouldDiscardAndClose()
        {
            Services.MockPopups.TwoOptionPopupResult = Notes.Services.TwoOptionPopupResult.RightButton;
            ViewModel.CurrentName = "hi";
            ViewModel.CloseCommand.Execute(null);

            Assert.Empty(Services.MockNoteDatabase.SaveNoteCalls);

            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }
    }
}
