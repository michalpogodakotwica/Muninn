using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Serialization
{
    public static class FileUtility
    {
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        public static void CreateDirectoryIfDoesNotExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }

        public static string GenerateUniqueFileName(string directoryPath, string baseFileName)
        {
            var i = 0;
            var newFileName = baseFileName;
            while (File.Exists(Path.Combine(directoryPath, newFileName)))
            {
                newFileName = baseFileName + i;
                i++;
            }
            return newFileName;
        }

        public static string[] SortedExistingFiles(string directoryPath, bool ascending)
        {
            var sortedAscending = Directory.GetFiles(directoryPath)
                .Select(f => new FileInfo(f))
                .OrderBy(f => f.LastAccessTime)
                .Select(f => f.Name);

            return (ascending ? sortedAscending : sortedAscending.Reverse()).ToArray();
        }

        public static string[] ExistingFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath)
                .Select(f => new FileInfo(f).Name)
                .ToArray();
        }

        public static string[] ExistingDirectories(string directoryPath)
        {
            return Directory.GetDirectories(directoryPath)
                .Select(f => new FileInfo(f).FullName)
                .ToArray();
        }

        public static string FilePath(Location location)
        {
            switch (location)
            {
                case Location.PersistentDataPath:
                    return Application.persistentDataPath;

                case Location.StreamingAssets:
                    return Application.streamingAssetsPath;

                default:
                    throw new ArgumentOutOfRangeException("location", location, null);
            }
        }
    }
}