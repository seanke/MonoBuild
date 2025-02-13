using System.Collections.Immutable;
using System.Numerics;
using Engine.Art;

namespace Engine.Map;

public enum MeshType
{
    Floor,
    Ceiling,
    UpperWall,
    LowerWall,
    SolidWall
}

public class Mesh(
    IEnumerable<Vertex> vertices,
    IEnumerable<int> indices,
    Tile texture,
    Sector sector,
    MeshType type
)
{
    public ImmutableList<Vertex> Vertices { get; } = vertices.ToImmutableList();
    public ImmutableList<int> Indices { get; } = indices.ToImmutableList();
    public Tile Texture { get; } = texture;

    public int SectorId => sector.Id;

    public MeshType Type { get; } = type;
}
