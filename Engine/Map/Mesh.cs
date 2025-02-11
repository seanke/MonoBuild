using System.Numerics;
using Engine.Art;

namespace Engine.Map;

public class Mesh
{
    public List<Vector3> Vertices { get;}
    public List<int> Indices { get; }
    public Tile Texture { get; }
}