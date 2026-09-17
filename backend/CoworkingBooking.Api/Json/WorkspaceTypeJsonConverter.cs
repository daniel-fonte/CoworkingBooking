using CoworkingBooking.Core.Workspace.Enums;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CoworkingBooking.Api.Json
{
    public class WorkspaceTypeJsonConverter : JsonConverter<WorkspaceType>
    {
        public override WorkspaceType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var raw = reader.GetString();

                if (!string.IsNullOrWhiteSpace(raw) && TryParseWorkspaceType(raw, out var parsed))
                {
                    return parsed;
                }
            }

            if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var numberValue))
            {
                if (Enum.IsDefined(typeof(WorkspaceType), numberValue))
                {
                    return (WorkspaceType)numberValue;
                }
            }

            throw new JsonException("Invalid workspace type.");
        }

        public override void Write(Utf8JsonWriter writer, WorkspaceType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }

        private static bool TryParseWorkspaceType(string input, out WorkspaceType workspaceType)
        {
            foreach (WorkspaceType candidate in Enum.GetValues<WorkspaceType>())
            {
                if (Normalize(candidate.ToString()) == Normalize(input))
                {
                    workspaceType = candidate;
                    return true;
                }
            }

            workspaceType = default;
            return false;
        }

        private static string Normalize(string value)
        {
            return new string(value
                .Where(char.IsLetterOrDigit)
                .Select(char.ToLowerInvariant)
                .ToArray());
        }
    }
}
