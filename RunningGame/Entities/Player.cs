﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;

namespace RunningGame.Entities
{
    /*
     * This is the player - yo.
     * It's a lot like other entities, the only special things
     * are the faceLeft, and faceRight methods.
     * These change which way the player sprite is looking.
     */
    class Player : Entity
    {

        float defaultWidth = 20;
        float defaultHeight = 20;

        float startingX;
        float startingY;

        Bitmap leftImage;
        Bitmap rightImage;

        public Player(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            startingX = x;
            startingY = y;

            addMyComponents(x, y);

        }

        public Player(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            startingX = x;
            startingY = y;

            addMyComponents(x, y);
        }



        public void addMyComponents(float x, float y)
        {

            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));

            //Velocity Component
            addComponent(new VelocityComponent(0, 0));

            //Draw component
            //Right image
            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream("RunningGame.Resources.Player.bmp");
            rightImage = new Bitmap(myStream);
            //Left Image
            leftImage = new Bitmap(myStream);
            leftImage.RotateFlip(RotateFlipType.RotateNoneFlipX);

            myStream.Close();

            addComponent(new DrawComponent(leftImage, (int)defaultWidth, (int)defaultHeight, false));

            //Player Component
            addComponent(new PlayerComponent());

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.PLAYER_COLLIDER_TYPE));


            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));

        }

        public override Entity CopyStartingState()
        {
            Player newEnt = new Player(level, randId, startingX, startingY);
            return newEnt;
        }

        /*
        public override void resetInitialState()
        {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().changePosition(posComp, startingX, startingY);
            level.getMovementSystem().changeSize(posComp, defaultWidth, defaultHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;
        }
        */

        public void faceRight()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.sprite = rightImage;
        }
        public void faceLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.sprite = leftImage;
        }

        public bool isLookingLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.sprite == leftImage);
        }
    }
}
