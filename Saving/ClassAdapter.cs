using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Architecture.Variables;
using UnityEngine;

namespace Saving
{
    [Serializable]
    public class Class : ISaveableVariableCollection
    {
        [SerializeField] private Variable<bool>[] _bools;
        [SerializeField] private Variable<int>[] _ints;
        [SerializeField] private Variable<float>[] _floats;
        [SerializeField] private Variable<string>[] _strings;

        public void FromFile(string path)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var file = File.Open(path, FileMode.Open))
            {
                var data = (DataFormat)binaryFormatter.Deserialize(file);
                
                for(var i = 0; i < data.Bools.Length; i++)
                    _bools[i].RuntimeValue = data.Bools[i];
                for (var i = data.Bools.Length; i < _bools.Length; i++)
                    _bools[i].RestoreDefaultValue();

                for (var i = 0; i < data.Ints.Length; i++)
                    _ints[i].RuntimeValue = data.Ints[i];
                for (var i = data.Ints.Length; i < _ints.Length; i++)
                    _ints[i].RestoreDefaultValue();

                for (var i = 0; i < data.Floats.Length; i++)
                    _floats[i].RuntimeValue = data.Floats[i];
                for (var i = data.Floats.Length; i < _floats.Length; i++)
                    _floats[i].RestoreDefaultValue();

                for (var i = 0; i < data.Strings.Length; i++)
                    _strings[i].RuntimeValue = data.Strings[i];
                for (var i = data.Strings.Length; i < _strings.Length; i++)
                    _strings[i].RestoreDefaultValue();
            }
        }

        public void ToFile(string path)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var file = File.Create(path))
            {
                var data = new DataFormat
                {
                    Bools = ToValues(_bools),
                    Ints = ToValues(_ints),
                    Floats = ToValues(_floats),
                    Strings = ToValues(_strings)
                };
                binaryFormatter.Serialize(file, data);
            }
        }

        public void RestoreDefaultValues()
        {
            foreach (var variable in _bools)
                variable.RestoreDefaultValue();
            foreach (var variable in _ints)
                variable.RestoreDefaultValue();
            foreach (var variable in _floats)
                variable.RestoreDefaultValue();
            foreach (var variable in _strings)
                variable.RestoreDefaultValue();
        }

        public void UnloadVariables()
        {
            foreach (var variable in _bools)
                Resources.UnloadAsset(variable);
            foreach (var variable in _ints)
                Resources.UnloadAsset(variable);
            foreach (var variable in _floats)
                Resources.UnloadAsset(variable);
            foreach (var variable in _strings)
                Resources.UnloadAsset(variable);
        }

        private static T[] ToValues<T>(IEnumerable<Variable<T>> variables)
        {
            return variables
                .Select(o => o.RuntimeValue)
                .ToArray();
        }
        
        [Serializable]
        public class DataFormat
        {
            public bool[] Bools;
            public int[] Ints;
            public float[] Floats;
            public string[] Strings;
        }
    }
}