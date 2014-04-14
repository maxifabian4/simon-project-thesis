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
    class GamePlayScreenChooser : GameplayScreen
    {
        private Random random = new Random();
        // Phisycs world parameters.
        private World physicsWorld;
        protected List<GameElement> physicsElments;
        private string[] movements;
        private BoxGame leftBox, rightBox;
        // Kinect parameters.
        Skeleton skeleton;
        // Zones.
        Rectangle zoneL, zoneR, boxL, boxR, zoneFault;
        // Logic Game parameters
        private bool simulate, camera, video, hideBody;
        private int hits, faults, currentElement, elements;
        private GameTime gameTime;
        private Color circleColor, circleEdgeColor, circleJointColor;

        /// <summary>
        /// Class constructor where initializes all dinamic game structures .
        /// </summary>
        public GamePlayScreenChooser(int w, int h, IList<Level> l)
        {
            // Create the statistics.
            currentStatistics = new Statistics("clasificador");
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            generateInput();
            simulate = true;
            camera = false;
            video = true;
            hideBody = false;
            currentLevel = 0;
            levels = l;
            elements = Convert.ToInt16(levels[currentLevel].getAttribute("elements"));
            width = w;
            height = h;
            circleColor = new Color(227, 117, 64);
            circleEdgeColor = new Color(193, 82, 28);
            circleJointColor = new Color(206, 103, 0);
            //zones and boxes
            zoneL = new Rectangle(width / 4, 200, 100, 100);
            zoneR = new Rectangle(width * 3 / 4 - 100, 200, 100, 100);
            boxL = new Rectangle(100, height - 125, 200, 100);
            boxR = new Rectangle(width - 300, height - 125, 200, 100);
            zoneFault = new Rectangle(0, height * 2 / 3, width, height / 2);
            //boxes
            leftBox = new BoxGame(100 / CommonConstants.PIXELS_TO_METERS, (height - 125) / CommonConstants.PIXELS_TO_METERS, 200 / CommonConstants.PIXELS_TO_METERS, 100 / CommonConstants.PIXELS_TO_METERS);
            leftBox.setProperty("left");
            rightBox = new BoxGame((width - 300) / CommonConstants.PIXELS_TO_METERS, (height - 125) / CommonConstants.PIXELS_TO_METERS, 200 / CommonConstants.PIXELS_TO_METERS, 100 / CommonConstants.PIXELS_TO_METERS);
            rightBox.setProperty("right");
            loadWorld();
        }
        /// <summary>
        /// It initializes a physic world, elements and player skeleton.
        /// </summary>
        private void loadWorld()
        {
            physicsElments = new List<GameElement>();
            movements = new string[elements];

            if (!levels[currentLevel].exist("move"))
                for (int i = 0; i < elements; i++)
                    movements[i] = getRandomMove();
            else
                movements = (string[]) levels[currentLevel].getAttribute("move");

            physicsWorld = new World(new Vector2(0, Convert.ToInt16(levels[currentLevel].getAttribute("gravity"))), true);
            simulate = true;
            //This game doesn't need a skeleton at physic world, it's a reason why skeleton constructor recibes a null physic world.
            skeleton = new Skeleton(null);
            createScreenLimits(width, height);
            currentElement = 0;
            loadElements();
            hits = 0;
            faults = 0;
            timeSpan = TimeSpan.FromMilliseconds(Convert.ToDouble(levels[currentLevel].getAttribute("time")));
        }
        /// <summary>
        /// It returns a random move to asign color circles.
        /// </summary>
        private String getRandomMove()
        {
            String move = "left";
            double value = random.NextDouble();
            if (value >= 0 && value < 0.5f)
                move = "left";
            else
                move = "right";
            return move;
        }
        /// <summary>
        /// It creates circles on the screen.
        /// </summary>
        private void loadElements()
        {
            Body body;
            int x, y;

            if (currentElement < Convert.ToInt16(levels[currentLevel].getAttribute("elements")))
            {
                x = width / 2;
                y = 30;
                body = createCircle(new Vector2(x, y), CommonConstants.CIRCLERADIUS, physicsWorld);
                Circle element = new Circle(physicsWorld, body.GetPosition(), CommonConstants.CIRCLERADIUS, false);
                element.setLinearVelocity(Vector2.Zero);

                element.setProperty(movements[currentElement]);

                if (movements[currentElement].Equals("left"))
                    element.change(CommonConstants.greenColor, CommonConstants.greenColor);
                else
                    element.change(CommonConstants.goldColor, CommonConstants.goldColor);

                physicsElments.Add(element);
            }
            currentElement++;
        }
        /// <summary>
        /// It creates game screen limits.
        /// </summary>
        private void createScreenLimits(int bbwidth, int bbheight)
        {
            // Add ground.
            BodyDef bd = new BodyDef();
            // Ground.
            Body ground = physicsWorld.CreateBody(bd);
            PolygonShape shape_flor = new PolygonShape();
            PolygonShape shape_roof = new PolygonShape();
            PolygonShape shape_wall_left = new PolygonShape();
            PolygonShape shape_wall_right = new PolygonShape();
            shape_flor.SetAsEdge(new Vector2(0.0f, height - 20), new Vector2(width, height - 20));
            shape_roof.SetAsEdge(new Vector2(0.0f, 1.0f), new Vector2(width, 0.0f));
            shape_wall_left.SetAsEdge(new Vector2(0.0f, height - 20), new Vector2(0.0f, 0.0f));
            shape_wall_right.SetAsEdge(new Vector2(width, 0.0f), new Vector2(width, height - 20));
            ground.CreateFixture(shape_flor, 900.5f);
            ground.CreateFixture(shape_roof, 900.5f);
            ground.CreateFixture(shape_wall_left, 900.5f);
            ground.CreateFixture(shape_wall_right, 900.5f);
        }
        /// <summary>
        /// It returns player state which indicates if a player won or lose the game.
        /// </summary>
        public override int getPlayerState()
        {
            // Playing state by default.
            int state = 0;
            if (currentElement > Convert.ToInt16(levels[currentLevel].getAttribute("elements")) && (faults == 0))
                state = 1;
            else if ((currentElement > Convert.ToInt16(levels[currentLevel].getAttribute("elements"))) && (faults > 0))
                state = -1;
            else if ((timeSpan.Minutes == TimeSpan.Zero.Minutes) && (timeSpan.Seconds == TimeSpan.Zero.Seconds))
                state = -1;
            return state;
        }
        /// <summary>
        /// It sets statistics game on a string array to show it after finalize the game level. 
        /// </summary>
        public override string[] setCurrentStatistics()
        {
            return new string[] { 
                "Nivel|" + currentLevel, 
                "Tiempo|" + timeSpan.Minutes + "." + timeSpan.Seconds,
                "Circulos totales|" + elements,
                "Fallos|" + faults,
                "Aciertos|" + (hits)};
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
        public override void Unload()
        {
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>       
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (IsActive)
            {
                this.gameTime = gameTime;
                timeSpan -= gameTime.ElapsedGameTime;
                controllZone();
                verifyElementBox();
                verifyGameStatus();
            }
        }
        /// <summary>
        /// It verifies if an element is inside a box.
        /// </summary>
        private void verifyElementBox()
        {
            if (((Circle) physicsElments[0]).getBody().Position.Y > (height - 45) / CommonConstants.PIXELS_TO_METERS)
            {
                faults++;
                physicsWorld.DestroyBody(((Circle) physicsElments[0]).getBody());
                physicsElments.Remove(physicsElments[0]);
                loadElements();
            }
            else
            {
                if (leftBox.inside(((Circle) physicsElments[0]).getBody().Position) && leftBox.isStorable((Circle) physicsElments[0]))
                {
                    hits++;
                    physicsWorld.DestroyBody(((Circle) physicsElments[0]).getBody());
                    physicsElments.Remove(physicsElments[0]);
                    loadElements();
                }
                else
                    if (leftBox.inside(((Circle) physicsElments[0]).getBody().Position) && !leftBox.isStorable((Circle) physicsElments[0]))
                    {
                        faults++;
                        physicsWorld.DestroyBody(((Circle) physicsElments[0]).getBody());
                        physicsElments.Remove(physicsElments[0]);
                        loadElements();
                    }
                    else
                        if (rightBox.inside(((Circle) physicsElments[0]).getBody().Position) && rightBox.isStorable((Circle) physicsElments[0]))
                        {
                            hits++;
                            physicsWorld.DestroyBody(((Circle) physicsElments[0]).getBody());
                            physicsElments.Remove(physicsElments[0]);
                            loadElements();
                        }
                        else
                            if (rightBox.inside(((Circle) physicsElments[0]).getBody().Position) && !rightBox.isStorable((Circle) physicsElments[0]))
                            {
                                faults++;
                                physicsWorld.DestroyBody(((Circle) physicsElments[0]).getBody());
                                physicsElments.Remove(physicsElments[0]);
                                loadElements();
                            }
            }
        }
        /// <summary>
        /// It verifies if a skeleton joint is inside a zone.
        /// </summary>
        private void controllZone()
        {
            if (KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).X < 300 + 100 && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).X > width / 4 &&
                        KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).Y < (200 + 100) && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).Y > 200)
            {
                ((Circle) physicsElments[0]).getBody().Position = new Vector2(200, height - height / 2) / CommonConstants.PIXELS_TO_METERS;

            }
            else
                if (KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).X < width * 3 / 4 && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).X > (width * 3 / 4 - 100) &&
                KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).Y < 200 + 100 && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).Y > 200)
                {
                    ((Circle) physicsElments[0]).getBody().Position = new Vector2((width - 200) / CommonConstants.PIXELS_TO_METERS, (height - height / 2) / CommonConstants.PIXELS_TO_METERS);
                }
        }
        /// <summary>
        /// It reloads the physic world and elements to replay the game level. 
        /// </summary>
        public override void restartStage()
        {
            loadWorld();
        }
        /// <summary>
        /// It loads next game level to play.
        /// </summary>
        public override void nextLevel()
        {
            if (levels.Count > currentLevel + 1)
                currentLevel++;
            loadWorld();
        }
        /// <summary>
        /// It recibes all player input from kinect, keyboard, etc.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int) ControllingPlayer.Value;

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
                {

                    physicsWorld.Step((float) gameTime.ElapsedGameTime.TotalSeconds, 10, 10);
                }
            }
        }
        /// <summary>
        /// It displays player skeleton, kinect video, game statistics panel and game elements.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.BasicEffect.CurrentTechnique.Passes[0].Apply();

            //draw elements
            drawElementsInTheWorld(physicsElments);
            if (!hideBody)
            {
                // Draw skeleton.
                skeleton.display(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            }
            // Draw statistics panel.
            drawStatisticsPanel(spriteBatch, new String[] { "nivel " + currentLevel,
                                                            timeSpan.Minutes.ToString() + "." + timeSpan.Seconds.ToString() + " segundos",
                                                            hits + " aciertos",
                                                            faults + " fallos" });
            // draw camera            
            if (camera && video)
            {
                spriteBatch.Begin();
                KinectSDK.Instance.DrawVideoCam(spriteBatch, new Rectangle(width - 323, 3, 320, 240));
                spriteBatch.End();
            }
            //draw zones
            ElementPolygon rectangleL = new ElementPolygon(zoneL.X, zoneL.Y, zoneL.Width, zoneL.Height, Color.Gray, 0.3f, true);
            ElementPolygon rectangleR = new ElementPolygon(zoneR.X, zoneR.Y, zoneR.Width, zoneR.Height, Color.Gray, 0.3f, true);
            rectangleL.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            rectangleR.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);

            spriteBatch.Begin();
            spriteBatch.Draw(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX), new Vector2(boxL.X, boxL.Y), null, Color.Green, 0, new Vector2(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Width / 3 + 100, GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Height / 2), 1, SpriteEffects.None, 0);
            spriteBatch.Draw(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX), new Vector2(boxR.X, boxR.Y), null, Color.Yellow, 0, new Vector2(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Width / 3 + 100, GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Height / 2), 1, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}