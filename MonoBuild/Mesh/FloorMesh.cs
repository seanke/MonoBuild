using System;
using System.Linq;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class FloorMesh(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private BasicEffect _effect;

    public void LoadContent()
    {
        // Dispose old buffers if they exist
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        var tessellatedSector = MeshHelper.GetTessellatedSector(sector, sector.FloorZ);

        // Convert tessellated data to MonoGame format
        var vertices = tessellatedSector
            .Vertices.Select(v => new VertexPositionColor(
                new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
                Color.Lerp(Color.Red, Color.Blue, v.Position.Y) // Gradient coloring
            ))
            .ToArray();

        var indices = tessellatedSector.Elements.Select(i => (short)i).ToArray(); // Cast to `short`

        try
        {
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
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load sector {sector.Id}: {ex.Message}");
        }
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
