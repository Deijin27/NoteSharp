using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Notes.Models;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Collections;
using SQLitePCL;
using System.Runtime.InteropServices.ComTypes;
using Xamarin.Forms;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Net;
using Notes.Pages;
using System.Dynamic;
using System.ComponentModel.Design;

namespace Notes.Data
{
    public enum ErrorEncountered
    {
        False,
        True
    }

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

        public NoteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
            _database.CreateTableAsync<Folder>().Wait();
            _database.CreateTableAsync<CSS>().Wait();
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
        /// WARNING, really does delete everything permanently.
        /// </summary>
        public async Task DeleteAllAsync()
        {
            await _database.DeleteAllAsync<Note>();
            await _database.DeleteAllAsync<Folder>();
            await _database.DeleteAllAsync<CSS>();
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

        public Task<int> DeleteAsync(Note note)
        {
            return _database.DeleteAsync(note);
        }

        #endregion

        #region Check Name Exists of Type in Folder

        public async Task<bool> DoesNoteNameExistAsync(string name, Guid folderID)
        {
            int count = await _database.Table<Note>()
                                       .Where(i => i.FolderID == folderID && i.Name == name)
                                       .CountAsync();
            return count > 0;
        }

        public async Task<bool> DoesFolderNameExistAsync(string name, Guid parentID)
        {
            int count = await _database.Table<Folder>()
                                       .Where(i => i.ParentID == parentID && i.Name == name)
                                       .CountAsync();
            return count > 0;
        }

        #endregion

        #region Check Name Exists of Type in Quick Access

        public async Task<bool> DoesQuickAccessFolderNameExistAsync(string name)
        {
            int count = await _database.Table<Folder>()
                                       .Where(i => i.IsQuickAccess == true && i.Name == name)
                                       .CountAsync();
            return count > 0;
        }

        public async Task<bool> DoesQuickAccessNoteNameExistAsync(string name)
        {
            int count = await _database.Table<Note>()
                                       .Where(i => i.IsQuickAccess == true && i.Name == name)
                                       .CountAsync();
            return count > 0;
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

        public Task<int> DeleteSheetAsync(CSS sheet)
        {
            return _database.DeleteAsync(sheet);
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

        public async Task<int> DeleteFolderAndAllContentsAsync(Folder folder)
        {
            List<Folder> query = await _database.Table<Folder>().Where(i => i.ParentID == folder.ID).ToListAsync();

            foreach (Folder subfolder in query)
            {
                await DeleteFolderAndAllContentsAsync(subfolder);
            }

            await _database.Table<Note>().Where(i => i.FolderID == folder.ID).DeleteAsync();
            await _database.DeleteAsync(folder);

            return default;
        }

        #region Template Stuff

        private static Regex DiRegex = new Regex(@"(?<!\\)<di\s+((?<key>[^""\s]+?)|(""(?<key>[^""]*?)""))\s*((>(?<value>.*?)(?<!\\)</\s*di\s*>)|(\s*(""(?<value>[^""]*?)"")?\s*/>))");
        private static Regex TiRegex = new Regex(@"(?<!\\)<ti\s+""(?<path>[^>]*?)""\s*(?<datasets>[^>]*?)?/>");
        private static Regex DatasetPathRegex = new Regex(@"\s*""(?<dataset>.*?)""\s*");

        public Task<(string, ErrorEncountered)> InterpolateAndInputTemplatesAsync(string text, Page currentPage, Guid folderID)
        {
            // first interpolate anything in this string, inputting the default values specified
            text = InterpolateValues(text, new List<Dictionary<string, string>>());
            // then input all templates
            return InputTemplates(text, currentPage, folderID, folderID);
        }

        private async Task<(Folder, ErrorEncountered)> FollowFolderPath(Folder currentFolder, IEnumerable<string> folderNameSequence, Page currentPage)
        {
            foreach (string folderName in folderNameSequence)
            {
                
                if (folderName[0] == '*') // look for in quick access
                {
                    currentFolder = await GetQuickAccessFolderByNameAsync(folderName.Remove(0, 1));

                    if (currentFolder == null)
                    {
                        await currentPage.DisplayAlert("Template Error", $"Quick Access Folder \"{currentFolder.Name}\" not found.", "OK");
                        return (null, ErrorEncountered.True);
                    }
                }
                else if (folderName == "..") // step up a folder
                {
                    if (currentFolder.ID == Guid.Empty) // current folder is root folder
                    {
                        await currentPage.DisplayAlert("Template Error", "Root folder does not have a parent folder.", "OK");
                        return (null, ErrorEncountered.True);
                    }
                    // current folder is not root folder
                    if (currentFolder.ParentID == Guid.Empty) // parent folder is root folder
                    {
                        currentFolder = new Folder() { ID = Guid.Empty };
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
                        await currentPage.DisplayAlert("Template Error", $"Folder \"{folderName}\" not found in folder \"{currentFolder.Name}\"", "OK");
                        return (null, ErrorEncountered.True);
                    }

                    currentFolder = newFolder;
                }
            }
            return (currentFolder, ErrorEncountered.False);
        }

        private async Task<(Note, ErrorEncountered)> GetNoteByPath(string path, Page currentPage, Guid folderID, Guid mainFolderID)
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
                startFolder = new Folder() { ID = Guid.Empty };
            else 
                startFolder = await GetFolderAsync(folderID);

            IEnumerable<string> folderNameSequence = pathSplit.Take(pathSplit.Length - 1);

            Folder endFolder;
            
            ErrorEncountered errorEncountered;
            (endFolder, errorEncountered) = await FollowFolderPath(startFolder, folderNameSequence, currentPage);
            if (errorEncountered == ErrorEncountered.True)
                return (null, errorEncountered);

            string noteName = pathSplit.Last();

            Note noteFile;

            if (noteName[0] == '*')
            {
                noteName = noteName.Remove(0, 1);
                noteFile = await GetQuickAccessNoteByNameAsync(noteName);

                if (noteFile == null)
                {
                    await currentPage.DisplayAlert("Template Error", $"Quick Access Note \"{noteName}\" not found", "OK");
                    return (null, ErrorEncountered.True);
                }
            }
            else 
            { 
                noteFile = await GetNoteByNameAsync(endFolder.ID, noteName);

                if (noteFile == null)
                {
                    await currentPage.DisplayAlert("Template Error", $"Note \"{noteName}\" not found in folder \"{endFolder.Name}\"", "OK");
                    return (null, ErrorEncountered.True);
                }
            }

            return (noteFile, ErrorEncountered.False);
        }

        private Dictionary<string, string> LoadDatasetString(string datasetString)
        {

            var dataset = new Dictionary<string, string>();

            foreach (Match match in DiRegex.Matches(datasetString))
            {
                string value = match.Groups["value"].Value;

                dataset.Add(match.Groups["key"].Value.Trim(), value);
            }

            return dataset;
        }

        private async Task<(string, ErrorEncountered)> InputTemplates(string text, Page currentPage, Guid folderID, Guid mainFolderID)
        { 
            string template;
            string templatePath;
            ErrorEncountered errorEncountered;
            Note datasetFile;
            Note templateFile;
            List<Dictionary<string, string>> datasets;

            foreach (Match match in TiRegex.Matches(text))
            {
                templatePath = match.Groups["path"].Value;

                var datasetPaths = new List<string>();

                foreach (Match dmatch in DatasetPathRegex.Matches(match.Groups["datasets"].Value))
                {
                    datasetPaths.Add(dmatch.Groups["dataset"].Value);
                }

                (templateFile, errorEncountered) = await GetNoteByPath(templatePath, currentPage, folderID, mainFolderID);
                if (errorEncountered == ErrorEncountered.True) 
                    return (null, ErrorEncountered.True);
                template = templateFile.Text;

                datasets = new List<Dictionary<string, string>>();
                foreach (string datasetPath in datasetPaths)
                {
                    (datasetFile, errorEncountered) = await GetNoteByPath(datasetPath, currentPage, folderID, mainFolderID);
                    if (errorEncountered == ErrorEncountered.True)
                        return (null, ErrorEncountered.True);
                    datasets.Add(LoadDatasetString(datasetFile.Text));
                }

                template = InterpolateValues(template, datasets);

                (template, errorEncountered) = await InputTemplates(template, currentPage, templateFile.FolderID, mainFolderID);
                if (errorEncountered == ErrorEncountered.True)
                    return (null, ErrorEncountered.True);

                text = text.Replace(match.Value, template);
            }

            return (text, ErrorEncountered.False);
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
