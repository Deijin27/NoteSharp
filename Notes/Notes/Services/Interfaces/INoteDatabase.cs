using Notes.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notes.Services
{
    public interface INoteDatabase
    {
        Guid CacheId { get; }
        Task ClearCachedNote();
        Task<Note> GetCachedNote();
        Task SetCachedNote(Note note);

        Task BackupAsync(string path);
        Task CloseAsync();
        Task DeleteAllAsync();
        Task DeleteAsync(Note note);
        Task DeleteFolderAndAllContentsAsync(Folder folder);
        Task DeleteFolderNotContentAsync(Folder folder);
        Task<Folder> GetFolderAsync(Guid id);
        Task<Folder> GetFolderByNameAsync(Guid parentID, string name);
        Task<List<Folder>> GetFoldersAsync(Guid folderID);
        Task<Note> GetNoteAsync(Guid id);
        Task<Note> GetNoteByNameAsync(Guid folderID, string name);
        Task<List<Note>> GetNotesAsync(Guid folderID);
        Task<CSS> GetSheetAsync(Guid id);
        Task<List<CSS>> GetAllSheetsAsync();
        Task<int> InsertAllAsync(IEnumerable<CSS> enumerable);
        Task<int> InsertAllAsync(IEnumerable<Folder> enumerable);
        Task<int> InsertAllAsync(IEnumerable<Note> enumerable);
        Task<int> SaveAsync(Folder folder);
        Task<int> SaveAsync(Note note);
        Task<int> SaveAsync(CSS sheet);
        bool ValidateDatabaseAtPath(string path);
        Task DeleteAsync(CSS sheet);

        Task<Folder> GetQuickAccessFolderByNameAsync(string name);
        Task<List<Folder>> GetQuickAccessFoldersAsync();
        Task<Note> GetQuickAccessNoteByNameAsync(string name);
        Task<List<Note>> GetQuickAccessNotesAsync();
        Task ClearCachedCss();
        Task<CSS> GetCachedCss();
        Task SetCachedCss(CSS note);
    }
}