﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Drawing;

namespace RunningGame {

    /*
     * This class is what is used to store images.
     * It can hold a single image, or a set of images that come together
     * To form an animation.
     */

    [Serializable()]
    public class Sprite {

        public List<Bitmap> images { get; set; }
        public string name { get; set; }
        public int currentImageIndex { get; set; }

        public Sprite( string name, List<Bitmap> images ) {
            this.name = name;
            this.images = images;
            currentImageIndex = 0;
        }
        public Sprite( string name, Bitmap image ) {
            this.name = name;
            images = new List<Bitmap>();
            images.Add( image );
            currentImageIndex = 0;
        }

        public Bitmap getCurrentImage() {
            if ( currentImageIndex < images.Count )
                return images[currentImageIndex];
            else {
                Console.WriteLine( "Trying to access non-existant image in sprite " + name );
                return null;
            }
        }
        public int getNumImages() {
            return ( images.Count );
        }
    }
}
