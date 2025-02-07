using System;
using System.Linq;
using LibTessDotNet;
using MonoBuild.Map;
using MonoBuild.Mesh;

namespace MonoBuild.Render;

public class SectorRenderer(GraphicsDevice graphicsDevice) : IDisposable
{
    private readonly List<WallMesh> _wallMeshes = [];
    private FloorMesh _floorMesh;
    private CeilingMesh _ceilingMesh;

    public void LoadContent(RawSector sector)
    {
        var walls = State.GetSectorWalls(sector).ToArray();
        if (walls.Length < 3)
        {
            Console.WriteLine($"Sector {sector.Id} has less than 3 walls.");
            return; // We need at least three points to form a floor polygon
        }

        foreach (var wall in walls)
        {
            var wallMesh = new WallMesh(graphicsDevice, wall);
            wallMesh.LoadContent();
            _wallMeshes.Add(wallMesh);
        }

        _floorMesh = new FloorMesh(graphicsDevice, sector);
        _floorMesh.LoadContent();

        _ceilingMesh = new CeilingMesh(graphicsDevice, sector);
        _ceilingMesh.LoadContent();
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        foreach (var wallMesh in _wallMeshes)
        {
            wallMesh.Draw(viewMatrix, projectionMatrix);
        }

        _floorMesh.Draw(viewMatrix, projectionMatrix);

        _ceilingMesh.Draw(viewMatrix, projectionMatrix);
    }

    public void Dispose()
    {
        _wallMeshes.ForEach(w => w.Dispose());
        _floorMesh.Dispose();
        _ceilingMesh.Dispose();
    }
}
