﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer_v1
{
    class Player : I_WorldObject
    {
        Texture2D playerTexture;
        Texture2D playerTextureLeft;
        Texture2D playerTextureRight;
        Vector2 playerTextureOrigin;
        Vector2 playerPosition;
        float playerRotation;
        Vector2 playerVelocity;
        String playerName;
        Color playerColor;
        BoundingBox playerBoundingBox;
        bool collidable;
        QuadTreeNode mNode;

        public bool leftRight;

        int PLAYER_SPEED;
        Vector2 GRAVITY;

        enum State
        {
            GROUNDED,
            HIGH
        }

        KeyboardState kbCurrentState;
        Vector2 currentPosition;

        GamePadState gpCurrentState;

        bool rigidness;
        bool aliveness;

        bool gravityState;
        List<I_WorldObject> worldObjects;

        State playerState;

        float frame;
        int frameWidth;
        Vector2 animCenter;
        float scale;

        public Player(String playerName, Vector2 iniPos, List<I_WorldObject> objectsList)
        {
            leftRight = true;
            this.playerName = playerName;
            this.playerPosition = iniPos;
            this.playerTextureOrigin = Vector2.Zero;
            this.playerVelocity = Vector2.Zero;
            this.playerRotation = 0;
            this.playerColor = Color.White;
            rigidness = false;
            aliveness = true;
            currentPosition = Vector2.Zero;
            PLAYER_SPEED = WorldData.GetInstance().playerSpeed;
            GRAVITY = WorldData.GetInstance().Gravity;
            this.worldObjects = objectsList;
            this.gravityState = true;
            this.collidable = true;
            this.playerState = State.HIGH;
       

            frame = 0;
            frameWidth = 0;
            animCenter = Vector2.Zero;
            scale = 0;


        }

        public void LoadContent(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>("spriteArt/" + playerName);
            playerTextureLeft = content.Load<Texture2D>("spriteArt/" + playerName + "L");
            playerTextureRight = content.Load<Texture2D>("spriteArt/" + playerName + "R");

            UpdateBoundingBox();
        }

        public void Update(GameTime gameTime)
        {
            // player controls go in here

            this.playerColor = Color.White;

            kbCurrentState = Keyboard.GetState();
            gpCurrentState = GamePad.GetState(PlayerIndex.One);

            gravityState = true;
            checkForStandingOnSomething();

            UpdateMovement(kbCurrentState, gpCurrentState);

            adjustPositionByVelocity();

            UpdateBoundingBox();

        }

        private void UpdateMovement(KeyboardState kbCurrentState, GamePadState gpCurrentState)
        {

            // prevent player from going out of left side of screen
            if (this.playerBoundingBox.Min.X < 0)
            {
                this.playerPosition = new Vector2(0, playerPosition.Y);
            }

            if (kbCurrentState.IsKeyDown(Keys.Up) || gpCurrentState.IsButtonDown(Buttons.A))
            {
                if (!gravityState || playerState == State.GROUNDED)
                {
                    this.playerState = State.HIGH;
                    this.playerVelocity = new Vector2(0, -4);
                    gravityState = true;
                }

            }

            if (kbCurrentState.IsKeyDown(Keys.Left) || gpCurrentState.IsButtonDown(Buttons.LeftThumbstickLeft))
            {
                leftRight = false;
                playerTexture = playerTextureLeft;
                this.playerPosition += new Vector2(-2, 0);
            }

            if (kbCurrentState.IsKeyDown(Keys.Right) || gpCurrentState.IsButtonDown(Buttons.LeftThumbstickRight))
            {
                leftRight = true;
                playerTexture = playerTextureRight;
                this.playerPosition += new Vector2(2, 0);
            }
        }

        private void adjustPositionByVelocity()
        {
            if (this.gravityState)
            {
                this.playerVelocity += this.GRAVITY;
            }


            this.playerPosition += this.playerVelocity;
        }

        public void alertCollision(I_WorldObject collidedObject)
        {


            if (collidedObject.getName() == "nun")
            {
                this.setVelocity(Vector2.Zero);
                this.setDirection(new Vector2(1, 0));
                this.setSpeed(Vector2.Zero);
                this.setPhysics(false);

                playerPosition = WorldData.GetInstance().playerInitialPosition;
            }

            if (!collidedObject.isRigid())
            {
                if (collidedObject.getName() == "Spikes" || collidedObject.getName() == "SpikeField")
                {
                    setAlive(false);

                    this.setVelocity(Vector2.Zero);
                    this.setDirection(new Vector2(1, 0));
                    this.setSpeed(Vector2.Zero);
                    this.setPhysics(false);

                    playerPosition = WorldData.GetInstance().playerInitialPosition;

                    setAlive(true);
                }


                if (collidedObject.getName() == "scroll")
                {
                    collidedObject.setAlive(false);
                    WorldData.level++;
                }

            }

            if (collidedObject.isRigid())
            {
                //this.playerColor = Color.Red;

                BoundingBox myAABB = this.playerBoundingBox;
                BoundingBox other = collidedObject.getBoundingBox();

                int leftBound = (int)Math.Max(myAABB.Min.X, other.Min.X);
                int rightBound = (int)Math.Min(myAABB.Max.X, other.Max.X);
                int upperBound = (int)Math.Max(myAABB.Min.Y, other.Min.Y);
                int lowerBound = (int)Math.Min(myAABB.Max.Y, other.Max.Y);

                int xMovement = rightBound - leftBound;
                int yMovement = lowerBound - upperBound;

                if (xMovement < yMovement)
                {
                    if (myAABB.Min.X <= other.Min.X)
                    {
                        this.playerPosition.X -= (xMovement + 1);
                    }
                    else
                    {
                        this.playerPosition.X += xMovement + 1;
                    }
                }
                else
                {
                    if (myAABB.Min.Y < other.Min.Y)
                    {
                        this.playerState = State.GROUNDED;

                        this.playerPosition.Y -= (yMovement);
                        this.setVelocity(new Vector2(0, 0));
                    }
                    else
                    {
                        //this.playerState = State.HIGH;
                        this.playerPosition.Y += (yMovement);
                        this.setVelocity(new Vector2(0, 0));
                        //this.gravityState = true;
                    }
                }
            }
        }

        private void checkForStandingOnSomething()
        {
            foreach (I_WorldObject x in worldObjects)
            {
                if (x.isRigid())
                {
                    // first check that player intersects object via x projection
                    if ((this.playerBoundingBox.Min.X >= x.getBoundingBox().Min.X && this.playerBoundingBox.Min.X <= x.getBoundingBox().Max.X)
                         || this.playerBoundingBox.Max.X >= x.getBoundingBox().Min.X && this.playerBoundingBox.Max.X <= x.getBoundingBox().Max.X)
                    {
                        // now we check if were above the object
                        if (x.getBoundingBox().Min.Y - this.playerBoundingBox.Max.Y >= -2 && x.getBoundingBox().Min.Y - this.playerBoundingBox.Max.Y <= 2)
                        {
                            if (x.getName() == "movingTile")
                            {
                                this.playerVelocity = x.getVelocity();
                            }

                            gravityState = false;
                        }
                    }
                }
            }
        }


        public Texture2D getTexture()
        {
            return playerTexture;
        }

        public Vector2 getTextureOrigin()
        {
            return playerTextureOrigin;
        }

        public Vector2 getPosition()
        {
            return playerPosition;
        }

        public void setPosition(Vector2 newPosition)
        {
            playerPosition += newPosition;
        }

        public float getRotation()
        {
            return playerRotation;
        }

        public Vector2 getVelocity()
        {
            return playerVelocity;
        }

        public void setVelocity(Vector2 newVelocity)
        {
            playerVelocity = newVelocity;
        }

        public Vector2 getDirection()
        {
            return new Vector2(0, 0);
        }

        public void setDirection(Vector2 newDirection)
        {

        }

        public Vector2 getSpeed()
        {
            return new Vector2(0, 0);
        }

        public void setSpeed(Vector2 newSpeed)
        {

        }

        public BoundingBox getBoundingBox()
        {
            return playerBoundingBox;
        }

        public bool hasPhysics()
        {
            return true;
        }

        public void setPhysics(bool p)
        {

        }

        public bool isAlive()
        {
            return aliveness;
        }

        public void setAlive(bool a)
        {
            aliveness = a;
        }

        public String getName()
        {
            return this.playerName;
        }

        public Color getColor()
        {
            return this.playerColor;
        }

        public bool isRigid()
        {
            return rigidness;
        }

        public void setRigid(bool r)
        {
            rigidness = r;
        }

        public bool isCollidable()
        {
            return collidable;
        }

        public void setCollidability(bool c)
        {
            collidable = c;
        }

        public QuadTreeNode getNode()
        {

            return mNode;
        }

        public void setNode(QuadTreeNode n)
        {
            mNode = n;
        }

        public float getFrame()
        {
            return frame;
        }

        public int getFrameWidth()
        {
            return frameWidth;
        }

        public Vector2 getAnimCenter()
        {
            return animCenter;
        }

        public float getScale()
        {
            return scale;
        }

        protected void UpdateBoundingBox()
        {
            this.playerBoundingBox.Min.X = this.getPosition().X;
            this.playerBoundingBox.Min.Y = this.getPosition().Y;
            this.playerBoundingBox.Max.X = this.getPosition().X + this.getTexture().Width;
            this.playerBoundingBox.Max.Y = this.getPosition().Y + this.getTexture().Height;

        }
    }
}
