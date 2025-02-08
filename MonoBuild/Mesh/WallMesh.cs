using System;
using MonoBuild.Art;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class WallMesh(GraphicsDevice device, RawSector sector, RawWall wall, RawWall nextWall)
    : IDisposable
{
    private VertexBuffer _vertexBufferBottomWall;
    private VertexBuffer _vertexBufferTopWall;

    private IndexBuffer _indexBufferBottomWall;
    private IndexBuffer _indexBufferTopWall;

    private BasicEffect _effect;
    private Texture2D _texture;

    public void LoadContent()
    {
        try
        {
            LoadTopWall();
            LoadBottomWall();

            // Initialize BasicEffect for rendering
            _effect = new BasicEffect(device)
            {
                TextureEnabled = true,
                VertexColorEnabled = false,
                LightingEnabled = false,
                World = Matrix.Identity,
                View = Matrix.CreateLookAt(new Vector3(0, 0, 5), Vector3.Zero, Vector3.Up),
                Projection = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    device.Viewport.AspectRatio,
                    0.1f,
                    100f
                )
            };

            // Load texture
            var tile = State.LoadedGroupArt.Tiles[wall.Picnum];
            _texture = TextureHelper.CreateTextureFromTile(
                device,
                tile,
                State.LoadedPaletteFile.Colors
            );
            _effect.Texture = _texture;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load wall mesh: {ex.Message}");
        }
    }

    private void LoadBottomWall()
    {
        // Dispose old buffers if they exist
        _vertexBufferBottomWall?.Dispose();
        _indexBufferBottomWall?.Dispose();

        var top = sector.CeilingZ;
        var bottom = sector.FloorZ;

        var isPortal = wall.NextWall != -1;
        if (isPortal)
        {
            var nextSector = State.LoadedRawMap.Sectors[wall.NextSector];
            if (nextSector == null)
                return; // Skip if wall is a portal

            if (sector.FloorZ <= nextSector.FloorZ)
                return;

            bottom = sector.FloorZ;
            top = nextSector.FloorZ;
        }

        // Define the four corners of the wall quad
        var wallPoints = new List<Vector3>
        {
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.X, wall.Y, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(nextWall.X, nextWall.Y, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(nextWall.X, nextWall.Y, top)),
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.X, wall.Y, top))
        };

        // Create vertex array (with UV mapping for textures)
        var vertices = new[]
        {
            new VertexPositionTexture(wallPoints[0], new Vector2(0, 1)), // Bottom Left
            new VertexPositionTexture(wallPoints[1], new Vector2(1, 1)), // Bottom Right
            new VertexPositionTexture(wallPoints[2], new Vector2(1, 0)), // Top Right
            new VertexPositionTexture(wallPoints[3], new Vector2(0, 0)) // Top Left
        };

        // Define indices for two triangles forming the quad
        var indices = new short[]
        {
            0,
            1,
            2, // First triangle
            2,
            3,
            0 // Second triangle
        };

        // Create and set the vertex buffer
        _vertexBufferBottomWall = new VertexBuffer(
            device,
            typeof(VertexPositionTexture),
            vertices.Length,
            BufferUsage.WriteOnly
        );
        _vertexBufferBottomWall.SetData(vertices);

        // Create and set the index buffer
        _indexBufferBottomWall = new IndexBuffer(
            device,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly
        );
        _indexBufferBottomWall.SetData(indices);
    }

    private void LoadTopWall()
    {
        // Dispose old buffers if they exist
        _vertexBufferTopWall?.Dispose();
        _indexBufferTopWall?.Dispose();

        var isPortal = wall.NextWall != -1;
        if (!isPortal)
            return; // Only the bottom wall is needed if not a portal

        var nextSector = State.LoadedRawMap.Sectors[wall.NextSector];
        if (nextSector == null)
            return; // Skip if wall is a portal

        if (sector.CeilingZ >= nextSector.CeilingZ)
            return;

        var bottom = sector.CeilingZ;
        var top = nextSector.CeilingZ;

        // Define the four corners of the wall quad
        var wallPoints = new List<Vector3>
        {
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.X, wall.Y, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(nextWall.X, nextWall.Y, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(nextWall.X, nextWall.Y, top)),
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.X, wall.Y, top))
        };

        // Create vertex array (with UV mapping for textures)
        var vertices = new[]
        {
            new VertexPositionTexture(wallPoints[0], new Vector2(0, 1)), // Bottom Left
            new VertexPositionTexture(wallPoints[1], new Vector2(1, 1)), // Bottom Right
            new VertexPositionTexture(wallPoints[2], new Vector2(1, 0)), // Top Right
            new VertexPositionTexture(wallPoints[3], new Vector2(0, 0)) // Top Left
        };

        // Define indices for two triangles forming the quad
        var indices = new short[]
        {
            0,
            1,
            2, // First triangle
            2,
            3,
            0 // Second triangle
        };

        // Create and set the vertex buffer
        _vertexBufferTopWall = new VertexBuffer(
            device,
            typeof(VertexPositionTexture),
            vertices.Length,
            BufferUsage.WriteOnly
        );
        _vertexBufferTopWall.SetData(vertices);

        // Create and set the index buffer
        _indexBufferTopWall = new IndexBuffer(
            device,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly
        );
        _indexBufferTopWall.SetData(indices);
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        DrawWall(viewMatrix, projectionMatrix, _vertexBufferTopWall, _indexBufferTopWall);
        DrawWall(viewMatrix, projectionMatrix, _vertexBufferBottomWall, _indexBufferBottomWall);
    }

    private void DrawWall(
        Matrix viewMatrix,
        Matrix projectionMatrix,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer
    )
    {
        if (vertexBuffer == null || indexBuffer == null || _texture == null)
            return;

        device.SamplerStates[0] = SamplerState.LinearWrap;

        _effect.View = viewMatrix;
        _effect.Projection = projectionMatrix;
        _effect.Texture = _texture;

        device.SetVertexBuffer(vertexBuffer);
        device.Indices = indexBuffer;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                indexBuffer.IndexCount / 3
            );
        }
    }

    public void Dispose()
    {
        _vertexBufferBottomWall?.Dispose();
        _indexBufferBottomWall?.Dispose();
        _effect?.Dispose();
        _texture?.Dispose();
    }
}
