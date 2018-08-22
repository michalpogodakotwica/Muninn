using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Architecture.Variables;
using UnityEngine;

namespace Saving
{
    [Serializable]
    public class MiniJsonSerializer : ISerializer
    {
        [SerializeField] private Variable[] _toSave;

        public void Restore()
        {
            foreach (var variable in _toSave)
                variable.Restore();
        }

        public void Load(string path)
        {
            var jsonText = File.ReadAllText(path);
            var dictionary = (Dictionary<string, object>)MiniJson.MiniJson.Deserialize(jsonText);

            foreach (var variable in _toSave)
            {
                if (dictionary.ContainsKey(variable.name))
                    variable.Load(dictionary[variable.name]);
                else
                    variable.Restore();
            }
        }

        public void Save(string path)
        {
            var dictionary = _toSave.ToDictionary(variable => variable.name, variable => variable.Save());
            var jsonText = MiniJson.MiniJson.Serialize(dictionary, true);
            File.WriteAllText(path, jsonText);
        }
    }
}