using System;
using MonoBuild.Art;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class WallTopMesh(GraphicsDevice device, RawSector sector, RawWall wall, RawWall nextWall)
    : IDisposable
{
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private BasicEffect _effect;
    private Texture2D _texture;

    public void LoadContent()
    {
        try
        {
            LoadTopWall();

            // Initialize Top wall
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
            var tileTop = State.LoadedGroupArt.Tiles[wall.Picnum];
            _texture = TextureHelper.CreateTextureFromTile(
                device,
                tileTop,
                State.LoadedPaletteFile.Colors
            );
            _effect.Texture = _texture;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load wall mesh: {ex.Message}");
        }
    }

    private void LoadTopWall()
    {
        // Dispose old buffers if they exist
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();

        var top = sector.CeilingZ;
        var bottom = sector.FloorZ;

        var isPortal = wall.NextWall != -1;

        if (!isPortal)
            return;

        var nextSector = State.LoadedRawMap.Sectors[wall.NextSector];
        if (nextSector == null)
            return;

        if (nextSector.CeilingZ >= sector.CeilingZ)
            return;

        bottom = nextSector.CeilingZ;

        LoadWallBuffers(sector, bottom, top, ref _vertexBuffer, ref _indexBuffer);
    }

    private void LoadWallBuffers(
        RawSector sector,
        int bottom,
        int top,
        ref VertexBuffer vertexBuffer,
        ref IndexBuffer indexBuffer
    )
    {
        // Define the four corners of the wall quad
        var wallPoints = new List<Vector3>
        {
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.X, wall.Y, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(nextWall.X, nextWall.Y, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(nextWall.X, nextWall.Y, top)),
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.X, wall.Y, top))
        };

        var wallHeight = (top - bottom) * -MapHelper.BuildHeightUnitMeterRatio;
        var wallWidth = Vector2.Distance(
            new Vector2(wall.X, wall.Y),
            new Vector2(nextWall.X, nextWall.Y)
        );

        var vertices = CreateVerticesWithTextureMappings(wallPoints, wallHeight, wallWidth, wall);

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
        vertexBuffer = new VertexBuffer(
            device,
            typeof(VertexPositionTexture),
            vertices.Length,
            BufferUsage.WriteOnly
        );
        vertexBuffer.SetData(vertices);

        // Create and set the index buffer
        indexBuffer = new IndexBuffer(
            device,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly
        );
        indexBuffer.SetData(indices);
    }

    private VertexPositionTexture[] CreateVerticesWithTextureMappings(
        List<Vector3> wallPoints,
        float wallHeight,
        float wallWidth,
        RawWall wall
    )
    {
        // WIDTH AND X
        float tileWidth = State.LoadedGroupArt.Tiles[wall.Picnum].Width * 8;
        float tileXShift = State.LoadedGroupArt.Tiles[wall.Picnum].XCenterOffset;

        var xRatio = wallWidth / tileWidth;
        var xRepeat = wall.XRepeat / 8f;
        var xScale = xRatio * xRepeat;

        float xOffset = 0;
        if (wall.XPanning != 0)
            xOffset += wall.XPanning / 256f;

        if (tileXShift != 0)
            xOffset += tileXShift / 256f;

        var texture = State.LoadedGroupArt.Tiles[wall.Picnum];

        if (wall.IsBottomTextureSwapped)
            texture = State.LoadedGroupArt.Tiles[State.LoadedRawMap.Walls[wall.NextWall].Picnum];

        // HEIGHT AND Y
        float tileHeight = texture.Height;
        var yScale = wallHeight / tileHeight * (wall.YRepeat / 8f);
        yScale *= -1;

        float yOffset = 0;

        // Correct UVs to scale texture properly to wall size
        var bottomLeftUv = new Vector2(0 + xOffset, yScale + yOffset);
        var bottomRightUv = new Vector2(xScale + xOffset, yScale + yOffset);
        var topRightUv = new Vector2(xScale + xOffset, 0 + yOffset);
        var topLeftUv = new Vector2(0 + xOffset, 0 + yOffset);

        //TEST
        var right = wall.XRepeat / 8f;
        right = 1;
        var top = 1;

        bottomLeftUv = new Vector2(0, 0);
        bottomRightUv = new Vector2(right, 0);
        topRightUv = new Vector2(right, top);
        topLeftUv = new Vector2(0, top);

        /*var vertices = new[]
        {
            new VertexPositionTexture(wallPoints[0], new Vector2(0, 1)), // Bottom Left
            new VertexPositionTexture(wallPoints[1], new Vector2(1, 1)), // Bottom Right
            new VertexPositionTexture(wallPoints[2], new Vector2(1, 0)), // Top Right
            new VertexPositionTexture(wallPoints[3], new Vector2(0, 0)) // Top Left
        };*/

        // Handle CStat flipping
        var flipX = (wall.CStat & (1 << 3)) != 0;
        var flipY = (wall.CStat & (1 << 8)) != 0;

        if (flipX)
        {
            (bottomLeftUv.X, bottomRightUv.X) = (bottomRightUv.X, bottomLeftUv.X);
            (topLeftUv.X, topRightUv.X) = (topRightUv.X, topLeftUv.X);
        }
        if (flipY)
        {
            (bottomLeftUv.Y, topLeftUv.Y) = (topLeftUv.Y, bottomLeftUv.Y);
            (bottomRightUv.Y, topRightUv.Y) = (topRightUv.Y, bottomRightUv.Y);
        }

        return new[]
        {
            new VertexPositionTexture(wallPoints[0], bottomLeftUv),
            new VertexPositionTexture(wallPoints[1], bottomRightUv),
            new VertexPositionTexture(wallPoints[2], topRightUv),
            new VertexPositionTexture(wallPoints[3], topLeftUv)
        };
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        DrawWall(viewMatrix, projectionMatrix, _vertexBuffer, _indexBuffer);
    }

    private void DrawWall(
        Matrix viewMatrix,
        Matrix projectionMatrix,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer
    )
    {
        if (vertexBuffer == null || indexBuffer == null)
            return;

        device.SamplerStates[0] = SamplerState.LinearWrap;

        DrawWallPart(viewMatrix, projectionMatrix, vertexBuffer, indexBuffer, _effect, _texture);
    }

    private void DrawWallPart(
        Matrix viewMatrix,
        Matrix projectionMatrix,
        VertexBuffer vertexBuffer,
        IndexBuffer indexBuffer,
        BasicEffect effect,
        Texture2D texture
    )
    {
        if (effect == null || texture == null)
            return;

        effect.View = viewMatrix;
        effect.Projection = projectionMatrix;
        effect.Texture = texture;

        device.SetVertexBuffer(vertexBuffer);
        device.Indices = indexBuffer;

        foreach (var pass in effect.CurrentTechnique.Passes)
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
        _vertexBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _effect?.Dispose();
        _texture?.Dispose();
    }
}
