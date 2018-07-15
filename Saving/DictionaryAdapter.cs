using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Architecture.Variables;
using UnityEngine;

namespace Saving
{
    [Serializable]
    public class Dictionary : ISaveableVariableCollection
    {
        [SerializeField] private Variable[] _variables;
        [SerializeField] private bool _prettyPrint;

        public void FromFile(string path)
        {
            var data = (Dictionary<string, object>)MiniJson.Deserialize(File.ReadAllText(path));
            foreach (var variable in _variables)
                variable.SetRuntimeValue(data[variable.name]);
        }

        public void ToFile(string path)
        {
            var dictionary =_variables.
                ToDictionary(variable => variable.name, variable => variable.GetBoxedRuntimeValue());

            if(dictionary.Count > _variables.Length)
                foreach (var variable in _variables.Where(o => dictionary.ContainsKey(o.name)))
                    variable.RestoreDefaultValue();

            File.WriteAllText(path, MiniJson.Serialize(dictionary, _prettyPrint));
        }

        public void RestoreDefaultValues()
        {
            foreach (var variable in _variables)
                variable.RestoreDefaultValue();
        }

        public void UnloadVariables()
        {
            foreach (var variable in _variables)
                Resources.UnloadAsset(variable);
        }
    }
}