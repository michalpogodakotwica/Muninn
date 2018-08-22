using System;
using Saving;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Architecture.Variables
{
    public abstract class Variable : SerializedScriptableObject, ISaveable
    {
        public abstract void Restore();
        public abstract void Load(object data);
        public abstract object Save();
    }

    public abstract class Variable<T> : Variable, IValue<T>
    {
        public T Value { get; set; }
        [SerializeField] private T _defaultValue;
        
        private void OnEnable()
        {
            Restore();
        }

        [HideInEditorMode, Button]
        public override void Restore()
        {
            var bytes = SerializationUtility.SerializeValue(_defaultValue, DataFormat.Binary);
            Value = SerializationUtility.DeserializeValue<T>(bytes, DataFormat.Binary);
        }

        public override void Load(object data)
        {
            // Data is a boxed object and sometimes loaded from JSON. Since for example 1 represents both
            // an integer and a float data sometimes can't be casted directly - whole number floats are
            // passed as boxed integer and those can only be casted to integers. Conversion is needed.
            Value = (T)Convert.ChangeType(data, typeof(T)); 
        }

        public override object Save()
        {
            return Value;
        }
    }
}