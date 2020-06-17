using Notes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Notes.Data
{
    public enum SortingMode
    {
        Name,
        DateCreated,
        DateModified,
        Size
    }

    static class SortingExtensions
    {
        public static long ByteCount(this string stringToTest)
        {
            return (long)stringToTest.Length * sizeof(char);
        }

        public static string ToReadableByteCountString(this long byteCount)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (byteCount == 0)
                return $"0 {suf[0]}";
            long bytes = Math.Abs(byteCount);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
            double num = Math.Round(bytes / Math.Pow(1024, place), 1);
            return $"{Math.Sign(byteCount) * num} {suf[place]}";
        }

        public static IOrderedEnumerable<Folder> OrderBySortingMode(this IEnumerable<Folder> folders, SortingMode sortingMode)
        { 
            switch (sortingMode)
            {
                case SortingMode.DateCreated:
                    return folders.OrderByDescending(i => i.DateCreated);
                case SortingMode.DateModified:
                    return folders.OrderByDescending(i => i.DateModified);
                case SortingMode.Name:
                default:
                    return folders.OrderBy(i => i.Name);
            }
        }

        public static IOrderedEnumerable<Note> OrderBySortingMode(this IEnumerable<Note> notes, SortingMode sortingMode)
        {
            switch (sortingMode)
            {
                case SortingMode.DateCreated:
                    return notes.OrderByDescending(i => i.DateCreated);
                case SortingMode.DateModified:
                    return notes.OrderByDescending(i => i.DateModified);
                case SortingMode.Size:
                    return notes.OrderByDescending(i => i.Text.ByteCount());
                case SortingMode.Name:
                default:
                    return notes.OrderBy(i => i.Name);
            }
        }

        public static IOrderedEnumerable<CSS> OrderBySortingMode(this IEnumerable<CSS> sheets, SortingMode sortingMode)
        {
            switch (sortingMode)
            {
                case SortingMode.DateCreated:
                    //return sheets.OrderByDescending(i => i.DateCreated).ToList();
                case SortingMode.DateModified:
                    //return sheets.OrderByDescending(i => i.DateModified).ToList();
                case SortingMode.Size:
                    //return sheets.OrderByDescending(i => i.Text.ByteCount()).ToList();
                case SortingMode.Name:
                default:
                    return sheets.OrderBy(i => i.Name);
            }
        }
    }
}
