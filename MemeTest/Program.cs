// See https://aka.ms/new-console-template for more information

using MemeText.Models;
using MemeText.Parsing;

var testText = @"This is a test Meme-Text file.

This is some *e emphasised ** text.

This is some *s strong ** text.

*- How do we handle whitespace around control characters?
    *- Indent 1 list item
        *- Indent another list item
    *- Back up one level
      *- Back down one level (note different offset)

Plain text back at the start.

";

var parser = new Parser();

void Log(ConsoleColor color, string? message, bool newLine = false)
{
    Console.ForegroundColor = color;
    if (newLine) Console.WriteLine(message); else Console.Write(message);
}

parser.OnToken += token =>
{    
    Log(ConsoleColor.Magenta, $"{token.LineNumber,4}  ");
    Log(ConsoleColor.Cyan, $"{token.Level,2}  ");
    Log(ConsoleColor.Yellow, $"{token.Type,-15}  ");

    if (token.Type != TokenType.LineBreak)
        Log(ConsoleColor.Gray, token.Text, true);    
    else
        Console.WriteLine(); 
};

var document = parser.Parse(testText);

int indent = 0;
var spc = "                                                                                ";
LogNode(document);

void LogNode(Node node)
{    
    Log(ConsoleColor.Gray, spc.Substring(0, indent * 4));
    
    if (node is TextLiteral t)
        Log(ConsoleColor.Gray, t.Text, true);    
    else
        Log(ConsoleColor.Yellow, node.GetType().Name, true);

    indent += 1;
    foreach (var child in node.Children)
        LogNode(child);

    indent -= 1;
}