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
    class GameObject
    {
        string name;
        public string Name
        {
            get { return name; }
        }

        //Position & movement variables
        protected Rectangle rect;
        public Rectangle Rect
        {
            get { return rect; }
            set { rect = value; }
        }

        //Health variables
        protected int[] health;
        public int[] Health
        {
            get { return health; }
        }
        public void SetHealth(int number, int damage)
        {
            health[number] -= damage;
            if (health[number] < 0)
            {
                health[number] = 0;
            }
            else if (health[number] > maxHealth[number])
            {
                health[number] = maxHealth[number];
            }
            bool kill = true;
            for (int i = 0; i < health.Length; i++)
            {
                if (health[i] > 0)
                {
                    kill = false;
                }
            }
            if (kill)
            {
                Kill();
            }
        }
        protected int[] maxHealth;
        public int[] MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }

        //Drawing variables
        protected Texture2D sprite;
        public Texture2D Sprite
        {
            get { return sprite; }
            set { sprite = value; }
        }
        protected bool alive;
        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        protected Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Centre
        {
            get { return new Vector2(sprite.Width / 2f, sprite.Height / 2f); }
        }
        protected float rotation;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        bool invulnerable;
        public bool Invulnerable
        {
            get { return invulnerable; }
            set { invulnerable = value; }
        }

        //Methods for instantiation
        public GameObject()
        {
            position = Vector2.Zero;
            rect = Rectangle.Empty;
        }
        public GameObject(Texture2D newSprite, int nHealth, string nName, int width, int height, Rectangle nRect)
        {
            name = nName;
            sprite = newSprite;

            //Automatically set values
            alive = false;
            position = Vector2.Zero;

            health = new int[width];
            maxHealth = new int[width];
            for (int i = 0; i < width; i++)
            {
                health[i] = nHealth;
                maxHealth[i] = nHealth;
            }

            rect = nRect;

            rotation = 0f;

            invulnerable = false;
        }

        protected virtual void Kill()
        {
            Alive = false;
        }

        public void Update(Vector2 velocity, Rectangle viewportRect)
        {
            position += velocity;
            CheckBoundaryCollision(velocity, viewportRect);
        }

        void CheckBoundaryCollision(Vector2 velocity, Rectangle viewportRect)
        {
            int MaxX = viewportRect.Width + viewportRect.X - (int)(Centre.X * 3f);
            int MinX = viewportRect.X + (int)Centre.X;
            int MaxY = viewportRect.Height + viewportRect.Y - (int)(Centre.Y * 3f);
            int MinY = viewportRect.Y + (int)Centre.Y;

            if (position.X > MaxX)
            {
                velocity.X *= -1;
                position.X = MaxX;
            }
            else if (position.X < MinX)
            {
                velocity.X *= -1;
                position.X = MinX;
            }

            if (position.Y > MaxY)
            {
                velocity.Y *= -1;
                position.Y = MaxY;
            }
            else if (position.Y < MinY)
            {
                velocity.Y *= -1;
                position.Y = MinY;
            }
        }

        public void CheckForDamage(Point shot, int damage, int squareWidth, int squareHeight)
        {
            if (!invulnerable)
            {
                shot.X += squareWidth / 2;
                shot.Y += squareHeight / 2;

                if (Rotation == 0)
                {
                    for (int i = 0; i < health.Length; i++)
                    {
                        if (new Rectangle(Rect.X + (squareWidth * i), Rect.Y, squareWidth, squareHeight).Contains(shot))
                        {
                            SetHealth(i, damage);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < health.Length; i++)
                    {
                        if (new Rectangle(Rect.X, Rect.Y + (squareHeight * i), squareWidth, squareHeight).Contains(shot))
                        {
                            SetHealth(i, damage);
                        }
                    }
                }
            }
        }

        public void Reinforce(Point shot, int damage, int squareWidth, int squareHeight)
        {
            shot.X += squareWidth / 2;
            shot.Y += squareHeight / 2;

            if (Rotation == 0)
            {
                for (int i = 0; i < health.Length; i++)
                {
                    if (new Rectangle(Rect.X + (squareWidth * i), Rect.Y, squareWidth, squareHeight).Contains(shot))
                    {
                        if (Health[i] > 0 && Alive)
                        {
                            MaxHealth[i]++;
                            Health[i]++;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < health.Length; i++)
                {
                    if (new Rectangle(Rect.X, Rect.Y + (squareHeight * i), squareWidth, squareHeight).Contains(shot))
                    {
                        if (Health[i] > 0 && Alive)
                        {
                            MaxHealth[i]++;
                            Health[i]++;
                        }
                    }
                }
            }

        }

        public void Repair(Point shot, int damage, int squareWidth, int squareHeight)
        {
            shot.X += squareWidth / 2;
            shot.Y += squareHeight / 2;

            if (Rotation == 0)
            {
                for (int i = 0; i < health.Length; i++)
                {
                    if (new Rectangle(Rect.X + (squareWidth * i), Rect.Y, squareWidth, squareHeight).Contains(shot))
                    {
                        if (Health[i] > 0 && Alive)
                        {
                            Health[i] = MaxHealth[i];
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < health.Length; i++)
                {
                    if (new Rectangle(Rect.X, Rect.Y + (squareHeight * i), squareWidth, squareHeight).Contains(shot))
                    {
                        if (Health[i] > 0 && Alive)
                        {
                            Health[i] = MaxHealth[i];
                        }
                    }
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font1, int squareWidth, int squareHeight)
        {
            if (Rotation == 0)
            {
                for (int i = 0; i < health.Length; i++)
                {
                    spriteBatch.DrawString(font1, health[i].ToString(), new Vector2(Rect.X + (squareWidth * i), Rect.Y), Color.White);
                }
            }
            else
            {
                for (int i = 0; i < health.Length; i++)
                {
                    spriteBatch.DrawString(font1, health[i].ToString(), new Vector2(Rect.X, Rect.Y + (squareHeight * i)), Color.White);
                }
            }
        }
    }
}

