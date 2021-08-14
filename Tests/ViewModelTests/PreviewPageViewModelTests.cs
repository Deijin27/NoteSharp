using System;
using Tests.Mocks;
using Notes.ViewModels;
using Notes.Models;
using Notes.RouteUtil;
using Xunit;

namespace Tests.ViewModelTests
{
    public class PreviewPageViewModelTests
    {
        MockAppServiceProvider Services;
        public PreviewPageViewModelTests()
        {
            Services = new MockAppServiceProvider();
            ViewModel = new PreviewPageViewModel(Services);
        }

        PreviewPageViewModel ViewModel;

        [Fact]
        public void CopyCommand_ShouldCopyToClipboard()
        {
            ViewModel.EditorText = "something";
            ViewModel.CopyCommand.Execute(null);

            Assert.Single(Services.MockClipboard.SetTextCalls);
            Assert.Equal("something", Services.MockClipboard.SetTextCalls.Dequeue());
        }

        [Fact]
        public void OnMarkdownBuildError_ShouldNavigateBack()
        {
            Services.MockNoteDatabase.GetCachedNoteResult = new Note() { Text = "some text" };
            Services.MockMarkdownBuilder.BuildMarkdownResult = "other text";
            Services.MockMarkdownBuilder.BuildMarkdownErrorEncountered = true;

            var setupParams = new PreviewPageSetupParameters(Guid.NewGuid(), PreviewPageMode.Markdown);
            ViewModel.Setup(setupParams);

            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }

        [Fact]
        public void IfMarkdownMode_ShouldDisplayMarkdown()
        {
            Services.MockNoteDatabase.GetCachedNoteResult = new Note() { Text = "some text" };
            Services.MockMarkdownBuilder.BuildMarkdownResult = "other text";
            Services.MockMarkdownBuilder.BuildMarkdownErrorEncountered = false;

            var setupParams = new PreviewPageSetupParameters(Guid.NewGuid(), PreviewPageMode.Markdown);
            ViewModel.Setup(setupParams);

            Assert.Equal("other text", ViewModel.EditorText);
        }

        [Fact]
        public void IfHtmlMode_ShouldDisplayHtml()
        {
            Services.MockNoteDatabase.GetCachedNoteResult = new Note() { Text = "some text" };
            Services.MockMarkdownBuilder.BuildMarkdownResult = "other text";
            Services.MockMarkdownBuilder.BuildMarkdownErrorEncountered = false;
            Services.MockMarkdownBuilder.BuildHtmlResult = "html text";

            var setupParams = new PreviewPageSetupParameters(Guid.NewGuid(), PreviewPageMode.Html);
            ViewModel.Setup(setupParams);

            Assert.Equal("html text", ViewModel.EditorText);
        }

    }
}
