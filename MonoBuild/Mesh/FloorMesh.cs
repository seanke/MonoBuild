using System;
using System.Linq;
using MonoBuild.Map;
using MonoBuild.Render;

namespace MonoBuild.Mesh;

public class FloorMesh(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    private List<LoopMesh> _loopMeshes = new();
    private List<WallRenderer> _wallsRenderers = new();

    public void LoadContent()
    {
        var sectorWallLoops = RawSector.GetSectorWallLoops(sector);

        foreach (var wallLoop in sectorWallLoops)
        {
            var loopMesh = new LoopMesh(graphicsDevice, sector, wallLoop);
            loopMesh.LoadContent();
            _loopMeshes.Add(loopMesh);

            foreach (var wall in wallLoop)
            {
                var point2Wall = wallLoop.FirstOrDefault(w => w.Id == wall.Point2);
                if (point2Wall == null)
                    continue;

                var wallMesh = new WallRenderer(graphicsDevice, sector, wall, point2Wall);
                wallMesh.LoadContent();
                _wallsRenderers.Add(wallMesh);
            }
        }
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        foreach (var loopMesh in _loopMeshes)
        {
            loopMesh.Draw(viewMatrix, projectionMatrix);
        }

        foreach (var wallsRenderer in _wallsRenderers)
        {
            wallsRenderer.Draw(viewMatrix, projectionMatrix);
        }
    }

    public void Dispose()
    {
        foreach (var loopMesh in _loopMeshes)
        {
            loopMesh.Dispose();
        }

        foreach (var wallsRenderer in _wallsRenderers)
        {
            wallsRenderer.Dispose();
        }
    }
}
