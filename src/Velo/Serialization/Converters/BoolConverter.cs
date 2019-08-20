using System;
using System.Text;

using Velo.Serialization.Tokenization;

namespace Velo.Serialization.Converters
{
    internal sealed class BoolConverter : IJsonConverter<bool>
    {
        public bool Deserialize(JsonTokenizer tokenizer)
        {
            var token = tokenizer.Current;

            switch (token.TokenType)
            {
                case JsonTokenType.True:
                    return true;
                case JsonTokenType.False:
                    return false;
                default:
                    throw new InvalidOperationException($"Invalid boolean token '{token}'");
            }
        }

        public void Serialize(bool value, StringBuilder builder)
        {
            builder.Append(value ? JsonTokenizer.TrueValue : JsonTokenizer.FalseValue);
        }

        void IJsonConverter.Serialize(object value, StringBuilder builder) => Serialize((bool) value, builder);
    }
}