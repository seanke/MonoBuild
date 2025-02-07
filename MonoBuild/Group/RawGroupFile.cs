using System.IO;
using System.Text;

namespace MonoBuild.Group;

/// <summary>
/// Represents a GRP (group) file for the Duke3D build engine,
/// which is a container of multiple lumps (data blocks).
/// </summary>
public class RawGroupFile
{
    /// <summary>
    /// Gets or sets the signature of the group file. Typically "KenSilverman".
    /// </summary>
    public string Signature { get; set; }

    /// <summary>
    /// Gets or sets the number of lumps in the group file.
    /// </summary>
    public int LumpCount { get; set; }

    /// <summary>
    /// Gets or sets the list of lumps contained in the group file.
    /// </summary>
    public List<RawGroupLump> Lumps { get; set; }

    public RawGroupFile()
    {
        Lumps = new List<RawGroupLump>();
    }

    /// <summary>
    /// Loads a GRP file from the provided stream.
    /// Assumes the file begins with a 12‑byte ASCII signature, followed by a 32‑bit integer lump count,
    /// and then a directory of lump entries (each with a 4‑byte offset, 4‑byte size, and an 8‑byte file name).
    /// </summary>
    /// <param name="stream">The stream to load the GRP file from.</param>
    /// <returns>A new RawGroup instance populated with the data from the stream.</returns>
    public static RawGroupFile LoadFromStream(Stream stream)
    {
        var group = new RawGroupFile();

        using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);

        // Read the 12-byte signature.
        var signatureBytes = reader.ReadBytes(12);
        group.Signature = Encoding.ASCII.GetString(signatureBytes).TrimEnd('\0');

        // Read the file count (lump count).
        group.LumpCount = reader.ReadInt32();

        // First, read all the file entries.
        var fileEntries = new List<RawGroupLump>();
        for (var i = 0; i < group.LumpCount; i++)
        {
            var lump = new RawGroupLump();

            // Read the filename (12 bytes).
            var nameBytes = reader.ReadBytes(12);
            lump.FileName = Encoding.ASCII.GetString(nameBytes).TrimEnd('\0');

            // Read the file size (4 bytes).
            lump.Size = reader.ReadInt32();

            fileEntries.Add(lump);
        }

        // Now, all file data follows immediately after the file entries.
        foreach (var lump in fileEntries)
        {
            // Read lump.Size bytes from the current position in the stream.
            lump.Data = reader.ReadBytes(lump.Size);
            group.Lumps.Add(lump);
        }

        return group;
    }
}
