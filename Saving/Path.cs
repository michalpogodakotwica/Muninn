using System;
using System.IO;
using System.Linq;
using Architecture.Variables;
using UnityEngine;

namespace Saving
{
    [Serializable]
    public class Path
    {
        [SerializeField] private Reference<string>[] _relativePath;
        [SerializeField] private Location _location;
        
        public string GenerateNewPath()
        {
            var newFilePath = GetFullPath();
            while (File.Exists(newFilePath))
            {
                _relativePath.Last().Value = System.IO.Path.GetRandomFileName();
                newFilePath = GetFullPath();
            }
            return newFilePath;
        }

        public string GetFullPath()
        {
            var root = LocationBasedPathRoot();
            var path = System.IO.Path.Combine(_relativePath.Select(v => v.Value).ToArray());
            return System.IO.Path.Combine(root, path);
        }

        private string LocationBasedPathRoot()
        {
            switch (_location)
            {
                case Location.PersistentDataPath:
                    return Application.persistentDataPath;
                case Location.StreamingAssets:
                    return Application.streamingAssetsPath;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_location), _location, null);
            }
        }

        public static string[] AccessTimeSortedExistingFiles(string directoryPath, bool ascending)
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
    }
}