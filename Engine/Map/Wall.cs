using System.Numerics;

namespace Engine.Map;

// Define the Wall structure
public class Wall
{
    internal int Id { get; private set; }
    internal Vector2 PositionStart { get; }
    internal Vector2 PositionEnd => _map.Walls[RawPoint2].PositionStart;
    internal bool IsBottomTextureSwapped => (RawCStat & 2) != 0;
    internal bool IsWallAlignedBottom => (RawCStat & 3) != 0;

    /// <summary>
    /// The X coordinate of the wall's starting point.
    /// </summary>
    private int RawX { get; }

    /// <summary>
    /// The Y coordinate of the wall's starting point.
    /// </summary>
    private int RawY { get; }

    /// <summary>
    /// Index of the next wall in the sector, forming a linked list of walls.
    /// </summary>
    internal short RawPoint2 { get; }

    /// <summary>
    /// Index of the wall on the opposite side of this wall, used for connecting sectors.
    /// </summary>
    private short RawNextWall { get; }

    /// <summary>
    /// Index of the sector on the opposite side of this wall, helping in defining sector neighbors.
    /// </summary>
    private short RawNextSector { get; }

    /// <summary>
    /// A bitfield containing flags related to the wall's rendering and behavior.
    /// </summary>
    private short RawCStat { get; }

    /// <summary>
    /// The texture index for the wall, referencing an entry in an ART file.
    /// </summary>
    private short RawPicnum { get; }

    /// <summary>
    /// The texture index for the over (masking) texture, used for features like doors and windows.
    /// </summary>
    private short RawOverPicnum { get; }

    /// <summary>
    /// The shade offset for the wall, affecting its brightness.
    /// </summary>
    private sbyte RawShade { get; }

    /// <summary>
    /// The palette number for the wall texture, which can change the color palette used.
    /// </summary>
    private byte RawPal { get; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    private byte RawXRepeat { get; }

    /// <summary>
    /// Horizontal and vertical repeat factors for the wall texture, affecting its size.
    /// </summary>
    private byte RawYRepeat { get; }

    /// <summary>
    /// Horizontal and vertical panning offsets for the wall texture, used for alignment.
    /// </summary>
    private byte RawXPanning { get; }

    /// <summary>
    /// Vertical panning offset for the wall texture, used for alignment.
    /// </summary>
    private byte RawYPanning { get; }

    /// <summary>
    /// Game-specific tags for triggering events or actions when interacting with the wall.
    /// </summary>
    private short RawLotag { get; }

    /// <summary>
    /// Additional game-specific tag, similar to Lotag but for different or additional purposes.
    /// </summary>
    private short RawHitag { get; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    private short RawExtra { get; }

    private bool IsPortal => RawNextSector > -1;

    private Mesh? UpperWallMesh { get; set; }
    private Mesh? LowerWallMesh { get; set; }
    private Mesh? SolidWallMesh { get; set; }

    public List<Mesh> Meshes { get; private set; }

    private readonly MapFile _map;
    private Sector _sector;

    public Wall(BinaryReader reader, int indexInRawWallsArray, MapFile map)
    {
        RawX = reader.ReadInt32();
        RawY = reader.ReadInt32();
        RawPoint2 = reader.ReadInt16();
        RawNextWall = reader.ReadInt16();
        RawNextSector = reader.ReadInt16();
        RawCStat = reader.ReadInt16();
        RawPicnum = reader.ReadInt16();
        RawOverPicnum = reader.ReadInt16();
        RawShade = reader.ReadSByte();
        RawPal = reader.ReadByte();
        RawXRepeat = reader.ReadByte();
        RawYRepeat = reader.ReadByte();
        RawXPanning = reader.ReadByte();
        RawYPanning = reader.ReadByte();
        RawLotag = reader.ReadInt16();
        RawHitag = reader.ReadInt16();
        RawExtra = reader.ReadInt16();

        PositionStart = new Vector2(
            RawX * Constants.BuildWidthUnitMeterRatio,
            RawY * Constants.BuildWidthUnitMeterRatio
        );

        Id = indexInRawWallsArray;

        _map = map;
    }

    internal void Load(Sector sector)
    {
        _sector = sector;
        UpperWallMesh = CreateUpperWallMesh();
        LowerWallMesh = CreateLowerWallMesh();
        SolidWallMesh = CreateSolidWallMesh();
        Meshes = new List<Mesh> { UpperWallMesh, LowerWallMesh, SolidWallMesh }
            .Where(wall => wall != null)
            .ToList();
    }

    private Mesh? CreateUpperWallMesh()
    {
        return null;
    }

    private Mesh? CreateLowerWallMesh()
    {
        return null;
    }

    private Mesh? CreateSolidWallMesh()
    {
        return null;
    }
}
