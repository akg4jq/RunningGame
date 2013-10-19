﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using RunningGame.Components;
using RunningGame.Entities;
using RunningGame.Systems;
using RunningGame.Properties;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

namespace RunningGame
{
    /*
     * A level is basically what it souds like.
     * It keeps track of all the entities within a stage
     * in the game, and updates them. It also holds
     * the SystemManager which is what controls the game systems.
     */
    [Serializable()]
    public class Level
    {

        Random rand; //for creating entitiy ids
        //Dictionary<int, Entity> entities; //all entities in the level
        public Graphics g { get; set; }
        public float cameraWidth { get; set; }
        public float cameraHeight { get; set; }
        public float levelWidth { get; set; }
        public float levelHeight {get;set;}
        public bool paused = false; //Is the game paused?

        public float fps;

        long prevTicks = DateTime.Now.Ticks;
        long currentTicks;
        long pastTicks;

        public SystemManager sysManager; //Controls all systems
        bool sysManagerInit = false;

        public bool levelFullyLoaded = false;

        public BackgroundEntity bkgEnt;

        public Level() {}

        public Level(float windowWidth, float windowHeight, string levelFile, bool isPaintFile, Graphics g)
        {
            if(isPaintFile)
                initializePaint(windowWidth, windowHeight, levelFile, g);
            else
                initializeNotPaint(windowWidth, windowHeight, levelFile, g);
        }

        public void initializeNotPaint(float windowWidth, float windowHeight, string levelFile, Graphics g)
        {
            BinaryFormatter bf = new BinaryFormatter();
            Stream f = Assembly.GetExecutingAssembly().GetManifestResourceStream(levelFile);

            List<Object> ents = (List<Object>)bf.Deserialize(f);

            cameraWidth = windowWidth;
            cameraHeight = windowHeight;
            this.levelWidth = (float)ents[0];
            this.levelHeight = (float)ents[1];

            if (!sysManagerInit) sysManager = new SystemManager(this);

            sysManagerInit = true;

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;


            for (int i = 2; i < ents.Count; i++)
            {
                Entity oldEnt = (Entity)ents[i];
                Entity newEnt = (Entity)Activator.CreateInstance(oldEnt.GetType(), this, 0, 0);

                newEnt.randId = oldEnt.randId;

                //Copy all the fields! Ugh
                CopyFields(oldEnt, newEnt);
                foreach (Component oldC in oldEnt.getComponents())
                {
                    if (newEnt.hasComponent(oldC.componentName))
                    {
                        Component newC = newEnt.getComponent(oldC.componentName);
                        CopyFields(oldC, newC);
                    }
                }

                newEnt.level = this;
                addEntity(newEnt.randId, newEnt);
            }



            /*
            for (int i = 2; i < ents.Count; i++)
            {
                Entity e = (Entity)ents[i];
                e.level = this;
                addEntity(e.randId, e);
            }
            */

            bkgEnt = getMyBackgroundEntity();
            addEntity(bkgEnt.randId, bkgEnt);

        }

        public void CopyFields(object oldObj, object newObj)
        {
            foreach (FieldInfo oldInfo in oldObj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (FieldInfo newInfo in newObj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (oldInfo.Name == newInfo.Name)
                    {
                        newInfo.SetValue(newObj, oldInfo.GetValue(oldObj));
                    }
                }
            }
        }

        public void initializePaint(float windowWidth, float windowHeight, string levelFile, Graphics g)
        {

            rand = new Random();
            this.g = g;


            System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.IO.Stream myStream = myAssembly.GetManifestResourceStream(levelFile);
            Bitmap lvlImg = new Bitmap(myStream);

            cameraWidth = windowWidth;
            cameraHeight = windowHeight;
            this.levelWidth = lvlImg.Width * GlobalVars.LEVEL_READER_TILE_WIDTH;
            this.levelHeight = lvlImg.Height * GlobalVars.LEVEL_READER_TILE_HEIGHT;

            //Entities
            //entities = new Dictionary<int, Entity>();

            if (!sysManagerInit) sysManager = new SystemManager(this);

            sysManagerInit = true;

            LevelImageReader lvlImgReader = new LevelImageReader(this, lvlImg);
            lvlImgReader.readImage(this);

            //levelBeginState = new Dictionary<int, Entity>(entities); //Copy the beginning game state

            prevTicks = DateTime.Now.Ticks;

            levelFullyLoaded = true;

            bkgEnt = getMyBackgroundEntity();
            addEntity(bkgEnt.randId, bkgEnt);

        }

        //Game logic
        public virtual void Update()
        {

            //Time in seconds between frames
            currentTicks = DateTime.Now.Ticks;
            pastTicks = currentTicks - prevTicks;
            prevTicks = currentTicks;

            float deltaTime = (float)(TimeSpan.FromTicks(pastTicks).TotalSeconds);
            fps = (1 / deltaTime);

            if (levelFullyLoaded && !paused)
            {
                sysManager.Update(deltaTime); //Update systems
            }

        }

        //When an entity is given a collider - notify collider system
        public virtual void colliderAdded(Entity e)
        {
            sysManager.colliderAdded(e);
        }


        //Reset the game to it's original startup state
        public virtual void resetLevel()
        {
            paused = true; // Pause the game briefly

            Entity[] ents = GlobalVars.allEntities.Values.ToArray();
            for (int i = 0; i < ents.Length; i++)
            {
                if (ents[i].isStartingEntity)
                    ents[i].revertToStartingState();
                else
                {
                    removeEntity(ents[i]);
                }
            }
            foreach (Entity e in GlobalVars.removedStartingEntities.Values)
            {
                e.revertToStartingState();
                addEntity(e.randId, e);
            }
            GlobalVars.removedStartingEntities.Clear();

            paused = false; //Restart the game  
            
        }

        public virtual void removeAllEntities()
        {
            while(GlobalVars.allEntities.Values.Count > 0)
            {
                Entity e = GlobalVars.allEntities.Values.ToArray()[0];
                //e.Destroy();
            }
            GlobalVars.allEntities.Clear();
        }

        //Input
        public virtual void KeyDown(KeyEventArgs e)
        {
            sysManager.KeyDown(e);
        }
        public virtual void KeyUp(KeyEventArgs e)
        {
            sysManager.KeyUp(e);
        }
        public virtual void KeyPressed(KeyPressEventArgs e)
        {
            sysManager.KeyPressed(e);
        }
        public virtual void MouseClick(MouseEventArgs e)
        {
            //getCollisionSystem().MouseClick(e.X, e.Y);
            sysManager.MouseClick(e);
        }
        public virtual void MouseMoved(MouseEventArgs e)
        {
            sysManager.MouseMoved(e);
        }



        //Draw everything!
        public virtual void Draw(Graphics g)
        {
            
            sysManager.Draw(g);
            g.DrawString(fps.ToString("F") + "", SystemFonts.DefaultFont, Brushes.Black, new RectangleF(10, 10, cameraWidth-20, cameraHeight-20));
        }

        //Add an entity to the list of entities
        public virtual void addEntity(Entity e)
        {
            addEntity(e.randId, e);
        }
        public virtual void addEntity(int id, Entity e)
        {
            if (!sysManagerInit)
            {
                sysManager = new SystemManager(this);
                sysManagerInit = true;
            }
            if (!GlobalVars.allEntities.ContainsKey(id))
            {
                GlobalVars.allEntities.Add(id, e);
                if (e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
                    colliderAdded(e);
            }
            else
            {
                Console.WriteLine("Trying to add duplicate entity : " + e);
            }
        }
        public virtual void removeEntity(Entity e)
        {
            if (e.isStartingEntity && !GlobalVars.removedStartingEntities.ContainsKey(e.randId))
                GlobalVars.removedStartingEntities.Add(e.randId, e);
            if(e.hasComponent(GlobalVars.COLLIDER_COMPONENT_NAME))
                getCollisionSystem().colliderRemoved(e);
            GlobalVars.allEntities.Remove(e.randId);
        }

        public BackgroundEntity getMyBackgroundEntity()
        {
            BackgroundEntity bkgEnt = new BackgroundEntity(this, levelWidth/2, levelHeight/2);
            DrawComponent drawComp = (DrawComponent)bkgEnt.getComponent(GlobalVars.DRAW_COMPONENT_NAME);
            PositionComponent posComp = (PositionComponent)bkgEnt.getComponent(GlobalVars.POSITION_COMPONENT_NAME);

            drawComp.addSprite("RunningGame.Resources.LevelBackgrounds.BackgroundWorld13.png", "MainBkg");
            drawComp.setSprite("MainBkg");

            float initWidth = drawComp.getImage().Width;
            float initHeight = drawComp.getImage().Height;

            float xDiff = Math.Abs(initWidth - levelWidth);
            float yDiff = Math.Abs(initHeight - levelHeight);

            //Make it fit horizontally
            if (xDiff < yDiff)
            {
                float ratio = initWidth / levelWidth;
                getMovementSystem().changeSize(posComp, levelWidth, initHeight * ratio);
                drawComp.resizeImages((int)levelWidth, (int)(initHeight * ratio));
                getMovementSystem().changePosition(posComp, levelWidth/2, levelHeight-initHeight*ratio/2, false);
            }
            //Make it fit vertically
            else
            {
                float ratio = initHeight / levelHeight;
                getMovementSystem().changeSize(posComp, initWidth*ratio , levelHeight);
                drawComp.resizeImages((int)(initWidth*ratio), (int)(levelHeight));
                getMovementSystem().changePosition(posComp, initWidth * ratio / 2, levelHeight / 2, false);
            }

            bkgEnt.isStartingEntity = true;

            return bkgEnt;

        }

        //Getters
        public virtual Dictionary<int, Entity> getEntities() {
            return GlobalVars.allEntities;
        }
        public virtual MovementSystem getMovementSystem()
        {
            return sysManager.moveSystem;
        }
        public virtual CollisionDetectionSystem getCollisionSystem()
        {
            return sysManager.colSystem;
        }
        public virtual InputSystem getInputSystem()
        {
            if (sysManager != null)
                return sysManager.inputSystem;
            else return null;
        }
        public virtual Entity getPlayer()
        {
            foreach (Entity e in GlobalVars.allEntities.Values)
            {
                if (e.hasComponent(GlobalVars.PLAYER_COMPONENT_NAME)) return e;
            }

            return null;
        }

    }
}
