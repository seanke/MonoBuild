using System;
using Microsoft.Xna.Framework.Input;

namespace MonoBuild.Player;

public class Camera
{
    public Vector3 Position { get; private set; }
    public Vector3 Forward { get; private set; }
    public Vector3 Right { get; private set; }
    public Vector3 Up { get; private set; }

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }

    private float yaw; // Rotation around Y-axis (left/right)
    private float pitch; // Rotation around X-axis (up/down)

    private const float MouseSensitivity = 0.002f;
    private const float MoveSpeed = 500f;
    private const float VerticalSpeed = 2.5f;

    private MouseState _prevMouseState;
    private GraphicsDevice _graphicsDevice;

    public Camera(GraphicsDevice graphicsDevice, Vector3 startPosition)
    {
        _graphicsDevice = graphicsDevice;
        Position = startPosition;

        // Set default orientation
        yaw = MathHelper.PiOver2; // Face forward
        pitch = 0f; // Level pitch

        UpdateVectors();
        UpdateView();
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            graphicsDevice.Viewport.AspectRatio,
            0.1f,
            100000f
        );

        // Center mouse to prevent jumpy movement at start
        ResetMousePosition();
    }

    private void UpdateVectors()
    {
        // Convert yaw & pitch to a directional vector
        Forward = new Vector3(
            (float)Math.Cos(yaw) * (float)Math.Cos(pitch),
            (float)Math.Sin(pitch),
            (float)Math.Sin(yaw) * (float)Math.Cos(pitch)
        );
        Forward = Vector3.Normalize(Forward);

        // Right is perpendicular to forward (strafe direction)
        Right = Vector3.Normalize(Vector3.Cross(Forward, Vector3.Up));

        // Up vector (recalculated to prevent drifting)
        Up = Vector3.Normalize(Vector3.Cross(Right, Forward));
    }

    private void UpdateView()
    {
        View = Matrix.CreateLookAt(Position, Position + Forward, Up);
    }

    public void Update(GameTime gameTime)
    {
        HandleMouseLook();
        HandleMovement(gameTime);
        UpdateView();
    }

    private void HandleMouseLook()
    {
        MouseState mouseState = Mouse.GetState();

        // Get delta movement from last frame
        float deltaX = mouseState.X - _prevMouseState.X;
        float deltaY = mouseState.Y - _prevMouseState.Y;

        // Apply sensitivity & adjust yaw/pitch
        yaw += deltaX * MouseSensitivity;
        pitch -= deltaY * MouseSensitivity;

        // Clamp pitch to prevent flipping
        pitch = MathHelper.Clamp(pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);

        // Update direction vectors
        UpdateVectors();

        // Reset mouse position (centers it)
        ResetMousePosition();
    }

    private void HandleMovement(GameTime gameTime)
    {
        KeyboardState keyboard = Keyboard.GetState();
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        Vector3 moveDirection = Vector3.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            moveDirection += Forward; // Forward
        if (keyboard.IsKeyDown(Keys.S))
            moveDirection -= Forward; // Backward
        if (keyboard.IsKeyDown(Keys.A))
            moveDirection -= Right; // Left
        if (keyboard.IsKeyDown(Keys.D))
            moveDirection += Right; // Right
        if (keyboard.IsKeyDown(Keys.Q))
            moveDirection -= Up; // Down
        if (keyboard.IsKeyDown(Keys.E))
            moveDirection += Up; // Up

        if (moveDirection != Vector3.Zero)
        {
            moveDirection = Vector3.Normalize(moveDirection);
            Position += moveDirection * MoveSpeed * dt;
        }
    }

    private void ResetMousePosition()
    {
        Mouse.SetPosition(_graphicsDevice.Viewport.Width / 2, _graphicsDevice.Viewport.Height / 2);
        _prevMouseState = Mouse.GetState();
    }
}
