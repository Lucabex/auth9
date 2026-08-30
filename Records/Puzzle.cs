using System.Text.Json.Serialization;

namespace auth9.Records;

public record PuzzleDaily(
    [property:JsonPropertyName("solution")] List<string> Solution,
    [property:JsonPropertyName("fen")]string Fen
);