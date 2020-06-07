using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Notes.Data
{
    public enum Option
    {
        Cancel,
        OK
    }

    class NameValidation
    {
        /// <summary>
        /// Get a valid name from the user for the Note. Return is a tuple of the Option selected, Cancel or OK, and the string the user desires.
        /// </summary>
        /// <param name="page">Current Page for the dialog to be raised over</param>
        /// <param name="folderID">ID of folder</param>
        /// <param name="title">Title of dialog</param>
        /// <param name="initialValue">Initial text in dialog entry when first raised</param>
        /// <returns>(Option option_clicked, string result)</returns>
        public static async Task<(Option, string)> GetUniqueNoteName(Page page, int folderID, string title, bool isQuickAccess = false, string message = "Input name for note", string initialValue = "")
        {
            string result = await page.DisplayPromptAsync(title, message, initialValue: initialValue);

            if (result == null)
                return (Option.Cancel, null);

            result = result.Trim();

            bool invalid = true;
            while (invalid)
            {

                if (invalid = !IsNameValid(result))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "Invalid name, please input a name that does not contain any of the characters /:{};#*",
                        initialValue: result
                    );
                }

                else if (isQuickAccess && (invalid = await App.Database.DoesQuickAccessNoteNameExistAsync(result)))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A note of that name already exists in Quick Access; please input a different name",
                        initialValue: result
                    );
                }

                else if (invalid = await App.Database.DoesNoteNameExistAsync(result, folderID))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A note of that name already exists in the current folder; please input a different name",
                        initialValue: result
                    );
                }

                if (result == null)
                    return (Option.Cancel, null);

                result = result.Trim();

            }
            return (Option.OK, result);
        }

        public static async Task<(Option, string)> GetUniqueDatasetName(Page page, int folderID, string title, bool isQuickAccess = false, string message = "Input name for dataset", string initialValue = "")
        {
            string result = await page.DisplayPromptAsync(title, message, initialValue: initialValue);

            if (result == null)
                return (Option.Cancel, null);

            result = result.Trim();

            bool invalid = true;
            while (invalid)
            {

                if (invalid = !IsNameValid(result))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "Invalid name, please input a name that does not contain any of the characters /:{};#*",
                        initialValue: result
                    );
                }

                else if (isQuickAccess && (invalid = await App.Database.DoesQuickAccessDatasetNameExistAsync(result)))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A dataset of that name already exists in Quick Access; please input a different name",
                        initialValue: result
                    );
                }

                else if (invalid = await App.Database.DoesDatasetNameExistAsync(result, folderID))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A dataset of that name already exists in the current folder; please input a different name",
                        initialValue: result
                    );
                }

                if (result == null)
                    return (Option.Cancel, null);

                result = result.Trim();
            }
            return (Option.OK, result);
        }

        /// <summary>
        /// Get a valid name from the user for the Folder. Return is the Option, Cancel or OK, and if OK, the string the user desires.
        /// </summary>
        /// <param name="page">Current Page for the dialog to be raised over</param>
        /// <param name="parentID">ID of parent folder</param>
        /// <param name="title">Title of dialog</param>
        /// <param name="initialValue">Initial text in dialog entry when first raised</param>
        /// <returns>(Option option_clicked, string result)</returns>
        public static async Task<(Option, string)> GetUniqueFolderName(Page page, int parentID, string title, bool isQuickAccess = false, string message = "Input name for folder", string initialValue = "")
        {
            string result = await page.DisplayPromptAsync(title, message, initialValue: initialValue);

            if (result == null)
                return (Option.Cancel, null);

            result = result.Trim();

            bool invalid = true;
            while (invalid)
            {
                if (invalid = !IsNameValid(result))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "Invalid name, please input a name that does not contain any of the characters /:{};#*",
                        initialValue: result
                    );
                }

                else if (isQuickAccess && (invalid = await App.Database.DoesQuickAccessFolderNameExistAsync(result)))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A folder of that name already exists in Quick Access; please input a different name",
                        initialValue: result
                    );
                }

                else if (invalid = await App.Database.DoesFolderNameExistAsync(result, parentID))
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A folder of that name already exists in the current folder; please input a different name",
                        initialValue: result
                    );
                }

                if (result == null)
                    return (Option.Cancel, null);

                result = result.Trim();
            }
            return (Option.OK, result);
        }

        /// <summary>
        /// Gives that doesn't contain any forbidden characters.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>false if the name contains forbidden characters.</returns>
        public static bool IsNameValid(string name)
        {
            return !(name.Contains(":") || name.Contains("{") || name.Contains("}") ||
                     name.Contains("/") || name.Contains(";") || name.Contains("#") ||
                     name.Contains("*") || name.Contains("=") || name == "T"
                     );
        }
    }
}
