using UnityEditor;

namespace Patterns.Variables.Editor
{
    [CustomEditor(typeof(IntVariable))]
    public class IntVariableInspector : VariableCustomInspector<IntVariable, int> { }
}