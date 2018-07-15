using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Architecture.Variables
{
    [Serializable]
    public class StringCondition : ICondition
    {
        [SerializeField, HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(1)]
        private Equality _equality;

        [SerializeField, HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(0)]
        private Reference<string> _first;

        [SerializeField, HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(2)]
        private Reference<string> _second;

        public bool IsMet()
        {
            switch (_equality)
            {
                case Equality.Equal:
                    return _first.Value == _second.Value;
                case Equality.NotEqual:
                    return _first.Value != _second.Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}