﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunningGame.Components
{
    class SwitchListenerComponent:Component
    {
        //Has the switch changed state?
        private bool changed; //Private to protect from accessing before switch has been set. Use getChanged instead.
        public SwitchComponent mySwitch { get; set; }
        public int switchId { get; set; }
        public string eventType { get; set; } //What does the switch do? Is it a door? Maybe a moving platform? See Global Vars.

        //Instantiate with a switch id
        public SwitchListenerComponent(int switchId, string eventType)
        {

            this.componentName = GlobalVars.SWITCH_LISTENER_COMPONENT_NAME;

            this.switchId = switchId;
            this.eventType = eventType;
            changed = false;
        }

        public bool getChanged()
        {
            //If not assigned to a switch - first get the switch
            if (mySwitch == null)
            {
                subscribe();
                changed = true;
            }
            if (mySwitch != null)
                return changed;
            else
            {
                Console.WriteLine("Trying to access changed with a null switch. ID: " + switchId);
                return false;
            }
        }

        public bool getSwitchActive()
        {
            //Get switch if need be
            if (mySwitch == null)
            {
                subscribe();
            }
            if (mySwitch != null)
                return mySwitch.active;
            else
            {
                Console.WriteLine("Trying to access active for a null switch. ID: " + switchId);
                return false;
            }
        }

        public void subscribe()
        {
            foreach (int id in GlobalVars.allEntities.Keys)
            {
                if (id == this.switchId)
                {
                    this.mySwitch = (SwitchComponent)GlobalVars.allEntities[id].getComponent(GlobalVars.SWITCH_COMPONENT_NAME);
                    this.mySwitch.listeners.Add(this);   
                }
            }
        }

        public void setChanged(bool changed)
        {
            this.changed = changed;
        }

    }
}
