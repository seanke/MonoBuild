using System;
using System.Linq;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class FloorMesh(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    private List<LoopMesh> _loopMeshes = new();
    private List<WallMesh> _wallMeshes = new();

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
                var nextWall = wallLoop.FirstOrDefault(w => w.Id == wall.Point2);
                if (nextWall == null)
                    continue;

                var wallMesh = new WallMesh(graphicsDevice, sector, wall, nextWall);
                wallMesh.LoadContent();
                _wallMeshes.Add(wallMesh);
            }
        }
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        foreach (var loopMesh in _loopMeshes)
        {
            loopMesh.Draw(viewMatrix, projectionMatrix);
        }

        foreach (var wallMesh in _wallMeshes)
        {
            wallMesh.Draw(viewMatrix, projectionMatrix);
        }
    }

    public void Dispose()
    {
        foreach (var loopMesh in _loopMeshes)
        {
            loopMesh.Dispose();
        }

        foreach (var wallMesh in _wallMeshes)
        {
            wallMesh.Dispose();
        }
    }
}
