using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Architecture.Variables
{
    [Serializable]
    public class Reference<TVariable> : IValue<TVariable>
    {
        [SerializeField, HideInPlayMode, HorizontalGroup]
        private bool UseConstant = true;
        [SerializeField, ShowIf("UseConstant"), HideInPlayMode, HorizontalGroup, HideLabel]
        private TVariable ConstantValue;
        [SerializeField, HideIf("UseConstant"), HideInPlayMode, HorizontalGroup, HideLabel]
        private Variable<TVariable> Variable;

        [ShowInInspector, HideInEditorMode, HideLabel]
        public TVariable Value
        {
            get { return UseConstant ? ConstantValue : ( Variable ? Variable.Value : default(TVariable)); }

            set
            {
                if (UseConstant)
                    ConstantValue = value;
                else
                    Variable.Value = value;
            }
        }
    }
}