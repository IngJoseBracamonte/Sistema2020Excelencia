using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SistemaSatHospitalario.WebAPI.Infrastructure.Json
{
    /// <summary>
    /// Convertidor personalizado para deserializar suavemente cadenas vacías o espacios en blanco hacia Nullable Guid (Guid?).
    /// Evita que System.Text.Json lance un JsonException produciendo un HTTP 400 Bad Request cuando el frontend Angular envía "" para selects no elegidos.
    /// </summary>
    public class NullableGuidJsonConverter : JsonConverter<Guid?>
    {
        public override Guid? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null;
                }

                if (Guid.TryParse(stringValue, out Guid result))
                {
                    return result;
                }
            }

            return null;
        }

        public override void Write(Utf8JsonWriter writer, Guid? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}
