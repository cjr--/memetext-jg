using MemeText.Models;
using System.Text;

namespace MemeText.Parsing;

public class Parser
{
    public event Action<Token>? OnToken;

    public Document Parse(string text)
    {
        var tokenizer = new Tokenizer();
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        return Parse(tokenizer.Tokenize(stream));
    }

    public Document Parse(Stream stream)
    {
        var tokenizer = new Tokenizer();
        return Parse(tokenizer.Tokenize(stream));
    }

    public Document Parse(IEnumerable<Token> tokens)
    {
        var document = new Document();

        Node? node = document;

        foreach (var token in tokens)
        {
            OnToken?.Invoke(token);

            switch (token.Type)
            {
                case TokenType.Text when token.Text is not null:
                    node?.AddChild(new TextLiteral(token.Text));
                    break;

                case TokenType.StartEmphasis:
                    node = node?.AddChild(new Emphasis());
                    break;

                case TokenType.StartStrong:
                    node = node?.AddChild(new Strong());
                    break;

                case TokenType.LineBreak:
                    node?.AddChild(new LineBreak());
                    break;

                case TokenType.StartListItem:
                    node = node?.AddChild(new ListItem());
                    break;

                case TokenType.Dedent:
                    node = node?.Parent?.Parent;
                    break;

                case TokenType.EndAuto:
                case TokenType.EndEmphasis:
                case TokenType.EndStrong:
                case TokenType.EndUnknown:
                case TokenType.EndCode:
                case TokenType.EndComment:
                case TokenType.EndEmoji:
                case TokenType.EndFootnote:
                case TokenType.EndImage:
                case TokenType.EndItemLink:
                case TokenType.EndListItem:
                case TokenType.EndUserLink:
                    node = node?.Parent;
                    break;
            }
        }

        return document;
    }
}