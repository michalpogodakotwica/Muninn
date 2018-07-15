using System;
using System.Collections.Generic;
using System.Linq;
using Patterns.Variables;

namespace Serialization
{
    public class ClassAdapter : IAdapter<ClassFormat>
    {
        public ClassFormat ToData(Variable[] template)
        {
            return new ClassFormat
            {
                Floats = ToValues<FloatVariable, float>(template),
                Bools = ToValues<BoolVariable, bool>(template),
                Ints = ToValues<IntVariable, int>(template),
                Strings = ToValues<StringVariable, string>(template)
            };
        }

        public void FromData(Variable[] template, ClassFormat data)
        {
            if (!DoesClassFormatCoverAllTemplateTypes(template))
                throw new ArgumentException();

            FromValues<FloatVariable, float>(template, data.Floats);
            FromValues<BoolVariable, bool>(template, data.Bools);
            FromValues<IntVariable, int>(template, data.Ints);
            FromValues<StringVariable, string>(template, data.Strings);
        }

        private static bool DoesClassFormatCoverAllTemplateTypes(IEnumerable<Variable> template)
        {
            var allowedTypes = new[]
            {
                typeof(FloatVariable), typeof(BoolVariable),
                typeof(IntVariable), typeof(StringVariable)
            };
            return template.All(x => allowedTypes.Contains(x.GetType()));
        }

        private static TValue[] ToValues<TVariable, TValue>(IEnumerable<Variable> template)
            where TVariable : Variable<TValue>
        {
            return template
                .Where(o => o.GetType() == typeof(TVariable))
                .Cast<TVariable>()
                .Select(o => o.Serialize())
                .Cast<TValue>()
                .ToArray();
        }

        private static void FromValues<TVariable, TValue>(IEnumerable<Variable> template, IList<TValue> values)
            where TVariable : Variable<TValue>
        {
            var j = 0;
            foreach (var variable in template)
                if (variable.GetType() == typeof(TVariable))
                {
                    variable.Deserialize(values[j]);
                    j++;
                }
        }
    }
}