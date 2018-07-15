using System.Collections.Generic;
using System.Linq;
using Patterns.Variables;

namespace Serialization
{
    public class DictionaryAdapter : IAdapter<Dictionary<string, object>>
    {
        public Dictionary<string, object> ToData(Variable[] template)
        {
            return template.ToDictionary(variable => variable.name, variable => variable.Serialize());
        }

        public void FromData(Variable[] template, Dictionary<string, object> data)
        {
            foreach (var variable in template)
                variable.Deserialize(data[variable.name]);
        }
    }
}