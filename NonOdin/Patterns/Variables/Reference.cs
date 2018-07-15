namespace Patterns.Variables
{
    public abstract class Reference<TVariable, TVariableType> where TVariableType : Variable<TVariable>
    {
        public bool UseConstant = true;
        public TVariable ConstantValue;
        public TVariableType Variable;

        public TVariable GetValue()
        {
            return UseConstant 
                    ? ConstantValue
                    : (TVariable)Variable.GetValue(); 
        }

        public void SetValue(TVariable newValue)
        {
            if (UseConstant)
                ConstantValue = newValue;
            else
                Variable.SetValue(newValue);
        }
    }
}