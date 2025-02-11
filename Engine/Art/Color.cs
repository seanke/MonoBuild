namespace Engine.Art;

public class Color
{
    public int Red { get; }
    public int Green { get; }
    public int Blue { get; }

    internal Color(int red, int green, int blue)
    {
        Red = red;
        Green = green;
        Blue = blue;
    }

    internal Color(byte positionInPalete, Palette palette)
    {
        var color = palette.Colors[positionInPalete];
        Red = color.Red;
        Green = color.Green;
        Blue = color.Blue;
    }
}
