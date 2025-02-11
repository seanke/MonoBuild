using System.Text;

namespace Engine.Art;

/// <summary>
/// Represents a palette file containing 256 colors.
/// </summary>
internal class Palette
{
    /// <summary>
    /// The list of 256 colors stored in this palette file.
    /// </summary>
    internal Color[] Colors { get; } = new Color[256];

    /// <summary>
    /// Loads a palette from a given stream.
    /// </summary>
    /// <param name="stream">The stream containing palette data.</param>
    /// <returns>A new RawPaletteFile instance populated with the data from the stream.</returns>
    internal Palette(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

        if (stream.Length < 768)
        {
            throw new InvalidDataException(
                "Invalid palette file size. Expected at least 768 bytes."
            );
        }

        for (var i = 0; i < 256; i++)
        {
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();

            // Convert 6-bit color range (0-63) to 8-bit (0-255)
            Colors[i] = new Color(r * 4, g * 4, b * 4);
        }
    }

    /// <summary>
    /// Loads a palette from a byte array.
    /// </summary>
    /// <param name="paletteData">The byte array containing palette data.</param>
    /// <returns>A new RawPaletteFile instance populated with the data.</returns>
    internal Palette(byte[] paletteData)
        : this(new MemoryStream(paletteData)) { }
}
