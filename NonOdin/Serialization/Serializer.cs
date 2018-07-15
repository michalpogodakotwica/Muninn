using Patterns.Variables;
using UnityEngine;

namespace Serialization
{
    // Non-generic scriptable object provides ability to drag it in inspector.
    public abstract class Serializer : ScriptableObject
    {
        public abstract void Serialize(Variable[] template, string filePath);
        public abstract void Deserialize(Variable[] template, string filePath);
    }
}