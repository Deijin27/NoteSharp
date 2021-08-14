using Notes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Notes
{
    public enum SortingMode
    {
        Name,
        DateCreated,
        DateModified,
        Size
    }

    public static class SortingExtensions
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
            return sortingMode switch
            {
                SortingMode.DateCreated => folders.OrderByDescending(i => i.DateCreated),
                SortingMode.DateModified => folders.OrderByDescending(i => i.DateModified),
                _ => folders.OrderBy(i => i.Name),
            };
        }

        public static IOrderedEnumerable<Note> OrderBySortingMode(this IEnumerable<Note> notes, SortingMode sortingMode)
        {
            return sortingMode switch
            {
                SortingMode.DateCreated => notes.OrderByDescending(i => i.DateCreated),
                SortingMode.DateModified => notes.OrderByDescending(i => i.DateModified),
                SortingMode.Size => notes.OrderByDescending(i => i.Text.ByteCount()),
                _ => notes.OrderBy(i => i.Name),
            };
        }
    }
}
