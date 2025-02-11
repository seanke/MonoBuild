using System;
using MonoBuild.Art;
using MonoBuild.Map;

namespace MonoBuild.Mesh;

public class WallTopMesh(GraphicsDevice device, RawSector sector, RawWall wall, RawWall point2Wall)
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

            LoadTopWall();
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

        var isPortal = wall.NextWall != -1;

        if (!isPortal)
            return;

        var nextSector = State.LoadedRawMap.Sectors[wall.NextSector];
        if (nextSector == null)
            return;

        if (nextSector.CeilingZ <= sector.CeilingZ)
            return;

        var top = sector.CeilingZ;
        var bottom = nextSector.CeilingZ;

        LoadWallBuffers(sector, bottom, top);
    }

    private void LoadWallBuffers(RawSector sector, int bottom, int top)
    {
        // Define the four corners of the wall quad
        var wallPoints = new List<Vector3>
        {
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.RawX, wall.RawY, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(point2Wall.RawX, point2Wall.RawY, bottom)),
            MapHelper.ConvertDuke3DToMono(new Vector3(point2Wall.RawX, point2Wall.RawY, top)),
            MapHelper.ConvertDuke3DToMono(new Vector3(wall.RawX, wall.RawY, top))
        };

        var wallHeight = bottom - top;
        var wallHeightAdjusted = wallHeight * MapHelper.BuildHeightUnitMeterRatio;

        var wallWidth = Vector2.Distance(
            new Vector2(wall.RawX, wall.RawY),
            new Vector2(point2Wall.RawX, point2Wall.RawY)
        );
        var wallWidthAdjusted = wallWidth * MapHelper.BuildWidthUnitMeterRatio;

        var vertices = CreateVerticesWithTextureMappings(
            wallPoints,
            wallHeightAdjusted,
            wallWidthAdjusted,
            wall
        );
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
        _vertexBuffer = new VertexBuffer(
            device,
            typeof(VertexPositionTexture),
            vertices.Length,
            BufferUsage.WriteOnly
        );
        _vertexBuffer.SetData(vertices);

        // Create and set the index buffer
        _indexBuffer = new IndexBuffer(
            device,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly
        );
        _indexBuffer.SetData(indices);
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

        var xTextureWidth = _texture.Width;
        var xWallWidth = wallWidth;
        var xRatio = xWallWidth / xTextureWidth;
        var xRepeat = wall.YRepeat / 8f; // WHY DOES THIS WORK WITH YREPEAT?
        var xScale = xRatio * xRepeat;

        var xOffset = 0f + wall.XPanning / 256f; //wall.YPanning;

        if (wall.Id == 795)
            Console.WriteLine("hi");

        // HEIGHT AND Y
        var yTextureHeight = _texture.Height;
        var yRatio = wallHeight / yTextureHeight;
        var yRepeat = wall.YRepeat / 8f;
        var yScale = yRatio * yRepeat;
        var yModRation = wallHeight % yTextureHeight;
        var yPanning = wall.YPanning / 256f;

        var yOffset = 0f + yPanning;

        // Correct UVs to scale texture properly to wall size
        var bottomLeftUv = new Vector2(xOffset, yScale + yOffset);
        var bottomRightUv = new Vector2(xScale + xOffset, yScale + yOffset);
        var topRightUv = new Vector2(xScale + xOffset, yOffset);
        var topLeftUv = new Vector2(0 + xOffset, yOffset);

        if (wall.Id == 1001)
            Console.WriteLine("hi");

        return
        [
            new VertexPositionTexture(wallPoints[0], bottomLeftUv),
            new VertexPositionTexture(wallPoints[1], bottomRightUv),
            new VertexPositionTexture(wallPoints[2], topRightUv),
            new VertexPositionTexture(wallPoints[3], topLeftUv)
        ];

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
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        if (_vertexBuffer == null || _indexBuffer == null || _effect == null || _texture == null)
            return;

        device.SamplerStates[0] = SamplerState.LinearWrap;

        _effect.View = viewMatrix;
        _effect.Projection = projectionMatrix;
        _effect.Texture = _texture;

        device.SetVertexBuffer(_vertexBuffer);
        device.Indices = _indexBuffer;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawIndexedPrimitives(
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
        _texture?.Dispose();
    }
}
