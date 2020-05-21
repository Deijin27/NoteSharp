﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using Notes.Models;
using Xamarin.Forms.Internals;
using System.Linq;
using System.Collections;
using SQLitePCL;

namespace Notes.Data
{
    public class NoteDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public NoteDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Note>().Wait();
            _database.CreateTableAsync<Folder>().Wait();
            _database.CreateTableAsync<CSS>().Wait();
        }

        public Task<List<Note>> GetNotesAsync(int folderID)
        {
            var query = _database.Table<Note>()
                                 .Where(i => i.FolderID == folderID);

            SortingMode sortingMode = App.GetSortingMode();

            switch (sortingMode)
            {
                case SortingMode.Name:
                    query = query.OrderBy(i => i.Name);
                    break;
                case SortingMode.DateCreated:
                    query = query.OrderByDescending(i => i.DateCreated);
                    break;
                case SortingMode.DateModified:
                    query = query.OrderByDescending(i => i.DateModified);
                    break;
                default:
                    query = query.OrderBy(i => i.Name);
                    break;
            }

            return query.ToListAsync();

        }

        public async Task<bool> DoesCSSNameExist(string name)
        {
            int count = await _database.Table<CSS>()
                                 .Where(i => i.Name == name)
                                 .CountAsync();

            return count > 0;
        }

        public Task<List<CSS>> GetSheetsAsync()
        {
            return _database.Table<CSS>()
                            .OrderBy(i => i.Name)
                            .ToListAsync();
        }

        public Task<List<Folder>> GetFoldersAsync(int folderID)
        {
            return _database.Table<Folder>()
                            .Where(i => i.ParentID == folderID)
                            .OrderBy(i => i.Name)
                            .ToListAsync();
        }

        public Task<Note> GetNoteAsync(int id)
        {
            return _database.Table<Note>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<CSS> GetSheetAsync(int id)
        {
            return _database.Table<CSS>()
                            .Where(i => i.ID == id)
                            .FirstOrDefaultAsync();
        }

        public Task<int> SaveNoteAsync(Note note)
        {
            if (note.ID != 0)
            {
                return _database.UpdateAsync(note);
            }
            else
            {
                return _database.InsertAsync(note);
            }
        }

        public Task<int> SaveSheetAsync(CSS sheet)
        {
            if (sheet.ID != 0)
            {
                return _database.UpdateAsync(sheet);
            }
            else
            {
                return _database.InsertAsync(sheet);
            }
        }

        public Task<int> DeleteSheetAsync(CSS sheet)
        {
            return _database.DeleteAsync(sheet);
        }

        public Task<int> DeleteNoteAsync(Note note)
        {
            return _database.DeleteAsync(note);
        }

        public Task<int> CreateFolderAsync(Folder folder)
        {
            return _database.InsertAsync(folder);
        }
    }
}
