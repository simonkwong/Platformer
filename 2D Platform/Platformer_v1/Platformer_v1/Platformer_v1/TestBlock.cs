﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer_v1
{
    class TestBlock : I_WorldObject
    {
        Texture2D blockTexture;
        Vector2 blockTextureOrigin;
        Vector2 blockPosition;
        float blockRotation;
        Vector2 blockVelocity;
        BoundingBox blockBoundingBox;
        Color blockColor;
        bool physics;
        Vector2 blockDirection;
        Vector2 blockSpeed;
        String platformName;
        bool collidable;
        bool rigidness;
        bool aliveness;
        QuadTreeNode mNode;

        private SoundEffect suspenceMusic;
        private SoundEffectInstance suspenseMusicInstance;


        float frame;
        int frameWidth;
        Vector2 animCenter;
        float scale;

        Vector2 currentPosition;
        ScaryImage scaryImage;

        World gameWorld;

        enum Orientation
        {
            Left, Right
        }

        Orientation state = Orientation.Right;

        public TestBlock(String platformName, Vector2 iniPos, ScaryImage scaryImg, World gWorld)
        {
            this.gameWorld = gWorld;
            scaryImage = scaryImg;
            currentPosition = iniPos;
            this.platformName = platformName;
            this.blockPosition = iniPos;
            this.blockTextureOrigin = Vector2.Zero;
            this.blockVelocity = Vector2.Zero;
            this.blockRotation = 0;
            this.blockColor = Color.White;
            this.physics = false;
            this.blockDirection = Vector2.Zero;
            this.blockSpeed = Vector2.Zero;
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

            if (platformName == "ScareBlock")
            {
                blockTexture = content.Load<Texture2D>("spriteArt/" + "Demon");
                suspenceMusic = content.Load<SoundEffect>("sounds/suspence");
                suspenseMusicInstance = suspenceMusic.CreateInstance();
                suspenseMusicInstance.IsLooped = true;
                suspenseMusicInstance.Volume = 0;
                suspenseMusicInstance.Play();
            }
            else
            {
                blockTexture = content.Load<Texture2D>("spriteArt/" + platformName);

            }

            UpdateBoundingBox();
        }

        public bool isCollidable()
        {
            return collidable;
        }

        public void setCollidability(bool c)
        {
            collidable = c;
        }


        public void Update(GameTime gameTime)
        {


            this.blockColor = Color.White;
            UpdateBoundingBox();

            if (this.getName() == "ScareBlock")
            {
                Vector2 distanceV = this.blockPosition - this.gameWorld.p.getPosition();
     
                    float targetVolume = 20000 / (float) Math.Pow(distanceV.Length(), 2);
                    if (targetVolume >= 1)
                    {
                        // floor max value
                        suspenseMusicInstance.Volume = 1;
                    }
                    else
                    {
                        suspenseMusicInstance.Volume = targetVolume;
                    }
        
            }

            if (this.getName() == "movingTile")
            {

                if (blockPosition.X - currentPosition.X > 105)
                {
                    blockVelocity = new Vector2(-1, 0);
                    blockPosition += blockVelocity;
                    state = Orientation.Left;

                }
                if (state == Orientation.Right)
                {
                    blockVelocity = new Vector2(1, 0);
                    blockPosition += blockVelocity;
                }
                if (state == Orientation.Left)
                {
                    blockVelocity = new Vector2(-1, 0);
                    blockPosition += blockVelocity;
                }
                if (currentPosition.X - blockPosition.X > 105)
                {
                    blockVelocity = new Vector2(1, 0);
                    blockPosition += blockVelocity;
                    state = Orientation.Right;
                }
            }
        }

        public Texture2D getTexture()
        {
            return blockTexture;
        }


        public Vector2 getTextureOrigin()
        {
            return blockTextureOrigin;
        }

        public Vector2 getPosition()
        {
            return blockPosition;
        }

        public void setPosition(Vector2 newPosition)
        {
            blockPosition += newPosition;
        }

        public float getRotation()
        {
            return blockRotation;
        }

        public Vector2 getVelocity()
        {
            return blockVelocity;
        }

        public void setVelocity(Vector2 newVelocity)
        {
            blockVelocity = newVelocity;
        }

        public Vector2 getDirection()
        {
            return blockDirection;
        }

        public void setDirection(Vector2 newDirection)
        {
            blockDirection = newDirection;
        }

        public Vector2 getSpeed()
        {
            return blockSpeed;
        }

        public void setSpeed(Vector2 newSpeed)
        {
            blockSpeed = newSpeed;
        }

        public BoundingBox getBoundingBox()
        {
            return blockBoundingBox;
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

            if (platformName == "ScareBlock")
            {
                suspenseMusicInstance.Stop();
                scaryImage.scare(0, 1);
                this.setAlive(false);
            }
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
            return platformName;
        }

        public Color getColor()
        {
            return this.blockColor;
        }

        public bool isRigid()
        {
            return rigidness;
        }

        public void setRigid(bool r)
        {
            rigidness = r;
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
            this.blockBoundingBox.Min.X = this.getPosition().X;
            this.blockBoundingBox.Min.Y = this.getPosition().Y;
            this.blockBoundingBox.Max.X = this.getPosition().X + this.getTexture().Width;
            this.blockBoundingBox.Max.Y = this.getPosition().Y + this.getTexture().Height;

        
        }
    }
}
