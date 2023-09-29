namespace MemeText.Models;

public class TextLiteral : Node
{
    public string Text { get; set; } = "";

    public TextLiteral() { }

    public TextLiteral(string text)
    {
        this.Text = text;
    }
}
