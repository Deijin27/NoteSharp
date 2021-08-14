using Markdig;
using Notes.Models;
using Notes.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Notes.Services
{
    public class MarkdownBuilder : IMarkdownBuilder
    {
        readonly IAppServiceProvider Services;
        public MarkdownBuilder(IAppServiceProvider services)
        {
            Services = services;
        }

        public Task<(string result, bool errorEncountered)> BuildMarkdown(string text, Guid folderId)
        {
            return InterpolateAndInputTemplatesAsync(text, folderId);
        }

        public async Task<string> BuildHtml(string text)
        {
            return await Task.Run(() => MarkdigBuildHtml(text));
        }

        private static string MarkdigBuildHtml(string text)
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

        private Task<(string result, bool errorEncountered)> InterpolateAndInputTemplatesAsync(string text, Guid folderID)
        {
            // first interpolate anything in this string, inputting the default values specified
            text = InterpolateValues(text, new Dictionary<string, string>());
            // then input all templates
            return InputTemplates(text, folderID, folderID);
        }

        private async Task<(Folder finalFolder, bool errorEncountered)> FollowFolderPath(Folder currentFolder, IEnumerable<string> folderNameSequence)
        {
            foreach (string folderName in folderNameSequence)
            {

                if (folderName[0] == '*') // look for in quick access
                {
                    currentFolder = await Services.NoteDatabase.GetQuickAccessFolderByNameAsync(folderName.Remove(0, 1));

                    if (currentFolder == null)
                    {
                        await Services.Popups.AlertPopup(
                            AppResources.Alert_TemplateError_Title,
                            string.Format(AppResources.Alert_QuickAccessFolderNotFound_Message, currentFolder.Name),
                            AppResources.AlertOption_OK
                        );
                        return (null, true);
                    }
                }
                else if (folderName == "..") // step up a folder
                {
                    if (currentFolder.ID == Guid.Empty) // current folder is root folder
                    {
                        await Services.Popups.AlertPopup(
                            AppResources.Alert_TemplateError_Title,
                            AppResources.Alert_RootFolderHasNoParent_Message,
                            AppResources.AlertOption_OK
                        );
                        return (null, true);
                    }
                    // current folder is not root folder
                    if (currentFolder.ParentID == Guid.Empty) // parent folder is root folder
                    {
                        currentFolder = new Folder() { ID = Guid.Empty, Name = AppResources.PageTitle_RootFolder };
                    }
                    else // parent folder is not root folder, i.e. it exists in database
                    {
                        currentFolder = await Services.NoteDatabase.GetFolderAsync(currentFolder.ParentID);
                    }
                }
                else if (folderName == ".") // stay same folder
                {
                    // do nothing
                }
                else // look for folder in current folder
                {
                    Folder newFolder = await Services.NoteDatabase.GetFolderByNameAsync(currentFolder.ID, folderName);

                    if (newFolder == null)
                    {
                        await Services.Popups.AlertPopup(                        
                            AppResources.Alert_TemplateError_Title,
                            string.Format(AppResources.Alert_FolderNotFoundInFolder_Message, folderName, currentFolder.Name),
                            AppResources.AlertOption_OK
                        );
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
                startFolder = await Services.NoteDatabase.GetFolderAsync(folderID);

            IEnumerable<string> folderNameSequence = pathSplit.Take(pathSplit.Length - 1);

            (Folder endFolder, bool errorEncountered) = await FollowFolderPath(startFolder, folderNameSequence);
            if (errorEncountered) return (null, errorEncountered);

            string noteName = pathSplit.Last();

            Note noteFile = await Services.NoteDatabase.GetNoteByNameAsync(endFolder.ID, noteName);

            if (noteFile == null)
            {
                await Services.Popups.AlertPopup(
                    AppResources.Alert_TemplateError_Title,
                    string.Format(AppResources.Alert_NoteNotFoundInFolder_Message, noteName, endFolder.Name),
                    AppResources.AlertOption_OK
                );
            }


            return (noteFile, false);
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

        private async Task<(string result, bool errorEncountered)> InputTemplates(string text, Guid folderID, Guid mainFolderID)
        {
            foreach (Match match in TiRegex.Matches(text))
            {
                string templatePath = match.Groups["path"].Value;

                var (templateFile, errorEncountered) = await GetNoteByPath(templatePath, folderID, mainFolderID);
                if (errorEncountered) return (null, true);

                string template = templateFile.Text;

                IEnumerable<string> datasetPaths = DatasetPathRegex.Matches(match.Groups["datasets"].Value)
                                                                   .OfType<Match>()
                                                                   .Select(i => i.Groups["dataset"].Value)
                                                                   .Reverse(); // reverse order so that the first dataset in the list has priority

                var mergedDataset = new Dictionary<string, string>();
                foreach (string datasetPath in datasetPaths)
                {
                    var result = await GetNoteByPath(datasetPath, folderID, mainFolderID);
                    if (result.errorEncountered) return (null, true);

                    foreach (KeyValuePair<string, string> kvp in LoadDatasetString(result.note.Text))
                    {
                        mergedDataset[kvp.Key] = kvp.Value;
                    }
                }

                foreach (Match lk in LooseKeyRegex.Matches(match.Groups["loosekeys"].Value))
                {
                    mergedDataset[lk.Groups["key"].Value] = lk.Groups["value"].Value;
                }

                template = InterpolateValues(template, mergedDataset);

                (template, errorEncountered) = await InputTemplates(template, templateFile.FolderID, mainFolderID);
                if (errorEncountered) return (null, true);

                text = text.Replace(match.Value, template);
            }

            return (text, false);
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
