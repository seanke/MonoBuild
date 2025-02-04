using System.Security.Cryptography.X509Certificates;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoBuild;

public class Game : Microsoft.Xna.Framework.Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    #region TUT
    Texture2D ballTexture;
    Vector2 ballPosition;
    float ballSpeed;
    #endregion

    public Game()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        #region TUT

        ballPosition = new Vector2(
            _graphics.PreferredBackBufferWidth / 2f,
            _graphics.PreferredBackBufferHeight / 2f
        );
        ballSpeed = 100f;
        #endregion

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        #region TUT
        ballTexture = Content.Load<Texture2D>("ball");
        #endregion
    }

    protected override void Update(GameTime gameTime)
    {
        if (
            GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
            || Keyboard.GetState().IsKeyDown(Keys.Escape)
        )
            Exit();

        #region TUT

        var updateBallSpeed = ballSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.Up))
            ballPosition.Y -= updateBallSpeed;
        if (keyboardState.IsKeyDown(Keys.Down))
            ballPosition.Y += updateBallSpeed;
        if (keyboardState.IsKeyDown(Keys.Left))
            ballPosition.X -= updateBallSpeed;
        if (keyboardState.IsKeyDown(Keys.Right))
            ballPosition.X += updateBallSpeed;

        #endregion

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        #region TUT
        _spriteBatch.Begin();
        _spriteBatch.Draw(
            ballTexture,
            ballPosition,
            null,
            Color.White,
            0f,
            new Vector2(ballTexture.Width / 2f, ballTexture.Height / 2f),
            Vector2.One * 5,
            SpriteEffects.None,
            0f
        );
        _spriteBatch.End();
        #endregion

        base.Draw(gameTime);
    }
}
