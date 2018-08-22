using System;
using System.IO;
using System.Linq;
using Architecture.Variables;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Saving
{
    [Serializable]
    public class Path
    {
        [SerializeField] private string _pathPrefix;
        [SerializeField] private string _pathSuffix;
        [OdinSerialize] private Reference<string> _path;
        [SerializeField] private Location _location;

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

        private string GetDirectoryPath()
        {
            var root = LocationBasedPathRoot();
            return _pathPrefix.IsNullOrWhitespace()
                ? root
                : System.IO.Path.Combine(root, _pathPrefix);
        }

        private string GetFilePath(string path)
        {
            var directory = GetDirectoryPath();
            return _pathSuffix.IsNullOrWhitespace()
                ? System.IO.Path.Combine(directory, path)
                : System.IO.Path.Combine(directory, path, _pathSuffix);
        }

        public string GetFilePath()
        {
            return GetFilePath(_path.Value);
        }

        public void SetNewUniquePath()
        {
            while (File.Exists(GetFilePath()))
                _path.Value = System.IO.Path.GetRandomFileName();
        }

        public void SetPath(string newPath)
        {
            _path.Value = newPath;
        }

        public string[] AvailableFiles()
        {
            return AvailableFilePaths().Select(System.IO.Path.GetFileNameWithoutExtension).ToArray();
        }

        public string[] AvailableFilePaths()
        {
            var directoryPath = GetDirectoryPath();
            var files = _pathSuffix.IsNullOrWhitespace()
                ? Directory.GetFiles(directoryPath)
                : Directory.GetDirectories(directoryPath);
            return files;
        }
    }
}