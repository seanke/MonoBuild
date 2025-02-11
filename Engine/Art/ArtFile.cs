using System.Text;

namespace Engine.Art;

/// <summary>
/// Represents an ART file containing textures (tiles) used in the Build engine.
/// </summary>
internal class ArtFile
{
    /// <summary>
    /// The version number of the ART file.
    /// </summary>
    internal int RawVersion { get; }

    /// <summary>
    /// The first tile index in this ART file.
    /// </summary>
    internal int RawFirstTile { get; }

    /// <summary>
    /// The last tile index in this ART file.
    /// </summary>
    internal int RawLastTile { get; }

    /// <summary>
    /// A list of tile metadata entries, including width, height, and other properties.
    /// </summary>
    internal List<Tile> RawTiles { get; } = [];

    private Palette Palette { get; }

    /// <summary>
    /// Loads an ART file from a given stream.
    /// </summary>
    /// <param name="stream">The stream containing ART file data.</param>
    /// <param name="palette"></param>
    /// <returns>A new RawArtFile instance populated with the data from the stream.</returns>
    internal ArtFile(Stream stream, Palette palette)
    {
        Palette = palette;

        using var reader = new BinaryReader(stream, Encoding.Default, leaveOpen: true);

        RawVersion = reader.ReadInt32();
        _ = reader.ReadInt32(); // Skip the number of tiles, Ken said it's not reliable
        RawFirstTile = reader.ReadInt32();
        RawLastTile = reader.ReadInt32();

        var tileCount = RawLastTile - RawFirstTile + 1;

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
        RawTiles = new List<Tile>(tileCount);
        for (var i = 0; i < tileCount; i++)
        {
            var pixelDataSize = widths[i] * heights[i];
            var pixelData = reader.ReadBytes(pixelDataSize);

            RawTiles.Add(
                new Tile(RawFirstTile + i, widths[i], heights[i], picanm[i], pixelData, palette)
            );
        }
    }

    /// <summary>
    /// Loads an ART file from a byte array.
    /// </summary>
    /// <param name="artData">The byte array containing ART file data.</param>
    /// <param name="palette"></param>
    /// <returns>A new RawArtFile instance populated with the data.</returns>
    internal ArtFile(byte[] artData, Palette palette)
        : this(new MemoryStream(artData), palette) { }
}
