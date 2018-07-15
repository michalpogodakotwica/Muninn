using System;
using Architecture.DependencyInjection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Saving
{
    [CreateAssetMenu(menuName = "Saving/Template")]
    public class Template : SerializedScriptableObject
    {
        [SerializeField] private Subject _loadEvent;
        [SerializeField] private Path _path;
        [SerializeField] private ISaveableVariableCollection _saveableVariableCollection;
        [SerializeField] private Action _onLoadFail;
        
        public void New()
        {
            _path.GenerateNewPath();
            _saveableVariableCollection.RestoreDefaultValues();
            if (_loadEvent)
                _loadEvent.Invoke();
            Save();
        }

        public void Save()
        {
            _saveableVariableCollection.ToFile(_path.GetFullPath());
        }

        public void Load(Action onLoadFail)
        {
            try
            {
                _saveableVariableCollection.FromFile(_path.GetFullPath());
                if (_loadEvent != null)
                    _loadEvent.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError("Couldn't load file. " + e.Message);
                _onLoadFail?.Invoke();
            }
        }
    }
}