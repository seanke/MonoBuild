using System;
using System.Linq;
using System.Timers;
using LibTessDotNet;

namespace MonoBuild.Map;

public class SectorMesh(GraphicsDevice graphicsDevice) : IDisposable
{
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private BasicEffect _effect;

    private Tess GetTessellatedSector(RawSector sector)
    {
        var walls = State.GetSectorWalls(sector).ToArray();

        // Create a list of points (as Vector3) for the floor.
        var floorPoints = walls
            .Select(w => MapHelper.ConvertDuke3DToMono(new Vector3(w.X, w.Y, sector.FloorZ)))
            .ToList();

        // Use LibTessDotNet to triangulate the polygon.
        var tess = new Tess();

        // Convert your floorPoints to LibTessDotNet's ContourVertex format.
        var contour = floorPoints
            .Select(p => new ContourVertex
            {
                Position = new Vec3
                {
                    X = p.X,
                    Y = p.Y,
                    Z = p.Z
                }
            })
            .ToArray();

        // Add the contour (polygon). Specify the original orientation.
        tess.AddContour(contour, ContourOrientation.Original);

        // Tessellate using an appropriate winding rule and specify that we want triangles.
        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

        return tess;
    }

    public void LoadContent(RawSector sector)
    {
        var walls = State.GetSectorWalls(sector).ToArray();
        if (walls.Length < 3)
            return; // We need at least three points to form a floor polygon

        // Dispose old buffers if they exist
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        var tessellatedSector = GetTessellatedSector(sector);

        // Convert tessellated data to MonoGame format
        var vertices = tessellatedSector
            .Vertices.Select(v => new VertexPositionColor(
                new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
                Color.Lerp(Color.Red, Color.Blue, v.Position.Y) // Gradient coloring
            ))
            .ToArray();

        var indices = tessellatedSector.Elements.Select(i => (short)i).ToArray(); // Cast to `short`

        // Create and set the vertex buffer
        _vertexBuffer = new VertexBuffer(
            graphicsDevice,
            typeof(VertexPositionColor),
            vertices.Length,
            BufferUsage.WriteOnly
        );
        _vertexBuffer.SetData(vertices);

        // Create and set the index buffer
        _indexBuffer = new IndexBuffer(
            graphicsDevice,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly
        );
        _indexBuffer.SetData(indices);

        // Create a basic effect for rendering
        _effect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            World = Matrix.Identity,
            View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up),
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                graphicsDevice.Viewport.AspectRatio,
                0.1f,
                100f
            )
        };
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        if (_vertexBuffer == null || _indexBuffer == null)
            return; // Don't draw if buffers aren't loaded

        _effect.View = viewMatrix; // Update the View matrix from the camera
        _effect.Projection = projectionMatrix; // Update the Projection matrix

        graphicsDevice.SetVertexBuffer(_vertexBuffer);
        graphicsDevice.Indices = _indexBuffer;

        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            graphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                _indexBuffer.IndexCount / 3
            );
        }
    }

    public void Dispose()
    {
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _effect?.Dispose();
    }
}
