using System;
using System.IO;
using System.Text;
using MonoBuild.Group;

namespace MonoBuild.Palette;

/// <summary>
/// Represents a palette file containing 256 colors.
/// </summary>
public class RawPaletteFile
{
    /// <summary>
    /// The list of 256 colors stored in this palette file.
    /// </summary>
    public Color[] Colors { get; set; } = new Color[256];

    /// <summary>
    /// Loads a palette from a given stream.
    /// </summary>
    /// <param name="stream">The stream containing palette data.</param>
    /// <returns>A new RawPaletteFile instance populated with the data from the stream.</returns>
    public static RawPaletteFile LoadFromStream(Stream stream)
    {
        var paletteFile = new RawPaletteFile();

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
            paletteFile.Colors[i] = new Color(r * 4, g * 4, b * 4);
        }

        return paletteFile;
    }

    /// <summary>
    /// Loads a palette from a byte array.
    /// </summary>
    /// <param name="paletteData">The byte array containing palette data.</param>
    /// <returns>A new RawPaletteFile instance populated with the data.</returns>
    public static RawPaletteFile LoadFromBytes(byte[] paletteData)
    {
        using var stream = new MemoryStream(paletteData);
        return LoadFromStream(stream);
    }

    /// <summary>
    /// Loads a palette from a file path.
    /// </summary>
    /// <param name="filePath">The file path to load the palette from.</param>
    /// <returns>A new RawPaletteFile instance populated with the data.</returns>
    public static RawPaletteFile LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Palette file not found: {filePath}");

        var paletteData = File.ReadAllBytes(filePath);
        return LoadFromBytes(paletteData);
    }

    public static RawPaletteFile Load(RawGroupFile loadedRawGroup)
    {
        var paletteLump = loadedRawGroup.Lumps.Find(x => x.FileName == "PALETTE.DAT");

        if (paletteLump == null)
            throw new Exception("PALETTE.DAT not found in group file.");

        return LoadFromBytes(paletteLump.Data);
    }
}
