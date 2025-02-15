using Engine.Map;

namespace MonoBuild;

public class Mesh
{
    public VertexBuffer VertexBuffer { get; set; }
    public IndexBuffer IndexBuffer { get; set; }
    public BasicEffect Effect { get; set; }
    public Texture2D Texture { get; set; }
    public Wall Wall { get; set; }
    public Sector Sector { get; set; }
    public MeshType Type { get; set; }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
        Effect.Dispose();
        Texture.Dispose();
    }
}
