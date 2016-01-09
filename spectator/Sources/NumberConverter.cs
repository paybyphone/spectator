using System;
using Newtonsoft.Json;

namespace spectator.Sources
{
    public class NumberConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var number = value as Number;

            if (number == null)
            {
                throw new InvalidCastException("Could not cast given value to a Number");
            }

            if (number.IntegerValue.HasValue)
            {
                writer.WriteValue(number.IntegerValue.Value);
            }

            if (number.FloatValue.HasValue)
            {
                writer.WriteValue(number.FloatValue);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var asFloat = existingValue as float?;

            if (asFloat != null)
            {
                return new Number(asFloat.Value);
            }

            var asInteger = existingValue as int?;

            if (asInteger != null)
            {
                return new Number(asInteger.Value);
            }

            throw new NotSupportedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Number).IsAssignableFrom(objectType);
        }
    }
}