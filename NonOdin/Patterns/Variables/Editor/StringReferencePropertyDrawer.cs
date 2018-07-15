using UnityEditor;

namespace Patterns.Variables.Editor
{
    [CustomPropertyDrawer(typeof(StringReference))]
    public class StringReferencePropertyDrawer : ReferencePropertyDrawer { }
}