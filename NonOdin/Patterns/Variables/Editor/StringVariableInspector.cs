using UnityEditor;

namespace Patterns.Variables.Editor
{
    [CustomEditor(typeof(StringVariable))]
    public class StringVariableInspector : VariableCustomInspector<StringVariable, string> { }
}