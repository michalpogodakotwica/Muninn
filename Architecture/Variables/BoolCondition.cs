using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Architecture.Variables
{
    [Serializable]
    public class BoolCondition : ICondition
    {
        [SerializeField, HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(0)]
        private Equality _equality;

        [SerializeField, HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(1)]
        private Reference<bool> _reference;

        public bool IsMet()
        {
            switch (_equality)
            {
                case Equality.Equal:
                    return _reference.Value;
                case Equality.NotEqual:
                    return !_reference.Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}