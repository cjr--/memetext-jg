namespace MemeText.Parsing;

public class Tokenizer
{
    private int currentLineIndex;
    private int currentCharIndex;
    private string? currentLine;
    private StreamReader? reader;
    private int currentIndentLevel;
    private int currentLevel;
    private Stack<int> indentLevels = new();

    public int TabSize { get; set; } = 4;

    public Tokenizer()
    {

    }

    public IEnumerable<Token> Tokenize(Stream inputStream)
    {        
        reader = new(inputStream);
        currentLineIndex = 0;
        currentCharIndex = 0;
        currentLevel = 0;

        char? c;
        string text = "";
        indentLevels.Clear();
        indentLevels.Push(0);
        bool inControl = false;

        while (!reader.EndOfStream || currentCharIndex < (currentLine?.Length ?? 0))
        {
            while (!reader.EndOfStream && (currentCharIndex >= (currentLine?.Length ?? 0)))
            {
                // If we have some text, emit that first as plain text.
                if (text.Length > 0)                
                    yield return new(TokenType.Text, text, currentLineIndex, currentLevel);

                // Emit a line break if we're reading after the first line.
                if (currentLineIndex > 1)
                    yield return new(TokenType.LineBreak, "\n", currentLineIndex, currentLevel);

                // Read the next line of the file.
                currentLine = reader.ReadLine();
                currentCharIndex = 0;
                currentLineIndex++;
                text = "";

                (currentIndentLevel, currentLine) = StripIndent(currentLine);
                int lastLevel = indentLevels.Peek();

                if (currentIndentLevel > lastLevel)
                {
                    indentLevels.Push(currentIndentLevel);
                    currentLevel++;
                    yield return new(TokenType.Indent, null, currentLineIndex, currentLevel);
                }
                else if (currentIndentLevel < lastLevel)
                {
                    // How far back are we going?  Handles e.g.
                    // *- item 1
                    //    *- item 2
                    //
                    // Some text
                    //
                    while (currentIndentLevel < indentLevels.Peek())
                    {
                        indentLevels.Pop();
                        currentLevel--;
                        yield return new(TokenType.Dedent, null, currentLineIndex, currentLevel);
                    }
                    indentLevels.Push(currentIndentLevel);
                }
            }

            if (currentCharIndex < (currentLine?.Length ?? 0))
            {
                c = currentLine?[currentCharIndex++];
                switch (c)
                {
                    case '*':
                        if (inControl) // **
                        {
                            text = "";
                            inControl = false;
                            yield return new(TokenType.EndAuto, "**", currentLineIndex, currentLevel);
                        }
                        else
                        {
                            // If we have some text, treat this as a 'close' control sequence if
                            // the text doesn't end with " ".
                            if (text.Length > 0)
                            {
                                if (text.EndsWith(" "))
                                    yield return new(TokenType.Text, text, currentLineIndex, currentLevel); 
                                else
                                    yield return GetCloseControlToken(text);
                            }

                            inControl = true;
                            text = "*";
                        }

                        break;

                    case ' ' when inControl:
                        // If we are currently looking at a control sequence (e.g. *e), a space 'breaks' it, so 
                        // emit the control sequence we have so far.
                        yield return GetControlToken(text);
                        text = "";
                        inControl = false;
                        break;

                    default:
                        text += c;
                        break;
                }
            }
        }

        if (text.Length > 0)
            yield return new(TokenType.Text, text, currentLineIndex, currentLevel);

        reader.Dispose();
        reader = null;
    }
       

    protected Token GetControlToken(string controlSequence)
        => new(
            controlSequence switch
            {
                "*e" => TokenType.StartEmphasis,
                "*s" => TokenType.StartStrong,
                "*-" => TokenType.StartListItem,
                "*!" => TokenType.StartComment,
                "*~" => TokenType.StartItemLink,
                "*@" => TokenType.StartUserLink,
                "*a" => TokenType.StartAnchor,
                "*i" => TokenType.StartImage,
                "*#" => TokenType.StartFootnote,
                "*+" => TokenType.StartCode,
                "*:" => TokenType.StartEmoji,
                _ => TokenType.StartUnknown
            },
            controlSequence, currentLineIndex, currentLevel);

    protected Token GetCloseControlToken(string controlSequence)
        => new(
            controlSequence switch
            {
                "e*" => TokenType.EndEmphasis,
                "s*" => TokenType.EndStrong,
                "-*" => TokenType.EndListItem,
                "!*" => TokenType.EndComment,
                "~*" => TokenType.EndItemLink,
                "@*" => TokenType.EndUserLink,
                "a*" => TokenType.EndAnchor,
                "i*" => TokenType.EndImage,
                "#*" => TokenType.EndFootnote,
                "+*" => TokenType.EndCode,
                ":*" => TokenType.EndEmoji,
                _ => TokenType.EndUnknown
            },
            controlSequence, currentLineIndex, currentLevel);

    protected (int, string?) StripIndent(string? line)
    {
        if (line == null)
            return (0, null);

        var whitespace = String.Join("", line.TakeWhile(Char.IsWhiteSpace));      
        int level = whitespace.Length + whitespace.Where(c => c == 9).Count() * (TabSize - 1); // Add extra space for tabs.

        return (level, line[whitespace.Length..]);
    }


}
