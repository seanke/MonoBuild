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

    public float Yaw { get; private set; }
    public float Pitch { get; private set; }

    private const float MouseSensitivity = 0.002f;
    private const float MoveSpeed = 500f;

    private MouseState _prevMouseState;
    private readonly GraphicsDevice _graphicsDevice;

    public Camera(GraphicsDevice graphicsDevice, Vector3 startPosition)
    {
        _graphicsDevice = graphicsDevice;
        Position = startPosition;

        // Set default orientation
        Yaw = MathHelper.PiOver2; // Face forward
        Pitch = 0f; // Level pitch

        UpdateVectors();
        UpdateView();
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            graphicsDevice.Viewport.AspectRatio,
            0.1f,
            100000000f
        );

        // Center mouse to prevent jumpy movement at start
        ResetMousePosition();
    }

    private void UpdateVectors()
    {
        // Convert yaw & pitch to a directional vector
        Forward = new Vector3(
            (float)Math.Cos(Yaw) * (float)Math.Cos(Pitch),
            (float)Math.Sin(Pitch),
            (float)Math.Sin(Yaw) * (float)Math.Cos(Pitch)
        );
        Forward = Vector3.Normalize(Forward);

        // Instead of using world Up in Right calculation, use a fixed up axis
        var worldUp = Vector3.Up;

        // Calculate Right (perpendicular to Forward and world up)
        Right = Vector3.Normalize(Vector3.Cross(worldUp, Forward));

        // Up is strictly perpendicular to Forward and Right, preventing roll
        Up = Vector3.Normalize(Vector3.Cross(Forward, Right));
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
        var mouseState = Mouse.GetState();

        // Get delta movement from last frame
        float deltaX = mouseState.X - _prevMouseState.X;
        float deltaY = mouseState.Y - _prevMouseState.Y;

        // Apply sensitivity & adjust yaw/pitch
        Yaw += deltaX * MouseSensitivity;
        Pitch -= deltaY * MouseSensitivity;

        // Clamp pitch to prevent flipping
        Pitch = MathHelper.Clamp(Pitch, -MathHelper.PiOver2 + 0.01f, MathHelper.PiOver2 - 0.01f);

        // Update direction vectors
        UpdateVectors();

        // Reset mouse position (centers it)
        ResetMousePosition();
    }

    private void HandleMovement(GameTime gameTime)
    {
        var keyboard = Keyboard.GetState();
        var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        var moveDirection = Vector3.Zero;

        if (keyboard.IsKeyDown(Keys.W))
            moveDirection += Forward; // Forward
        if (keyboard.IsKeyDown(Keys.S))
            moveDirection -= Forward; // Backward
        if (keyboard.IsKeyDown(Keys.A))
            moveDirection += Right; // Left
        if (keyboard.IsKeyDown(Keys.D))
            moveDirection -= Right; // Right
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
