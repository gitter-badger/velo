using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Velo.Utils;

namespace Velo.Serialization.Tokenization
{
    [DebuggerDisplay("Position: {_position} Value: '{_builder}'")]
    internal ref struct JsonTokenizer
    {
        public const string TokenFalseValue = "false";
        public const string TokenNullValue = "null";
        public const string TokenTrueValue = "true";

        public JsonToken Current { get; private set; }

        private StringBuilder _builder;
        private int _position;
        private string _serialized;

        private bool _disposed;

        public JsonTokenizer(string serialized, StringBuilder stringBuilder = null)
        {
            _serialized = serialized;
            _builder = stringBuilder ?? new StringBuilder();
            _position = 0;
            _disposed = false;
            
            Current = default;
        }

        public bool MoveNext()
        {
            EnsureNotDisposed();

            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];

                if (ch == ',' || ch == ' ')
                {
                    continue;
                }

                if (char.IsDigit(ch) || ch == '-')
                {
                    Current = ReadNumber();
                    return true;
                }

                switch (ch)
                {
                    case '[':
                        Current = Read(JsonTokenType.ArrayStart);
                        return true;
                    case ']':
                        Current = Read(JsonTokenType.ArrayEnd);
                        return true;
                    case '{':
                        Current = Read(JsonTokenType.ObjectStart);
                        return true;
                    case '}':
                        Current = Read(JsonTokenType.ObjectEnd);
                        return true;
                    case '"':
                        var stringValue = ReadString();
                        Current = MaybeProperty(stringValue);
                        return true;
                    case 'n':
                        Current = ReadNull();
                        return true;
                    case 'f':
                        Current = ReadFalse();
                        return true;
                    case 't':
                        Current = ReadTrue();
                        return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            EnsureNotDisposed();
            
            _position = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private readonly void EnsureNotDisposed()
        {
            if (_disposed) throw Error.Disposed(nameof(JsonTokenizer));
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken MaybeProperty(string stringToken)
        {
            var isProperty = _serialized.Length != _position && _serialized[_position] == ':';

            if (!isProperty)
            {
                return new JsonToken(JsonTokenType.String, stringToken);
            }

            SkipChar();
            return new JsonToken(JsonTokenType.Property, stringToken);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken Read(JsonTokenType tokenType)
        {
            SkipChar();
            return new JsonToken(tokenType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadFalse()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JsonToken(JsonTokenType.False);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadTrue()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JsonToken(JsonTokenType.True);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadNull()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (char.IsPunctuation(ch)) break;
            }

            return new JsonToken(JsonTokenType.Null);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private JsonToken ReadNumber()
        {
            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];
                if (!char.IsDigit(ch) && ch != '.') break;

                _builder.Append(ch);
            }

            var result = new JsonToken(JsonTokenType.Number, _builder.ToString());
            _builder.Clear();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private string ReadString()
        {
            SkipChar();

            var serialized = _serialized;
            for (; _position < serialized.Length; _position++)
            {
                var ch = serialized[_position];

                if (ch == '"')
                {
                    SkipChar();
                    break;
                }

                if (ch == '\\')
                {
                    SkipChar();
                    ch = serialized[_position];
                }

                _builder.Append(ch);
            }

            var result = _builder.ToString();
            _builder.Clear();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SkipChar()
        {
            _position++;
        }

        public void Dispose()
        {
            if (_disposed) return;

            Current = default;

            _builder = null;
            _position = -1;
            _serialized = null;

            _disposed = true;
        }
    }
}