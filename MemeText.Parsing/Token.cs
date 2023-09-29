namespace MemeText.Parsing;

public record Token(
    TokenType Type,
    string? Text,
    int LineNumber,
    int Level);
