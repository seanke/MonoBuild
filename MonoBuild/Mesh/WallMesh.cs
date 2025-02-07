using System;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class WallMesh(GraphicsDevice graphicsDevice, RawWall wall) : IDisposable
{
    public void LoadContent() { }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix) { }

    public void Dispose() { }
}
