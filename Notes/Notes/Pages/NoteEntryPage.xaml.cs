using System;
using Xamarin.Forms;
using Notes.Models;
using System.Threading.Tasks;
using Notes.Data;
using Notes.Resources;
using Notes.PopupPages;
using Rg.Plugins.Popup.Services;

namespace Notes.Pages
{
    public delegate void ChangesSavedEventHandler();

    public partial class NoteEntryPage : ContentPage
    {
        public bool NewNote = false;
        private string InitialText;
        private string InitialName;

        public Note CurrentNote { get; set; }

        // The following property wrapper needs to exist so that it can be updated visually when it is changed inside
        // the save button dialog.
        public string CurrentNoteName { get => CurrentNote.Name; set => CurrentNote.Name = value; }

        public bool UnsavedChangesExist { get; set; }


        /// <summary>
        /// Use when loading an existing note
        /// </summary>
        /// <param name="note"></param>
        public NoteEntryPage(Note note)
        {
            InitialText = note.Text;
            InitialName = note.Name;

            InitializeComponent();
            ApplySettings();

            //NoteStorage = note;

            CurrentNote = note;
            BindingContext = this;
            UnsavedChangesExist = false;
        }

        public void UnfocusAll() 
        {
            // used to close keyboard in situations where it would stay open chen closing page
            // e.g. text is selected (no edits have been made) and close button is clicked.

            MainEditor.Unfocus();
            NameEntry.Unfocus();
        }

        /// <summary>
        /// Use for creating a new note
        /// </summary>
        /// <param name="folderID"></param>
        /// <param name="previousPage"></param>
        public NoteEntryPage(Guid folderID)
        {

            Note note = new Note()
            {
                FolderID = folderID 
            };

            InitialText = note.Text;
            InitialName = note.Name;
            NewNote = true;

            InitializeComponent();
            ApplySettings();

            CurrentNote = note;
            BindingContext = this;
            UnsavedChangesExist = false;
        }

        public void ApplySettings()
        {
            bool spellcheck = App.IsSpellCheckEnabled;
            MainEditor.IsSpellCheckEnabled = spellcheck;
            MainEditor.IsTextPredictionEnabled = spellcheck;
        }

        #region Close Clicked

        async void Close_Clicked(object sender, EventArgs e)
        {
            Note note = CurrentNote;

            if (UnsavedChangesExist)
            {
                var popup = new TwoOptionPopupPage
                (
                    "Save Changes?",
                    "Unsaved changes exist, would you like to save them?",
                    "Yes",
                    "No"
                );
                popup.BackgroundClicked += CancelClose;
                popup.HardwareBackClicked += CancelClose;
                popup.LeftOptionClicked += SaveThenClose;
                popup.RightOptionClicked += ProceedWithClose;
                await PopupNavigation.Instance.PushAsync(popup);
            }
            else
            {
                UnfocusAll();
                await Navigation.PopModalAsync();
            }
        }

        async void CancelClose() { await PopupNavigation.Instance.PopAsync(); }

        async void ProceedWithClose()
        {
            
            CurrentNote.Name = InitialName;
            CurrentNote.Text = InitialText;
            UnfocusAll();
            await PopupNavigation.Instance.PopAsync();
            await Navigation.PopModalAsync();
        }

        async void SaveThenClose()
        {
            await PopupNavigation.Instance.PopAsync();
            CloseAfter = true;
            Save();
        }

        #endregion

        #region Save Clicked
        void Save_Clicked(object sender, EventArgs e)
        {
            if (UnsavedChangesExist)
            {
                Save();
            }
        }

        async void Save()
        {
            Note note = CurrentNote;
            if (NewNote)
            {
                if (note.Name != InitialName)
                {

                    if (await App.Database.DoesNoteNameExistAsync(note.Name, note.FolderID, note.ID))
                    {
                        var popup = new UniqueNamePromptPopupPage
                        (
                            "Note name conflict",
                            "Input a different name for the note",
                            "OK",
                            "Cancel",
                            note.FolderID,
                            note.ID,
                            App.Database.DoesNoteNameExistAsync,
                            "Name invalid, it contains one of" + " \"/*.~",
                            "Name conflict encountered; try a different name",
                            "",
                            "Input note name..."
                        );
                        popup.AcceptOptionClicked += ProceedRenameAndSaveNote;
                        popup.CancelOptionClicked += CancelSaveNote;
                        popup.BackgroundClicked += CancelSaveNote;
                        popup.HardwareBackClicked += CancelSaveNote;

                        await PopupNavigation.Instance.PushAsync(popup);
                    }
                    else
                    {
                        note.DateModified = DateTime.UtcNow;
                        note.DateCreated = note.DateModified;
                        await App.Database.SaveAsync(note);
                        ChangesSaved?.Invoke();
                        if (CloseAfter)
                        {
                            await Navigation.PopModalAsync();
                        }
                        else
                        {
                            PostSaveUpdateWithoutClose();
                        }
                    }
                }
                else
                {
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Note name",
                        "Input a name for the note",
                        "OK",
                        "Cancel",
                        note.FolderID,
                        note.ID,
                        App.Database.DoesNoteNameExistAsync,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        "",
                        "Input note name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameAndSaveNote;
                    popup.CancelOptionClicked += CancelSaveNote;
                    popup.BackgroundClicked += CancelSaveNote;
                    popup.HardwareBackClicked += CancelSaveNote;

                    await PopupNavigation.Instance.PushAsync(popup);
                }

            }
            else
            {
                UniqueNamePromptPopupPage.DoesNameExist namechecker;
                if (note.IsQuickAccess) namechecker = App.Database.DoesQuickAccessOrOtherwiseNoteNameExistAsync;
                else namechecker = App.Database.DoesNoteNameExistAsync;

                if (note.Name != InitialName && (await namechecker.Invoke(note.Name, note.FolderID, note.ID)))
                {
                    var popup = new UniqueNamePromptPopupPage
                    (
                        "Note name conflict",
                        "Input a different name for the note",
                        "OK",
                        "Cancel",
                        note.FolderID,
                        note.ID,
                        namechecker,
                        "Name invalid, it contains one of" + " \"/*.~",
                        "Name conflict encountered; try a different name",
                        "",
                        "Input note name..."
                    );
                    popup.AcceptOptionClicked += ProceedRenameAndSaveNote;
                    popup.CancelOptionClicked += CancelSaveNote;
                    popup.BackgroundClicked += CancelSaveNote;
                    popup.HardwareBackClicked += CancelSaveNote;

                    await PopupNavigation.Instance.PushAsync(popup);
                }
                else
                {
                    note.DateModified = DateTime.UtcNow;
                    await App.Database.SaveAsync(note);
                    ChangesSaved?.Invoke();
                    if (CloseAfter)
                    {
                        await Navigation.PopModalAsync();
                    }
                    else
                    {
                        PostSaveUpdateWithoutClose();
                    }
                }
            }
        }

        bool CloseAfter = false;
        public async void ProceedRenameAndSaveNote(PromptPopupOptionEventArgs e)
        {
            CurrentNote.Name = e.Text;
            CurrentNote.DateModified = DateTime.UtcNow;
            CurrentNote.DateCreated = CurrentNote.DateModified;
            await PopupNavigation.Instance.PopAsync();
            await App.Database.SaveAsync(CurrentNote);
            ChangesSaved?.Invoke();

            if (CloseAfter)
            {
                await Navigation.PopModalAsync();
            }
            else
            {
                PostSaveUpdateWithoutClose();
            }
        }

        void PostSaveUpdateWithoutClose()
        {
            InitialName = CurrentNote.Name;
            InitialText = CurrentNote.Text;
            OnPropertyChanged("CurrentNoteName");
            UnsavedChangesExist = false; // this must happen after property changed notification, because text changed event
                                         // is triggered by it the update. And within the text changed this is set back to true.
            NewNote = false; // No Longer a new note after it has been saved, avoids weird behaviour of asking you to rename it when it's already named
        }

        public async void CancelSaveNote(PromptPopupOptionEventArgs e)
        {
            await PopupNavigation.Instance.PopAsync();
        }

        #endregion

        public event ChangesSavedEventHandler ChangesSaved;

        async void HtmlPreview_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new HtmlPreviewPage(CurrentNote.Text, CurrentNote.FolderID));
        }

        async void MarkdownPreview_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MarkdownPreviewPage(CurrentNote.Text, CurrentNote.FolderID));
        }

        async void MarkdownView_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MarkdownViewPage(CurrentNote.Text, CurrentNote.FolderID));
        }

        private void NameEntry_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnsavedChangesExist = true;
        }

        private void MainEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            UnsavedChangesExist = true;
        }
    }
}
 