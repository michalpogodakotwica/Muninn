using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Architecture.Variables
{
    [Serializable]
    public class Reference<TVariable>
    {
        [SerializeField, HideInPlayMode]
        private bool UseConstant = true;
        [SerializeField, ShowIf("UseConstant"), HideInPlayMode]
        private TVariable ConstantValue;
        [SerializeField, HideIf("UseConstant"), HideInPlayMode]
        private Variable<TVariable> Variable;

        [ShowInInspector, HideInEditorMode, HideLabel]
        public TVariable Value
        {
            get { return UseConstant ? ConstantValue : ( Variable ? Variable.RuntimeValue : default(TVariable)); }

            set
            {
                if (UseConstant)
                    ConstantValue = value;
                else
                    Variable.RuntimeValue = value;
            }
        }
    }
}