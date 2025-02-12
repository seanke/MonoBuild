using System.Collections.Immutable;
using System.Numerics;
using Engine.Art;

namespace Engine.Map;

public class Mesh
{
    public ImmutableList<Vector3> Vertices { get; }
    public ImmutableList<int> Indices { get; }
    public Tile Texture { get; }

    public Mesh(IEnumerable<Vector3> vertices, IEnumerable<int> indices, Tile texture)
    {
        Vertices = vertices.ToImmutableList();
        Indices = indices.ToImmutableList();
        Texture = texture;
    }
}
