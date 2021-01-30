using Notes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Data
{
    public interface INoteDatabase
    {
        Task BackupAsync(string path);
        Task CloseAsync();
        Task DeleteAllAsync();
        Task DeleteAsync(Note note);
        Task DeleteFolderAndAllContentsAsync(Folder folder);
        Task DeleteFolderNotContentAsync(Folder folder);
        Task<bool> DoesFolderNameExistAsync(string name, Guid parentID, Guid id);
        Task<bool> DoesNoteNameExistAsync(string name, Guid folderID, Guid id);
        Task<bool> DoesQuickAccessFolderNameExistAsync(string name, Guid id);
        Task<bool> DoesQuickAccessNoteNameExistAsync(string name, Guid id);
        Task<bool> DoesQuickAccessOrOtherwiseFolderNameExistAsync(string name, Guid parentID, Guid id);
        Task<bool> DoesQuickAccessOrOtherwiseNoteNameExistAsync(string name, Guid folderID, Guid id);
        Task<List<Folder>> GetAllFoldersAsync();
        Task<List<Note>> GetAllNotesAsync();
        Task<List<CSS>> GetAllSheetsAsync();
        Task<Folder> GetFolderAsync(Guid id);
        Task<Folder> GetFolderByNameAsync(Guid parentID, string name);
        Task<List<Folder>> GetFoldersAsync(Guid folderID);
        Task<Note> GetNoteAsync(Guid id);
        Task<Note> GetNoteByNameAsync(Guid folderID, string name);
        Task<List<Note>> GetNotesAsync(Guid folderID);
        Task<Folder> GetQuickAccessFolderByNameAsync(string name);
        Task<List<Folder>> GetQuickAccessFoldersAsync();
        Task<Note> GetQuickAccessNoteByNameAsync(string name);
        Task<List<Note>> GetQuickAccessNotesAsync();
        Task<CSS> GetSheetAsync(Guid id);
        Task<List<CSS>> GetSheetsAsync();
        Task<int> InsertAllAsync(IEnumerable<CSS> enumerable);
        Task<int> InsertAllAsync(IEnumerable<Folder> enumerable);
        Task<int> InsertAllAsync(IEnumerable<Note> enumerable);
        Task<int> SaveAsync(Folder folder);
        Task<int> SaveAsync(Note note);
        Task<int> SaveSheetAsync(CSS sheet);
    }
}