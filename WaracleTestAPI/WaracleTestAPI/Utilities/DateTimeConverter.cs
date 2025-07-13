using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WaracleTestAPI.Utilities
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd/MM/yyyy";
        private static readonly string[] SupportedFormats = 
        {
            "dd/MM/yyyy",                    // Our preferred format
            "yyyy-MM-dd",                    // ISO database format
            "MM/dd/yyyy",                    // US format
            "dd-MM-yyyy",                    // Alternative format
            "yyyy/MM/dd",                    // Alternative ISO format
            "dd.MM.yyyy",                    // European format with dots
            "yyyy-MM-ddTHH:mm:ss.fffZ",     // ISO 8601 UTC with milliseconds
            "yyyy-MM-ddTHH:mm:ssZ",         // ISO 8601 UTC
            "yyyy-MM-ddTHH:mm:ss.fff",      // ISO 8601 with milliseconds
            "yyyy-MM-ddTHH:mm:ss",          // ISO 8601 basic
            "yyyy-MM-dd HH:mm:ss",          // SQL Server format
            "dd/MM/yyyy HH:mm:ss"           // DD/MM/YYYY with time
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                {
                    throw new JsonException($"Date string cannot be null or empty. Expected format: {DateFormat}");
                }

                // Try parsing with all supported formats
                foreach (var format in SupportedFormats)
                {
                    if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        return date;
                    }
                }
                
                // Try parsing ISO 8601 formats with timezone handling
                if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime isoDate))
                {
                    return isoDate;
                }
                
                // Try parsing with DateTimeOffset for timezone support, then convert to DateTime
                if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateTimeOffset))
                {
                    return dateTimeOffset.DateTime;
                }
                
                // Fallback to default parsing with both InvariantCulture and current culture
                if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fallbackDate))
                {
                    return fallbackDate;
                }
                
                if (DateTime.TryParse(dateString, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime cultureDate))
                {
                    return cultureDate;
                }
                
                // Check if it's an invalid date format and provide specific guidance
                if (dateString.Contains("T") && dateString.Contains("Z"))
                {
                    throw new JsonException($"Unable to parse ISO 8601 datetime '{dateString}'. Check that the date is valid (e.g., month 1-12, day 1-31). For date-only values, use format: {DateFormat}");
                }
                
                // If all parsing fails, provide detailed error message
                throw new JsonException($"Unable to parse date '{dateString}'. Expected format: {DateFormat}. Supported formats: {string.Join(", ", SupportedFormats)}");
            }
            
            throw new JsonException($"Expected string token for date parsing, got {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }

    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        private const string DateFormat = "dd/MM/yyyy";
        private static readonly string[] SupportedFormats = 
        {
            "dd/MM/yyyy",                    // Our preferred format
            "yyyy-MM-dd",                    // ISO database format
            "MM/dd/yyyy",                    // US format
            "dd-MM-yyyy",                    // Alternative format
            "yyyy/MM/dd",                    // Alternative ISO format
            "dd.MM.yyyy",                    // European format with dots
            "yyyy-MM-ddTHH:mm:ss.fffZ",     // ISO 8601 UTC with milliseconds
            "yyyy-MM-ddTHH:mm:ssZ",         // ISO 8601 UTC
            "yyyy-MM-ddTHH:mm:ss.fff",      // ISO 8601 with milliseconds
            "yyyy-MM-ddTHH:mm:ss",          // ISO 8601 basic
            "yyyy-MM-dd HH:mm:ss",          // SQL Server format
            "dd/MM/yyyy HH:mm:ss"           // DD/MM/YYYY with time
        };

        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var dateString = reader.GetString();
                if (string.IsNullOrEmpty(dateString))
                {
                    return null;
                }

                // Try parsing with all supported formats
                foreach (var format in SupportedFormats)
                {
                    if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                    {
                        return date;
                    }
                }
                
                // Try parsing ISO 8601 formats with timezone handling
                if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime isoDate))
                {
                    return isoDate;
                }
                
                // Try parsing with DateTimeOffset for timezone support, then convert to DateTime
                if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTimeOffset dateTimeOffset))
                {
                    return dateTimeOffset.DateTime;
                }
                
                // Fallback to default parsing with both InvariantCulture and current culture
                if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fallbackDate))
                {
                    return fallbackDate;
                }
                
                if (DateTime.TryParse(dateString, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime cultureDate))
                {
                    return cultureDate;
                }
                
                // Check if it's an invalid date format and provide specific guidance
                if (dateString.Contains("T") && dateString.Contains("Z"))
                {
                    throw new JsonException($"Unable to parse ISO 8601 datetime '{dateString}'. Check that the date is valid (e.g., month 1-12, day 1-31). For date-only values, use format: {DateFormat}");
                }
                
                // If all parsing fails, provide detailed error message
                throw new JsonException($"Unable to parse date '{dateString}'. Expected format: {DateFormat}. Supported formats: {string.Join(", ", SupportedFormats)}");
            }
            
            throw new JsonException($"Expected string or null token for date parsing, got {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(value.Value.ToString(DateFormat, CultureInfo.InvariantCulture));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
} 