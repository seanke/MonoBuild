using System;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class CeilingMesh(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    public void LoadContent() { }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix) { }

    public void Dispose() { }
}
