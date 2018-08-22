using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

namespace Saving.MiniJson
{
    public sealed class Parser : IDisposable
    {
        private const string WordBreak = "{}[],:\"";
        private StringReader _json;

        private Parser(string jsonString)
        {
            _json = new StringReader(jsonString);
        }

        private char PeekChar => Convert.ToChar(_json.Peek());
        private char NextChar => Convert.ToChar(_json.Read());

        private string NextWord
        {
            get
            {
                var word = new StringBuilder();

                while (!IsWordBreak(PeekChar))
                {
                    word.Append(NextChar);

                    if (_json.Peek() == -1)
                        break;
                }

                return word.ToString();
            }
        }

        private Token NextToken
        {
            get
            {
                EatWhitespace();

                if (_json.Peek() == -1)
                    return Token.None;

                switch (PeekChar)
                {
                    case '{':
                        return Token.CurlyOpen;
                    case '}':
                        _json.Read();
                        return Token.CurlyClose;
                    case '[':
                        return Token.SquaredOpen;
                    case ']':
                        _json.Read();
                        return Token.SquaredClose;
                    case ',':
                        _json.Read();
                        return Token.Comma;
                    case '"':
                        return Token.String;
                    case ':':
                        return Token.Colon;
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
                        return Token.Number;
                }

                switch (NextWord)
                {
                    case "false":
                        return Token.False;
                    case "true":
                        return Token.True;
                    case "null":
                        return Token.Null;
                }

                return Token.None;
            }
        }

        public void Dispose()
        {
            _json.Dispose();
            _json = null;
        }

        public static bool IsWordBreak(char c)
        {
            return char.IsWhiteSpace(c) || WordBreak.IndexOf(c) != -1;
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
            _json.Read();

            // {
            while (true)
                switch (NextToken)
                {
                    case Token.None:
                        return null;
                    case Token.Comma:
                        continue;
                    case Token.CurlyClose:
                        return table;
                    default:
                        // name
                        var name = ParseString();
                        if (name == null)
                            return null;

                        // :
                        if (NextToken != Token.Colon)
                            return null;
                        // ditch the colon
                        _json.Read();

                        // value
                        table[name] = ParseValue();
                        break;
                }
        }

        private List<object> ParseArray()
        {
            var array = new List<object>();

            // ditch opening bracket
            _json.Read();

            // [
            var parsing = true;
            while (parsing)
            {
                var nextToken = NextToken;

                switch (nextToken)
                {
                    case Token.None:
                        return null;
                    case Token.Comma:
                        continue;
                    case Token.SquaredClose:
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

        private object ParseByToken(Token token)
        {
            switch (token)
            {
                case Token.String:
                    return ParseString();
                case Token.Number:
                    return ParseNumber();
                case Token.CurlyOpen:
                    return ParseObject();
                case Token.SquaredOpen:
                    return ParseArray();
                case Token.True:
                    return true;
                case Token.False:
                    return false;
                case Token.Null:
                    return null;
                default:
                    return null;
            }
        }

        private string ParseString()
        {
            var s = new StringBuilder();

            // ditch opening quote
            _json.Read();

            var parsing = true;
            while (parsing)
            {
                if (_json.Peek() == -1)
                    break;

                var c = NextChar;
                switch (c)
                {
                    case '"':
                        parsing = false;
                        break;

                    case '\\':
                        if (_json.Peek() == -1)
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

                                s.Append((char)Convert.ToInt32(new string(hex), 16));
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
            float.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedSingle);
            return parsedSingle;
        }

        private void EatWhitespace()
        {
            while (char.IsWhiteSpace(PeekChar))
            {
                _json.Read();

                if (_json.Peek() == -1)
                    break;
            }
        }

        private enum Token
        {
            None,
            CurlyOpen,
            CurlyClose,
            SquaredOpen,
            SquaredClose,
            Colon,
            Comma,
            String,
            Number,
            True,
            False,
            Null
        }
    }

}