using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Patterns.Variables;
using UnityEngine;

namespace Serialization
{
    [CreateAssetMenu(menuName = "Serialization/BinarySerializer")]
    public class BinarySerializer : Serializer
    {
        public override void Serialize(Variable[] template, string filePath)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var file = File.Create(filePath))
            {
                binaryFormatter.Serialize(file, new ClassAdapter().ToData(template));
            }
        }

        public override void Deserialize(Variable[] template, string filePath)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var file = File.Open(filePath, FileMode.Open))
            {
                new ClassAdapter().FromData(template, (ClassFormat) binaryFormatter.Deserialize(file));
            }
        }
    }
}