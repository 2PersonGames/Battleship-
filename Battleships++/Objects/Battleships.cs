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
    class Battleships
    {
        Random powerupGen;
        int powerupNumber;

        Rectangle drawAreaRectLeft;
        Rectangle drawAreaRectRight;
        Rectangle drawSquareRect;
        Rectangle[,] gridRects;
        List<Point> activeGrids;

        Texture2D grid;
        Texture2D[] powerups;
        Texture2D sunk;
        Texture2D hit;
        Texture2D miss;
        Texture2D keyMap;
        Texture2D gameOver;

        GameObject[] playerOneShips;
        GameObject[] playerTwoShips;
        GameObject pointer;

        int playerOneBombs;
        int playerTwoBombs;

        int playerOnePower;
        int PlayerOnePower
        {
            get { return playerOnePower; }
            set
            {
                playerOnePower = value;
                if (playerOnePower > 5)
                {
                    playerOnePower = 5;
                }
                else if (playerOnePower < 0)
                {
                    playerOnePower = 0;
                }
            }
        }
        int playerTwoPower;
        int PlayerTwoPower
        {
            get { return playerTwoPower; }
            set
            {
                playerTwoPower = value;
                if (playerTwoPower > 5)
                {
                    playerTwoPower = 5;
                }
                else if (playerTwoPower < 0)
                {
                    playerTwoPower = 0;
                }
            }
        }

        //False not shot at
        //True shot at
        bool[,] playerOneGrid;
        bool[,] playerTwoGrid;

        int width;
        public int Width
        {
            get { return width; }
        }
        int height;
        public int Height
        {
            get { return height; }
        }

        //Game state controllers
        bool setup;
        bool playerOneActive;
        bool turnOver;
        bool fired;
        bool powerupUsed;
        bool isPaused;
        public bool IsPaused
        {
            get { return isPaused; }
        }
        bool endgame;

        public Battleships(Rectangle viewportRect, int nWidth, int nHeight)
        {
            drawAreaRectLeft = new Rectangle(viewportRect.X, viewportRect.Y, (int)(viewportRect.Width / 2f), viewportRect.Height);
            drawAreaRectRight = new Rectangle(viewportRect.X + (int)(viewportRect.Width / 2f), viewportRect.Y, drawAreaRectLeft.Width - 2, viewportRect.Height - 6);
            drawSquareRect = new Rectangle(viewportRect.X, viewportRect.Y, drawAreaRectLeft.Width / nWidth, drawAreaRectLeft.Height / nHeight);

            playerOneShips = new GameObject[5];
            playerTwoShips = new GameObject[5];

            width = nWidth;
            height = nHeight;

            playerOneGrid = new bool[nWidth, nHeight];
            playerTwoGrid = new bool[nWidth, nHeight];
            for (int i = 0; i < nWidth; i++)
            {
                for (int j = 0; j < nHeight; j++)
                {
                    playerOneGrid[i, j] = false;
                    playerTwoGrid[i, j] = false;
                }
            }

            setup = true;
            playerOneActive = true;

            gridRects = new Rectangle[Width, Height];
            activeGrids = new List<Point>();

            playerOneBombs = 5;
            playerTwoBombs = 5;
            PlayerOnePower = 1;
            PlayerTwoPower = 0;

            powerupGen = new Random();
            powerupNumber = powerupGen.Next(0, 6);

            powerupUsed = false;
            fired = false;
            turnOver = false;
            endgame = false;
        }

        public void LoadShips(Texture2D carrier, Texture2D battleship, Texture2D cruiser1, Texture2D cruiser2, Texture2D destroyer, Texture2D nGrid, Texture2D nPointer)
        {
            playerOneShips[0] = new GameObject(carrier, 2, "Carrier", 5, 1, GenRect(5, 1));
            playerOneShips[1] = new GameObject(battleship, 2, "Battleship", 4, 1, GenRect(4, 1));
            playerOneShips[2] = new GameObject(cruiser1, 2, "1st Cruiser", 3, 1, GenRect(3, 1));
            playerOneShips[3] = new GameObject(cruiser2, 2, "2nd Cruiser", 3, 1, GenRect(3, 1));
            playerOneShips[4] = new GameObject(destroyer, 2, "Destroyer", 2, 1, GenRect(2, 1));
            playerOneShips[4].Alive = true;

            playerTwoShips[0] = new GameObject(carrier, 2, "Carrier", 5, 1, GenRect(5, 1));
            playerTwoShips[1] = new GameObject(battleship, 2, "Battleship", 4, 1, GenRect(4, 1));
            playerTwoShips[2] = new GameObject(cruiser1, 2, "1st Cruiser", 3, 1, GenRect(3, 1));
            playerTwoShips[3] = new GameObject(cruiser2, 2, "2nd Cruiser", 3, 1, GenRect(3, 1));
            playerTwoShips[4] = new GameObject(destroyer, 2, "Destroyer", 2, 1, GenRect(2, 1));
            playerTwoShips[4].Alive = true;

            pointer = new GameObject(nPointer, 0, "pointer", 0, 0, Rectangle.Empty);

            grid = nGrid;

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    drawSquareRect.Y = (drawSquareRect.Height * j) + drawAreaRectRight.Y;
                    drawSquareRect.X = (drawSquareRect.Width * i) + drawAreaRectRight.X;
                    gridRects[i, j] = new Rectangle((drawSquareRect.Width * i) + drawAreaRectRight.X, (drawSquareRect.Height * j) + drawAreaRectRight.Y, drawSquareRect.Width, drawSquareRect.Height);
                }
            }
        }

        public void LoadPowerups(Texture2D powerup1, Texture2D powerup2, Texture2D powerup3, Texture2D powerup4, Texture2D powerup5, Texture2D powerup6)
        {
            powerups = new Texture2D[6];
            powerups[0] = powerup1;
            powerups[1] = powerup2;
            powerups[2] = powerup3;
            powerups[3] = powerup4;
            powerups[4] = powerup5;
            powerups[5] = powerup6;
        }

        public void LoadGridSprites(Texture2D nMiss, Texture2D nHit, Texture2D nSunk, Texture2D nKeyMap, Texture2D gameover)
        {
            sunk = nSunk;
            miss = nMiss;
            hit = nHit;
            keyMap = nKeyMap;
            gameOver = gameover;
        }

        Rectangle GenRect(int width, int height)
        {
            Rectangle rect = new Rectangle(drawAreaRectRight.X, drawAreaRectRight.Y, (width * drawSquareRect.Width), (height * drawSquareRect.Height));
            return rect;
        }

        public void Update(GamePadState previousGamePadStateOne, GamePadState previousGamePadStateTwo)
        {
            Vector2 velocity;
            GamePadState previousState;
            GamePadState currentState;
            GameObject[] player;
            GameObject[] enemy;

            if (playerOneActive)
            {
                //Player one's turn
                velocity.X = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 5;
                velocity.Y = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 5;
                previousState = previousGamePadStateOne;
                player = playerOneShips;
                enemy = playerTwoShips;
                currentState = GamePad.GetState(PlayerIndex.One);
            }
            else
            {
                //Player two's turn
                velocity.X = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 5;
                velocity.Y = -GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.Y * 5;
                previousState = previousGamePadStateOne;
                player = playerTwoShips;
                enemy = playerOneShips;
                currentState = GamePad.GetState(PlayerIndex.One);
            }

            pointer.Update(velocity, drawAreaRectRight);

            if (!setup)
            {
                PlayerTurn(velocity, currentState, previousState, enemy, player);
            }
            else
            {
                PlayerSetup(velocity, player, currentState, previousState);
            }

            if (endgame)
            {
                isPaused = true;
            }
        }

        void PlayerSetup(Vector2 velocity, GameObject[] player, GamePadState playerPad, GamePadState previousPlayerPad)
        {
            pointer.Update(velocity, drawAreaRectRight);

            //Update ship.rect according to pointer location
            SetupShipLocation(player, playerPad, previousPlayerPad);
        }

        void SetupShipLocation(GameObject[] player, GamePadState playerPad, GamePadState previousPlayerPad)
        {
            Point pointerPosition = new Point((int)pointer.Position.X, (int)pointer.Position.Y);
            activeGrids = new List<Point>();

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].Alive)
                {
                    foreach (Rectangle rect in gridRects)
                    {
                        if (rect.Contains(pointerPosition))
                        {
                            Rectangle nShipRect = new Rectangle(rect.X, rect.Y, player[i].Rect.Width, player[i].Rect.Height);

                            int shipRectWidth = player[i].Rect.Width / drawSquareRect.Width;
                            int shipRectHeight = player[i].Rect.Height / drawSquareRect.Height;

                            if (nShipRect.Y + nShipRect.Height > drawAreaRectRight.Y + drawAreaRectRight.Height)
                            {
                                nShipRect.Y = drawAreaRectRight.Y + drawAreaRectRight.Height - nShipRect.Height;
                            }
                            else if (nShipRect.Y < drawAreaRectRight.Y)
                            {
                                nShipRect.Y = drawAreaRectRight.Y;
                            }
                            if (nShipRect.X + nShipRect.Width > drawAreaRectRight.X + drawAreaRectRight.Width)
                            {
                                nShipRect.X = drawAreaRectRight.X + drawAreaRectRight.Width - nShipRect.Width;
                            }
                            else if (nShipRect.X < drawAreaRectRight.X)
                            {
                                nShipRect.X = drawAreaRectRight.X;
                            }

                            for (int m = 0; m < shipRectWidth; m++)
                            {
                                for (int j = 0; j < shipRectHeight; j++)
                                {
                                    activeGrids.Add(new Point(nShipRect.X + (drawSquareRect.Width * m), nShipRect.Y + (drawSquareRect.Height * j)));
                                }
                            }

                            player[i].Rect = nShipRect;

                            if (playerPad.Buttons.A == ButtonState.Pressed && previousPlayerPad.Buttons.A == ButtonState.Released)
                            {
                                bool checkOkay = true;
                                foreach (GameObject ship in player)
                                {
                                    if (ship.Alive && ship.Rect.Intersects(player[i].Rect) && ship.Rect != player[i].Rect)
                                    {
                                        checkOkay = false;
                                        break;
                                    }
                                }
                                if (checkOkay)
                                {
                                    activeGrids = new List<Point>();
                                    pointer.Position = new Vector2(drawAreaRectRight.X, drawAreaRectRight.Y);

                                    if (i != 0)
                                    {
                                        player[i - 1].Alive = true;
                                    }
                                    else if (!playerOneActive)
                                    {
                                        playerOneActive = true;
                                        setup = false;
                                    }
                                    else
                                    {
                                        playerOneActive = false;
                                    }
                                }
                            }

                            if (playerPad.Buttons.B == ButtonState.Pressed && previousPlayerPad.Buttons.B == ButtonState.Released)
                            {
                                //Rotate ship 90 degrees
                                int rectWidth = player[i].Rect.Width;
                                int rectHeight = player[i].Rect.Height;

                                player[i].Rect = new Rectangle(player[i].Rect.X, player[i].Rect.Y, shipRectHeight * drawSquareRect.Width, shipRectWidth * drawSquareRect.Height);
                                player[i].Rotation += MathHelper.PiOver2;
                                if (player[i].Rotation > MathHelper.PiOver2)
                                {
                                    player[i].Rotation = 0f;
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font1, Rectangle viewportRect)
        {
            if (!setup)
            {
                if (playerOneActive)
                {
                    DrawGrid(spriteBatch, drawAreaRectRight, playerOneGrid, playerTwoShips);
                    PlayerDraw(spriteBatch, playerOneShips, playerTwoShips, font1, GamePad.GetState(PlayerIndex.One), viewportRect);
                }
                else
                {
                    DrawGrid(spriteBatch, drawAreaRectRight, playerTwoGrid, playerOneShips);
                    PlayerDraw(spriteBatch, playerTwoShips, playerOneShips, font1, GamePad.GetState(PlayerIndex.One), viewportRect);
                }
            }
            else
            {
                if (playerOneActive)
                {
                    DrawGrid(spriteBatch, drawAreaRectRight, playerOneGrid, playerTwoShips);
                    SetUpDraw(spriteBatch, playerOneShips, GamePad.GetState(PlayerIndex.One), viewportRect, font1);
                }
                else
                {
                    DrawGrid(spriteBatch, drawAreaRectRight, playerTwoGrid, playerOneShips);
                    SetUpDraw(spriteBatch, playerTwoShips, GamePad.GetState(PlayerIndex.One), viewportRect, font1);
                }
            }

            if (endgame)
            {
                string winText;
                if (playerOneActive)
                {
                    winText = "Player one Victory!";
                }
                else
                {
                    winText = "Player two Victory!";
                }
                spriteBatch.Draw(gameOver, viewportRect, Color.DarkBlue);
                spriteBatch.DrawString(font1, winText, new Vector2(viewportRect.Width / 3.5f, viewportRect.Height / 2.5f), Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            }
        }

        void DrawGrid(SpriteBatch spriteBatch, Rectangle drawRect, bool[,] playerGrid, GameObject[] enemy)
        {
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Color activeColor = Color.White;
                    foreach (Point active in activeGrids)
                    {
                        if (active.X == gridRects[i, j].X && active.Y == gridRects[i, j].Y)
                        {
                            activeColor = Color.Red;
                        }
                    }
                    spriteBatch.Draw(grid, gridRects[i, j], activeColor);

                    Texture2D shotSprite = miss;
                    if (playerGrid[i, j])
                    {
                        foreach (GameObject ship in enemy)
                        {
                            if (ship.Rect.Contains(new Point(drawAreaRectRight.X + (int)(drawSquareRect.Width * i), drawAreaRectRight.Y + (int)(drawSquareRect.Height * j))))
                            {
                                if (!ship.Alive)
                                {
                                    spriteBatch.Draw(sunk, gridRects[i, j], Color.Yellow);
                                }
                                shotSprite = hit;
                                break;
                            }
                        }
                        spriteBatch.Draw(shotSprite, gridRects[i, j], Color.White);
                    }
                }
            }
        }

        void DrawShip(GameObject ship, SpriteBatch spriteBatch, Color drawColor)
        {
            DrawShip(ship, spriteBatch, drawColor, ship.Rect);
        }
        void DrawShip(GameObject ship, SpriteBatch spriteBatch, Color drawColor, Rectangle shipRect)
        {
            Rectangle sourceRect = new Rectangle(0, 0, ship.Sprite.Width / ship.Health.Length, ship.Sprite.Height);
            Rectangle drawRect = new Rectangle(shipRect.X, shipRect.Y, drawSquareRect.Width, drawSquareRect.Height);
            Color actualDrawColor;
            float rotation = 0f;

            if (ship.Rect == shipRect)
            {
                rotation = ship.Rotation;
            }

            for (int i = 0; i < ship.Health.Length; i++)
            {
                if (ship.Health[i] == 0)
                {
                    actualDrawColor = Color.Red;
                }
                else
                {
                    actualDrawColor = drawColor;
                }
                sourceRect.X = (ship.Sprite.Width / ship.Health.Length) * i;
                if (rotation == 0)
                {
                    drawRect.X = shipRect.X + (drawSquareRect.Width * i);
                }
                else
                {
                    drawRect.X = shipRect.X + drawSquareRect.Width;
                    drawRect.Y = shipRect.Y + (drawSquareRect.Width * i);
                }
                spriteBatch.Draw(ship.Sprite, drawRect, sourceRect, actualDrawColor, rotation, Vector2.Zero, SpriteEffects.None, 0f);
            }
        }

        void PlayerDraw(SpriteBatch spriteBatch, GameObject[] player, GameObject[] enemy, SpriteFont font1, GamePadState playerPad, Rectangle viewportRect)
        {
            if (playerPad.Triggers.Right > 0)
            {
                spriteBatch.Draw(keyMap, viewportRect, Color.White);
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }

            //Draw sunken enemy ships
            if (!isPaused)
            {
                foreach (GameObject ship in enemy)
                {
                    if (!ship.Alive)
                    {
                        DrawShip(ship, spriteBatch, Color.White);
                    }
                }

                Rectangle shipDrawRect = new Rectangle(drawAreaRectLeft.X, drawAreaRectLeft.Y, 0, drawSquareRect.Height);
                for (int i = 0; i < player.Length; i++)
                {
                    if (player[i].Rect.Height > player[i].Rect.Width)
                    {
                        shipDrawRect.Width = player[i].Rect.Height;
                    }
                    else
                    {
                        shipDrawRect.Width = player[i].Rect.Width;
                    }

                    shipDrawRect.Y = (shipDrawRect.Height * i) + drawAreaRectLeft.Y;
                    DrawShip(player[i], spriteBatch, Color.White, shipDrawRect);
                }

                Rectangle powerupRect = new Rectangle(shipDrawRect.X, shipDrawRect.Y + shipDrawRect.Height, drawAreaRectLeft.Width, 0);
                powerupRect.Height = drawAreaRectLeft.Height - powerupRect.Y + drawAreaRectLeft.Y - 4;

                string turnString;
                string powerString;
                string bombString;
                if (playerOneActive)
                {
                    turnString = "Player One's Turn";
                    powerString = "Power " + PlayerOnePower.ToString();
                    bombString = playerOneBombs.ToString() + " Bombs left";
                }
                else
                {
                    turnString = "Player Two's Turn";
                    powerString = "Power " + PlayerTwoPower.ToString();
                    bombString = playerTwoBombs.ToString() + " Bombs left";
                }

                shipDrawRect.Y = drawAreaRectLeft.Y;
                shipDrawRect.X = player[0].Rect.Width + drawAreaRectLeft.X;
                spriteBatch.DrawString(font1, turnString, new Vector2(shipDrawRect.X, shipDrawRect.Y), Color.White);
                shipDrawRect.Y += shipDrawRect.Height;
                spriteBatch.DrawString(font1, powerString, new Vector2(shipDrawRect.X, shipDrawRect.Y), Color.White);
                shipDrawRect.Y += shipDrawRect.Height;
                spriteBatch.DrawString(font1, bombString, new Vector2(shipDrawRect.X, shipDrawRect.Y), Color.White);

                spriteBatch.Draw(powerups[powerupNumber], powerupRect, Color.White);

                if (playerPad.Triggers.Left > 0)
                {
                    foreach (GameObject ship in player)
                    {
                        DrawShip(ship, spriteBatch, Color.Green);
                        ship.Draw(spriteBatch, font1, drawSquareRect.Width, drawSquareRect.Height);
                    }
                }
            }
        }

        void SetUpDraw(SpriteBatch spriteBatch, GameObject[] player, GamePadState playerPad, Rectangle viewportRect, SpriteFont font1)
        {
            if (playerPad.Triggers.Right > 0)
            {
                spriteBatch.Draw(keyMap, viewportRect, Color.White);
                isPaused = true;
            }
            else
            {
                isPaused = false;
            }

            if (!isPaused)
            {
                foreach (GameObject ship in player)
                {
                    if (ship.Alive)
                    {
                        DrawShip(ship, spriteBatch, Color.DarkBlue);
                    }
                }

                string turnString;
                if (playerOneActive)
                {
                    turnString = "Player One's Turn";
                }
                else
                {
                    turnString = "Player Two's Turn";
                }
                spriteBatch.DrawString(font1, turnString, new Vector2(drawAreaRectLeft.X, drawAreaRectLeft.Y), Color.White);
            }
            //spriteBatch.Draw(pointer.Sprite, pointer.Position, Color.White);
        }

        void PlayerTurn(Vector2 velocity, GamePadState playerPad, GamePadState previousPlayerPad, GameObject[] enemy, GameObject[] player)
        {
            activeGrids = new List<Point>();

            if (!turnOver)
            {
                pointer.Update(velocity, drawAreaRectRight);
                Point pointerPosition = new Point((int)pointer.Position.X, (int)pointer.Position.Y);
                foreach (Rectangle rect in gridRects)
                {
                    if (rect.Contains(pointerPosition))
                    {
                        activeGrids.Add(new Point(rect.X, rect.Y));
                        break;
                    }
                }

                if (playerPad.Buttons.B == ButtonState.Pressed && !fired)
                {
                    //Changes firing solution to bomb
                    activeGrids.Add(new Point(activeGrids[0].X + drawSquareRect.Width, activeGrids[0].Y));
                    activeGrids.Add(new Point(activeGrids[0].X - drawSquareRect.Width, activeGrids[0].Y));
                    activeGrids.Add(new Point(activeGrids[0].X, activeGrids[0].Y + drawSquareRect.Height));
                    activeGrids.Add(new Point(activeGrids[0].X, activeGrids[0].Y - drawSquareRect.Height));
                }
                if (playerOneActive && playerOnePower == 5 || !playerOneActive && playerTwoPower == 5)
                {
                    //Changes firing solution to super bomb
                    if (powerupNumber == 4 && playerPad.Buttons.X == ButtonState.Pressed && !fired && !powerupUsed)
                    {
                        activeGrids.Add(new Point(activeGrids[0].X + drawSquareRect.Width, activeGrids[0].Y));
                        activeGrids.Add(new Point(activeGrids[0].X - drawSquareRect.Width, activeGrids[0].Y));
                        activeGrids.Add(new Point(activeGrids[0].X, activeGrids[0].Y + drawSquareRect.Height));
                        activeGrids.Add(new Point(activeGrids[0].X, activeGrids[0].Y - drawSquareRect.Height));

                        activeGrids.Add(new Point(activeGrids[0].X + drawSquareRect.Width, activeGrids[0].Y - drawSquareRect.Height));
                        activeGrids.Add(new Point(activeGrids[0].X + drawSquareRect.Width, activeGrids[0].Y + drawSquareRect.Height));
                        activeGrids.Add(new Point(activeGrids[0].X - drawSquareRect.Width, activeGrids[0].Y - drawSquareRect.Height));
                        activeGrids.Add(new Point(activeGrids[0].X - drawSquareRect.Width, activeGrids[0].Y + drawSquareRect.Height));

                        activeGrids.Add(new Point(activeGrids[0].X + drawSquareRect.Width * 2, activeGrids[0].Y));
                        activeGrids.Add(new Point(activeGrids[0].X - drawSquareRect.Width * 2, activeGrids[0].Y));
                        activeGrids.Add(new Point(activeGrids[0].X, activeGrids[0].Y + drawSquareRect.Height * 2));
                        activeGrids.Add(new Point(activeGrids[0].X, activeGrids[0].Y - drawSquareRect.Height * 2));
                    }
                }

                if (playerPad.Buttons.A == ButtonState.Pressed && previousPlayerPad.Buttons.A == ButtonState.Released && !fired)
                {
                    //Fires one bomb
                    if (playerPad.Buttons.B == ButtonState.Pressed)
                    {
                        if (playerOneActive && playerOneBombs > 0)
                        {
                            FireBomb(enemy);
                            playerOneBombs--;
                        }
                        else if (playerTwoBombs > 0)
                        {
                            FireBomb(enemy);
                            playerTwoBombs--;
                        }
                    }
                    else
                    {
                        FireShot(enemy);
                    }
                    fired = true;
                }
                else if (playerPad.Buttons.X == ButtonState.Released && previousPlayerPad.Buttons.X == ButtonState.Pressed && !powerupUsed)
                {
                    //Use powerup
                    powerupUsed = true;
                    PowerupSelector(player);
                }

                if (powerupUsed && fired)
                {
                    turnOver = true;
                }
            }

            if (playerPad.Buttons.Y == ButtonState.Pressed && previousPlayerPad.Buttons.Y == ButtonState.Released)
            {
                if (playerOneActive)
                {
                    playerOneActive = false;
                    PlayerTwoPower++;
                }
                else
                {
                    playerOneActive = true;
                    PlayerOnePower++;
                }

                if (powerupNumber == 4 && !fired)
                {
                    if (playerOneActive && playerOnePower == 5 || !playerOneActive && playerTwoPower == 5)
                    {
                        FireBomb(enemy);
                        fired = true;
                    }
                }

                powerupNumber = powerupGen.Next(0, 6);
                turnOver = false;
                fired = false;
                powerupUsed = false;
                TestEndGame(enemy);
                pointer.Position = new Vector2(drawAreaRectRight.X, drawAreaRectRight.Y);
                foreach (GameObject ship in enemy)
                {
                    if (ship.Invulnerable)
                    {
                        ship.Invulnerable = false;
                    }
                }
            }
        }

        void FireShot(GameObject[] enemy)
        {
            foreach (GameObject ship in enemy)
            {
                ship.CheckForDamage(activeGrids[0], 2, drawSquareRect.Width, drawSquareRect.Height);
            }
            AddShots();
        }

        void FireBomb(GameObject[] enemy)
        {
            //Will fire a bomb in the pattern of a cross
            //This includes superbomb
            foreach (GameObject ship in enemy)
            {
                for (int i = 0; i < activeGrids.Count(); i++)
                {
                    int damage = 0;
                    if (activeGrids.Count() < 6)
                    {
                        if (i == 0)
                        {
                            damage = 2;
                        }
                        else
                        {
                            damage = 1;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            damage = 3;
                        }
                        else if (i < 9)
                        {
                            damage = 2;
                        }
                        else
                        {
                            damage = 1;
                        }
                    }

                    ship.CheckForDamage(activeGrids[i], damage, drawSquareRect.Width, drawSquareRect.Height);
                }
            }
            AddShots();
        }

        void AddShots()
        {
            if (playerOneActive)
            {
                foreach (Point shot in activeGrids)
                {
                    int width = (shot.X - drawAreaRectRight.X) / drawSquareRect.Width;
                    int height = (shot.Y - drawAreaRectRight.Y) / drawSquareRect.Height;
                    if (width < 0 || width > Width - 1)
                    {
                        break;
                    }
                    else if (height < 0 || height > Height - 1)
                    {
                        break;
                    }
                    else
                    {
                        playerOneGrid[width, height] = true;
                    }
                }
            }
            else
            {
                foreach (Point shot in activeGrids)
                {
                    int width = (shot.X - drawAreaRectRight.X) / drawSquareRect.Width;
                    int height = (shot.Y - drawAreaRectRight.Y) / drawSquareRect.Height;
                    if (width < 0 || width > Width)
                    {
                        break;
                    }
                    else if (height < 0 || height > Height)
                    {
                        break;
                    }
                    else
                    {
                        playerTwoGrid[width, height] = true;
                    }
                }
            }
        }

        void PowerupSelector(GameObject[] player)
        {
            switch (powerupNumber)
            {
                case 0:
                    {
                        Repair(player);
                        break;
                    }
                case 1:
                    {
                        Reinforce(player);
                        break;
                    }
                case 2:
                    {
                        Rejuvenate(player);
                        break;
                    }
                case 3:
                    {
                        ReStock();
                        break;
                    }
                case 4:
                    {
                        Nuke();
                        break;
                    }
                case 5:
                    {
                        Stealth(player);
                        break;
                    }
            }
        }

        //Powerup
        void Repair(GameObject[] player)
        {
            Point pointerPosition = new Point((int)pointer.Position.X, (int)pointer.Position.Y);
            foreach (GameObject ship in player)
            {
                if (ship.Rect.Contains(pointerPosition))
                {
                    ship.Repair(pointerPosition, -10, drawSquareRect.Width, drawSquareRect.Height);
                    break;
                }
            }
            if (playerOneActive)
            {
                PlayerOnePower -= 2;
            }
            else
            {
                PlayerTwoPower -= 2;
            }
        }

        //Powerup
        void Reinforce(GameObject[] player)
        {
            Point pointerPosition = new Point((int)pointer.Position.X, (int)pointer.Position.Y);
            foreach (GameObject ship in player)
            {
                if (ship.Rect.Contains(pointerPosition))
                {
                    ship.Reinforce(pointerPosition, -1, drawSquareRect.Width, drawSquareRect.Height);
                    break;
                }
            }
            if (playerOneActive)
            {
                PlayerOnePower -= 2;
            }
            else
            {
                PlayerTwoPower -= 2;
            }
        }

        //Powerup
        void Rejuvenate(GameObject[] player)
        {
            Point pointerPosition = new Point((int)pointer.Position.X, (int)pointer.Position.Y);
            foreach (GameObject ship in player)
            {
                if (ship.Rect.Contains(pointerPosition) && !ship.Alive)
                {
                    for (int i = 0; i < ship.Health.Length; i++)
                    {
                        ship.Health[i] = 1;
                        ship.Alive = true;
                    }
                    break;
                }
            }
            if (playerOneActive)
            {
                PlayerOnePower -= 5;
            }
            else
            {
                PlayerTwoPower -= 5;
            }
        }

        //Powerup
        void ReStock()
        {
            if (playerOneActive)
            {
                if (playerOnePower >= 3)
                {
                    playerOneBombs = 5;
                    playerOnePower -= 3;
                }
            }
            else
            {
                if (playerTwoPower >= 3)
                {
                    playerTwoBombs = 5;
                    playerTwoPower -= 3;
                }
            }
        }

        //Powerup
        void Nuke()
        {
            if (playerOneActive)
            {
                PlayerOnePower -= 5;
            }
            else
            {
                PlayerTwoPower -= 5;
            }
        }

        //Powerup
        void Stealth(GameObject[] player)
        {
            foreach (GameObject ship in player)
            {
                if (ship.Rect.Contains(new Point((int)pointer.Position.X, (int)pointer.Position.Y)))
                {
                    ship.Invulnerable = true;
                    break;
                }
            }
            if (playerOneActive)
            {
                PlayerOnePower -= 1;
            }
            else
            {
                PlayerTwoPower -= 1;
            }
        }

        void TestEndGame(GameObject[] enemy)
        {
            endgame = true;
            foreach (GameObject ship in enemy)
            {
                if (ship.Alive)
                {
                    endgame = false;
                    break;
                }
            }
        }
    }
}
