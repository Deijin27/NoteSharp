using Notes.Data;
using Notes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Notes.Pages
{

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DatasetEntryPage : ContentPage
    {
        public bool NewDataset = false;
        public int FolderID = 0;

        public DatasetEntryPage()
        {
            InitializeComponent();
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            Dataset dataset = (Dataset)BindingContext;
            dataset.FolderID = FolderID;

            if (NewDataset)
            {
                (Option option, string result) = await NameValidation.GetUniqueDatasetName(this, dataset.FolderID, "Name Dataset");

                if (option == Option.OK)
                {
                    dataset.Name = result;
                    dataset.DateModified = DateTime.UtcNow;
                    dataset.DateCreated = dataset.DateModified;
                    await App.Database.SaveDatasetAsync(dataset);
                    await Navigation.PopAsync();
                }

            }
            else
            {
                dataset.DateModified = DateTime.UtcNow;
                await App.Database.SaveDatasetAsync(dataset);
                await Navigation.PopAsync();
            }
        }



        async void OnSettingsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }
    }
}