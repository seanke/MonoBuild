using System;
using MonoBuild.Map;

namespace MonoBuild.Render;

public class WallsRenderer(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    public void LoadContent() { }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix) { }

    public void Dispose() { }
}
