﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using RunningGame.Components;
using RunningGame.Entities;

namespace RunningGame.Systems
{
    public class BackgroundPositionSystem:GameSystem
    {
        
        //All systems MUST have an ArrayList of requiredComponents (May need to add using System.Collections at start of file)
        //To access components you may need to also add "using RunningGame.Components"
        ArrayList requiredComponents = new ArrayList();
        //All systems MUST have a variable holding the level they're contained in
        Level level;


        //0 = no scroll, static background
        //1 = proportion method
        public int scrollType = 0;


        //Constructor - Always read in the level! You can read in other stuff too if need be.
        public BackgroundPositionSystem(Level level)
        {
            //Here is where you add the Required components
            requiredComponents.Add(GlobalVars.POSITION_COMPONENT_NAME); //Position
            requiredComponents.Add(GlobalVars.BACKGROUND_COMPONENT_NAME); //Background

            this.level = level; //Always have this
        }

        //-------------------------------------- Overrides -------------------------------------------
        // Must have this. Same for all Systems.
        public override ArrayList getRequiredComponents()
        {
            return requiredComponents;
        }
        
        //Must have this. Same for all Systems.
        public override Level GetActiveLevel()
        {
            return level;
        }

        public override void Update(float deltaTime)
        {


            //Proportion Method
            if (scrollType == 1)
            {
                foreach (Entity e in getApplicableEntities())
                {
                    PositionComponent posComp = (PositionComponent)e.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

                    float viewX = level.sysManager.drawSystem.mainView.x;
                    float viewY = level.sysManager.drawSystem.mainView.y;

                    float fractionInX = (viewX / level.levelWidth);
                    float fractionInY = (viewY / level.levelHeight);

                    float bkgX = (viewX - fractionInX * posComp.width);
                    float bkgY = (viewY - fractionInY * posComp.height);

                    bkgX += posComp.width / 2;
                    bkgY += posComp.height / 2;

                    if (posComp.x != bkgX || posComp.y != bkgY)
                    {
                        level.getMovementSystem().changePosition(posComp, bkgX, bkgY, false);
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------


    }
}
