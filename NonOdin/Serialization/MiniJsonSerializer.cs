using System.Collections.Generic;
using System.IO;
using Patterns.Variables;
using Serialization.MiniJSON;
using UnityEngine;

namespace Serialization
{
    [CreateAssetMenu(menuName = "Serialization/MiniJsonSerializer")]
    public class MiniJsonSerializer : Serializer
    {
        public override void Serialize(Variable[] template, string filePath)
        {
            File.WriteAllText(filePath, Json.Serialize(new DictionaryAdapter().ToData(template), true));
        }

        public override void Deserialize(Variable[] template, string filePath)
        {
            new DictionaryAdapter().FromData(template,
                Json.Deserialize(File.ReadAllText(filePath)) as Dictionary<string, object>);
        }
    }
}