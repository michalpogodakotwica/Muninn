using System;
using System.IO;
using System.Linq;
using Sirenix.Serialization;
using UnityEngine;

namespace Saving
{
    [Serializable]
    public class OdinSerializer : ISerializer
    {
        [SerializeField] private DataFormat _format;
        [SerializeField] private ISaveable[] _toSave;
        
        public void Restore()
        {
            foreach (var saveable in _toSave)
                saveable.Restore();
        }

        public void Load(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var loadedData = SerializationUtility.DeserializeValue<object[]>(bytes, _format);

            for (var i = 0; i < loadedData.Length; i++)
                if (i < _toSave.Length)
                    _toSave[i].Load(loadedData[i]);
                else
                    _toSave[i].Restore();
        }

        public void Save(string path)
        {
            var toSave = _toSave.Select(o => o.Save()).ToArray();
            var bytes = SerializationUtility.SerializeValue(toSave, _format);
            File.WriteAllBytes(path, bytes);
        }
    }
}