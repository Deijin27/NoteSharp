using System;
using System.Collections.Generic;
using Xunit;
using Notes.ViewModels;
using Notes.Models;
using Tests.Mocks;

namespace Tests.ViewModelTests
{
    public class StyleSheetSelectionPageViewModelTests
    {
        MockAppServiceProvider Services;
        public StyleSheetSelectionPageViewModelTests()
        {
            Services = new MockAppServiceProvider();
            Services.MockStyleSheets.GetAllStyleSheetsReturnValue = new List<CSS>() { new CSS() };
            ViewModel = new CssSelectionPageViewModel(Services);
            ViewModel.ApplyQueryAttributes(new Dictionary<string,string>());
        }

        CssSelectionPageViewModel ViewModel;

        [Fact]
        public void AfterInitialisation_ListViewItemsShouldBePopulated()
        {
            Assert.Same(Services.MockStyleSheets.GetAllStyleSheetsReturnValue, ViewModel.ListViewItems);
            Assert.Null(ViewModel.ListViewSelectedItem);
        }

        [Fact]
        public void AfterSelection_PreferencesShouldBeSet()
        {
            var css = new CSS() { ID = Guid.NewGuid() };
            ViewModel.ListViewSelectedItem = css;
            Assert.Equal(css.ID, Services.MockPreferences.StyleSheetID);
        }

        [Fact]
        public void AfterSelection_ShouldNavigateBack()
        {
            var css = new CSS() { ID = Guid.NewGuid() };
            ViewModel.ListViewSelectedItem = css;
            Assert.Equal(1, Services.MockNavigation.GoBackCallCount);
        }
    }
}
