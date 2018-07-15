using UnityEngine;
using Sirenix.OdinInspector;

namespace Architecture.Variables
{
    public abstract class Variable : ScriptableObject
    {
        public abstract void RestoreDefaultValue();
        public abstract void SetRuntimeValue(object newValue);
        public abstract object GetBoxedRuntimeValue();
    }

    public abstract class Variable<T> : Variable
    {
        [SerializeField]
        private T _defaultValue;

        [ShowInInspector, ReadOnly, HideInEditorMode]
        public T RuntimeValue { get; set; }

        private void OnEnable()
        {
            RestoreDefaultValue();
        }

        [HideInEditorMode, Button]
        public override void RestoreDefaultValue()
        {
            RuntimeValue = _defaultValue;
        }

        public override void SetRuntimeValue(object newValue)
        {
            RuntimeValue = (T)newValue;
        }

        public override object GetBoxedRuntimeValue()
        {
            return RuntimeValue;
        }
    }
}