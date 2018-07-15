/* Copyright (c) 2013 Calvin Rien
*
* Based on the JSON parser by Patrick van Bergen
* http://techblog.procurios.nl/k/618/news/view/14605/14863/How-do-I-write-my-own-parser-for-JSON.html
*
* Simplified it so that it doesn't throw exceptions
* and can be used in Unity iPhone with maximum code stripping.
*
* Permission is hereby granted, free of charge, to any person obtaining
* a copy of this software and associated documentation files (the
* "Software"), to deal in the Software without restriction, including
* without limitation the rights to use, copy, modify, merge, publish,
* distribute, sublicense, and/or sell copies of the Software, and to
* permit persons to whom the Software is furnished to do so, subject to
* the following conditions:
*
* The above copyright notice and this permission notice shall be
* included in all copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
* EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
* MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
* IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
* CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
* TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
* SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Serialization.MiniJSON
{
    // Example usage:
    //
    //  using UnityEngine;
    //  using System.Collections;
    //  using System.Collections.Generic;
    //  using MiniJSON;
    //
    //  public class MiniJSONTest : MonoBehaviour {
    //      void Start () {
    //          var jsonString = "{ \"array\": [1.44,2,3], " +
    //                          "\"object\": {\"key1\":\"value1\", \"key2\":256}, " +
    //                          "\"string\": \"The quick brown fox \\\"jumps\\\" over the lazy dog \", " +
    //                          "\"unicode\": \"\\u3041 Men\u00fa sesi\u00f3n\", " +
    //                          "\"int\": 65536, " +
    //                          "\"float\": 3.1415926, " +
    //                          "\"bool\": true, " +
    //                          "\"null\": null }";
    //
    //          var dict = Json.Deserialize(jsonString) as Dictionary<string,object>;
    //
    //          Debug.Log("deserialized: " + dict.GetType());
    //          Debug.Log("dict['array'][0]: " + ((List<object>) dict["array"])[0]);
    //          Debug.Log("dict['string']: " + (string) dict["string"]);
    //          Debug.Log("dict['float']: " + (float) dict["float"]);
    //          Debug.Log("dict['int']: " + (long) dict["int"]); // ints come out as longs
    //          Debug.Log("dict['unicode']: " + (string) dict["unicode"]);
    //
    //          var str = Json.Serialize(dict);
    //
    //          Debug.Log("serialized: " + str);
    //      }
    //  }

    /// <summary>
    ///     This class encodes and decodes JSON strings.
    ///     Spec. details, see http://www.json.org/
    ///     JSON uses Arrays and Objects. These correspond here to the datatypes IList and IDictionary.
    ///     All numbers are parsed to floats.
    /// </summary>
    public static class Json
    {
        /// <summary>
        ///     Parses the string json into a value
        /// </summary>
        /// <param name="json">A JSON string.</param>
        /// <returns>An List&lt;object&gt;, a Dictionary&lt;string, object&gt;, a float, an integer,a string, null, true, or false</returns>
        public static object Deserialize(string json)
        {
            // save the string for debug information
            if (json == null)
                return null;

            return Parser.Parse(json);
        }

        /// <summary>
        ///     Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="json">A Dictionary&lt;string, object&gt; / List&lt;object&gt;</param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        /// <summary>
        ///     Converts a IDictionary / IList object or a simple type (string, int, etc.) into a JSON string
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="prettyPrint"></param>
        /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
        public static string Serialize(object obj, bool prettyPrint = false)
        {
            return Serializer.Serialize(obj, prettyPrint);
        }

        private sealed class Parser : IDisposable
        {
            private const string WORD_BREAK = "{}[],:\"";
            private StringReader json;

            private Parser(string jsonString)
            {
                json = new StringReader(jsonString);
            }

            private char PeekChar
            {
                get { return Convert.ToChar(json.Peek()); }
            }

            private char NextChar
            {
                get { return Convert.ToChar(json.Read()); }
            }

            private string NextWord
            {
                get
                {
                    var word = new StringBuilder();

                    while (!IsWordBreak(PeekChar))
                    {
                        word.Append(NextChar);

                        if (json.Peek() == -1)
                            break;
                    }

                    return word.ToString();
                }
            }

            private TOKEN NextToken
            {
                get
                {
                    EatWhitespace();

                    if (json.Peek() == -1)
                        return TOKEN.NONE;

                    switch (PeekChar)
                    {
                        case '{':
                            return TOKEN.CURLY_OPEN;
                        case '}':
                            json.Read();
                            return TOKEN.CURLY_CLOSE;
                        case '[':
                            return TOKEN.SQUARED_OPEN;
                        case ']':
                            json.Read();
                            return TOKEN.SQUARED_CLOSE;
                        case ',':
                            json.Read();
                            return TOKEN.COMMA;
                        case '"':
                            return TOKEN.STRING;
                        case ':':
                            return TOKEN.COLON;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return TOKEN.NUMBER;
                    }

                    switch (NextWord)
                    {
                        case "false":
                            return TOKEN.FALSE;
                        case "true":
                            return TOKEN.TRUE;
                        case "null":
                            return TOKEN.NULL;
                    }

                    return TOKEN.NONE;
                }
            }

            public void Dispose()
            {
                json.Dispose();
                json = null;
            }

            public static bool IsWordBreak(char c)
            {
                return char.IsWhiteSpace(c) || WORD_BREAK.IndexOf(c) != -1;
            }

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString))
                {
                    return instance.ParseValue();
                }
            }

            private Dictionary<string, object> ParseObject()
            {
                var table = new Dictionary<string, object>();

                // ditch opening brace
                json.Read();

                // {
                while (true)
                    switch (NextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.CURLY_CLOSE:
                            return table;
                        default:
                            // name
                            var name = ParseString();
                            if (name == null)
                                return null;

                            // :
                            if (NextToken != TOKEN.COLON)
                                return null;
                            // ditch the colon
                            json.Read();

                            // value
                            table[name] = ParseValue();
                            break;
                    }
            }

            private List<object> ParseArray()
            {
                var array = new List<object>();

                // ditch opening bracket
                json.Read();

                // [
                var parsing = true;
                while (parsing)
                {
                    var nextToken = NextToken;

                    switch (nextToken)
                    {
                        case TOKEN.NONE:
                            return null;
                        case TOKEN.COMMA:
                            continue;
                        case TOKEN.SQUARED_CLOSE:
                            parsing = false;
                            break;
                        default:
                            var value = ParseByToken(nextToken);

                            array.Add(value);
                            break;
                    }
                }

                return array;
            }

            private object ParseValue()
            {
                var nextToken = NextToken;
                return ParseByToken(nextToken);
            }

            private object ParseByToken(TOKEN token)
            {
                switch (token)
                {
                    case TOKEN.STRING:
                        return ParseString();
                    case TOKEN.NUMBER:
                        return ParseNumber();
                    case TOKEN.CURLY_OPEN:
                        return ParseObject();
                    case TOKEN.SQUARED_OPEN:
                        return ParseArray();
                    case TOKEN.TRUE:
                        return true;
                    case TOKEN.FALSE:
                        return false;
                    case TOKEN.NULL:
                        return null;
                    default:
                        return null;
                }
            }

            private string ParseString()
            {
                var s = new StringBuilder();
                char c;

                // ditch opening quote
                json.Read();

                var parsing = true;
                while (parsing)
                {
                    if (json.Peek() == -1)
                    {
                        parsing = false;
                        break;
                    }

                    c = NextChar;
                    switch (c)
                    {
                        case '"':
                            parsing = false;
                            break;
                        case '\\':
                            if (json.Peek() == -1)
                            {
                                parsing = false;
                                break;
                            }

                            c = NextChar;
                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);
                                    break;
                                case 'b':
                                    s.Append('\b');
                                    break;
                                case 'f':
                                    s.Append('\f');
                                    break;
                                case 'n':
                                    s.Append('\n');
                                    break;
                                case 'r':
                                    s.Append('\r');
                                    break;
                                case 't':
                                    s.Append('\t');
                                    break;
                                case 'u':
                                    var hex = new char[4];

                                    for (var i = 0; i < 4; i++)
                                        hex[i] = NextChar;

                                    s.Append((char) Convert.ToInt32(new string(hex), 16));
                                    break;
                            }
                            break;
                        default:
                            s.Append(c);
                            break;
                    }
                }

                return s.ToString();
            }

            private object ParseNumber()
            {
                var number = NextWord;

                if (number.IndexOf('.') == -1)
                {
                    int parsedInt;
                    int.TryParse(number, out parsedInt);
                    return parsedInt;
                }

                float parsedSingle;
                float.TryParse(number, out parsedSingle);
                return parsedSingle;
            }

            private void EatWhitespace()
            {
                while (char.IsWhiteSpace(PeekChar))
                {
                    json.Read();

                    if (json.Peek() == -1)
                        break;
                }
            }

            private enum TOKEN
            {
                NONE,
                CURLY_OPEN,
                CURLY_CLOSE,
                SQUARED_OPEN,
                SQUARED_CLOSE,
                COLON,
                COMMA,
                STRING,
                NUMBER,
                TRUE,
                FALSE,
                NULL
            }
        }

        private sealed class Serializer
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
                    _builder.Append((bool) value ? "true" : "false");
                else if ((asList = value as IList) != null)
                    SerializeArray(asList);
                else if ((asDict = value as IDictionary) != null)
                    SerializeObject(asDict);
                else if (value is char)
                    SerializeString(new string((char) value, 1));
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
                    _builder.Append(((float) value).ToString("R", CultureInfo.InvariantCulture));
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
}