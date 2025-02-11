using System;
using System.Linq;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class LoopOutlineMesh(
    GraphicsDevice graphicsDevice,
    RawSector sector,
    List<RawWall> wallLoop
) : IDisposable
{
    private VertexBuffer _wallLineBuffer;
    private BasicEffect _effect;

    public void LoadContent()
    {
        // Dispose old buffers if they exist
        _wallLineBuffer?.Dispose();

        try
        {
            CreateWallLoopDebugBuffer();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load sector {sector.Id}: {ex.Message}");
        }
    }

    private void CreateWallLoopDebugBuffer()
    {
        var wallVertices = new List<VertexPositionColor>();

        for (var i = 0; i < wallLoop.Count; i++)
        {
            var currentWall = wallLoop[i];
            var nextWall = wallLoop.FirstOrDefault(w => w.Id == currentWall.Point2); // Get the next wall

            if (nextWall == null)
                continue; // Skip if next wall is missing

            var start = new Vector3(currentWall.RawX, currentWall.RawY, sector.FloorZ);
            var end = new Vector3(nextWall.RawX, nextWall.RawY, sector.FloorZ);

            start = MapHelper.ConvertDuke3DToMono(start);
            end = MapHelper.ConvertDuke3DToMono(end);

            var isPortal = currentWall.NextWall != -1; // && nextWall.NextWall != -1;
            var isImpassablePortal = isPortal && (currentWall.CStat & 0x01) != 0;

            var lineColor = isPortal ? Color.Red : Color.White;

            if (isImpassablePortal)
                lineColor = Color.Purple;

            wallVertices.Add(new VertexPositionColor(start, lineColor)); // Start point
            wallVertices.Add(new VertexPositionColor(end, lineColor)); // End point
        }

        if (wallVertices.Count > 0)
        {
            _wallLineBuffer = new VertexBuffer(
                graphicsDevice,
                typeof(VertexPositionColor),
                wallVertices.Count,
                BufferUsage.WriteOnly
            );
            _wallLineBuffer.SetData(wallVertices.ToArray());
        }

        _effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            World = Matrix.Identity
        };
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        if (_wallLineBuffer == null)
            return; // Don't draw if buffers aren't loaded

        _effect.View = viewMatrix;
        _effect.Projection = projectionMatrix;

        graphicsDevice.SetVertexBuffer(_wallLineBuffer);

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawPrimitives(
                PrimitiveType.LineList,
                0,
                _wallLineBuffer.VertexCount / 2
            );
        }
    }

    public void Dispose()
    {
        _wallLineBuffer?.Dispose();
        _effect?.Dispose();
    }
}
