﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities {
    class spawnBlockEntity : Entity {
        float defaultWidth = 20;
        float defaultHeight = 20;

        public string blockNormName = "normal";
        public string blockAnimationName = "blockAnimation";

        public spawnBlockEntity( Level level, float x, float y ) {
            this.level = level;

            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            addMyComponents( x, y );
        }

        public void addMyComponents( float x, float y ) {

            this.updateOutOfView = true;

            //Position Component
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            //Draw component
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );
            drawComp.addSprite( "Artwork.Foreground.BlockSquare", "RunningGame.Resources.Artwork.Foreground.BlockSquare.png", blockNormName );
            drawComp.setSprite( blockNormName );


            List<string> animImgDefaults = new List<string>()
            {
                "RunningGame.Resources.Artwork.Foreground.BlockSquare.png",
                //"RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof1.png",
                "RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof2.png",
                //"RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof3.png",
                "RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof4.png",
                //"RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof5.png",
                "RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof6.png",
                "RunningGame.Resources.Artwork.Foreground.SpawnBlock.SpawnPoof7.png"
            };
            List<string> animImg = new List<string>()
            {
                "Artwork.Foreground.BlockSquare",
                //"Artwork.Foreground.SpawnBlock.SpawnPoof1",
                "Artwork.Foreground.SpawnBlock.SpawnPoof2",
                //"Artwork.Foreground.SpawnBlock.SpawnPoof3",
                "Artwork.Foreground.SpawnBlock.SpawnPoof4",
                //"Artwork.Foreground.SpawnBlock.SpawnPoof5",
                "Artwork.Foreground.SpawnBlock.SpawnPoof6",
                "Artwork.Foreground.SpawnBlock.SpawnPoof7"
            };

            drawComp.addAnimatedSprite( animImg, animImgDefaults, blockAnimationName );

            AnimationComponent animComp = (AnimationComponent)addComponent( new AnimationComponent( 0.0001f ), true );
            animComp.animationOn = false;
            animComp.destroyAfterCycle = true;

            //Velocity Component
            addComponent( new VelocityComponent( 0, 0 ), true );

            //Collider
            addComponent( new ColliderComponent( this, GlobalVars.SPAWN_BLOCK_COLLIDER_TYPE), true );

            //Gravity Component
            addComponent( new GravityComponent( 0, GlobalVars.STANDARD_GRAVITY ), true );

            //Spawn Block Component
            addComponent( new SpawnBlockComponent(), true );

            //Off side of screen
            addComponent( new ScreenEdgeComponent( 3, 3, 3, 3 ), true );

            /*
            //Pushable
            PushableComponent pushComp = (PushableComponent)addComponent( new PushableComponent(), true );
            pushComp.horiz = true;
            pushComp.vert = false;
            */
            
        }

        public override void revertToStartingState() {
            PositionComponent posComp = ( PositionComponent )this.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().changePosition( posComp, posComp.startingX, posComp.startingY, true, true );
            level.getMovementSystem().changeSize( posComp, posComp.startingWidth, posComp.startingHeight );

            VelocityComponent velComp = ( VelocityComponent )this.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            velComp.x = 0;
            velComp.y = 0;
        }
    }
}
