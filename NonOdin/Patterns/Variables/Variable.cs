using UnityEngine;

namespace Patterns.Variables
{
    // Scriptable Object enable assigning this in inspector.
    public abstract class Variable : ScriptableObject
    {
        public abstract void Restore();
        public abstract object Serialize();
        public abstract void Deserialize(object newValue);
    }

    public abstract class Variable<T> : Variable
    {
        [SerializeField] private T _defaultValue;
        private T _runtimeValue;

        private void OnEnable()
        {
            Restore();
        }

        public override void Restore()
        {
            _runtimeValue = _defaultValue;
        }

        public override void Deserialize(object newValue)
        {
            SetValue((T) newValue);
        }

        public override object Serialize()
        {
            return _runtimeValue;
        }

        public void SetValue(T newValue)
        {
            _runtimeValue = newValue;
        }

        public T GetValue()
        {
            return _runtimeValue;
        }
    }
}