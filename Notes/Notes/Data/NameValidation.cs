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
        /// Get a valid name from the user for the Note. Return is the Option, Cancel or OK, and if OK, the string the user desires.
        /// </summary>
        /// <param name="folderID"></param>
        /// <returns>(bool ok_clicked, string result)</returns>
        public static async Task<(Option, string)> GetUniqueNoteName(Page page, int folderID, string title, string initialValue = "")
        {
            string result = await page.DisplayPromptAsync(title, "Input name for note", initialValue: initialValue);

            if (result == null)
                return (Option.Cancel, null);

            bool valid;
            while (!(valid = IsNameValid(result)) || await App.Database.DoesNoteNameExistAsync(result, folderID))
            {

                if (!valid)
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "Invalid name, please input a name that does not contain any of the characters /:{}",
                        initialValue: result
                    );
                }

                else
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "A file of that name already exists in the current folder; please input a different name",
                        initialValue: result
                    );
                }

                if (result == null)
                    return (Option.Cancel, null);

            }
            return (Option.OK, result);
        }

        public static async Task<(Option, string)> GetUniqueFolderName(Page page, int parentID, string title, string initialValue = "")
        {
            string result = await page.DisplayPromptAsync(title, "Input name for folder", initialValue: initialValue);

            if (result == null)
                return (Option.Cancel, null);

            bool valid;
            while (!(valid = IsNameValid(result)) || await App.Database.DoesFolderNameExistAsync(result, parentID))
            {
                if (!valid)
                {
                    result = await page.DisplayPromptAsync
                    (
                        title,
                        "Invalid name, please input a name that does not contain any of the characters /:{}",
                        initialValue: result
                    );
                }

                else
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

            }
            return (Option.OK, result);
        }

        public static bool IsNameValid(string name)
        {
            return !(name.Contains(":") || name.Contains("{") || name.Contains("}") || name.Contains("/"));
        }
    }
}
