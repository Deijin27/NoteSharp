using Notes.Models;
using Notes.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Notes.Services
{
    public class StyleSheetService : IStyleSheetService
    {
        public StyleSheetService(IAppServiceProvider services)
        {
            Services = services;
        }
        IAppServiceProvider Services;

        private CSS GetNonUserStyleSheet(Guid id)
        {
            return GetDefaultStyleSheets().Where(i => i.ID == id).FirstOrDefault();
        }

        public Guid DefaultStyleSheetID => DefaultStyleSheetGuids[0];

        List<Guid> DefaultStyleSheetGuids = new List<Guid>()
        {
            Guid.Parse("ac67e818-73bd-4d85-a269-12f78a3f46d7"),
            Guid.Parse("4ad15e4c-e1db-4ff5-a620-61af06060ff1")
        };

        private List<CSS> GetDefaultStyleSheets()
        {
            var sheets = new List<CSS>
            {
                new CSS()
                {
                    ID = DefaultStyleSheetGuids[0],
                    IsReadOnly = true,
                    Name = "Modern Light",
                    Text = AppResources.ModernLightCSS
                },

                new CSS()
                {
                    ID = DefaultStyleSheetGuids[1],
                    IsReadOnly = true,
                    Name = "Modern Dark",
                    Text = AppResources.ModernDarkCSS
                }
            };

            return sheets;
        }

        public async Task<CSS> GetStyleSheetAsync(Guid sheetID)
        {
            CSS sheet;

            if (!DefaultStyleSheetGuids.Contains(sheetID)) // is user defined
            {
                sheet = await Services.NoteDatabase.GetSheetAsync(sheetID);
            }
            else
            {
                sheet = GetNonUserStyleSheet(sheetID);
            }

            return sheet;
        }

        public async Task<List<CSS>> GetAllStyleSheetsAsync()
        {
            var styleSheets = GetDefaultStyleSheets();
            styleSheets = styleSheets.Concat(await Services.NoteDatabase.GetAllSheetsAsync()).ToList();
            return styleSheets;
        }
    }
}
