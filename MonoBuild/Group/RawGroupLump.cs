namespace MonoBuild.Group;

/// <summary>
/// Represents a single lump (data block) within a GRP file.
/// </summary>
public class RawGroupLump
{
    /// <summary>
    /// Gets or sets the offset (in bytes) of this lump within the GRP file.
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// Gets or sets the size (in bytes) of this lump.
    /// </summary>
    public int Size { get; set; }

    /// <summary>
    /// Gets or sets the file name associated with this lump.
    /// </summary>
    public string FileName { get; set; }
}
