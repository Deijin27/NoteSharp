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

        public NoteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
            _database.CreateTableAsync<Folder>().Wait();
            _database.CreateTableAsync<CSS>().Wait();
            _database.CreateTableAsync<Dataset>().Wait();
        }

        public static AsyncTableQuery<Note> SortNotes(AsyncTableQuery<Note> query)
        {
            SortingMode sortingMode = App.SortingMode;

            switch (sortingMode)
            {
                case SortingMode.Name:
                    query = query.OrderBy(i => i.Name);
                    break;
                case SortingMode.DateCreated:
                    query = query.OrderByDescending(i => i.DateCreated);
                    break;
                case SortingMode.DateModified:
                    query = query.OrderByDescending(i => i.DateModified);
                    break;
                default:
                    query = query.OrderBy(i => i.Name);
                    break;
            }
            return query;
        }

        public static AsyncTableQuery<Dataset> SortDatasets(AsyncTableQuery<Dataset> query)
        {
            SortingMode sortingMode = App.SortingMode;

            switch (sortingMode)
            {
                case SortingMode.Name:
                    query = query.OrderBy(i => i.Name);
                    break;
                case SortingMode.DateCreated:
                    query = query.OrderByDescending(i => i.DateCreated);
                    break;
                case SortingMode.DateModified:
                    query = query.OrderByDescending(i => i.DateModified);
                    break;
                default:
                    query = query.OrderBy(i => i.Name);
                    break;
            }
            return query;
        }

        public static AsyncTableQuery<Folder> SortFolders(AsyncTableQuery<Folder> query)
        {
            SortingMode sortingMode = App.SortingMode;

            switch (sortingMode)
            {
                case SortingMode.Name:
                    query = query.OrderBy(i => i.Name);
                    break;
                case SortingMode.DateCreated:
                    query = query.OrderByDescending(i => i.DateCreated);
                    break;
                case SortingMode.DateModified:
                    query = query.OrderByDescending(i => i.DateModified);
                    break;
                default:
                    query = query.OrderBy(i => i.Name);
                    break;
            }
            return query;
        }

        public Task<List<Note>> GetNotesAsync(int folderID)
        {
            var query = _database.Table<Note>()
                                 .Where(i => i.FolderID == folderID);

            query = SortNotes(query);

            return query.ToListAsync();

        }

        public Task<List<Dataset>> GetDatasetsAsync(int folderID)
        {
            var query = _database.Table<Dataset>()
                                 .Where(i => i.FolderID == folderID);

            query = SortDatasets(query);

            return query.ToListAsync();
        }

        public Task<List<Note>> GetQuickAccessNotesAsync()
        {
            var query = _database.Table<Note>()
                                 .Where(i => i.IsQuickAccess == true);

            query = SortNotes(query);

            return query.ToListAsync();

        }

        public Task<List<Dataset>> GetQuickAccessDatasetsAsync()
        {
            var query = _database.Table<Dataset>()
                                 .Where(i => i.IsQuickAccess == true);

            query = SortDatasets(query);

            return query.ToListAsync();
        }

        public async Task<bool> DoesCSSNameExist(string name)
        {
            int count = await _database.Table<CSS>()
                                 .Where(i => i.Name == name)
                                 .CountAsync();

            return count > 0;
        }

        public Task<List<CSS>> GetSheetsAsync()
        {
            return _database.Table<CSS>()
                            .OrderBy(i => i.Name)
                            .ToListAsync();
        }

        public Task<List<Folder>> GetFoldersAsync(int folderID)
        {
            var query = _database.Table<Folder>()
                            .Where(i => i.ParentID == folderID);

            query = SortFolders(query);

            return query.ToListAsync();
        }

        public Task<List<Folder>> GetQuickAccessFoldersAsync()
        {
            var query = _database.Table<Folder>()
                            .Where(i => i.IsQuickAccess == true);

            query = SortFolders(query);

            return query.ToListAsync();
        }

        public Task<Folder> GetFolderAsync(int id)
        {
            return _database.Table<Folder>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<Note> GetNoteAsync(int id)
        {
            return _database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public async Task<bool> DoesNoteNameExistAsync(string name, int folderID)
        {
            int count = await _database.Table<Note>()
                                       .Where(i => i.FolderID == folderID && i.Name == name)
                                       .CountAsync();

            return count > 0;
        }

        public async Task<bool> DoesDatasetNameExistAsync(string name, int folderID)
        {
            int count = await _database.Table<Dataset>()
                                       .Where(i => i.FolderID == folderID && i.Name == name)
                                       .CountAsync();

            return count > 0;
        }

        public async Task<bool> DoesFolderNameExistAsync(string name, int parentID)
        {
            int count = await _database.Table<Folder>()
                                       .Where(i => i.ParentID == parentID && i.Name == name)
                                       .CountAsync();

            return count > 0;
        }

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

        public async Task<bool> DoesQuickAccessDatasetNameExistAsync(string name)
        {
            int count = await _database.Table<Dataset>()
                                       .Where(i => i.IsQuickAccess == true && i.Name == name)
                                       .CountAsync();
            return count > 0;
        }

        public Task<CSS> GetSheetAsync(int id)
        {
            return _database.Table<CSS>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.ID != 0)
            {
                return _database.UpdateAsync(note);
            }
            else
            {
                return _database.InsertAsync(note);
            }
        }

        public Task<int> SaveDatasetAsync(Dataset dataset)
        {
            if (dataset.ID != 0)
            {
                return _database.UpdateAsync(dataset);
            }
            else
            {
                return _database.InsertAsync(dataset);
            }
        }

        public Task<int> SaveFolderAsync(Folder folder)
        {
            if (folder.ID != 0)
            {
                return _database.UpdateAsync(folder);
            }
            else
            {
                return _database.InsertAsync(folder);
            }
        }

        public Task<int> SaveSheetAsync(CSS sheet)
        {
            if (sheet.ID != 0)
            {
                return _database.UpdateAsync(sheet);
            }
            else
            {
                return _database.InsertAsync(sheet);
            }
        }

        public Task<int> DeleteSheetAsync(CSS sheet)
        {
            return _database.DeleteAsync(sheet);
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            return _database.DeleteAsync(note);
        }

        public Task<int> DeleteDatasetAsync(Dataset dataset)
        {
            return _database.DeleteAsync(dataset);
        }

        public async Task<int> DeleteFolderAndAllContentsAsync(Folder folder)
        {
            List<Folder> query = await _database.Table<Folder>().Where(i => i.ParentID == folder.ID).ToListAsync();

            foreach (Folder subfolder in query)
            {
                await DeleteFolderAndAllContentsAsync(subfolder);
            }

            await _database.Table<Note>().Where(i => i.FolderID == folder.ID).DeleteAsync();
            await _database.Table<Dataset>().Where(i => i.FolderID == folder.ID).DeleteAsync();
            await _database.DeleteAsync(folder);

            return default;
        }

        public Task<int> CreateFolderAsync(Folder folder)
        {
            return _database.InsertAsync(folder);
        }

        public Task<Note> GetNoteByNameAsync(int folderID, string name)
        {
            return _database.Table<Note>()
                            .Where(i => i.FolderID == folderID && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Note> GetQuickAccessNoteByNameAsync(string name)
        {
            return _database.Table<Note>()
                            .Where(i => i.IsQuickAccess == true && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Dataset> GetDatasetByNameAsync(int folderID, string name)
        {
            return _database.Table<Dataset>()
                            .Where(i => i.FolderID == folderID && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Dataset> GetQuickAccessDatasetByNameAsync(string name)
        {
            return _database.Table<Dataset>()
                            .Where(i => i.IsQuickAccess == true && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Folder> GetFolderByNameAsync(int parentID, string name)
        {
            return _database.Table<Folder>()
                            .Where(i => i.ParentID == parentID && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<Folder> GetQuickAccessFolderByNameAsync(string name)
        {
            return _database.Table<Folder>()
                            .Where(i => i.IsQuickAccess == true && i.Name == name)
                            .FirstOrDefaultAsync();
        }

        public Task<(string, ErrorEncountered)> InterpolateAndInputTemplatesAsync(string text, Page currentPage, int folderID)
        {
            // first interpolate anything in this string, inputting the default values specified
            Console.WriteLine("DEBUG: Initial InterpolateValues started");
            text = InterpolateValues(text, new List<Dictionary<string, string>>());
            Console.WriteLine("DEBUG: Intitial InterpolateValues finished");
            // then input all templates
            Console.WriteLine("DEBUG: Main InputTemplates started");
            return InputTemplates(text, currentPage, folderID);
        }

        private async Task<(string, ErrorEncountered)> GetTemplate(string path, Page currentPage, int folderID)
        {
            Note templateFile;
            string noteName;
            string[] pathSplit = path.Split('/');

            if (pathSplit.Length == 1) // is just a note
            {
                noteName = pathSplit[0];

                switch (noteName[0])
                {
                    case '*':  // must be quick access
                        {
                            noteName = noteName.Skip(1).ToString();
                            templateFile = await GetQuickAccessNoteByNameAsync(noteName);
                            if (templateFile == null)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Quick Access Note {{{noteName}}} not found.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                    case '/': // must be in root folder
                        {
                            templateFile = await GetNoteByNameAsync(0, noteName);
                            if (templateFile == null)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Note {{{noteName}}} not found in root folder.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                    default: // must be relative path
                        {
                            templateFile = await GetNoteByNameAsync(folderID, noteName);
                            if (templateFile == null)
                            {
                                Folder folder = await GetFolderAsync(folderID);
                                await currentPage.DisplayAlert("Template Error", $"Note {{{noteName}}} not found in folder {{{folder.Name}}}.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                }
            }
            else
            {
                var folderNames = pathSplit.Take(pathSplit.Length - 1).Skip(1);
                noteName = pathSplit.Last();
                string firstFolderName = pathSplit.First();

                Folder currentFolder;
                Folder newFolder;
                switch (firstFolderName[0])
                {
                    case '*': // the first folder must be quick access
                        {
                            firstFolderName = firstFolderName.Skip(1).ToString();
                            currentFolder = await GetQuickAccessFolderByNameAsync(firstFolderName);
                            if (currentFolder == null)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Quick Access Folder {{{firstFolderName}}} not found.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                    case '/': // root folder
                        {
                            currentFolder = new Folder() { ID = 0 };
                            break;
                        }
                    default: // relative path
                        {
                            if (folderID == 0) // current folder is root folder
                            {
                                currentFolder = new Folder() { ID = 0 };
                            }
                            else // current folder is not root folder, i.e. it exists in the database
                            {
                                currentFolder = await GetFolderAsync(folderID);
                            }
                            break;
                        }
                }

                foreach (string folderName in folderNames)
                {
                    if (folderName == "..") // go up one level from current folder
                    {
                        if (currentFolder.ID == 0) // current folder is root folder
                        {
                            await currentPage.DisplayAlert("Template Error", "Root folder does not have a parent folder.", "OK");
                            return (null, ErrorEncountered.True);
                        }
                        // current folder is not root folder
                        if (currentFolder.ParentID == 0) // parent folder is root folder
                        {
                            newFolder = new Folder() { ID = 0 };
                        }
                        else // parent folder is not root folder, i.e. it exists in database
                        {
                            newFolder = await GetFolderAsync(currentFolder.ParentID);
                        }
                    }
                    else // is a sub folder of current folder
                    {
                        newFolder = await GetFolderByNameAsync(currentFolder.ID, folderName);

                        if (newFolder == null)
                        {
                            if (currentFolder.ID == 0)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Folder {{{folderName}}} not found in root folder", "OK");
                            }
                            else
                            { 
                                await currentPage.DisplayAlert("Template Error", $"Folder {{{folderName}}} not found in {{{currentFolder.Name}}}", "OK");
                            }
                            return (null, ErrorEncountered.True);
                        }
                    }
                    currentFolder = newFolder;
                }

                templateFile = await GetNoteByNameAsync(currentFolder.ID, noteName);
                if (templateFile == null)
                {
                    await currentPage.DisplayAlert("Template Error", $"Note {{{noteName}}} not found in {{{currentFolder.Name}}}.", "OK");
                    return (null, ErrorEncountered.True);
                }
            }
            Console.WriteLine($"DEBUG: Get Template {templateFile.Name} Completed");
            return (templateFile.Text, ErrorEncountered.False);
        }

        private async Task<(Dictionary<string, string>, ErrorEncountered)> GetDataset(string path, Page currentPage, int folderID)
        {
            Dataset datasetFile;
            string datasetName;
            string[] pathSplit = path.Split('/');

            if (pathSplit.Length == 1) // is just a note
            {
                datasetName = pathSplit[0];

                switch (datasetName[0])
                {
                    case '*':  // must be quick access
                        {
                            datasetName = datasetName.Skip(1).ToString();
                            datasetFile = await GetQuickAccessDatasetByNameAsync(datasetName);
                            if (datasetFile == null)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Quick Access Dataset {{{datasetName}}} not found.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                    case '/': // must be in root folder
                        {
                            datasetFile = await GetDatasetByNameAsync(0, datasetName);
                            if (datasetFile == null)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Dataset {{{datasetName}}} not found in root folder.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                    default: // must be relative path
                        {
                            datasetFile = await GetDatasetByNameAsync(folderID, datasetName);
                            if (datasetFile == null)
                            {
                                Folder folder = await GetFolderAsync(folderID);
                                await currentPage.DisplayAlert("Template Error", $"Dataset {{{datasetName}}} not found in folder {{{folder.Name}}}.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                }
            }
            else
            {
                var folderNames = pathSplit.Take(pathSplit.Length - 1).Skip(1);
                datasetName = pathSplit.Last();
                string firstFolderName = pathSplit.First();

                Folder currentFolder;
                Folder newFolder;
                switch (firstFolderName[0])
                {
                    case '*': // the first folder must be quick access
                        {
                            firstFolderName = firstFolderName.Skip(1).ToString();
                            currentFolder = await GetQuickAccessFolderByNameAsync(firstFolderName);
                            if (currentFolder == null)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Quick Access Folder {{{firstFolderName}}} not found.", "OK");
                                return (null, ErrorEncountered.True);
                            }
                            break;
                        }
                    case '/': // root folder
                        {
                            currentFolder = new Folder() { ID = 0 };
                            break;
                        }
                    default: // relative path
                        {
                            if (folderID == 0) // current folder is root folder
                            {
                                currentFolder = new Folder() { ID = 0 };
                            }
                            else // current folder is not root folder, i.e. it exists in the database
                            {
                                currentFolder = await GetFolderAsync(folderID);
                            }
                            break;
                        }
                }

                foreach (string folderName in folderNames)
                {
                    if (folderName == "..") // go up one level from current folder
                    {
                        if (currentFolder.ID == 0) // current folder is root folder
                        {
                            await currentPage.DisplayAlert("Template Error", "Root folder does not have a parent folder.", "OK");
                            return (null, ErrorEncountered.True);
                        }
                        // current folder is not root folder
                        if (currentFolder.ParentID == 0) // parent folder is root folder
                        {
                            newFolder = new Folder() { ID = 0 };
                        }
                        else // parent folder is not root folder, i.e. it exists in database
                        {
                            newFolder = await GetFolderAsync(currentFolder.ParentID);
                        }
                    }
                    else // is a sub folder of current folder
                    {
                        newFolder = await GetFolderByNameAsync(currentFolder.ID, folderName);

                        if (newFolder == null)
                        {
                            if (currentFolder.ID == 0)
                            {
                                await currentPage.DisplayAlert("Template Error", $"Folder {{{folderName}}} not found in root folder", "OK");
                            }
                            else
                            {
                                await currentPage.DisplayAlert("Template Error", $"Folder {{{folderName}}} not found in {{{currentFolder.Name}}}", "OK");
                            }
                            return (null, ErrorEncountered.True);
                        }
                    }
                    currentFolder = newFolder;
                }

                datasetFile = await GetDatasetByNameAsync(currentFolder.ID, datasetName);
                if (datasetFile == null)
                {
                    await currentPage.DisplayAlert("Template Error", $"Dataset {{{datasetName}}} not found in {{{currentFolder.Name}}}.", "OK");
                    return (null, ErrorEncountered.True);
                }
            }

            Console.WriteLine($"DEBUG: DatasetFile {datasetFile.Name} found, Loading file...");
            return (LoadDatasetString(datasetFile.Text), ErrorEncountered.False);
        }

        private Dictionary<string, string> LoadDatasetString(string datasetString)
        {
            Regex datasetRegex = new Regex(@"(}>)?(?<key>[^{}:]*?):\s*<{(?<value>.*?)}>");

            var dataset = new Dictionary<string, string>();

            foreach (Match match in datasetRegex.Matches(datasetString))
            {
                dataset.Add(match.Groups["key"].ToString().Trim(), match.Groups["value"].ToString());
            }

            Console.WriteLine("DEBUG: Dataset loaded");
            return dataset;
        }

        private async Task<(string, ErrorEncountered)> InputTemplates(string text, Page currentPage, int folderID)
        {
            Regex templateRegex = new Regex(@"(?<!\\){\s*?T\s*?:(?<path>[^:{}]*?)(:(?<datasets>[^:{}]*?))?(?<!\\)}");

            string template;
            Dictionary<string, string> dataset;
            ErrorEncountered errorEncountered;
            string path;
            List<Dictionary<string, string>> datasets;

            foreach (Match match in templateRegex.Matches(text))
            {
                Console.WriteLine("DEBUG: A match has been started...");
                path = match.Groups["path"].ToString().Trim();
                string[] datasetPaths = match.Groups["datasets"].ToString().Trim().Split(';');

                (template, errorEncountered) = await GetTemplate(path, currentPage, folderID);
                if (errorEncountered == ErrorEncountered.True) 
                    return (null, ErrorEncountered.True);

                Console.WriteLine($"DEBUG: After GetTemplate, the template appears to be [{template}]");

                datasets = new List<Dictionary<string, string>>();
                foreach (string datasetPath in datasetPaths)
                {
                    (dataset, errorEncountered) = await GetDataset(datasetPath, currentPage, folderID);
                    if (errorEncountered == ErrorEncountered.True)
                        return (null, ErrorEncountered.True);
                    datasets.Add(dataset);
                }

                template = InterpolateValues(template, datasets);
                Console.WriteLine($"DEBUG: Another InterpolateValues has been done, template is now [{template}]");

                (template, errorEncountered) = await InputTemplates(template, currentPage, folderID);
                if (errorEncountered == ErrorEncountered.True)
                    return (null, ErrorEncountered.True);

                text = text.Replace(match.Value, template);
                Console.WriteLine($"DEBUG: That match [{match.Value}] has been replaced by [{template}]");
            }

            return (text, ErrorEncountered.False);
        }

        private static string InterpolateValues(string template, List<Dictionary<string, string>> datasets)
        {
            Regex interpolationRegex = new Regex(@"(?<!\\){(?!#)(?<key>[^:={}]*?)(=(?<default>[^:{}]*?))?(?<!\\)}");

            foreach (Match match in interpolationRegex.Matches(template))
            {
                string replacement = "";

                bool keyFound = false;
                // take the value from the first dataset with a match
                foreach (var dataset in datasets)
                {
                    try
                    {
                        replacement = dataset[match.Groups["key"].ToString().Trim()];
                        keyFound = true;
                        break;
                    }
                    catch (KeyNotFoundException) { }
                }

                if (!keyFound)
                {
                    replacement = match.Groups["default"].ToString();

                    //string trimmed = replacement.Trim();
                    //if (trimmed[0] == '{' && trimmed[trimmed.Length - 1] == '}')
                    //{
                    //    replacement = trimmed.Skip(1).Take(trimmed.Length - 2).ToString();
                    //}
                }

                template = template.Replace(match.Value, replacement);
            }

            return template;
        }
    }
}
