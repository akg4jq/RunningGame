﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;

namespace RunningGame.Components
{
    class SoundComponent : Component //Always extend Component
    {
        public string soundLocation { get; set; }
        public bool playSound { get; set; }
        public SoundPlayer player;

        public SoundComponent(String soundLocation, bool playSound)
        {
            this.componentName = GlobalVars.SOUND_COMPONENT_NAME;
            this.soundLocation = soundLocation;
            this.playSound = playSound;
            player = new SoundPlayer(soundLocation);

        }

        public void setSound(String soundLocation)
        {
            player.SoundLocation = soundLocation;
            player.Load();
        }
        
    }
}