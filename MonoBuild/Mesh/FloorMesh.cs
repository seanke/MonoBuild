using System;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class FloorMesh(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    private List<LoopMesh> _loopMeshes = new();

    public void LoadContent()
    {
        var sectorWallLoops = RawSector.GetSectorWallLoops(sector);

        foreach (var wallLoop in sectorWallLoops)
        {
            var loopMesh = new LoopMesh(graphicsDevice, sector, wallLoop);
            loopMesh.LoadContent();
            _loopMeshes.Add(loopMesh);
        }
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        foreach (var loopMesh in _loopMeshes)
        {
            loopMesh.Draw(viewMatrix, projectionMatrix);
        }
    }

    public void Dispose()
    {
        foreach (var loopMesh in _loopMeshes)
        {
            loopMesh.Dispose();
        }
    }
}
