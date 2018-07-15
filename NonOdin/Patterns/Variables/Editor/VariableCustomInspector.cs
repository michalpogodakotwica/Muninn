using UnityEngine;

namespace Patterns.Variables.Editor
{
    public class VariableCustomInspector<TVariable, TVariableType> : UnityEditor.Editor
        where TVariable : Variable<TVariableType>
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var myTarget = (TVariable) target;
            if (GUILayout.Button("Reload Value"))
                myTarget.Restore();
        }
    }
}