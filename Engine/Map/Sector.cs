namespace Engine.Map;

/// <summary>
/// Defines the structure for a sector, including its geometry, appearance, and special attributes.
/// </summary>
public class Sector
{
    /// <summary>
    /// Index to the first wall in the sector, used to identify where the sector's wall definitions start.
    /// </summary>
    internal short RawWallPtr { get; }

    /// <summary>
    /// The number of walls in this sector, determining how many walls are associated with this sector.
    /// </summary>
    internal short RawWallNum { get; }

    /// <summary>
    /// Z-coordinate (height) of the sector's ceiling at its first point.
    /// </summary>
    internal int RawCeilingZ { get; }

    /// <summary>
    /// Z-coordinate (height) of the sector's floor at its first point.
    /// </summary>
    internal int RawFloorZ { get; }

    /// <summary>
    /// Bitfield containing various flags related to the sector's ceiling, such as parallaxing and slope.
    /// </summary>
    internal short RawCeilingStat { get; }

    /// <summary>
    /// Bitfield containing various flags related to the sector's floor, such as parallaxing and slope.
    /// </summary>
    internal short RawFloorStat { get; }

    /// <summary>
    /// Texture index for the ceiling, referencing an entry in an ART file.
    /// </summary>
    internal short RawCeilingPicnum { get; }

    /// <summary>
    /// Shade offset for the ceiling, affecting its brightness.
    /// </summary>
    internal sbyte RawCeilingShade { get; }

    /// <summary>
    /// Slope of the ceiling, determining how steep the slope is if the ceiling is sloped.
    /// </summary>
    internal short RawCeilingHeinum { get; }

    /// <summary>
    /// Palette number for the ceiling texture, which can change the color palette used.
    /// </summary>
    internal byte RawCeilingPal { get; }

    /// <summary>
    /// Horizontal panning offset for the ceiling texture, useful for texture alignment.
    /// </summary>
    internal byte RawCeilingXpanning { get; }

    /// <summary>
    /// Vertical panning offset for the ceiling texture, useful for texture alignment.
    /// </summary>
    internal byte RawCeilingYpanning { get; }

    /// <summary>
    /// Texture index for the floor, referencing an entry in an ART file.
    /// </summary>
    internal short RawFloorPicnum { get; }

    /// <summary>
    /// Slope of the floor, determining how steep the slope is if the floor is sloped.
    /// </summary>
    internal short RawFloorHeinum { get; }

    /// <summary>
    /// Shade offset for the floor, affecting its brightness.
    /// </summary>
    internal sbyte RawFloorShade { get; }

    /// <summary>
    /// Palette number for the floor texture, which can change the color palette used.
    /// </summary>
    internal byte RawFloorPal { get; }

    /// <summary>
    /// Horizontal panning offset for the floor texture, useful for texture alignment.
    /// </summary>
    internal byte RawFloorXpanning { get; }

    /// <summary>
    /// Vertical panning offset for the floor texture, useful for texture alignment.
    /// </summary>
    internal byte RawFloorYpanning { get; }

    /// <summary>
    /// Affects how quickly sectors fade to darkness with distance. Lower values result in quicker darkening.
    /// </summary>
    internal byte RawVisibility { get; }

    /// <summary>
    /// Padding byte, not used for any game logic but necessary for structure alignment.
    /// </summary>
    internal byte RawFiller { get; }

    /// <summary>
    /// Game-specific use, often for triggering events or actions within the sector.
    /// </summary>
    internal short RawLotag { get; }

    /// <summary>
    /// Additional tag for game-specific use, similar to Lotag but for different or additional purposes.
    /// </summary>
    internal short RawHitag { get; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    internal short RawExtra { get; }

    internal int Id { get; }

    private readonly MapFile _mapFile;

    /// <summary>
    /// Reads and constructs a sector from a binary reader stream, typically used for map loading.
    /// </summary>
    /// <param name="reader">The binary reader to read the sector data from.</param>
    /// <param name="indexInRawSectorsArray"></param>
    /// <param name="mapFile"></param>
    /// <returns>A new instance of a Sector populated with data from the binary reader.</returns>
    public Sector(BinaryReader reader, int indexInRawSectorsArray, MapFile mapFile)
    {
        RawWallPtr = reader.ReadInt16();
        RawWallNum = reader.ReadInt16();
        RawCeilingZ = reader.ReadInt32();
        RawFloorZ = reader.ReadInt32();
        RawCeilingStat = reader.ReadInt16();
        RawFloorStat = reader.ReadInt16();
        RawCeilingPicnum = reader.ReadInt16();
        RawCeilingHeinum = reader.ReadInt16();
        RawCeilingShade = reader.ReadSByte();
        RawCeilingPal = reader.ReadByte();
        RawCeilingXpanning = reader.ReadByte();
        RawCeilingYpanning = reader.ReadByte();
        RawFloorPicnum = reader.ReadInt16();
        RawFloorHeinum = reader.ReadInt16();
        RawFloorShade = reader.ReadSByte();
        RawFloorPal = reader.ReadByte();
        RawFloorXpanning = reader.ReadByte();
        RawFloorYpanning = reader.ReadByte();
        RawVisibility = reader.ReadByte();
        RawFiller = reader.ReadByte();
        RawLotag = reader.ReadInt16();
        RawHitag = reader.ReadInt16();
        RawExtra = reader.ReadInt16();

        Id = indexInRawSectorsArray;

        _mapFile = mapFile;
    }

    private List<Wall> GetSectorWalls(Sector sector)
    {
        var result = new List<Wall>();

        if (sector.RawWallPtr < 0 || sector.RawWallPtr >= _mapFile.Walls.Count)
            return result;

        var walls = _mapFile.Walls.Skip(sector.RawWallPtr).Take(sector.RawWallNum);

        return walls.ToList();
    }

    internal List<List<Wall>> GetSectorWallLoops(Sector sector)
    {
        var result = new List<List<Wall>>();

        var walls = GetSectorWalls(sector);

        foreach (var wall in walls)
        {
            if (result.Any(loop => loop.Contains(wall)))
                continue;

            var loop = new List<Wall> { wall };
            var currentWall = wall;

            do
            {
                loop.Add(currentWall);
                currentWall = _mapFile.Walls[currentWall.RawPoint2];
            } while (currentWall.RawPoint2 != wall.RawPoint2);

            result.Add(loop);
        }

        return result;
    }
}
