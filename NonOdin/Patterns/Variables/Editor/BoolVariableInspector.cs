using UnityEditor;

namespace Patterns.Variables.Editor
{
    [CustomEditor(typeof(BoolVariable))]
    public class BoolVariableInspector : VariableCustomInspector<BoolVariable, bool> { }
}