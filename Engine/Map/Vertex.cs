using System.Numerics;

namespace Engine.Map;

public class Vertex
{
    public Vector3 Position { get; }
    public Vector2 TextureCoordinate { get; }

    public Vertex(Vector3 position, Vector2 textureCoordinate)
    {
        Position = position;
        TextureCoordinate = textureCoordinate;
    }
}
