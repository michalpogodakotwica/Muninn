using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Saving.MiniJson
{
    public sealed class Serializer
    {
        private readonly StringBuilder _builder;
        private int _indentLevel;
        private int _indentSize;
        private bool _prettyPrint;

        private Serializer()
        {
            _builder = new StringBuilder();
        }

        public static string Serialize(object obj, bool prettyPrint = false, int indentSize = 2)
        {
            var instance = new Serializer
            {
                _prettyPrint = prettyPrint,
                _indentSize = indentSize
            };
            instance.SerializeValue(obj);
            return instance._builder.ToString();
        }

        private void SerializeValue(object value)
        {
            IList asList;
            IDictionary asDict;
            string asStr;

            if (value == null)
                _builder.Append("null");
            else if ((asStr = value as string) != null)
                SerializeString(asStr);
            else if (value is bool)
                _builder.Append((bool)value ? "true" : "false");
            else if ((asList = value as IList) != null)
                SerializeArray(asList);
            else if ((asDict = value as IDictionary) != null)
                SerializeObject(asDict);
            else if (value is char)
                SerializeString(new string((char)value, 1));
            else
                SerializeOther(value);
        }

        private void SerializeObject(IDictionary obj)
        {
            _builder.Append('{');
            _indentLevel++;
            NewLine();

            var first = true;
            foreach (DictionaryEntry entry in obj)
            {
                if (!first)
                {
                    _builder.Append(',');
                    NewLine();
                }

                SerializeObjectEntry(entry);
                first = false;
            }

            NewLine();
            _indentLevel--;
            Indent();
            _builder.Append('}');
        }

        private void SerializeObjectEntry(DictionaryEntry entry)
        {
            Indent();
            SerializeString(entry.Key.ToString());
            _builder.Append(':');
            Space();
            SerializeValue(entry.Value);
        }

        private void SerializeArray(IList anArray)
        {
            var isTopLevelArray = anArray.Cast<object>().Any(x => x is IList);
            if (isTopLevelArray)
                SerializeArray1(anArray);
            else
                SerializeArray2(anArray);
        }

        private void SerializeArray1(IList anArray)
        {
            _builder.Append('[');
            _indentLevel++;
            Indent();
            var first = true;
            foreach (var obj in anArray)
            {
                if (!first)
                {
                    _builder.Append(',');
                    Space();
                }
                NewLine();
                Indent();
                SerializeValue(obj);
                first = false;
            }
            NewLine();
            _indentLevel--;
            Indent();
            _builder.Append(']');
        }

        private void SerializeArray2(IList anArray)
        {
            _builder.Append('[');
            var first = true;
            foreach (var obj in anArray)
            {
                if (!first)
                {
                    _builder.Append(',');
                    Space();
                }
                SerializeValue(obj);
                first = false;
            }

            _builder.Append(']');
        }

        private void SerializeString(string str)
        {
            _builder.Append('\"');

            var charArray = str.ToCharArray();
            foreach (var c in charArray)
                switch (c)
                {
                    case '"':
                        _builder.Append("\\\"");
                        break;
                    case '\\':
                        _builder.Append("\\\\");
                        break;
                    case '\b':
                        _builder.Append("\\b");
                        break;
                    case '\f':
                        _builder.Append("\\f");
                        break;
                    case '\n':
                        _builder.Append("\\n");
                        break;
                    case '\r':
                        _builder.Append("\\r");
                        break;
                    case '\t':
                        _builder.Append("\\t");
                        break;
                    default:
                        var codepoint = Convert.ToInt32(c);
                        if (codepoint >= 32 && codepoint <= 126)
                        {
                            _builder.Append(c);
                        }
                        else
                        {
                            _builder.Append("\\u");
                            _builder.Append(codepoint.ToString("x4"));
                        }

                        break;
                }

            _builder.Append('\"');
        }

        private void SerializeOther(object value)
        {
            // NOTE: decimals lose precision during serialization.
            // They always have, I'm just letting you know.
            // Previously floats and doubles lost precision too.
            if (value is float)
                _builder.Append(((float)value).ToString("R", CultureInfo.InvariantCulture));
            else if (value is int
                      || value is uint
                      || value is long
                      || value is sbyte
                      || value is byte
                      || value is short
                      || value is ushort
                      || value is ulong)
                _builder.Append(value);
            else if (value is double
                      || value is decimal)
                _builder.Append(Convert.ToDouble(value).ToString("R", CultureInfo.InvariantCulture));
            else
                SerializeString(value.ToString());
        }

        private void Indent()
        {
            if (!_prettyPrint)
                return;

            var spaces = _indentLevel * _indentSize;
            for (var i = 0; i < spaces; ++i)
                _builder.Append(' ');
        }

        private void NewLine()
        {
            if (_prettyPrint)
                _builder.Append('\n');
        }

        private void Space()
        {
            if (_prettyPrint)
                _builder.Append(' ');
        }
    }
}