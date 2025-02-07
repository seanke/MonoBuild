using System.IO;
using System.Text;

namespace MonoBuild.Art;

/// <summary>
/// Represents an ART file containing textures (tiles) used in the Build engine.
/// </summary>
public class RawArtFile
{
    /// <summary>
    /// The version number of the ART file.
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// The first tile index in this ART file.
    /// </summary>
    public int FirstTile { get; set; }

    /// <summary>
    /// The last tile index in this ART file.
    /// </summary>
    public int LastTile { get; set; }

    /// <summary>
    /// A list of tile metadata entries, including width, height, and other properties.
    /// </summary>
    public List<RawTile> Tiles { get; set; } = [];

    /// <summary>
    /// Loads an ART file from a given stream.
    /// </summary>
    /// <param name="stream">The stream containing ART file data.</param>
    /// <returns>A new RawArtFile instance populated with the data from the stream.</returns>
    public static RawArtFile LoadFromStream(Stream stream)
    {
        var artFile = new RawArtFile();

        using var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

        artFile.Version = reader.ReadInt32();
        _ = reader.ReadInt32(); // Skip the number of tiles, Ken said it's not reliable
        artFile.FirstTile = reader.ReadInt32();
        artFile.LastTile = reader.ReadInt32();

        var tileCount = artFile.LastTile - artFile.FirstTile + 1;

        // Read tile metadata
        var widths = new short[tileCount];
        var heights = new short[tileCount];
        var picanm = new int[tileCount];

        for (var i = 0; i < tileCount; i++)
        {
            widths[i] = reader.ReadInt16();
        }
        for (var i = 0; i < tileCount; i++)
        {
            heights[i] = reader.ReadInt16();
        }
        for (var i = 0; i < tileCount; i++)
        {
            picanm[i] = reader.ReadInt32();
        }

        // Read tile pixel data
        artFile.Tiles = new List<RawTile>(tileCount);
        for (var i = 0; i < tileCount; i++)
        {
            var pixelDataSize = widths[i] * heights[i];
            var pixelData = reader.ReadBytes(pixelDataSize);

            artFile.Tiles.Add(
                new RawTile
                {
                    TileIndex = artFile.FirstTile + i,
                    Width = widths[i],
                    Height = heights[i],
                    Picanm = picanm[i],
                    PixelData = pixelData
                }
            );
        }

        return artFile;
    }

    /// <summary>
    /// Loads an ART file from a byte array.
    /// </summary>
    /// <param name="artData">The byte array containing ART file data.</param>
    /// <returns>A new RawArtFile instance populated with the data.</returns>
    public static RawArtFile LoadFromBytes(byte[] artData)
    {
        using var stream = new MemoryStream(artData);
        return LoadFromStream(stream);
    }
}
