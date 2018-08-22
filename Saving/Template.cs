using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Saving
{
    [CreateAssetMenu(menuName = "Saving/Template")]
    public class Template : SerializedScriptableObject
    {
        [OdinSerialize] private Path _path;
        [OdinSerialize] private ISerializer _serializer;
        [OdinSerialize] public Action OnLoadFail;

        public void Reload()
        {
            _serializer.Restore();
        }

        [HideInEditorMode, Button]
        public void Save()
        {
            _serializer.Save(_path.GetFilePath());
        }

        [HideInEditorMode, Button]
        public void Load()
        {
            try
            {
                _serializer.Load(_path.GetFilePath());
            }
            catch
            {
                OnLoadFail?.Invoke();
            }
        }
    }
}