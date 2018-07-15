using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Architecture.Variables
{
    [Serializable]
    public class FloatCondition : ICondition
    {
        private const float Tolerance = 0.0001f;
        
        [SerializeField]
        private bool _equalityComparision;

        [SerializeField, HideIf("_equalityComparision"), HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(1)]
        private Comparision _comparision;

        [SerializeField, ShowIf("_equalityComparision"), HideLabel, HorizontalGroup("EquationLine"), PropertyOrder(1)]
        private Equality _equality;

        [SerializeField, HorizontalGroup("EquationLine", 0.4f), PropertyOrder(0), HideLabel]
        private Reference<float> _first;

        [SerializeField, HorizontalGroup("EquationLine", 0.4f), PropertyOrder(2), HideLabel]
        private Reference<float> _second;

        public bool IsMet()
        {
            if (_equalityComparision)
                switch (_equality)
                {
                    case Equality.Equal:
                        return Math.Abs(_first.Value - _second.Value) < Tolerance;
                    case Equality.NotEqual:
                        return Math.Abs(_first.Value - _second.Value) > Tolerance;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            switch (_comparision)
            {
                case Comparision.Less:
                    return _first.Value < _second.Value;
                case Comparision.LessEqual:
                    return _first.Value <= _second.Value;
                case Comparision.MoreEqual:
                    return _first.Value >= _second.Value;
                case Comparision.More:
                    return _first.Value > _second.Value;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}