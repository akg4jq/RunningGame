﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;

namespace RunningGame.Entities {
    public class TimedShooterEntity : Entity{

        float defaultWidth = 20.0f;
        float defaultHeight = 20.0f;

        //-------------------------------------------Constructors--------------------------------------------
        public TimedShooterEntity( Level level, float x, float y, float timeBetweenBursts, int shotsPerBurst, int dir ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, timeBetweenBursts, shotsPerBurst, dir );
        }
        public TimedShooterEntity( Level level, int id, float x, float y, float timeBetweenBursts, int shotsPerBurst, int dir ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y, timeBetweenBursts, shotsPerBurst, dir );
        }

        //------------------------------------------------------------------------------------------------------------------

        //Here's where you add all the components the entity has.
        //You can just uncomment the ones you want.
        public void addMyComponents( float x, float y, float timeBetweenBursts, int shotsPerBurst, int dir ) {
            /*POSITION COMPONENT - Does it have a position?
             */
            addComponent(new PositionComponent(x, y, defaultWidth, defaultHeight, this), true);

            /*VELOCITY - Just cuz'
             */
            addComponent( new VelocityComponent(), true );

            /*DRAW COMPONENT - Does it get drawn to the game world?
             */
            DrawComponent drawComp = (DrawComponent)addComponent(new DrawComponent(defaultWidth, defaultHeight, level, true), true);
            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite("Artwork.Other.WhiteSquare", "RunningGame.Resources.Artwork.Other.WhiteSquare.png", "Main");
            drawComp.setSprite("Main"); //Set image to active image

            /* ANIMATION COMPONENT - Does it need animating?
             */
            //addComponent(new AnimationComponent(0.0005f), true);

            /*COLLIDER - Does it hit things?
             */
            addComponent(new ColliderComponent(this, GlobalVars.TIMED_SHOOTER_COLLIDER_TYPE), true);
            
            /* TIMER COMPONENT - It makes use of timed method execution.
             */
            TimerComponent timeComp = (TimerComponent)addComponent( new TimerComponent(), true );

            /* TIMED SHOOTER COMPONENT - It shoots at a given time interval.
             */
            TimedShooterComponent shooterComp = (TimedShooterComponent)addComponent( new TimedShooterComponent( timeBetweenBursts, shotsPerBurst ), true );
            timeComp.addTimer( shooterComp.fireTimerString, timeBetweenBursts );

            /* DIRECTION COMPONENT - It points in a particular direciton.
             */
            addComponent( new DirectionalComponent( dir ) );

        }

        //You must have this, but it may be empty.
        //What should the entity do in order to revert to its starting state?
        //Common things are:
        //Set position back to startingX and startingY
        //NOTE: If doing this, you probably want to use the MovementSystem's teleportToNoCollisionCheck() method
        //rather than the usual changePosition()
        //Set velocity to 0 in both directions
        //Note: Some things, like ground, dont move, and really don't need anything here.
        //Note: Some things, like a bullet, won't ever exist at the start of a level, so you could probably leave this empty.
        public override void revertToStartingState() {
            TimedShooterComponent shooterComp = ( TimedShooterComponent )this.getComponent( GlobalVars.TIMED_SHOOTER_COMPONENT );
            TimerComponent timerComp = ( TimerComponent )this.getComponent( GlobalVars.TIMER_COMPONENT_NAME );
            timerComp.clearAllTimers();
            timerComp.addTimer( shooterComp.fireTimerString, shooterComp.timeBetweenBursts );
        }

    }
}
