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
    /// Scenario: User opens new note, edits, then leaves the page by means other than closing such that the note is cached.
    /// Then the user re-opens the note page.
    /// </summary>
    public class CachedEditedNewNoteTests
    {
        MockAppServiceProvider Services;
        public CachedEditedNewNoteTests()
        {
            Services = new MockAppServiceProvider();
            Services.MockNoteDatabase.GetCachedNoteResult = new Note() { Text = "yes", Name = "flower" };
            ViewModel = new NotePageViewModel(Services);
            SetupParams = NotePageSetupParameters.NewNote(Guid.NewGuid());
            ViewModel.Setup(SetupParams);
        }

        NotePageSetupParameters SetupParams;
        NotePageViewModel ViewModel;

        [Fact]
        public void ImmediatelySaving_ShouldSaveNoteWithEdits()
        {
            ViewModel.SaveCommand.Execute(null);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("flower", note.Name);
            Assert.Equal("yes", note.Text);
        }

        [Fact]
        public void EditingNameThenSaving_ShouldSaveNoteWithNewNameAndCachedText()
        {
            ViewModel.CurrentName = "World";
            ViewModel.SaveCommand.Execute(null);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("World", note.Name);
            Assert.Equal("yes", note.Text);
        }

        [Fact]
        public void EditingTextThenSaving_ShouldSaveNoteWithNewTextAndCachedName()
        {
            ViewModel.CurrentText = "Hello";
            ViewModel.SaveCommand.Execute(null);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("Hello", note.Text);
            Assert.Equal("flower", note.Name);
        }

        [Fact]
        public void ImmediatelyClosingWithChoiceYes_ShouldSaveAndClose()
        {
            Services.MockPopups.TwoOptionPopupResult = Notes.Services.TwoOptionPopupResult.LeftButton;
            ViewModel.CloseCommand.Execute(null);

            Assert.Equal(1, Services.MockPopups.TwoOptionPopupCallCount);
            Assert.Single(Services.MockNoteDatabase.SaveNoteCalls);
            var note = Services.MockNoteDatabase.SaveNoteCalls.Dequeue();
            Assert.Equal("flower", note.Name);
            Assert.Equal("yes", note.Text);

            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }

        [Fact]
        public void ImmediatelyClosingWithChoiceNo_ShouldDiscardAndClose()
        {
            Services.MockPopups.TwoOptionPopupResult = Notes.Services.TwoOptionPopupResult.RightButton;
            ViewModel.CloseCommand.Execute(null);

            Assert.Equal(1, Services.MockPopups.TwoOptionPopupCallCount);
            Assert.Empty(Services.MockNoteDatabase.SaveNoteCalls);

            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }

        [Fact]
        public void EditingThenClosingWithChoiceYes_ShouldSaveAndClose()
        {
            Services.MockPopups.TwoOptionPopupResult = Notes.Services.TwoOptionPopupResult.LeftButton;
            ViewModel.CurrentName = "hi";
            ViewModel.CloseCommand.Execute(null);

            Assert.Equal(1, Services.MockPopups.TwoOptionPopupCallCount);
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

            Assert.Equal(1, Services.MockPopups.TwoOptionPopupCallCount);
            Assert.Empty(Services.MockNoteDatabase.SaveNoteCalls);
            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }
    }
}
