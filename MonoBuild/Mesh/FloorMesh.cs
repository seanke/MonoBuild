using System;
using System.Linq;
using MonoBuild.Art;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class FloorMesh(GraphicsDevice graphicsDevice, RawSector sector) : IDisposable
{
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private BasicEffect _effect;
    private Texture2D _texture;

    public void LoadContent()
    {
        // Dispose old buffers if they exist
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        var tile = State.LoadedGroupArt.Tiles[sector.FloorPicnum];

        var tessellatedSector = MeshHelper.GetTessellatedSector(sector, sector.FloorZ);

        // Get tile size
        var tileWidth = tile.Width;
        var tileHeight = tile.Height;

        var vertices = tessellatedSector
            .Vertices.Select(v => new VertexPositionTexture(
                new Vector3(v.Position.X, v.Position.Y, v.Position.Z),
                new Vector2(
                    v.Position.X / tileWidth + (sector.FloorXpanning / (float)tileWidth),
                    v.Position.Z / tileHeight + (sector.FloorYpanning / (float)tileHeight) // Flip Y if necessary
                )
            ))
            .ToArray();

        var indices = tessellatedSector.Elements.Select(i => (short)i).ToArray(); // Cast to `short`

        try
        {
            // Create and set the vertex buffer
            _vertexBuffer = new VertexBuffer(
                graphicsDevice,
                typeof(VertexPositionTexture),
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
                TextureEnabled = true, // Enable texturing
                VertexColorEnabled = false, // Disable vertex coloring (optional)
                World = Matrix.Identity,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    graphicsDevice.Viewport.AspectRatio,
                    0.1f,
                    100f
                )
            };

            // Set the texture
            _effect.Texture = _texture;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load sector {sector.Id}: {ex.Message}");
        }

        // Load a texture for the floor
        _texture = TextureHelper.CreateTextureFromTile(
            graphicsDevice,
            tile,
            State.LoadedPaletteFile.Colors
        );
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        if (_vertexBuffer == null || _indexBuffer == null || _texture == null)
            return; // Don't draw if buffers aren't loaded

        graphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

        _effect.View = viewMatrix; // Update the View matrix from the camera
        _effect.Projection = projectionMatrix; // Update the Projection matrix
        _effect.Texture = _texture; // Ensure the texture is set

        graphicsDevice.SetVertexBuffer(_vertexBuffer);
        graphicsDevice.Indices = _indexBuffer;

        foreach (var pass in _effect.CurrentTechnique.Passes)
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
