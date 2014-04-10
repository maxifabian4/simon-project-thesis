using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2D.XNA;
using System.Collections.Generic;
using System.Collections;
using System.Xml;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using ProyectoSimon.Elements;

namespace ProyectoSimon
{
    class GamePlayScreenCircles : GameplayScreen
    {
        private Random random = new Random();
        // Kinect parameters.
        Skeleton skeleton;
        // Input parameters.
        //protected InputAction cameraKey;
        // Physics world parameters.
        private World physicsWorld;
        protected List<GameElement> physicsElements;
        // Logic game parameters.
        private bool simulate, camera, video;
        private int kickeds, elements, bwidth, bheight;
        private GameTime gameTime;
        private Color circleColor, circleEdgeColor, circleJointColor;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamePlayScreenCircles(int w, int h, IList<Level> l)
        {
            // Create the statistics.
            currentStatistics = new Statistics("circulos");
            //currentStatistics.addAttribute("hits", 0);

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //pauseAction = new InputAction(
            //    new Buttons[] { },
            //    new Keys[] { Keys.Escape },
            //    true);

            //cameraKey = new InputAction(
            //    new Buttons[] { },
            //    new Keys[] { Keys.RightShift,Keys.LeftShift },
            //    true);
            generateInput();                 
            simulate = true;
            camera = false;
            video = true;
            currentLevel = 0;
            levels = l;
            elements = Convert.ToInt32(levels[currentLevel].getAttribute("elements"));
            bwidth = w;
            bheight = h;
            circleColor = new Color(227, 117, 64);
            circleEdgeColor = new Color(193, 82, 28);
            circleJointColor = new Color(206, 103, 0);

            loadWorld();
        }

        private void loadWorld()
        {
            // Inicialize physic elements list.
            physicsElements = new List<GameElement>();
            // Create physic world with a specific gravity.
            physicsWorld = new World(new Vector2(0, Convert.ToInt32(levels[currentLevel].getAttribute("gravity"))), true);
            // Asign a custom ContactListener.
            physicsWorld.ContactListener = new CirclesContactListener();
            simulate = true;            
            // Load Kinect's elements.
            skeleton = new Skeleton(physicsWorld);
            // Load Box2d elements.
            loadPhysicElements();
            kickeds = 0;
            timeSpan = TimeSpan.FromMilliseconds(Convert.ToInt32(levels[currentLevel].getAttribute("time")));       
        }

        private void loadPhysicElements()
        {
            int x, y;

            // Number of circles depend of the levels.
            for (int i = 0; i < Convert.ToInt32(levels[currentLevel].getAttribute("elements")); i++)
            {
                x = random.Next(350, bwidth - 350);
                y = random.Next(20, bheight - 100);
                physicsElements.Add(new Circle(physicsWorld, new Vector2(x, y), 20,false));
            }

            // Add floor element.
            physicsElements.Add(new Ground(physicsWorld, new Vector2(0.0f, bheight), new Vector2(bwidth, bheight)));
            // Add wall right element.
            physicsElements.Add(new Ground(physicsWorld, new Vector2(0.0f, 0.0f), new Vector2(0.0f, bheight)));
            // Add wall left element.
            physicsElements.Add(new Ground(physicsWorld, new Vector2(bwidth, 0.0f), new Vector2(bwidth, bheight)));
            // Add roof element.
            physicsElements.Add(new Ground(physicsWorld, new Vector2(0.0f, 0.0f), new Vector2(bwidth, 0.0f)));
        }

        //public void playSong()
        //{
        //    game.Play();
        //}

        //public void stopSong()
        //{
        //    game.Stop();
        //}
        //private void hideSkeletonWorld()
        //{
        //    //for (int i = 0; i < JOINTS_COUNT; i++)
        //    //    bodyJoints[i].getBody().Position = new Vector2(0, 0);
        //}

        public override int getPlayerState()
        {
            // Playing state by default.
            int state = 0;
            kickeds = getCountKicked();

            if ((timeSpan.Minutes > TimeSpan.Zero.Minutes) && (timeSpan.Seconds > TimeSpan.Zero.Seconds) && kickeds != elements)
                state = 0;
            else
                if ((timeSpan.Minutes == TimeSpan.Zero.Minutes) && (timeSpan.Seconds == TimeSpan.Zero.Seconds) && kickeds != elements)
                {
                    state = -1;
                    //loose.Play();
                }
                else
                    if (kickeds == elements)
                    {
                        state = 1;
                        // win.Play();
                    }

            return state;
        }

        public override string[] setCurrentStatistics()
        {
            return new string[] { 
                "nivel|" + currentLevel, 
                "tiempo|" + timeSpan.Minutes + "." + timeSpan.Seconds,
                "elementos totales|" + elements,
                "elementos golpeados|" + kickeds,
                "elementos faltantes|" + (elements - kickeds)};
        }

        private int getCountKicked()
        {
            GameElement element;
            int count = 0;

            for (int i = 0; i < physicsElements.Count; i++)
            {
                element = physicsElements[i];
                if (element.ToString().Contains("Circle") && ((Circle)element).isKicked())
                    count++;
            }
            
            return count;
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void Activate(bool instancePreserved)
        {
            if (!instancePreserved)
            {
                // A real game would probably have more content than this sample, so
                // it would take longer to load. We simulate that by delaying for a
                // while, giving you a chance to admire the beautiful loading screen.
                Thread.Sleep(1000);

                // once the load has finished, we use ResetElapsedTime to tell the game's
                // timing mechanism that we have just finished a very long frame, and that
                // it should not try to catch up.
                ScreenManager.Game.ResetElapsedTime();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void Unload() { }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Console.WriteLine("UPDATE");
            base.Update(gameTime, otherScreenHasFocus, false);

            if (IsActive)
            {
                simulate = true;
                this.gameTime = gameTime;

                if (KinectSDK.Instance.isInRange())
                {
                    timeSpan -= gameTime.ElapsedGameTime;
                    verifyGameStatus();
                }
                else simulate = false;
                //verifyGameStatus();
            }
        }

        public override void restartStage()
        {
            //levels[currentLevel].setAttribute("beginTime", DateTime.Now);
            loadWorld();
            elements = Convert.ToInt32(levels[currentLevel].getAttribute("elements"));
        }

        public override void nextLevel()
        {
            //stopSong();
            //levels[currentLevel].setAttribute("beginTime", DateTime.Now);
            if (levels.Count > currentLevel + 1)
                currentLevel++;

            
            //loadWorld();
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            PlayerIndex player;

            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
            {
                PauseMenuScreen pauseMenuScreen = new PauseMenuScreen();
                pauseMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
                pauseMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
                ScreenManager.AddScreen(pauseMenuScreen, ControllingPlayer);
            }
            else
            {
                skeleton.update(KinectSDK.Instance.getJoints());
                if (seatedMode.Evaluate(input, ControllingPlayer, out player))
                    KinectSDK.Instance.setSeatedMode();
                if (defaultMode.Evaluate(input, ControllingPlayer, out player))
                    KinectSDK.Instance.setDefaultMode();
                if (!camera && cameraKey.Evaluate(input, ControllingPlayer, out player))
                    camera = true;
                else
                    if (camera && cameraKey.Evaluate(input, ControllingPlayer, out player))
                        camera = false;

                if (simulate)
                    physicsWorld.Step(1.0f/60.0f, 8, 3);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Apply effects to draw primitives.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            if (getPlayerState() == 0)
            {
                ScreenManager.BasicEffect.CurrentTechnique.Passes[0].Apply();
                // Draw each physics elements in the world.
                drawElementsInTheWorld(physicsElements);
                // Draw skeleton if it isn't hidden.
                if (KinectSDK.Instance.isInRange())
                    skeleton.display(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            }
            
            // Draw statistics panel.
            drawStatisticsPanel(spriteBatch, new String[] { "nivel " + currentLevel,
                timeSpan.Minutes.ToString() + "." + timeSpan.Seconds.ToString() + " segundos",
                kickeds + "/" + elements + " aciertos" });
            
            // Show camera if it is active.
            if (camera && video)
            {
                spriteBatch.Begin();
                KinectSDK.Instance.DrawVideoCam(spriteBatch, new Rectangle(bwidth - 323, 3, 320, 240));
                spriteBatch.End();
            }
        }
    }
}