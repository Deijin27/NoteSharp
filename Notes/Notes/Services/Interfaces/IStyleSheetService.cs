using Notes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Services
{
    public interface IStyleSheetService
    {
        Task<List<CSS>> GetAllStyleSheetsAsync();
        Task<CSS> GetStyleSheetAsync(Guid sheetID);
        Guid DefaultStyleSheetID { get; }
    }
}