﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunningGame.Systems;
using RunningGame.Components;

namespace RunningGame.Entities {

    [Serializable()]
    public class Speedy : Entity {
        float defaultWidth = 11;
        float defaultHeight = 12;

        public Speedy( Level level, float x, float y ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( new Random().Next( Int32.MinValue, Int32.MaxValue ), level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }

        public Speedy( Level level, int id, float x, float y ) {
            //Set level.
            //Leave for all entities
            this.level = level;

            //Refers back to a class in the super Entity.
            //Leave this for all entities.
            initializeEntity( id, level );

            //Add the components.
            //Leave this for all entities.
            addMyComponents( x, y );
        }

        public void addMyComponents( float x, float y ) {
            //position and velocity
            addComponent( new PositionComponent( x, y, defaultWidth, defaultHeight, this ), true );
            DrawComponent drawComp = ( DrawComponent )addComponent( new DrawComponent( defaultWidth, defaultHeight, level, true ), true );

            //Add image - Use base name for first parameter (everything in file path after Resources. and before the numbers and .png)
            //Then second parameter is full filepath to a default image
            drawComp.addSprite( "Artwork.Foreground.BlueSplat", "RunningGame.Resources.Artwork.Foreground.BlueSplat.png", "Main" );
            drawComp.setSprite( "Main" ); //Set image to active image
            //Console.WriteLine("Speedy: " +drawComp.getImage());
            addComponent( new ColliderComponent( this, GlobalVars.SPEEDY_POSTGROUND_COLLIDER_TYPE ), true );
            
        }

        public override void revertToStartingState() {
        }


    }
}
