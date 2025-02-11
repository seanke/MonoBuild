using System.Text;
using Engine.Art;

namespace Engine.Group;

/// <summary>
/// Represents a GRP (group) file for the Duke3D build engine,
/// which is a container of multiple lumps (data blocks).
/// </summary>
public class GroupFile
{
    /// <summary>
    /// Gets the signature of the group file. Typically, "KenSilverman".
    /// </summary>
    internal string RawSignature { get; }

    /// <summary>
    /// Gets the number of lumps in the group file.
    /// </summary>
    internal int RawLumpCount { get; }

    /// <summary>
    /// Gets the list of lumps contained in the group file.
    /// </summary>
    internal List<Lump> Lumps { get; } = [];

    /// <summary>
    /// Gets the list of all tiles contained in all .ART files the group file.
    /// </summary>
    public List<Tile> Tiles { get; } = [];

    internal List<ArtFile> ArtFiles { get; } = [];

    internal Palette Palette { get; }

    /// <summary>
    /// Loads a GRP file from the provided stream.
    /// Assumes the file begins with a 12‑byte ASCII signature, followed by a 32‑bit integer lump count,
    /// and then a directory of lump entries (each with a 4‑byte offset, 4‑byte size, and an 8‑byte file name).
    /// </summary>
    /// <param name="stream">The stream to load the GRP file from.</param>
    /// <returns>A new RawGroup instance populated with the data from the stream.</returns>
    internal GroupFile(Stream stream)
    {
        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

        // Read the 12-byte signature.
        var signatureBytes = reader.ReadBytes(12);
        RawSignature = Encoding.ASCII.GetString(signatureBytes).TrimEnd('\0');

        // Read the file count (lump count).
        RawLumpCount = reader.ReadInt32();

        // First, read all the file entries.
        var fileEntries = new List<Tuple<string, int>>();

        for (var i = 0; i < RawLumpCount; i++)
        {
            // Read the filename (12 bytes).
            var nameBytes = reader.ReadBytes(12);
            var lumpFileName = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');

            // Read the file size (4 bytes).
            var lumpFileSize = reader.ReadInt32();

            fileEntries.Add(new Tuple<string, int>(lumpFileName, lumpFileSize));
        }

        // Now, all file data follows immediately after the file entries.
        foreach (var lump in fileEntries)
        {
            // Read lump.Size bytes from the current position in the stream.
            var lumpBytes = reader.ReadBytes(lump.Item2);
            Lumps.Add(new Lump(lump.Item1, lump.Item2, lumpBytes));
        }

        // Load Palette from the group file.
        Palette = new Palette(Lumps.Find(x => x.RawFileName == "PALETTE.DAT")!.RawData);

        // Load ART files from the group file.
        Tiles.AddRange(
            Lumps
                .Where(x => x.RawFileName.EndsWith(".ART"))
                .SelectMany(x =>
                {
                    var artFile = new ArtFile(x.RawData, Palette);
                    ArtFiles.Add(artFile);
                    return artFile.RawTiles;
                })
        );
    }

    public GroupFile(FileInfo filePath)
        : this(filePath.Open(FileMode.Open, FileAccess.Read, FileShare.Read)) { }
}
