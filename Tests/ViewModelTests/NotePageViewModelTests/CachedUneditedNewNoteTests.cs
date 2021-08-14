using Notes.RouteUtil;
using Notes.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Mocks;
using Xunit;
using Notes.Models;

namespace Tests.ViewModelTests.NotePageViewModelTests
{
    /// <summary>
    /// Scenario: User opens new note then, without changing anything, leaves the page by means other than closing such that the note is cached.
    /// Then the user re-opens the note page.
    /// </summary>
    public class CachedUneditedNewNoteTests
    {
        MockAppServiceProvider Services;
        public CachedUneditedNewNoteTests()
        {
            Services = new MockAppServiceProvider();
            Services.MockNoteDatabase.GetCachedNoteResult = new Note();
            ViewModel = new NotePageViewModel(Services);
            SetupParams = NotePageSetupParameters.NewNote(Guid.NewGuid());
            ViewModel.Setup(SetupParams);
        }

        NotePageSetupParameters SetupParams;
        NotePageViewModel ViewModel;

        [Fact]
        public void ImmediatelySaving_ShouldSaveEmptyNote()
        {
            ViewModel.SaveCommand.Execute(null);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("", note.Name);
            Assert.Equal("", note.Text);
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

            Assert.Equal(1, Services.MockPopups.TwoOptionPopupCallCount);
            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }

        [Fact]
        public void EditingThenClosingWithChoiceNo_ShouldDiscardAndClose()
        {
            Services.MockPopups.TwoOptionPopupResult = Notes.Services.TwoOptionPopupResult.RightButton;
            ViewModel.CurrentName = "hi";
            ViewModel.CloseCommand.Execute(null);

            Assert.Equal(1, Services.MockPopups.TwoOptionPopupCallCount);
            Assert.Empty(Services.MockNoteDatabase.SaveNoteCalls);

            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }
    }
}
