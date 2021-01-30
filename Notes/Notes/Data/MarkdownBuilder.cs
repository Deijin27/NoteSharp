using Markdig;
using Notes.Models;
using Notes.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Notes.Data
{
    static class MarkdownBuilder
    {
        public static Task<(string result, ErrorDetails errorDetails)> BuildMarkdown(string text, Guid folderID, INoteDatabase db)
        {
            return InterpolateAndInputTemplatesAsync(text, folderID, db);
        }

        public static string BuildHtml(string text)
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .Build();
            return Markdown.ToHtml(text, pipeline);
        }

        // --------------------------------------------------------------------

        private static readonly Regex DiRegex = new Regex(@"(?s)(?<!\\)<di\s+((?<key>[^""\s]+?)|(""(?<key>[^""]*?)""))\s*((>(?<value>.*?)(?<!\\)</\s*di\s*>)|(\s*(""(?<value>[^""]*?)"")?\s*/>))");
        private static readonly Regex TiRegex = new Regex(@"(?s)(?<!\\)<ti\s+""(?<path>[^""]*)""\s*(?<datasets>(\s*""([^""]*)""\s*)*?)\s*(?<loosekeys>(((\s*[^""\s]+?)|(\s*""[^""]*?""))=""[^""]*?""\s*)*?)\s*/>");
        private static readonly Regex DatasetPathRegex = new Regex(@"(?s)""(?<dataset>.*?)"""); // i removed spaces either side of this
        private static readonly Regex LooseKeyRegex = new Regex(@"(?s)((?<key>[^""\s]+?)|(""(?<key>[^""]*?)""))=""(?<value>[^""]*?)""");

        private static Task<(string result, ErrorDetails errorDetails)> InterpolateAndInputTemplatesAsync(string text, Guid folderID, INoteDatabase db)
        {
            // first interpolate anything in this string, inputting the default values specified
            text = InterpolateValues(text, new Dictionary<string, string>());
            // then input all templates
            return InputTemplates(text, folderID, folderID, db);
        }

        private static async Task<(Folder finalFolder, ErrorDetails errorDetails)> FollowFolderPath(Folder currentFolder, IEnumerable<string> folderNameSequence, INoteDatabase db)
        {
            foreach (string folderName in folderNameSequence)
            {

                if (folderName[0] == '*') // look for in quick access
                {
                    currentFolder = await db.GetQuickAccessFolderByNameAsync(folderName.Remove(0, 1));

                    if (currentFolder == null)
                    {
                        return (null, new ErrorDetails()
                        {
                            Title = AppResources.Alert_TemplateError_Title,
                            Message = string.Format(AppResources.Alert_QuickAccessFolderNotFound_Message, currentFolder.Name),
                            DismissButtonText = AppResources.AlertOption_OK
                        });
                    }
                }
                else if (folderName == "..") // step up a folder
                {
                    if (currentFolder.ID == Guid.Empty) // current folder is root folder
                    {
                        return (null, new ErrorDetails()
                        {
                            Title = AppResources.Alert_TemplateError_Title,
                            Message = AppResources.Alert_RootFolderHasNoParent_Message,
                            DismissButtonText = AppResources.AlertOption_OK
                        });
                    }
                    // current folder is not root folder
                    if (currentFolder.ParentID == Guid.Empty) // parent folder is root folder
                    {
                        currentFolder = new Folder() { ID = Guid.Empty, Name = AppResources.PageTitle_RootFolder };
                    }
                    else // parent folder is not root folder, i.e. it exists in database
                    {
                        currentFolder = await db.GetFolderAsync(currentFolder.ParentID);
                    }
                }
                else if (folderName == ".") // stay same folder
                {
                    // do nothing
                }
                else // look for folder in current folder
                {
                    Folder newFolder = await db.GetFolderByNameAsync(currentFolder.ID, folderName);

                    if (newFolder == null)
                    {
                        return (null, new ErrorDetails()
                        {
                            Title = AppResources.Alert_TemplateError_Title,
                            Message = string.Format(AppResources.Alert_FolderNotFoundInFolder_Message, folderName, currentFolder.Name),
                            DismissButtonText = AppResources.AlertOption_OK
                        });
                    }

                    currentFolder = newFolder;
                }
            }
            return (currentFolder, new ErrorDetails() { ErrorWasEncountered = false });
        }

        private static async Task<(Note note, ErrorDetails errorDetails)> GetNoteByPath(string path, Guid folderID, Guid mainFolderID, INoteDatabase db)
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
                startFolder = await db.GetFolderAsync(folderID);

            IEnumerable<string> folderNameSequence = pathSplit.Take(pathSplit.Length - 1);

            (Folder endFolder, ErrorDetails errorDetails) = await FollowFolderPath(startFolder, folderNameSequence, db);
            if (errorDetails.ErrorWasEncountered) return (null, errorDetails);

            string noteName = pathSplit.Last();

            Note noteFile;

            if (noteName[0] == '*')
            {
                noteName = noteName.Remove(0, 1);
                noteFile = await db.GetQuickAccessNoteByNameAsync(noteName);

                if (noteFile == null)
                {
                    return (null, new ErrorDetails()
                    {
                        Title = AppResources.Alert_TemplateError_Title,
                        Message = string.Format(AppResources.Alert_QuickAccessNoteNotFound_Message, noteName),
                        DismissButtonText = AppResources.AlertOption_OK
                    });
                }
            }
            else
            {
                noteFile = await db.GetNoteByNameAsync(endFolder.ID, noteName);

                if (noteFile == null)
                {
                    return (null, new ErrorDetails()
                    {
                        Title = AppResources.Alert_TemplateError_Title,
                        Message = string.Format(AppResources.Alert_NoteNotFoundInFolder_Message, noteName, endFolder.Name),
                        DismissButtonText = AppResources.AlertOption_OK
                    });
                }
            }

            return (noteFile, new ErrorDetails() { ErrorWasEncountered = false });
        }

        private static Dictionary<string, string> LoadDatasetString(string datasetString)
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

        private static async Task<(string result, ErrorDetails errorDetails)> InputTemplates(string text, Guid folderID, Guid mainFolderID, INoteDatabase db)
        {
            string template;
            string templatePath;
            ErrorDetails errorDetails;
            Note datasetFile;
            Note templateFile;
            Dictionary<string, string> mergedDataset;

            foreach (Match match in TiRegex.Matches(text))
            {
                templatePath = match.Groups["path"].Value;

                (templateFile, errorDetails) = await GetNoteByPath(templatePath, folderID, mainFolderID, db);
                if (errorDetails.ErrorWasEncountered) return (null, errorDetails);

                template = templateFile.Text;

                IEnumerable<string> datasetPaths = DatasetPathRegex.Matches(match.Groups["datasets"].Value)
                                                                   .OfType<Match>()
                                                                   .Select(i => i.Groups["dataset"].Value)
                                                                   .Reverse(); // reverse order so that the first dataset in the list has priority

                mergedDataset = new Dictionary<string, string>();
                foreach (string datasetPath in datasetPaths)
                {
                    (datasetFile, errorDetails) = await GetNoteByPath(datasetPath, folderID, mainFolderID, db);
                    if (errorDetails.ErrorWasEncountered) return (null, errorDetails);

                    foreach (KeyValuePair<string, string> kvp in LoadDatasetString(datasetFile.Text))
                    {
                        mergedDataset[kvp.Key] = kvp.Value;
                    }
                }

                foreach (Match lk in LooseKeyRegex.Matches(match.Groups["loosekeys"].Value))
                {
                    mergedDataset[lk.Groups["key"].Value] = lk.Groups["value"].Value;
                }

                template = InterpolateValues(template, mergedDataset);

                (template, errorDetails) = await InputTemplates(template, templateFile.FolderID, mainFolderID, db);
                if (errorDetails.ErrorWasEncountered) return (null, errorDetails);

                text = text.Replace(match.Value, template);
            }

            return (text, new ErrorDetails() { ErrorWasEncountered = false });
        }

        private static string InterpolateValues(string template, Dictionary<string, string> mergedDataset)
        {
            foreach (Match match in DiRegex.Matches(template))
            {
                string key = match.Groups["key"].Value;
                // take the value from the first dataset with a match
                if (!mergedDataset.TryGetValue(key, out string replacement))
                {
                    replacement = match.Groups["value"].Value;
                }

                template = template.Replace(match.Value, replacement);
            }

            return template;
        }
    }


}
