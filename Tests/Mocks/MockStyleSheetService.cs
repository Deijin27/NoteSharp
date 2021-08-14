using Notes.Models;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Mocks
{
    class MockStyleSheetService : IStyleSheetService
    {
        public Guid DefaultStyleSheetID { get; set; }

        public List<CSS> GetAllStyleSheetsReturnValue { get; set; }
        public Task<List<CSS>> GetAllStyleSheetsAsync() => Task.FromResult(GetAllStyleSheetsReturnValue);

        public CSS GetStyleSheetReturnValue { get; set; }
        public Task<CSS> GetStyleSheetAsync() => Task.FromResult(GetStyleSheetReturnValue);
        
    }
}
