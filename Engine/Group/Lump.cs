namespace Engine.Group;

/// <summary>
/// Represents a single lump (a file) within a GRP file.
/// </summary>
internal class Lump(string rawFileName, int rawSize, byte[] rawData)
{
    /// <summary>
    /// Gets or sets the offset (in bytes) of this lump within the GRP file.
    /// </summary>
    internal byte[] RawData { get; } = rawData;

    /// <summary>
    /// Gets or sets the size (in bytes) of this lump.
    /// </summary>
    internal int RawSize { get; } = rawSize;

    /// <summary>
    /// Gets or sets the file name associated with this lump.
    /// </summary>
    internal string RawFileName { get; } = rawFileName;
}
