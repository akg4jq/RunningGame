﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Collections;

namespace RunningGame.Entities
{
    [Serializable()]
    public class FlyingEnemyEntity:Entity
    {


        public float defaultWidth = 40;
        public float defaultHeight = 30;
        public string leftImageName = "EnemyWalkLeft";
        public string rightImageName = "EnemyRightLeft";

        //-------------------------------------------Constructors--------------------------------------------

        public FlyingEnemyEntity(Level level, float x, float y)
        {
            this.level = level;

            initializeEntity(new Random().Next(Int32.MinValue, Int32.MaxValue), level);

            addMyComponents(x, y);
        }
        public FlyingEnemyEntity(Level level, int id, float x, float y)
        {
            this.level = level;

            initializeEntity(id, level);

            addMyComponents(x, y);
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents(float x, float y)
        {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this));
            
            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent("RunningGame.Resources.Enemy1.png", "Main", defaultWidth, defaultHeight, true));

            ArrayList enemyAnimation = new ArrayList()
            {
                "RunningGame.Resources.FlyingEnemy1.png",
                "RunningGame.Resources.FlyingEnemy2.png",
                "RunningGame.Resources.FlyingEnemy1.png",
                "RunningGame.Resources.FlyingEnemy3.png",
            };


            drawComp.addAnimatedSprite(enemyAnimation, leftImageName);
            drawComp.setSprite(leftImageName);
            
            drawComp.addAnimatedSprite(enemyAnimation, rightImageName);
            drawComp.rotateFlipSprite(rightImageName, System.Drawing.RotateFlipType.RotateNoneFlipX);

            /* ANIMATION COMPONENT - Does it need animating?
             */
            addComponent(new AnimationComponent(0.05f));

            /*VELOCITY COMPONENT - Does it move?
             */
            addComponent(new VelocityComponent(0, 0));

            /*COLLIDER - Does it hit things?
             *The second field is the collider type. Look in GlobalVars for a string with the right name.
             */
            addComponent(new ColliderComponent(this, GlobalVars.SIMPLE_ENEMY_COLLIDER_TYPE));

            /*HEALTH COMPONENT - Does it have health, can it die?
             */
            addComponent(new HealthComponent(100, true, 0, 100.0f));

            /*SIMPLE ENEMY COMPONENT
             */
            SimpleEnemyComponent simpEnemyComp = (SimpleEnemyComponent)addComponent(new SimpleEnemyComponent(GlobalVars.SIMPLE_ENEMY_H_SPEED + new Random().Next(-50, 50), false));
            simpEnemyComp.hasLandedOnce = true;

            addComponent(new ScreenEdgeComponent(1, 1, 1, 1));

        }
        
        //Revert!
        public override void revertToStartingState()
        {
            PositionComponent posComp = (PositionComponent)getComponent(GlobalVars.POSITION_COMPONENT_NAME);
            level.getMovementSystem().teleportToNoCollisionCheck(posComp, posComp.startingX, posComp.startingY);

            VelocityComponent velComp = (VelocityComponent)getComponent(GlobalVars.VELOCITY_COMPONENT_NAME);
            velComp.x = 0;
            velComp.y = 0;

            SimpleEnemyComponent simpEnemyComp = (SimpleEnemyComponent)getComponent(GlobalVars.SIMPLE_ENEMY_COMPONENT_NAME);
            simpEnemyComp.hasLandedOnce = true;
            simpEnemyComp.hasRunOnce = false;

            HealthComponent healthComp = (HealthComponent)getComponent(GlobalVars.HEALTH_COMPONENT_NAME);
            healthComp.restoreHealth();
        }
        
        


        public void faceRight()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(rightImageName);
        }
        public void faceLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            drawComp.setSprite(leftImageName);
        }

        public bool isLookingLeft()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.activeSprite == leftImageName);
        }

        public bool isLookingRight()
        {
            DrawComponent drawComp = (DrawComponent)this.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            return (drawComp.activeSprite == rightImageName);
        }
    }
}