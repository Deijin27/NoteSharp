using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Notes.Models;
using System.Linq;
using System.Text.RegularExpressions;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;

namespace Notes.Data
{

    public class NoteDatabase
    {
        readonly SQLiteAsyncConnection _database;

        /// <summary>
        /// Test whether the file is a valid database by opening and grabbing a Note, Folder, or CSS.
        /// Any errors encountered will be caught and result in a false return.
        /// Thus a backup file must already have the necessary tables to be accepted.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsValid(string path)
        {
            try
            {
                SQLiteConnection testFile = new SQLiteConnection(path);
                Note test = testFile.Table<Note>().FirstOrDefault();
                Folder test2 = testFile.Table<Folder>().FirstOrDefault();
                CSS test3 = testFile.Table<CSS>().FirstOrDefault();
                testFile.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"-++-+-+--+-++--++-+- ERROR: [{e.Message}] -+-+-+--++--++-+-+-+-+-+-+-+-");
                return false;
            }
            Console.WriteLine($"-++-+-+--+-++--++-+- it is valid according to my calculations -+-+-+--++--++-+-+-+-+-+-+-+-");
            return true;
        }

        public Task BackupAsync(string path)
        {
            return _database.BackupAsync(path);
        }

        //public async IEnumerable<Note> IterateAllNotes() // why is this not in 7.3 aaaa update xamarin aaa
        //{
        //    var noteTable = _database.Table<Note>();

        //    int number = await noteTable.CountAsync();

        //    for (int i = 0; i < number; i++)
        //    {
        //        yield return await noteTable.ElementAtAsync(i);
        //    }
        //}

        //public Task<Note> NoteAtIndexAsync(int index)
        //{
        //    return _database.Table<Note>().ElementAtAsync(index);
        //}

        //public Task<int> CountNotesAsync()
        //{
        //    return _database.Table<Note>().CountAsync();
        //}

        public NoteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
            _database.CreateTableAsync<Folder>().Wait();
            _database.CreateTableAsync<CSS>().Wait();
            _database.CreateTableAsync<DeletedItem>().Wait();
        }

        /// <summary>
        /// Closes any pooled connections used by the database.
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync()
        {
            return _database.CloseAsync();
        }

        /// <summary>
        /// WARNING, really does delete everything permanently INCLUDING CLEARING DeletedItem TABLE!
        /// </summary>
        public async Task DeleteAllAsync()
        {
            await _database.DeleteAllAsync<Note>();
            await _database.DeleteAllAsync<Folder>();
            await _database.DeleteAllAsync<CSS>();
            await _database.DeleteAllAsync<DeletedItem>();
        }

        #region Sorting Methods
        //public static AsyncTableQuery<Note> SortNotes(AsyncTableQuery<Note> query)
        //{
        //    SortingMode sortingMode = App.SortingMode;

        //    switch (sortingMode)
        //    {
        //        case SortingMode.Name:
        //            query = query.OrderBy(i => i.Name);
        //            break;
        //        case SortingMode.DateCreated:
        //            query = query.OrderByDescending(i => i.DateCreated);
        //            break;
        //        case SortingMode.DateModified:
        //            query = query.OrderByDescending(i => i.DateModified);
        //            break;
        //        default:
        //            query = query.OrderBy(i => i.Name);
        //            break;
        //    }
        //    return query;
        //}

        //public static AsyncTableQuery<Folder> SortFolders(AsyncTableQuery<Folder> query)
        //{
        //    SortingMode sortingMode = App.SortingMode;

        //    switch (sortingMode)
        //    {
        //        case SortingMode.Name:
        //            query = query.OrderBy(i => i.Name);
        //            break;
        //        case SortingMode.DateCreated:
        //            query = query.OrderByDescending(i => i.DateCreated);
        //            break;
        //        case SortingMode.DateModified:
        //            query = query.OrderByDescending(i => i.DateModified);
        //            break;
        //        default:
        //            query = query.OrderBy(i => i.Name);
        //            break;
        //    }
        //    return query;
        //}

        #endregion

        #region Get All Objects of Type In Folder

        public Task<List<Note>> GetNotesAsync(Guid folderID)
        {
            return _database.Table<Note>()
                            .Where(i => i.FolderID == folderID)
                            .ToListAsync();
        }

        public Task<List<Folder>> GetFoldersAsync(Guid folderID)
        {
            return _database.Table<Folder>()
                            .Where(i => i.ParentID == folderID)
                            .ToListAsync();
        }

        #endregion

        #region Get All Quick Access Objects of Type

        public Task<List<Note>> GetQuickAccessNotesAsync()
        {
            return _database.Table<Note>()
                                 .Where(i => i.IsQuickAccess == true)
                                 .ToListAsync();

        }

        public Task<List<Folder>> GetQuickAccessFoldersAsync()
        {
            return _database.Table<Folder>()
                            .Where(i => i.IsQuickAccess == true)
                            .ToListAsync();
        }

        #endregion

        #region Get Single Object By ID

        public Task<Folder> GetFolderAsync(Guid id)
        {
            return _database.Table<Folder>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<Note> GetNoteAsync(Guid id)
        {
            return _database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        #endregion

        #region Get Single Object By Name and Folder ID

        public Task<Note> GetNoteByNameAsync(Guid folderID, string name)
        {
            return _database.Table<Note>()
                            .Where(i => i.FolderID == folderID && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Folder> GetFolderByNameAsync(Guid parentID, string name)
        {
            return _database.Table<Folder>()
                            .Where(i => i.ParentID == parentID && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        #endregion

        #region Get Single Quick Access Object By Name

        public Task<Note> GetQuickAccessNoteByNameAsync(string name)
        {
            return _database.Table<Note>()
                            .Where(i => i.IsQuickAccess == true && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Folder> GetQuickAccessFolderByNameAsync(string name)
        {
            return _database.Table<Folder>()
                            .Where(i => i.IsQuickAccess == true && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        #endregion

        #region Save Object

        public Task<int> SaveAsync(Note note)
        {
            if (note.ID != Guid.Empty)
            {
                return _database.UpdateAsync(note);
            }
            else
            {
                note.ID = Guid.NewGuid();
                return _database.InsertAsync(note);
            }
        }

        public Task<int> SaveAsync(Folder folder)
        {
            if (folder.ID != Guid.Empty)
            {
                return _database.UpdateAsync(folder);
            }
            else
            {
                folder.ID = Guid.NewGuid();
                return _database.InsertAsync(folder);
            }
        }

        #endregion

        #region Insert Multiple

        public Task<int> InsertAllAsync(IEnumerable<Note> enumerable)
        {
            return _database.InsertAllAsync(enumerable);
        }
        public Task<int> InsertAllAsync(IEnumerable<Folder> enumerable)
        {
            return _database.InsertAllAsync(enumerable);
        }
        public Task<int> InsertAllAsync(IEnumerable<CSS> enumerable)
        {
            return _database.InsertAllAsync(enumerable);
        }
        
        #endregion

        #region Delete Object

        public async Task DeleteAsync(Note note)
        {
            await _database.InsertAsync(new DeletedItem() { ID = note.ID, DateDeleted = DateTime.UtcNow });

            await _database.DeleteAsync(note);

            return;
        }

        #endregion

        #region Check Name Exists of Type in Folder

        public async Task<bool> DoesNoteNameExistAsync(string name, Guid folderID, Guid id)
        {
            var query = _database.Table<Note>().Where(i => i.FolderID == folderID && i.Name == name);

            if (await query.CountAsync() > 0)
            {
                List<Note> notes = await query.ToListAsync();
                if (notes.Any(i => i.ID != id)) // ensure that the thing with the same name isn't same note
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DoesFolderNameExistAsync(string name, Guid parentID, Guid id)
        {
            var query = _database.Table<Folder>().Where(i => i.ParentID == parentID && i.Name == name);

            if (await query.CountAsync() > 0)
            {
                List<Folder> folders = await query.ToListAsync();
                if (folders.Any(i => i.ID != id)) // ensure that the thing with the same name isn't same note
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Check Name Exists of Type in Quick Access

        public async Task<bool> DoesQuickAccessFolderNameExistAsync(string name, Guid id)
        {
            var query = _database.Table<Folder>().Where(i => i.IsQuickAccess == true && i.Name == name);

            if (await query.CountAsync() > 0)
            {
                List<Folder> folders = await query.ToListAsync();
                if (folders.Any(i => i.ID != id)) // ensure that the thing with the same name isn't same note
                {
                    return true;
                }
            }
            return false;

        }

        public async Task<bool> DoesQuickAccessNoteNameExistAsync(string name, Guid id)
        {
            var query = _database.Table<Note>().Where(i => i.IsQuickAccess == true && i.Name == name);

            if (await query.CountAsync() > 0)
            {
                List<Note> notes = await query.ToListAsync();
                if (notes.Any(i => i.ID != id)) // ensure that the thing with the same name isn't same note
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DoesQuickAccessOrOtherwiseFolderNameExistAsync(string name, Guid parentID, Guid id)
        {
            var query = _database.Table<Folder>().Where(i => (i.IsQuickAccess == true || i.ParentID == parentID) && i.Name == name);

            if (await query.CountAsync() > 0)
            {
                List<Folder> folders = await query.ToListAsync();
                if (folders.Any(i => i.ID != id)) // ensure that the thing with the same name isn't same note
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DoesQuickAccessOrOtherwiseNoteNameExistAsync(string name, Guid folderID, Guid id)
        {
            var query = _database.Table<Note>().Where(i => (i.IsQuickAccess == true || i.FolderID == folderID) && i.Name == name);

            if (await query.CountAsync() > 0)
            {
                List<Note> notes = await query.ToListAsync();
                if (notes.Any(i => i.ID != id)) // ensure that the thing with the same name isn't same note
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region Style Sheet Methods

        public Task<List<CSS>> GetSheetsAsync()
        {
            return _database.Table<CSS>()
                            .OrderBy(i => i.Name)
                            .ToListAsync();
        }

        //public async Task<bool> DoesCSSNameExist(string name)
        //{
        //    int count = await _database.Table<CSS>()
        //                         .Where(i => i.Name == name)
        //                         .CountAsync();

        //    return count > 0;
        //}

        public Task<CSS> GetSheetAsync(Guid id)
        {
            return _database.Table<CSS>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveSheetAsync(CSS sheet)
        {
            if (sheet.ID != Guid.Empty)
            {
                return _database.UpdateAsync(sheet);
            }
            else
            {
                sheet.ID = Guid.NewGuid();
                return _database.InsertAsync(sheet);
            }
        }

        public async Task DeleteSheetAsync(CSS sheet)
        {
            await _database.InsertAsync(new DeletedItem() { ID = sheet.ID, DateDeleted = DateTime.UtcNow });

            await _database.DeleteAsync(sheet);

            return;
        }

        #endregion

        #region Get All Object of Type

        public Task<List<Note>> GetAllNotesAsync()
        {
            return _database.Table<Note>().ToListAsync();

        }

        public Task<List<Folder>> GetAllFoldersAsync()
        {
            return _database.Table<Folder>().ToListAsync();
        }

        public Task<List<CSS>> GetAllSheetsAsync()
        {
            return _database.Table<CSS>().ToListAsync();
        }

        #endregion

        public async Task DeleteFolderAndAllContentsAsync(Folder folder)
        {
            List<Folder> query = await _database.Table<Folder>().Where(i => i.ParentID == folder.ID).ToListAsync();

            foreach (Folder subfolder in query)
            {
                await DeleteFolderAndAllContentsAsync(subfolder);
            }

            // I do the following rather than use the AsyncTableQuery.DeleteAllAsync so that I can add the ids to the deleted item table
            List<Note> containedNotes = await _database.Table<Note>().Where(i => i.FolderID == folder.ID).ToListAsync();
            foreach (Note note in containedNotes)
            {
                await DeleteAsync(note); 
            }

            await DeleteFolderNotContentAsync(folder);

            return;
        }

        public async Task DeleteFolderNotContentAsync(Folder folder)
        {
            await _database.InsertAsync(new DeletedItem() { ID = folder.ID, DateDeleted = DateTime.UtcNow });

            await _database.DeleteAsync(folder);

            return;
        }

        #region Template Stuff

        private static Regex DiRegex = new Regex(@"(?s)(?<!\\)<di\s+((?<key>[^""\s]+?)|(""(?<key>[^""]*?)""))\s*((>(?<value>.*?)(?<!\\)</\s*di\s*>)|(\s*(""(?<value>[^""]*?)"")?\s*/>))");
        private static Regex TiRegex = new Regex(@"(?s)(?<!\\)<ti\s+""(?<path>[^>]*?)""\s*(?<datasets>[^>]*?)?/>");
        private static Regex DatasetPathRegex = new Regex(@"(?s)\s*""(?<dataset>.*?)""\s*");

        public Task<(string result, bool errorEncountered)> InterpolateAndInputTemplatesAsync(string text, Guid folderID)
        {
            // first interpolate anything in this string, inputting the default values specified
            text = InterpolateValues(text, new List<Dictionary<string, string>>());
            // then input all templates
            return InputTemplates(text, folderID, folderID);
        }

        private async Task<(Folder finalFolder, bool errorEncountered)> FollowFolderPath(Folder currentFolder, IEnumerable<string> folderNameSequence)
        {
            foreach (string folderName in folderNameSequence)
            {
                
                if (folderName[0] == '*') // look for in quick access
                {
                    currentFolder = await GetQuickAccessFolderByNameAsync(folderName.Remove(0, 1));

                    if (currentFolder == null)
                    {
                        await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                        (
                            AppResources.Alert_TemplateError_Title, 
                            string.Format(AppResources.Alert_QuickAccessFolderNotFound_Message, currentFolder.Name), 
                            AppResources.AlertOption_OK
                        ));
                        return (null, true);
                    }
                }
                else if (folderName == "..") // step up a folder
                {
                    if (currentFolder.ID == Guid.Empty) // current folder is root folder
                    {
                        await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                        (
                            AppResources.Alert_TemplateError_Title, 
                            AppResources.Alert_RootFolderHasNoParent_Message,
                            AppResources.AlertOption_OK
                        ));
                        return (null, true);
                    }
                    // current folder is not root folder
                    if (currentFolder.ParentID == Guid.Empty) // parent folder is root folder
                    {
                        currentFolder = new Folder() { ID = Guid.Empty, Name = AppResources.PageTitle_RootFolder };
                    }
                    else // parent folder is not root folder, i.e. it exists in database
                    {
                        currentFolder = await GetFolderAsync(currentFolder.ParentID);
                    }
                }
                else if (folderName == ".") // stay same folder
                {
                    // do nothing
                }
                else // look for folder in current folder
                {
                    Folder newFolder = await GetFolderByNameAsync(currentFolder.ID, folderName);

                    if (newFolder == null)
                    {
                        await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                        (
                            AppResources.Alert_TemplateError_Title,
                            string.Format(AppResources.Alert_FolderNotFoundInFolder_Message, folderName, currentFolder.Name),
                            AppResources.AlertOption_OK
                        ));
                        return (null, true);
                    }

                    currentFolder = newFolder;
                }
            }
            return (currentFolder, false);
        }

        private async Task<(Note note, bool errorEncountered)> GetNoteByPath(string path, Guid folderID, Guid mainFolderID)
        {
            switch (path[0])
            {
                case '/': // start in root folder
                    {
                        folderID = Guid.Empty;
                        path = path.Remove(0, 1);
                        break;
                    }
                case '~': // relative path, but relative to the folder of the file you press the preview button in
                    {
                        folderID = mainFolderID;
                        path = path.Remove(0, 1);
                        break;
                    }
            }

            string[] pathSplit = path.Split('/');

            Folder startFolder;
            if (folderID == Guid.Empty) 
                startFolder = new Folder() { ID = Guid.Empty, Name = AppResources.PageTitle_RootFolder };
            else 
                startFolder = await GetFolderAsync(folderID);

            IEnumerable<string> folderNameSequence = pathSplit.Take(pathSplit.Length - 1);

            (Folder endFolder, bool errorEncountered) = await FollowFolderPath(startFolder, folderNameSequence);
            if (errorEncountered) return (null, errorEncountered);

            string noteName = pathSplit.Last();

            Note noteFile;

            if (noteName[0] == '*')
            {
                noteName = noteName.Remove(0, 1);
                noteFile = await GetQuickAccessNoteByNameAsync(noteName);

                if (noteFile == null)
                {
                    await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                    (
                        AppResources.Alert_TemplateError_Title,
                        string.Format(AppResources.Alert_QuickAccessNoteNotFound_Message, noteName),
                        AppResources.AlertOption_OK
                    ));
                    return (null, true);
                }
            }
            else 
            { 
                noteFile = await GetNoteByNameAsync(endFolder.ID, noteName);

                if (noteFile == null)
                {
                    await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                    (
                        AppResources.Alert_TemplateError_Title,
                        string.Format(AppResources.Alert_NoteNotFoundInFolder_Message, noteName, endFolder.Name),
                        AppResources.AlertOption_OK
                    ));
                    return (null, true);
                }
            }

            return (noteFile, false);
        }

        private Dictionary<string, string> LoadDatasetString(string datasetString)
        {

            var dataset = new Dictionary<string, string>();

            foreach (Match match in DiRegex.Matches(datasetString))
            {
                string key = match.Groups["key"].Value.Trim();
                string value = match.Groups["value"].Value;

                dataset[key] = value;
            }

            return dataset;
        }

        private async Task<(string result, bool errorEncountered)> InputTemplates(string text, Guid folderID, Guid mainFolderID)
        { 
            string template;
            string templatePath;
            bool errorEncountered;
            Note datasetFile;
            Note templateFile;
            List<Dictionary<string, string>> datasets;

            foreach (Match match in TiRegex.Matches(text))
            {
                templatePath = match.Groups["path"].Value;

                IEnumerable<string> datasetPaths = DatasetPathRegex.Matches(match.Groups["datasets"].Value).OfType<Match>().Select(i => i.Groups["dataset"].Value);

                (templateFile, errorEncountered) = await GetNoteByPath(templatePath, folderID, mainFolderID);
                if (errorEncountered) return (null, errorEncountered);

                template = templateFile.Text;

                datasets = new List<Dictionary<string, string>>();
                foreach (string datasetPath in datasetPaths)
                {
                    (datasetFile, errorEncountered) = await GetNoteByPath(datasetPath, folderID, mainFolderID);
                    if (errorEncountered) return (null, errorEncountered);

                    datasets.Add(LoadDatasetString(datasetFile.Text));
                }

                template = InterpolateValues(template, datasets);

                (template, errorEncountered) = await InputTemplates(template, templateFile.FolderID, mainFolderID);
                if (errorEncountered) return (null, errorEncountered);

                text = text.Replace(match.Value, template);
            }

            return (text, false);
        }

        private static string InterpolateValues(string template, List<Dictionary<string, string>> datasets)
        {
            foreach (Match match in DiRegex.Matches(template))
            {
                string replacement = "";

                bool keyFound = false;
                string key = match.Groups["key"].Value;
                // take the value from the first dataset with a match
                foreach (var dataset in datasets)
                {
                    if (dataset.ContainsKey(key))
                    {
                        replacement = dataset[key];
                        keyFound = true;
                        break;
                    }
                }

                if (!keyFound)
                {
                    replacement = match.Groups["value"].Value;
                }

                template = template.Replace(match.Value, replacement);
            }

            return template;
        }

        #endregion
    }
}
