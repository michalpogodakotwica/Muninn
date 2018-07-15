using System;
using System.IO;
using Patterns.Observer;
using Patterns.Variables;
using UnityEngine;

namespace Serialization
{
    [CreateAssetMenu(menuName = "Serialization/Template")]
    public class Template : ScriptableObject
    {
        public Subject _loadEvent;
        [SerializeField] private Location _location;
        [SerializeField] private StringReference[] _relativePath;
        [SerializeField] private Serializer _serializer;
        [Tooltip("Those variables don't restore on load!")] [SerializeField] private Variable[] _variables;

        private string GetPath(bool containsFileName)
        {
            if (_relativePath.Length == 0)
                Debug.LogError("File path is empty.");

            var path = FileUtility.FilePath(_location);
            for (var i = 0; i < _relativePath.Length - 1; i++)
                path = Path.Combine(path, _relativePath[i].GetValue());
            Directory.CreateDirectory(path);
            return containsFileName ? Path.Combine(path, _relativePath[_relativePath.Length - 1].GetValue()) : path;
        }

        public void New()
        {
            if (_relativePath.Length == 0)
            {
                Debug.LogError("File path is empty.");
                return;
            }

            _relativePath[_relativePath.Length - 1].SetValue(FileUtility.GenerateUniqueFileName(GetPath(false), name));
            foreach (var variable in _variables)
                variable.Restore();
            if (_loadEvent)
                _loadEvent.Invoke();
        }

        public void Save()
        {
            _serializer.Serialize(_variables, GetPath(true));
        }

        public void Load(Action onLoadFail)
        {
            try
            {
                _serializer.Deserialize(_variables, GetPath(true));
                if (_loadEvent != null)
                    _loadEvent.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't load file. " + e.Message);
                onLoadFail.Invoke();
            }
        }
    }
}