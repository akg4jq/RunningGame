﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities
{

    /*
     * Meet the ever so handy test entity!
     * It can be whatever you want it to be.
     */
    [Serializable()]
    public class TestEntity : Entity
    {

        float defaultWidth = 10;
        float defaultHeight = 10;

        string testAnimationName = "testAnimation";

        public TestEntity(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);
            
            addMyComponents(x, y);
        }

        public TestEntity(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y);
        }

        public void addMyComponents(float x, float y)
        {
            //Position Component
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            //Draw component
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, true));
            drawComp.addSprite("RunningGame.Resources.WhiteSquare.png", "Main");

            ArrayList testAnimationList = new ArrayList
            {
                "RunningGame.Resources.WhiteSquare.png", "RunningGame.Resources.DirtSquare.png",
                "RunningGame.Resources.GrassSquare.png", "RunningGame.Resources.Player.png"
            };

            drawComp.addAnimatedSprite(testAnimationList, testAnimationName);
            //drawComp.activeSprite = testAnimationName;
            drawComp.setSprite("Main");

            //AnimationComponent animComp = (AnimationComponent)addComponent(new AnimationComponent(0.5f));

            //Velocity Component
            addComponent(new VelocityComponent(0, 0));

            //Player Component
            //addComponent(new PlayerInputComponent());

            //Collider
            addComponent(new ColliderComponent(this, GlobalVars.BASIC_SOLID_COLLIDER_TYPE));

            //Gravity Component
            addComponent(new GravityComponent(0, GlobalVars.STANDARD_GRAVITY));
            
           
            //Squish Component
            //addComponent(new SquishComponent(defaultWidth, defaultHeight, defaultWidth * 3.0f, defaultHeight * 3.0f, defaultWidth / 3.0f, defaultHeight / 3.0f));
        }
        
        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)this.getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().changePosition(posComp, posComp.startingX, posComp.startingY, true);
            level.getMovementSystem().changeSize(posComp, posComp.startingWidth, posComp.startingHeight);

            VelocityComponent velComp = (VelocityComponent)this.getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;
        }
         
    }
}
