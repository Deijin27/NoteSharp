using Notes.Models;
using Notes.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Mocks
{
    public class MockNoteDatabase : INoteDatabase
    {
        public Guid CacheId { get; set; }

        public Note GetCachedNoteResult = null;
        public Task<Note> GetCachedNote()
        {
            return Task.FromResult(GetCachedNoteResult);
        }
        public Queue<Note> SetCachedNoteCalls = new Queue<Note>();
        public Task SetCachedNote(Note note)
        {
            SetCachedNoteCalls.Enqueue(note);
            return Task.CompletedTask;
        }

        public Task BackupAsync(string path)
        {
            throw new NotImplementedException();
        }

        public int ClearCachedNoteCallCount = 0;
        public Task ClearCachedNote()
        {
            ClearCachedNoteCallCount++;
            return Task.CompletedTask;
        }

        public Task CloseAsync()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAllAsync()
        {
            throw new NotImplementedException();
        }

        public Queue<Note> DeleteNoteCalls = new Queue<Note>();
        public Task DeleteAsync(Note note)
        {
            DeleteNoteCalls.Enqueue(note);
            return Task.CompletedTask;
        }

        public Queue<Folder> DeleteFolderAndAllContentsCalls = new Queue<Folder>();
        public Task DeleteFolderAndAllContentsAsync(Folder folder)
        {
            DeleteFolderAndAllContentsCalls.Enqueue(folder);
            return Task.CompletedTask;
        }

        public Task DeleteFolderNotContentAsync(Folder folder)
        {
            throw new NotImplementedException();
        }

        public Task<List<CSS>> GetAllSheetsAsync()
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, Folder> FolderTable = new Dictionary<Guid, Folder>();
        public Queue<Guid> GetFolderCalls = new Queue<Guid>();
        public Task<Folder> GetFolderAsync(Guid id)
        {
            return Task.FromResult(FolderTable[id]);
        }

        public Task<Folder> GetFolderByNameAsync(Guid parentID, string name)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, List<Folder>> GetFoldersReturnLookup = new Dictionary<Guid, List<Folder>>();
        public Task<List<Folder>> GetFoldersAsync(Guid folderID)
        {
            return Task.FromResult(GetFoldersReturnLookup.TryGetValue(folderID, out List<Folder> value) ? value : new List<Folder>());
        }


        public Dictionary<Guid, Note> NoteTable = new Dictionary<Guid, Note>();
        public Queue<Guid> GetNoteCalls = new Queue<Guid>();
        public Task<Note> GetNoteAsync(Guid id)
        {
            GetNoteCalls.Enqueue(id);
            return Task.FromResult(NoteTable.GetValueOrDefault(id));
        }

        public Task<Note> GetNoteByNameAsync(Guid folderID, string name)
        {
            throw new NotImplementedException();
        }

        public Dictionary<Guid, List<Note>> GetNotesReturnLookup = new Dictionary<Guid, List<Note>>();
        public Task<List<Note>> GetNotesAsync(Guid folderID)
        {
            return Task.FromResult(GetNotesReturnLookup.TryGetValue(folderID, out List<Note> value) ? value : new List<Note>());
        }

        public Task<CSS> GetSheetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAllAsync(IEnumerable<CSS> enumerable)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAllAsync(IEnumerable<Folder> enumerable)
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAllAsync(IEnumerable<Note> enumerable)
        {
            throw new NotImplementedException();
        }

        public Queue<Folder> SaveFolderCalls = new Queue<Folder>();
        public Task<int> SaveAsync(Folder folder)
        {
            SaveFolderCalls.Enqueue(folder);
            return Task.FromResult(1);
        }

        public Queue<Note> SaveNoteCalls = new Queue<Note>();
        public Task<int> SaveAsync(Note note)
        {
            SaveNoteCalls.Enqueue(note);
            return Task.FromResult(1);
        }

        public Task<int> SaveAsync(CSS sheet)
        {
            throw new NotImplementedException();
        }

        public bool ValidateDatabaseAtPath(string path)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(CSS sheet)
        {
            throw new NotImplementedException();
        }

        public Task<Folder> GetQuickAccessFolderByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<Folder>> GetQuickAccessFoldersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Note> GetQuickAccessNoteByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<List<Note>> GetQuickAccessNotesAsync()
        {
            throw new NotImplementedException();
        }
    }
}
