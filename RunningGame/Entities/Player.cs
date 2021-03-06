﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Components;
using System.Drawing;
using System.Collections;

namespace RunningGame.Entities {
    /*
     * This is the player - yo.
     * It's a lot like other entities, the only special things
     * are the faceLeft, and faceRight methods.
     * These change which way the player sprite is looking.
     */
    [Serializable()]
    public class Player : Entity {

        float defaultWidth = 40;
        float defaultHeight = 50;

        string rightImageName = "right";
        string leftImageName = "left";

        string walkLeft = "normLeft";
        string walkRight = "normRight";

        string walkBlueLeft = "bL";
        string walkBlueRight = "bR";

        string walkOrangeLeft = "oL";
        string walkOrangeRight = "oR";

        string walkGreenLeft = "pL";
        string walkGreenRight = "pE";

        string activeLeftImage;
        string activeRightImage;

        bool forceLargeStopImage = false;
        bool forceSmallStopImage = false;

        public Player() { }

        public Player( Level level, float x, float y ) {
            this.level = level;
            this.depth = 1;
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            activeLeftImage = walkLeft;
            activeRightImage = walkRight;

            addMyComponents( x, y );

        }

        public Player( Level level, int id, float x, float y ) {
            this.level = level;

            initializeEntity( id, level );

            addMyComponents( x, y );
        }



        public void addMyComponents( float x, float y ) {

            this.resetOnCheckpoint = true;

            //Position Component
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );

            //Velocity Component
            addComponent( new VelocityComponent( 0, 0 ), true );

            //Draw component
            DrawComponent drawComp = new DrawComponent( ( int )defaultWidth, ( int )defaultHeight, level, false );

            drawComp.addSprite( "Artwork.Creatures.player1", "RunningGame.Resources.Artwork.Creatures.player1.png", rightImageName );
            drawComp.addSprite( "Artwork.Creatures.player1", "RunningGame.Resources.Artwork.Creatures.player2.png", leftImageName );
            drawComp.rotateFlipSprite( leftImageName, RotateFlipType.RotateNoneFlipX );
            addComponent( drawComp );

            addWalkAnimation( "RunningGame.Resources.Artwork.Creatures.player", walkLeft, walkRight, drawComp );
            addWalkAnimation( "RunningGame.Resources.Artwork.Creatures.PlayerBlue", walkBlueLeft, walkBlueRight, drawComp );
            addWalkAnimation( "RunningGame.Resources.Artwork.Creatures.PlayerOrange", walkOrangeLeft, walkOrangeRight, drawComp );
            addWalkAnimation( "RunningGame.Resources.Artwork.Creatures.PlayerGreen", walkGreenLeft, walkGreenRight, drawComp );

            drawComp.setSprite( rightImageName, true );

            setNormalImage();


            //Animation Component
            AnimationComponent animComp = ( AnimationComponent )addComponent( new AnimationComponent( GlobalVars.playerAnimatonSpeed ), true );
            animComp.animationOn = false;

            //Player Component
            addComponent( new PlayerComponent(), true );

            //Player Input Component
            addComponent( new PlayerInputComponent( this ), true );

            //Collider
            addComponent( new ColliderComponent( this, GlobalVars.PLAYER_COLLIDER_TYPE , defaultWidth-4, defaultHeight-4), true);

            /*
            //Squish Component
            SquishComponent sqComp = ( SquishComponent )addComponent( new SquishComponent( defaultWidth, defaultHeight, defaultWidth * 1.2f, defaultHeight * 1.2f, defaultWidth / 2f, defaultHeight / 2f, defaultWidth * defaultHeight * 1.1f, defaultWidth * defaultHeight / 1.5f ), true );
            sqComp.maxHeight = defaultHeight;
            sqComp.maxWidth = defaultWidth * 1.1f;
            sqComp.minHeight = defaultHeight / 1.1f;
            sqComp.minWidth = defaultWidth / 1.1f;
            sqComp.maxSurfaceArea = defaultHeight * defaultWidth * 1.1f;
            sqComp.minSurfaceArea = defaultHeight * defaultWidth / 1.1f;
            */


            //Gravity Component
            addComponent( new GravityComponent( 0, GlobalVars.STANDARD_GRAVITY ), true );

            //Health Component
            addComponent( new HealthComponent( 100, true, 1, 0.5f, level ), true );

            //Screen Edge Stop/Wrap/End Level
            addComponent( new ScreenEdgeComponent( 1, 4, 1, 5 ), true );

        }


        public void addWalkAnimation( string baseName, string spriteNameLeft, string spriteNameRight, DrawComponent drawComp ) {
            string imgOne = baseName + "1.png";
            string imgTwo = baseName + "2.png";
            string imgThree = baseName + "3.png";
            List<string> walkAnimation = new List<string>
            {
                imgTwo,
                imgOne,
                imgTwo,
                imgThree
            };
            List<string> walkDefaults = new List<string>()
            {
                imgTwo,
                imgOne,
                imgTwo,
                imgThree
            };

            drawComp.addAnimatedSprite( walkAnimation, walkDefaults, spriteNameRight );
            drawComp.addAnimatedSprite( walkAnimation, walkDefaults, spriteNameLeft );

            drawComp.rotateFlipSprite( spriteNameLeft, RotateFlipType.RotateNoneFlipX );

        }

        public override void revertToStartingState() {
            
            PositionComponent posComp = ( PositionComponent )this.getComponent( GlobalVars.POSITION_COMPONENT_NAME );
            level.getMovementSystem().teleportToNoCollisionCheck( posComp, posComp.startingX, posComp.startingY );
            level.getMovementSystem().changeSize( posComp, posComp.startingWidth, posComp.startingHeight );

            if ( hasComponent( GlobalVars.PLAYER_INPUT_COMPONENT_NAME ) ) {
                PlayerInputComponent plInComp = ( PlayerInputComponent )this.getComponent( GlobalVars.PLAYER_INPUT_COMPONENT_NAME );
                plInComp.passedAirjumps = 0;
            }

            VelocityComponent velComp = ( VelocityComponent )this.getComponent( GlobalVars.VELOCITY_COMPONENT_NAME );
            velComp.x = 0;
            velComp.y = 0;

            AnimationComponent animComp = ( AnimationComponent )this.getComponent( GlobalVars.ANIMATION_COMPONENT_NAME );
            animComp.animationOn = false;
            setNormalImage();
            level.sysManager.spSystem.toNoPowerups();

            HealthComponent healthComp = ( HealthComponent )this.getComponent( GlobalVars.HEALTH_COMPONENT_NAME );
            healthComp.restoreHealth();

            removeComponent( GlobalVars.PLAYER_INPUT_COMPONENT_NAME );
            addComponent( new PlayerInputComponent( this ) );

            if ( !hasComponent( GlobalVars.GRAVITY_COMPONENT_NAME ) ) {
                addComponent( new GravityComponent( 0, GlobalVars.STANDARD_GRAVITY ) );
            }
        }


        public void faceRight() {

            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            drawComp.setSprite( activeRightImage );
        }
        public void faceLeft() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            drawComp.setSprite( activeLeftImage );
        }

        public bool isLookingLeft() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            return ( drawComp.activeSprite == activeLeftImage || drawComp.activeSprite == (activeLeftImage + "" + GlobalVars.PRECOLOR_SPRITE_NAME) );
        }

        public bool isLookingRight() {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
            return ( drawComp.activeSprite == activeRightImage || drawComp.activeSprite == (activeRightImage + "" + GlobalVars.PRECOLOR_SPRITE_NAME));
        }



        public void setNormalImage() {
            bool lookLeft = isLookingLeft();

            activeLeftImage = walkLeft;
            activeRightImage = walkRight;

            refreshImage( lookLeft );
        }
        public void setBlueImage() {
            bool lookLeft = isLookingLeft();

            activeLeftImage = walkBlueLeft;
            activeRightImage = walkBlueRight;

            refreshImage( lookLeft );
        }
        public void setOrangeImage() {
            bool lookLeft = isLookingLeft();

            activeLeftImage = walkOrangeLeft;
            activeRightImage = walkOrangeRight;

            refreshImage( lookLeft );
        }
        public void setGreenImage() {
            bool lookLeft = isLookingLeft();

            activeLeftImage = walkGreenLeft;
            activeRightImage = walkGreenRight;

            refreshImage( lookLeft );
        }

        public void refreshImage( bool left ) {
            DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );

            int spriteNum = drawComp.getSprite().currentImageIndex;

            if ( left ) drawComp.setSprite( activeLeftImage );
            else drawComp.setSprite( activeRightImage );

            drawComp.getSprite().currentImageIndex = spriteNum;
        }


        public void stopAnimation() {
            AnimationComponent animComp = ( AnimationComponent )this.getComponent( GlobalVars.ANIMATION_COMPONENT_NAME );
            if ( animComp.animationOn ) {
                animComp.animationOn = false;
                DrawComponent drawComp = ( DrawComponent )this.getComponent( GlobalVars.DRAW_COMPONENT_NAME );
                if ( forceLargeStopImage && ( drawComp.getSprite().currentImageIndex == 0 || drawComp.getSprite().currentImageIndex == 2 ) ) {
                    drawComp.getSprite().currentImageIndex++;
                } else if ( forceSmallStopImage ) {
                    drawComp.getSprite().currentImageIndex = 0;
                }
            }
        }
        public void startAnimation() {
            AnimationComponent animComp = ( AnimationComponent )this.getComponent( GlobalVars.ANIMATION_COMPONENT_NAME );
            if ( !animComp.animationOn ) {
                animComp.animationOn = true;
            }
        }
    }
}
