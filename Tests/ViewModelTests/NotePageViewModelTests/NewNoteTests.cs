using Notes.RouteUtil;
using Notes.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Mocks;
using Xunit;

namespace Tests.ViewModelTests.NotePageViewModelTests
{
    /// <summary>
    /// Secnario: User opens new note from folder page
    /// </summary>
    public class NewNoteTests
    {
        MockAppServiceProvider Services;
        public NewNoteTests()
        {
            Services = new MockAppServiceProvider();
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
