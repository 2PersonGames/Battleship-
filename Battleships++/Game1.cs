using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Battleships__
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Rectangle viewportRect;
        Rectangle titleSafeAreaRect;
        Texture2D background1;
        Texture2D background2;
        SpriteFont font1;
        Rectangle movingBackground1;
        Rectangle movingBackground2;

        Battleships game;

        bool onMenu;

        GamePadState previousGamePadStateOne;
        GamePadState previousGamePadStateTwo;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SetResolution(1280, 720);

            game = new Battleships(titleSafeAreaRect, 10, 10);
            game.LoadShips(Content.Load<Texture2D>("Sprites\\Ships\\carrier"),
                           Content.Load<Texture2D>("Sprites\\Ships\\battleship"),
                           Content.Load<Texture2D>("Sprites\\Ships\\cruiser1"),
                           Content.Load<Texture2D>("Sprites\\Ships\\cruiser1"),
                           Content.Load<Texture2D>("Sprites\\Ships\\destroyer"),
                           Content.Load<Texture2D>("Sprites\\grid"),
                           Content.Load<Texture2D>("Sprites\\pointer"));
            game.LoadPowerups(Content.Load<Texture2D>("Sprites\\Powerups\\powerup1"),
                              Content.Load<Texture2D>("Sprites\\Powerups\\powerup2"),
                              Content.Load<Texture2D>("Sprites\\Powerups\\powerup3"),
                              Content.Load<Texture2D>("Sprites\\Powerups\\powerup4"),
                              Content.Load<Texture2D>("Sprites\\Powerups\\powerup5"),
                              Content.Load<Texture2D>("Sprites\\Powerups\\powerup6"));
            game.LoadGridSprites(Content.Load<Texture2D>("Sprites\\miss"), 
                                 Content.Load<Texture2D>("Sprites\\hit"), 
                                 Content.Load<Texture2D>("Sprites\\sunk"),
                                 Content.Load<Texture2D>("Sprites\\keymap"),
                                 Content.Load<Texture2D>("Sprites\\gameover"));

            onMenu = false;

            background1 = Content.Load<Texture2D>("Sprites\\Backgrounds\\movingBackground1");
            background2 = Content.Load<Texture2D>("Sprites\\Backgrounds\\movingBackground2");
            movingBackground1 = new Rectangle(0, 0, viewportRect.Width, viewportRect.Height);
            movingBackground2 = new Rectangle(viewportRect.Width, 0, viewportRect.Width, viewportRect.Height);

            font1 = Content.Load<SpriteFont>("Sprites\\font1");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (!game.IsPaused)
            {
                //Moving background
                movingBackground1.X -= 1;
                if (movingBackground1.X == -viewportRect.Width)
                {
                    movingBackground1.X = viewportRect.Width;
                }
                movingBackground2.X -= 1;
                if (movingBackground2.X == -viewportRect.Width)
                {
                    movingBackground2.X = viewportRect.Width;
                }

                if (!onMenu)
                {
                    game.Update(previousGamePadStateOne, previousGamePadStateTwo);
                }
            }

            base.Update(gameTime);

            previousGamePadStateOne = GamePad.GetState(PlayerIndex.One);
            previousGamePadStateTwo = GamePad.GetState(PlayerIndex.Two);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Draw(background1, movingBackground1, Color.White);
            spriteBatch.Draw(background2, movingBackground2, Color.White);

            if (!onMenu)
            {
                game.Draw(spriteBatch, font1, viewportRect);
            }

            base.Draw(gameTime);
            spriteBatch.End();
        }

        void SetResolution(int width, int height)
        {
            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.ApplyChanges();

            viewportRect = new Rectangle(0, 0, width, height);
            titleSafeAreaRect = new Rectangle((int)(width * 0.1f), (int)(height * 0.1f), (int)(width * 0.8f), (int)(height * 0.8f));
        }
    }
}
