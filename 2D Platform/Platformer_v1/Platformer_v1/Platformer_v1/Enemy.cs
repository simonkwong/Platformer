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
    class Enemy : I_WorldObject
    {

        Texture2D enemyTexture;
        Texture2D enemyTextureLeft;
        Texture2D enemyTextureRight;
        Vector2 enemyTextureOrigin;
        Vector2 enemyPosition;
        float enemyRotation;
        Vector2 enemyVelocity;
        BoundingBox enemyBoundingBox;
        Color enemyColor;
        bool physics;
        Vector2 enemyDirection;
        Vector2 enemySpeed;
        String enemyName;
        bool rigidness;
        bool aliveness;
        bool collidable;
        static Random rndGen = new Random();
        QuadTreeNode mNode;

        float frame;
        int frameWidth;
        Vector2 animCenter;
        float scale;

        public static int nunCount = 0;

        Vector2 acceleration;

        bool inRangeSwitch;


        float thrusterX;
        float thrusterY;



        World gameWorld;

        public Enemy(String enemyName, Vector2 iniPos, World mWorld)
        {

            inRangeSwitch = false;
            this.gameWorld = mWorld;
            this.enemyName = enemyName;
            this.enemyPosition = iniPos;
            this.enemyTextureOrigin = Vector2.Zero;
            this.enemyVelocity = Vector2.Zero;
            this.enemyRotation = 0;
            this.enemyColor = Color.White;
            this.physics = false;
            this.enemyDirection = Vector2.Zero;
            this.enemySpeed = Vector2.Zero;
            rigidness = true;
            aliveness = true;
            collidable = true;

            frame = 0;
            frameWidth = 0;
            animCenter = Vector2.Zero;
            scale = 0;
        }

        public void LoadContent(ContentManager content)
        {
            enemyTexture = content.Load<Texture2D>("spriteArt/" + enemyName);
            enemyTextureLeft = content.Load<Texture2D>("spriteArt/" + enemyName + "L");
            enemyTextureRight = content.Load<Texture2D>("spriteArt/" + enemyName + "R");

            UpdateBoundingBox();
        }

        public void Update(GameTime gameTime)
        {
            // calibrate nuns thruster
            calibrateThruster();

            acceleration += new Vector2(thrusterX, thrusterY) * 0.05f;

            // cap acceleration
            if (acceleration.X <= -1.5f)
            {
                acceleration.X = -1.5f;
            }
            else if (acceleration.X >= 1.5f)
            {
                acceleration.X = 1.5f;
            }



                if (acceleration.Y <= -1.5f)
                {
                    acceleration.Y = -1.5f;
                }
                else if (acceleration.Y >= 1.5f)
                {
                    acceleration.Y = 1.5f;
                }




            enemyPosition += acceleration;


            UpdateBoundingBox();
        }





        private void calibrateThruster()
        {

            Vector2 playerPos = gameWorld.p.getPosition();

            if (Vector2.Distance(playerPos, this.getPosition()) <= 400 || inRangeSwitch)
            {
                inRangeSwitch = true;

                // calibrate x thrusters

                if (enemyPosition.X > gameWorld.p.getPosition().X)
                {
                    thrusterX = -1;
                    enemyTexture = enemyTextureLeft;
                }
                else
                {
                    thrusterX = 1;
                    enemyTexture = enemyTextureRight;
                }


                // calibrate y thrusters 
                if (enemyPosition.Y > gameWorld.p.getPosition().Y)
                {
                    thrusterY = -1;
                }
                else
                {
                    thrusterY = 1;
                }
            }
            else
            {

            }

        }



        private void RandomizeMovement()
        {
            int emm = WorldData.GetInstance().enemyMaxMovement;

            float iX = rndGen.Next(-emm, emm);
            float iY = rndGen.Next(-emm, emm);

            int direction = rndGen.Next(1, 8);
            int vmagnitude = rndGen.Next(1, 3);
            int hmagnitude = rndGen.Next(1, 3);


            if (direction == 1)
                setDirection(new Vector2(1 * hmagnitude, 0));
            if (direction == 2)
                setDirection(new Vector2(-1 * hmagnitude, 0));
            if (direction == 3)
                setDirection(new Vector2(0, 1 * vmagnitude));
            if (direction == 4)
                setDirection(new Vector2(0, -1 * vmagnitude));
            if (direction == 5)
                setDirection(new Vector2(1 * hmagnitude, 1 * vmagnitude));
            if (direction == 6)
                setDirection(new Vector2(-1 * hmagnitude, 1 * vmagnitude));
            if (direction == 7)
                setDirection(new Vector2(1 * hmagnitude, -1 * vmagnitude));
            if (direction == 8)
                setDirection(new Vector2(-1 * hmagnitude, -1 * vmagnitude));




            enemySpeed = new Vector2(iX * WorldData.GetInstance().enemySpeed, iY * WorldData.GetInstance().enemySpeed);
        }

        private void adjustPosition()
        {
            if (this.enemyBoundingBox.Min.X < 0)
            {
                this.enemyPosition = new Vector2(0, enemyPosition.Y);
                this.enemyDirection *= -1;
            }

            if (this.enemyBoundingBox.Min.Y < 0)
            {
                this.enemyPosition = new Vector2(enemyPosition.X, 0);
                this.enemyDirection *= -1;
            }


            if (this.enemyBoundingBox.Max.Y > WorldData.GetInstance().ScreenHeight)
            {
                this.enemyPosition = new Vector2(enemyPosition.X, WorldData.GetInstance().ScreenHeight - enemyTexture.Height);
                this.enemyDirection *= -1;
            }



            UpdateBoundingBox();
        }

        public Texture2D getTexture()
        {
            return enemyTexture;
        }


        public Vector2 getTextureOrigin()
        {
            return enemyTextureOrigin;
        }

        public Vector2 getPosition()
        {
            return enemyPosition;
        }

        public void setPosition(Vector2 newPosition)
        {
            enemyPosition += newPosition;
        }

        public float getRotation()
        {
            return enemyRotation;
        }

        public Vector2 getVelocity()
        {
            return enemyVelocity;
        }

        public void setVelocity(Vector2 newVelocity)
        {
            enemyVelocity = newVelocity;
        }

        public Vector2 getDirection()
        {
            return enemyDirection;
        }

        public void setDirection(Vector2 newDirection)
        {
            enemyDirection = newDirection;
        }

        public Vector2 getSpeed()
        {
            return enemySpeed;
        }

        public void setSpeed(Vector2 newSpeed)
        {
            enemySpeed = newSpeed;
        }

        public BoundingBox getBoundingBox()
        {
            return enemyBoundingBox;
        }


        public bool hasPhysics()
        {
            return physics;
        }

        public void setPhysics(bool p)
        {
            physics = p;
        }

        public void alertCollision(I_WorldObject collidedObject)
        {

            if (collidedObject.getName() == "fire particle")
            {
                nunCount++;
                gameWorld.nunCount.text = "enemies eliminated: " + nunCount;
                this.setAlive(false);
            }

            if (collidedObject.isRigid())
            {

                this.enemyVelocity *= -1;


            }

            //this.blockColor = Color.Red;
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
            return enemyName;
        }

        public Color getColor()
        {
            return this.enemyColor;
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
            this.enemyBoundingBox.Min.X = this.getPosition().X;
            this.enemyBoundingBox.Min.Y = this.getPosition().Y;
            this.enemyBoundingBox.Max.X = this.getPosition().X + this.getTexture().Width;
            this.enemyBoundingBox.Max.Y = this.getPosition().Y + this.getTexture().Height;


        }
    }
}
