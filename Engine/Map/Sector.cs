using System.Collections.Immutable;
using System.Numerics;
using Engine.Art;
using Engine.Group;
using LibTessDotNet;

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
    private short RawWallNum { get; }

    /// <summary>
    /// Z-coordinate (height) of the sector's ceiling at its first point.
    /// </summary>
    private int RawCeilingZ { get; }

    /// <summary>
    /// Z-coordinate (height) of the sector's floor at its first point.
    /// </summary>
    private int RawFloorZ { get; }

    /// <summary>
    /// Bitfield containing various flags related to the sector's ceiling, such as parallaxing and slope.
    /// </summary>
    private short RawCeilingStat { get; }

    /// <summary>
    /// Bitfield containing various flags related to the sector's floor, such as parallaxing and slope.
    /// </summary>
    private short RawFloorStat { get; }

    /// <summary>
    /// Texture index for the ceiling, referencing an entry in an ART file.
    /// </summary>
    private short RawCeilingPicnum { get; }

    /// <summary>
    /// Shade offset for the ceiling, affecting its brightness.
    /// </summary>
    private sbyte RawCeilingShade { get; }

    /// <summary>
    /// Slope of the ceiling, determining how steep the slope is if the ceiling is sloped.
    /// </summary>
    private short RawCeilingHeinum { get; }

    /// <summary>
    /// Palette number for the ceiling texture, which can change the color palette used.
    /// </summary>
    private byte RawCeilingPal { get; }

    /// <summary>
    /// Horizontal panning offset for the ceiling texture, useful for texture alignment.
    /// </summary>
    private byte RawCeilingXpanning { get; }

    /// <summary>
    /// Vertical panning offset for the ceiling texture, useful for texture alignment.
    /// </summary>
    private byte RawCeilingYpanning { get; }

    /// <summary>
    /// Texture index for the floor, referencing an entry in an ART file.
    /// </summary>
    private short RawFloorPicnum { get; }

    /// <summary>
    /// Slope of the floor, determining how steep the slope is if the floor is sloped.
    /// </summary>
    public short RawFloorHeinum { get; }

    /// <summary>
    /// Shade offset for the floor, affecting its brightness.
    /// </summary>
    private sbyte RawFloorShade { get; }

    /// <summary>
    /// Palette number for the floor texture, which can change the color palette used.
    /// </summary>
    private byte RawFloorPal { get; }

    /// <summary>
    /// Horizontal panning offset for the floor texture, useful for texture alignment.
    /// </summary>
    private byte RawFloorXpanning { get; }

    /// <summary>
    /// Vertical panning offset for the floor texture, useful for texture alignment.
    /// </summary>
    private byte RawFloorYpanning { get; }

    /// <summary>
    /// Affects how quickly sectors fade to darkness with distance. Lower values result in quicker darkening.
    /// </summary>
    private byte RawVisibility { get; }

    /// <summary>
    /// Padding byte, not used for any game logic but necessary for structure alignment.
    /// </summary>
    private byte RawFiller { get; }

    /// <summary>
    /// Game-specific use, often for triggering events or actions within the sector.
    /// </summary>
    private short RawLotag { get; }

    /// <summary>
    /// Additional tag for game-specific use, similar to Lotag but for different or additional purposes.
    /// </summary>
    private short RawHitag { get; }

    /// <summary>
    /// An extra value for game-specific use, can hold any additional information needed by the game engine.
    /// </summary>
    private short RawExtra { get; }

    public int Id { get; }

    public ImmutableList<Mesh> Meshes { get; private set; }

    private Mesh FloorMesh { get; set; }
    private Mesh CeilingMesh { get; set; }
    internal ImmutableList<Wall> Walls { get; set; }

    internal float CeilingYCoordinate { get; }
    public float FloorYCoordinate { get; }

    private Tile FloorTile => _groupFile.Tiles[RawFloorPicnum];
    private Tile CeilingTile => _groupFile.Tiles[RawCeilingPicnum];

    // Bit 0: Parallax flag (1 = parallaxing sky, 0 = not)
    internal bool HasParallaxCeiling => (RawCeilingStat & 1) != 0;
    internal bool HasParallaxFloor => (RawFloorStat & 1) != 0;
    
    // Bit 1: Slope flag
    internal bool IsCeilingSloped => (RawCeilingStat & (1 << 1)) != 0;
    internal bool IsFloorSloped => (RawFloorStat & (1 << 1)) != 0;
    
    // Bit 2: Swap X&Y (rotate 90 degrees)
    internal bool IsCeilingTextureSwapped => (RawCeilingStat & (1 << 2)) != 0;
    internal bool IsFloorTextureSwapped => (RawFloorStat & (1 << 2)) != 0;
    
    // Bit 4: X-flip
    internal bool IsCeilingXFlipped => (RawCeilingStat & (1 << 4)) != 0;
    internal bool IsFloorXFlipped => (RawFloorStat & (1 << 4)) != 0;
    
    // Bit 5: Y-flip
    internal bool IsCeilingYFlipped => (RawCeilingStat & (1 << 5)) != 0;
    internal bool IsFloorYFlipped => (RawFloorStat & (1 << 5)) != 0;
    
    // Bit 6: Relative alignment flag (1 = align texture to first wall of sector)
    internal bool IsCeilingAlignedToFirstWall => (RawCeilingStat & (1 << 6)) != 0;
    internal bool IsFloorAlignedToFirstWall => (RawFloorStat & (1 << 6)) != 0;

    private readonly MapFile _mapFile;
    private readonly GroupFile _groupFile;

    /// <summary>
    /// Reads and constructs a sector from a binary reader stream, typically used for map loading.
    /// </summary>
    /// <param name="reader">The binary reader to read the sector data from.</param>
    /// <param name="indexInRawSectorsArray"></param>
    /// <param name="mapFile"></param>
    /// <returns>A new instance of a Sector populated with data from the binary reader.</returns>
    public Sector(
        BinaryReader reader,
        int indexInRawSectorsArray,
        MapFile mapFile,
        GroupFile groupFile
    )
    {
        // Read sector properties from the binary reader
        // Ken Silverman's Build engine uses a custom binary format for its map files
        // Here we pared in the raw sector data to populate the sector object
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
        CeilingYCoordinate = RawCeilingZ * Constants.BuildHeightUnitMeterRatio;
        FloorYCoordinate = RawFloorZ * Constants.BuildHeightUnitMeterRatio;

        _mapFile = mapFile;
        _groupFile = groupFile;
    }

    /// <summary>
    /// Loads sector properties. It MUST be called after all walls have been read from the map file.
    /// </summary>
    /// <returns></returns>
    internal void Load()
    {
        // Populate the walls list for this sector
        Walls = _mapFile.Walls.Skip(RawWallPtr).Take(RawWallNum).ToImmutableList();

        // Get the wall loops for this sector, to be used for floor, ceiling and wall meshes
        var sectorWallLoops = GetSectorWallLoops(this);

        // Tessellate the sector wall loops to create the floor and ceiling meshes
        var tessellatedSector = GetTessellatedSector(sectorWallLoops, 0);

        // Create the floor and ceiling meshes
        FloorMesh = CreateFloorMesh(tessellatedSector);
        CeilingMesh = CreateCeilingMesh(tessellatedSector);

        Walls.ForEach(wall => wall.Load(this));
        var meshes = new List<Mesh> { FloorMesh, CeilingMesh };
        meshes.AddRange(Walls.SelectMany(wall => wall.Meshes));
        Meshes = meshes.ToImmutableList();
    }

    private Mesh CreateFloorMesh(Tess tessellatedSectorWallLoops)
    {
        var tile = _groupFile.Tiles[RawFloorPicnum];
        
        var vertices = tessellatedSectorWallLoops.Vertices.Select(v => new Vertex(
            new Vector3(
                v.Position.X,
                Utils.GetFloorHeightAt(new Vector2(v.Position.X, v.Position.Z), this),
                v.Position.Z
            ),
            CalculateFloorCeilingUV(
                new Vector2(v.Position.X, v.Position.Z),
                tile,
                RawFloorXpanning,
                RawFloorYpanning,
                IsFloorAlignedToFirstWall,
                IsFloorTextureSwapped,
                IsFloorXFlipped,
                IsFloorYFlipped
            )
        ));

        var indices = tessellatedSectorWallLoops.Elements;

        return new Mesh(
            vertices,
            indices,
            _groupFile.Tiles[RawFloorPicnum],
            this,
            MeshType.Floor,
            null
        );
    }

    private Mesh CreateCeilingMesh(Tess tessellatedSectorWallLoops)
    {
        var tile = _groupFile.Tiles[RawCeilingPicnum];
        
        var vertices = tessellatedSectorWallLoops.Vertices.Select(v => new Vertex(
            new Vector3(v.Position.X, CeilingYCoordinate, v.Position.Z),
            CalculateFloorCeilingUV(
                new Vector2(v.Position.X, v.Position.Z),
                tile,
                RawCeilingXpanning,
                RawCeilingYpanning,
                IsCeilingAlignedToFirstWall,
                IsCeilingTextureSwapped,
                IsCeilingXFlipped,
                IsCeilingYFlipped
            )
        ));

        var indices = tessellatedSectorWallLoops.Elements;

        return new Mesh(
            vertices,
            indices,
            _groupFile.Tiles[RawCeilingPicnum],
            this,
            MeshType.Ceiling,
            null
        );
    }

    private Vector2 CalculateFloorCeilingUV(Vector2 worldPos, Tile tile, byte xpanning, byte ypanning, 
        bool alignToFirstWall, bool swapXY, bool xFlip, bool yFlip)
    {
        float u, v;
        
        if (alignToFirstWall && Walls.Count > 0)
        {
            // Get the first wall of the sector
            var firstWall = Walls[0];
            var firstWallStart = firstWall.PositionStart;
            var firstWallEnd = firstWall.PositionEnd;
            
            // Calculate the first wall's direction vector
            var wallDir = Vector2.Normalize(firstWallEnd - firstWallStart);
            
            // Create perpendicular vector (rotated 90 degrees counter-clockwise)
            var wallPerp = new Vector2(-wallDir.Y, wallDir.X);
            
            // Transform world position to wall-aligned coordinates
            var relativePos = worldPos - firstWallStart;
            u = Vector2.Dot(relativePos, wallDir);
            v = Vector2.Dot(relativePos, wallPerp);
        }
        else
        {
            // Use world coordinates directly (no alignment)
            u = worldPos.X;
            v = worldPos.Y;
        }
        
        // Apply texture rotation flags BEFORE scaling
        // Bit 2: Swap X&Y (rotate 90 degrees)
        if (swapXY)
        {
            var temp = u;
            u = v;
            v = temp;
        }
        
        // Bit 4: X-flip
        if (xFlip)
        {
            u = -u;
        }
        
        // Bit 5: Y-flip  
        if (yFlip)
        {
            v = -v;
        }
        
        // Apply panning (0-255 maps to 0-1 of texture)
        u += (xpanning / 256f) * tile.Width;
        v += (ypanning / 256f) * tile.Height;
        
        // Scale by texture dimensions to get UV coordinates
        u /= tile.Width;
        v /= tile.Height;
        
        return new Vector2(u, v);
    }
    
    private List<List<Wall>> GetSectorWallLoops(Sector sector)
    {
        var result = new List<List<Wall>>();

        foreach (var wall in Walls)
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

    private static Tess GetTessellatedSector(List<List<Wall>> sectorWallLoops, int height)
    {
        var tess = new Tess();

        foreach (var sectorWallLoop in sectorWallLoops)
        {
            // Create a list of unique points for the floor.
            var floorPoints = sectorWallLoop
                .Select(w => new Vector3(w.PositionStart.X, height, w.PositionStart.Y))
                .ToList();

            // Ensure we have a valid polygon (at least 3 distinct points)
            if (floorPoints.Count < 3)
                continue;

            // Close the contour if necessary
            if (floorPoints[0] != floorPoints.Last())
                floorPoints.Add(floorPoints[0]);

            // Strip redundant points (collinear, degenerate, or very close)
            StripLoop(floorPoints);

            // Ensure we still have a valid polygon after cleaning
            if (floorPoints.Count < 3)
                continue;

            // Convert to LibTessDotNet's ContourVertex format
            var contour = floorPoints
                .Select(p => new ContourVertex { Position = new Vec3(p.X, p.Y, p.Z) })
                .ToArray();

            tess.AddContour(contour, ContourOrientation.Original);
        }

        // Try a more robust winding rule (NonZero instead of EvenOdd)
        tess.Tessellate(WindingRule.NonZero, ElementType.Polygons, 3);

        return tess;
    }

    /// <summary>
    /// Removes collinear and redundant points from the loop.
    /// Inspired by Build Engine's `StripLoop` function.
    /// </summary>
    private static void StripLoop(List<Vector3> points)
    {
        const float tolerance = 1 / 2560f;

        for (var p = 0; p < points.Count; p++)
        {
            var prev = (p == 0) ? points.Count - 1 : p - 1;
            var next = (p == points.Count - 1) ? 0 : p + 1;

            // If two neighboring points are equal, remove this one
            if (points[next] == points[prev])
            {
                points.RemoveAt(p);
                p = Math.Max(0, p - 1); // Backtrack to recheck
                continue; // Skip to next iteration to avoid out-of-bounds errors
            }

            // Remove collinear points (same X or Y direction)
            var isCollinear =
                (
                    Math.Abs(points[prev].X - points[p].X) < tolerance
                    && Math.Abs(points[next].X - points[p].X) < tolerance
                    && Math.Sign(points[next].Z - points[p].Z)
                        == Math.Sign(points[prev].Z - points[p].Z)
                )
                || (
                    Math.Abs(points[prev].Z - points[p].Z) < tolerance
                    && Math.Abs(points[next].Z - points[p].Z) < tolerance
                    && Math.Sign(points[next].X - points[p].X)
                        == Math.Sign(points[prev].X - points[p].X)
                )
                || Vector3.Distance(points[prev], points[next]) < tolerance; // Very close points

            if (isCollinear)
            {
                points.RemoveAt(p);
                p = Math.Max(0, p - 1); // Backtrack to recheck
            }
        }
    }
}
