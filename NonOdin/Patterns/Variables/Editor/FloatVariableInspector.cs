using UnityEditor;

namespace Patterns.Variables.Editor
{
    [CustomEditor(typeof(FloatVariable))]
    public class FloatVariableInspector : VariableCustomInspector<FloatVariable, float> { }
}