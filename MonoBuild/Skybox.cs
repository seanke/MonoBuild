using System.Linq;

namespace MonoBuild;

public class Skybox
{
    private readonly GraphicsDevice _graphicsDevice;
    private VertexBuffer _vertexBuffer;
    private IndexBuffer _indexBuffer;
    private BasicEffect _effect;

    public Skybox(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        CreateCube();
        SetupEffect();
    }

    private void CreateCube()
    {
        // Define cube vertices with gradient colors to indicate direction
        VertexPositionColor[] vertices =
        [
            new(new Vector3(-1, -1, -1), Color.DarkBlue), // 0: Bottom-Left-Front
            new(new Vector3(-1, 1, -1), Color.White), // 1: Top-Left-Front
            new(new Vector3(1, 1, -1), Color.White), // 2: Top-Right-Front
            new(new Vector3(1, -1, -1), Color.DarkBlue), // 3: Bottom-Right-Front
            new(new Vector3(-1, -1, 1), Color.DarkBlue), // 4: Bottom-Left-Back
            new(new Vector3(-1, 1, 1), Color.White), // 5: Top-Left-Back
            new(new Vector3(1, 1, 1), Color.White), // 6: Top-Right-Back
            new(new Vector3(1, -1, 1), Color.DarkBlue) // 7: Bottom-Right-Back
        ];

        // Define indices for each face using the 8 unique vertices
        short[] frontFace = [0, 1, 2, 0, 2, 3]; // Front
        short[] backFace = [4, 6, 5, 4, 7, 6]; // Back
        short[] leftFace = [0, 5, 1, 0, 4, 5]; // Left
        short[] rightFace = [3, 2, 6, 3, 6, 7]; // Right
        short[] topFace = [1, 5, 6, 1, 6, 2]; // Top
        short[] bottomFace = [0, 3, 7, 0, 7, 4]; // Bottom

        // Combine all index arrays into a single array
        var indices = frontFace
            .Concat(backFace)
            .Concat(leftFace)
            .Concat(rightFace)
            .Concat(topFace)
            .Concat(bottomFace)
            .ToArray();

        _vertexBuffer = new VertexBuffer(
            _graphicsDevice,
            typeof(VertexPositionColor),
            vertices.Length,
            BufferUsage.WriteOnly
        );
        _vertexBuffer.SetData(vertices);

        _indexBuffer = new IndexBuffer(
            _graphicsDevice,
            IndexElementSize.SixteenBits,
            indices.Length,
            BufferUsage.WriteOnly
        );
        _indexBuffer.SetData(indices);
    }

    private void SetupEffect()
    {
        _effect = new BasicEffect(_graphicsDevice)
        {
            VertexColorEnabled = true,
            LightingEnabled = false, // No lighting for skybox
        };
    }

    public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
    {
        // Remove translation from the view matrix so the skybox stays fixed around the camera
        var skyboxView = viewMatrix;
        skyboxView.Translation = Vector3.Zero; // Prevent movement of the skybox

        // Set effect parameters
        _effect.World = Matrix.CreateScale(100f); // Keep the skybox centered around the origin
        _effect.View = skyboxView;
        _effect.Projection = projectionMatrix;

        // Disable depth buffer to ensure the skybox renders behind everything
        var originalDepthState = _graphicsDevice.DepthStencilState;
        _graphicsDevice.DepthStencilState = DepthStencilState.None;

        // Set buffers and draw the skybox
        _graphicsDevice.SetVertexBuffer(_vertexBuffer);
        _graphicsDevice.Indices = _indexBuffer;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawIndexedPrimitives(
                PrimitiveType.TriangleList,
                0,
                0,
                _indexBuffer.IndexCount / 3
            );
        }

        // Restore the original depth state
        _graphicsDevice.DepthStencilState = originalDepthState;
    }
}
