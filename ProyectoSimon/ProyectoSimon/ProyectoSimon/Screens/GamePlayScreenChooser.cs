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
        private ElementPolygon quad, fillQuad;
        private Random random = new Random();
        // Phisycs world parameters.
        private World physicsWorld;
        protected List<GameElement> physicsElments, bodyJoints;
        private string[] movements;
        private BoxGame leftBox, rightBox;
        // Kinect parameters.
        Skeleton skeleton;
        // Zones.
        Rectangle zoneL, zoneR, boxL, boxR, zoneFault;
        // Logic Game parameters
        private bool simulate, camera, video, hideBody;
        private int hits, faults, currentElement, elements, bwidth, bheight;
        private GameTime gameTime;
        private Color circleColor, circleEdgeColor, circleJointColor;

        /// <summary>
        /// Constructor.
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
            bwidth = w;
            bheight = h;
            circleColor = new Color(227, 117, 64);
            circleEdgeColor = new Color(193, 82, 28);
            circleJointColor = new Color(206, 103, 0);
            //zones and boxes
            zoneL = new Rectangle(bwidth / 4, 200, 100, 100);
            zoneR = new Rectangle(bwidth * 3 / 4 - 100, 200, 100, 100);
            boxL = new Rectangle(100, bheight - 125, 200, 100);
            boxR = new Rectangle(bwidth - 300, bheight - 125, 200, 100);
            zoneFault = new Rectangle(0, bheight * 2 / 3, bwidth, bheight / 2);
            //boxes
            leftBox = new BoxGame(100 / CommonConstants.PIXELS_TO_METERS, (bheight - 125) / CommonConstants.PIXELS_TO_METERS, 200 / CommonConstants.PIXELS_TO_METERS, 100 / CommonConstants.PIXELS_TO_METERS);
            leftBox.setProperty("left");
            rightBox = new BoxGame((bwidth - 300) / CommonConstants.PIXELS_TO_METERS, (bheight - 125) / CommonConstants.PIXELS_TO_METERS, 200 / CommonConstants.PIXELS_TO_METERS, 100 / CommonConstants.PIXELS_TO_METERS);
            rightBox.setProperty("right");
            loadWorld();
        }

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
            //This game doesn't need a skeleton in physic world, it's a reason why skeleton constructor recibes a null physic world.
            skeleton = new Skeleton(null);
            crearBordes(bwidth, bheight);
            currentElement = 0;
            loadElements();
            hits = 0;
            faults = 0;
            timeSpan = TimeSpan.FromMilliseconds(Convert.ToDouble(levels[currentLevel].getAttribute("time")));
        }

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

        private void loadElements()
        {
            Body body;
            int x, y;

            if (currentElement < Convert.ToInt16(levels[currentLevel].getAttribute("elements")))
            {
                x = bwidth / 2;
                y = 30;
                body = createCircle(new Vector2(x, y), CommonConstants.CIRCLERADIUS, physicsWorld);
                Circle element = new Circle(physicsWorld, body.GetPosition(), CommonConstants.CIRCLERADIUS, false);
                element.setLinearVelocity(Vector2.Zero);

                element.setProperty(movements[currentElement]);

                if (movements[currentElement].Equals("left"))
                    element.change(Color.Green, Color.Green);
                else
                    element.change(Color.Gold, Color.Gold);

                physicsElments.Add(element);
            }
            currentElement++;
        }       

        private void crearBordes(int bbwidth, int bbheight)
        {
            // Add ground.
            BodyDef bd = new BodyDef();

            // Ground.
            Body ground = physicsWorld.CreateBody(bd);
            PolygonShape shape_flor = new PolygonShape();
            PolygonShape shape_roof = new PolygonShape();
            PolygonShape shape_wall_left = new PolygonShape();
            PolygonShape shape_wall_right = new PolygonShape();
            shape_flor.SetAsEdge(new Vector2(0.0f, bheight - 20), new Vector2(bwidth, bheight - 20));
            shape_roof.SetAsEdge(new Vector2(0.0f, 1.0f), new Vector2(bwidth, 0.0f));
            shape_wall_left.SetAsEdge(new Vector2(0.0f, bheight - 20), new Vector2(0.0f, 0.0f));
            shape_wall_right.SetAsEdge(new Vector2(bwidth, 0.0f), new Vector2(bwidth, bheight - 20));
            ground.CreateFixture(shape_flor, 900.5f);
            ground.CreateFixture(shape_roof, 900.5f);
            ground.CreateFixture(shape_wall_left, 900.5f);
            ground.CreateFixture(shape_wall_right, 900.5f);
        }

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

        private void verifyElementBox()
        {
            if (((Circle) physicsElments[0]).getBody().Position.Y > (bheight - 45) / CommonConstants.PIXELS_TO_METERS)
            {
                faults++;
                physicsWorld.DestroyBody(((Circle) physicsElments[0]).getBody());
                physicsElments.Remove(physicsElments[0]);
                //Console.WriteLine("fallo" + ((Circle)physicsElments[0]).getBody().Position.Y);
                loadElements();
            }
            else
            {
                //Console.WriteLine(" entro" + ((Circle) physicsElments[0]).getBody().Position.Y + " " + bheight);
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
        private void controllZone()
        {
            if (KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).X < 300 + 100 && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).X > bwidth / 4 &&
                        KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).Y < (200 + 100) && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandLeft).Y > 200)
            {
                //Console.WriteLine("izq");
                ((Circle) physicsElments[0]).getBody().Position = new Vector2(200, bheight - bheight / 2) / CommonConstants.PIXELS_TO_METERS;

            }
            else
                if (KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).X < bwidth * 3 / 4 && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).X > (bwidth * 3 / 4 - 100) &&
                KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).Y < 200 + 100 && KinectSDK.Instance.getJointPosition(Microsoft.Kinect.JointType.HandRight).Y > 200)
                {
                    //Console.WriteLine("der");
                    ((Circle) physicsElments[0]).getBody().Position = new Vector2((bwidth - 200) / CommonConstants.PIXELS_TO_METERS, (bheight - bheight / 2) / CommonConstants.PIXELS_TO_METERS);
                }
        }

        public override void restartStage()
        {
            //levels[currentLevel].setAttribute("beginTime", DateTime.Now);
            loadWorld();
        }

        public override void nextLevel()
        {
            //stopSong();
            //levels[currentLevel].setAttribute("beginTime", DateTime.Now);
            if (levels.Count > currentLevel + 1)
                currentLevel++;
            loadWorld();
        }

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
                //this.Activate(false);
                // pauseScreen = new PauseMenuScreen(content, stage);
                //ScreenManager.AddScreen(pauseScreen, ControllingPlayer);
                PauseMenuScreen pauseMenuScreen = new PauseMenuScreen();
                pauseMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
                pauseMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
                ScreenManager.AddScreen(pauseMenuScreen, ControllingPlayer);
            }
            else
            {
                //updateBodyJoints(ScreenManager.Kinect.getJoints());

                //jointsIDs = ScreenManager.Kinect.getJoints();               
                //updateJoints(KinectSDK.Instance.getJoints());

                //if (keyboardState.IsKeyDown(Keys.LeftControl))
                //    simulate = !simulate;
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
                    //Console.WriteLine((float)gameTime.ElapsedGameTime.TotalSeconds);
                    //rHand.Position = hRight;
                    //lHand.Position = hLeft;

                    //if (keyboardState.IsKeyDown(Keys.A))
                    //    press = true;
                    //if (keyboardState.IsKeyUp(Keys.A) && press)
                    //if (keyA.Evaluate(input, ControllingPlayer, out player))
                    //{
                    //    ////createCircle(new Vector2(random.Next(205, bwidth - 205), random.Next(25, bheight - 25)), 20.0f);                        
                    //    //int x = random.Next(400, bwidth - 300);
                    //    //int y = random.Next(20, bheight - 20);
                    //    //Body body = createCircle(new Vector2(x, y), CIRCLERADIUS, physicsWorld);
                    //    ////ElementCircle circle = new ElementCircle(8.0f, new Vector2(x, y), PrimitiveType.TriangleList);
                    //    ////circle.createSolidCircle(Color.Red);
                    //    //ElementPhysic element = new ElementPhysic(body);
                    //    //element.setColor(circleColor);
                    //    //physicsElments.Add(element);
                    //    //elements++;
                    //    ////press = false;                        
                    //}
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            ScreenManager.BasicEffect.CurrentTechnique.Passes[0].Apply();

            //draw elements
            drawElementsInTheWorld(physicsElments);
            //base.drawBodies(spriteBatch, physicsElments, CIRCLERADIUS, 1, circleEdgeColor);
            //draw joints
            //drawBodies(spriteBatch, bodyJoints, JOINTRADIUS, 0.5f);
            // Draw lines.
            //drawSkeleton(spriteBatch);
            if (!hideBody)
            {
                // Draw skeleton.
                skeleton.display(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            }
            // Draw ground.
            //drawGroundPrimitive(spriteBatch);            
            // Draw statistics panel.
            drawStatisticsPanel(spriteBatch, new String[] { "nivel " + currentLevel,
                                                            timeSpan.Minutes.ToString() + "." + timeSpan.Seconds.ToString() + " segundos",
                                                            hits + " aciertos",
                                                            faults + " fallos" });
            // draw camera            
            if (camera && video)
            {
                spriteBatch.Begin();
                KinectSDK.Instance.DrawVideoCam(spriteBatch, new Rectangle(bwidth - 323, 3, 320, 240));
                spriteBatch.End();
            }
            //draw zones
            ElementPolygon rectangleL = new ElementPolygon(zoneL.X, zoneL.Y, zoneL.Width, zoneL.Height, Color.Gray, 0.3f, true);
            ElementPolygon rectangleR = new ElementPolygon(zoneR.X, zoneR.Y, zoneR.Width, zoneR.Height, Color.Gray, 0.3f, true);
            //ElementPolygon rectangleFault = new ElementPolygon(zoneFault.X, zoneFault.Y, zoneFault.Width, zoneFault.Height, Color.Gray, 0.1f, true);
            //rectangleFault.drawPrimitive(ScreenManager);
            rectangleL.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            rectangleR.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);

            //draw boxes
            //ElementPolygon boxLeft = new ElementPolygon(boxL.X, boxL.Y, boxL.Width, boxL.Height, Color.SaddleBrown, 1, true);
            //boxLeft.drawPrimitive(ScreenManager);
            //ElementPolygon boxRight = new ElementPolygon(boxR.X, boxR.Y, boxR.Width, boxR.Height, Color.SaddleBrown, 1, true);
            //boxLeft.drawPrimitive(ScreenManager);
            //boxRight.drawPrimitive(ScreenManager);
            //ScreenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX);            
            spriteBatch.Begin();
            spriteBatch.Draw(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX), new Vector2(boxL.X, boxL.Y), null, Color.Green, 0, new Vector2(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Width / 3 + 100, GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Height / 2), 1, SpriteEffects.None, 0);
            spriteBatch.Draw(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX), new Vector2(boxR.X, boxR.Y), null, Color.Yellow, 0, new Vector2(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Width / 3 + 100, GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Height / 2), 1, SpriteEffects.None, 0);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void drawGroundPrimitive(SpriteBatch spriteBatch)
        {
            quad = new ElementPolygon(0, bheight - 30, bwidth, 30, new Color(124, 107, 70), 1, true);
            quad.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            fillQuad = new ElementPolygon(0, bheight - 30, bwidth, 30, new Color(152, 131, 87), 1, false);
            fillQuad.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
        }
    }
}