using System.IO;
using Patterns.Variables;
using UnityEngine;

namespace Serialization
{
    [CreateAssetMenu(menuName = "Serialization/JsonSerializer")]
    public class JsonSerializer : Serializer
    {
        public override void Serialize(Variable[] template, string filePath)
        {
            File.WriteAllText(filePath, JsonUtility.ToJson(new ClassAdapter().ToData(template), true));
        }

        public override void Deserialize(Variable[] template, string filePath)
        {
            new ClassAdapter().FromData(template, JsonUtility.FromJson<ClassFormat>(File.ReadAllText(filePath)));
        }
    }
}