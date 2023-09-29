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

parser.OnToken += token =>
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.Write($"{token.LineNumber,4}  ");

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write($"{token.Level,2}  ");

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write($"{token.Type,-15}  ");
    if (token.Type != TokenType.LineBreak)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(token.Text);
    }
    else
    {
        Console.WriteLine();
    }    
};

var document = parser.Parse(testText);

int indent = 0;
var spc = "                                                                                ";
WriteNode(document);

void WriteNode(Node node)
{    
    Console.Write(spc.Substring(0, indent * 4));
    if (node is TextLiteral t)
    {
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(t.Text);
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(node.GetType().Name);        
    }
    indent += 1;
    foreach (var child in node.Children)
        WriteNode(child);
    indent -= 1;
}