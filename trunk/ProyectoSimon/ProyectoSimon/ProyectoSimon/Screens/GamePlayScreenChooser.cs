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
        //protected InputAction keyA, cameraKey;
        // Phisycs world parameters.
        private World physicsWorld;
        protected List<GameElement> physicsElments, bodyJoints;
        private string[] movements;
        private BoxGame leftBox, rightBox;
        // Zones.
        Rectangle zoneL, zoneR, boxL, boxR, zoneFault;
        // Logic Game parameters
        private bool simulate, camera, video, hideBody;
        private int hits, faults, currentElement, elements, bwidth, bheight;
        private GameTime gameTime;
        private Color circleColor, circleEdgeColor, circleJointColor;
        private static int JOINTS_COUNT = 20;
        private static int CIRCLERADIUS = 20;
        private static int JOINTRADIUS = 10;
        private static int PIXELS_TO_METERS = 30;
        // Statistics.
        //private Statistics currentStatistics;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamePlayScreenChooser(int w, int h, IList<Level> l)
        {
            // Create the statistics.
            currentStatistics = new Statistics("clasificador");
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //pauseAction = new InputAction(
            //    new Buttons[] { },
            //    new Keys[] { Keys.Escape },
            //    true);

            //keyA = new InputAction(
            //    new Buttons[] { },
            //    new Keys[] { Keys.A },
            //    true);
            //cameraKey = new InputAction(
            //    new Buttons[] { },
            //    new Keys[] { Keys.RightShift, Keys.LeftShift },
            //    true);
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
            leftBox = new BoxGame(100 / PIXELS_TO_METERS, (bheight - 125) / PIXELS_TO_METERS, 200 / PIXELS_TO_METERS, 100 / PIXELS_TO_METERS);
            leftBox.setProperty("left");
            rightBox = new BoxGame((bwidth - 300) / PIXELS_TO_METERS, (bheight - 125) / PIXELS_TO_METERS, 200 / PIXELS_TO_METERS, 100 / PIXELS_TO_METERS);
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
                movements = (string[])levels[currentLevel].getAttribute("move");

            physicsWorld = new World(new Vector2(0, Convert.ToInt16(levels[currentLevel].getAttribute("gravity"))), true);
            simulate = true;
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
                body = createCircle(new Vector2(x, y), CIRCLERADIUS, physicsWorld);
                Circle element = new Circle(physicsWorld, body.GetPosition(), CIRCLERADIUS, false);
                element.setLinearVelocity(Vector2.Zero);

                element.setKind(movements[currentElement]);

                if (movements[currentElement].Equals("left"))
                    element.change(Color.Green, Color.Green);
                else
                    element.change(Color.Gold, Color.Gold);

                physicsElments.Add(element);
            }
            currentElement++;
        }

        //public void updateBodyJoints(IDictionary<Microsoft.Kinect.JointType, Vector2> js)
        //{
        //    jointsIDs = js;
        //    float deltha = 0.25f;
        //    Vector2 posNew, posOld, posResult;
        //    if (js != null)
        //    {
        //        ICollection<Microsoft.Kinect.JointType> keys = js.Keys;
        //        //float x,y;
        //        for (int i = 0; i < JOINTS_COUNT; i++)
        //        {
        //            ////x = js[keys.ElementAt<JointID>(i)].X;
        //            ////y = js[keys.ElementAt<JointID>(i)].Y;
        //            //posNew = js[keys.ElementAt<JointID>(i)];
        //            //posOld = bodyJoints[i].getBody().Position;
        //            //posResult = posNew - posOld;
        //            //if (posResult.LengthSquared() > deltha)
        //            //    bodyJoints[i].getBody().Position = js[keys.ElementAt<JointID>(i)];
        //        }               
        //    }
        //}       

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
                screenManager.Game.ResetElapsedTime();
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
            if (((Circle)physicsElments[0]).getBody().Position.Y > (bheight - 45) / PIXELS_TO_METERS)
            {
                faults++;
                physicsWorld.DestroyBody(((Circle)physicsElments[0]).getBody());
                physicsElments.Remove(physicsElments[0]);
                //Console.WriteLine("fallo" + ((Circle)physicsElments[0]).getBody().Position.Y);
                loadElements();
            }
            else
            {
                Console.WriteLine(" entro" + ((Circle)physicsElments[0]).getBody().Position.Y + " " + bheight);
                if (leftBox.inside(((Circle)physicsElments[0]).getBody().Position) && leftBox.isStorable(physicsElments[0]))
                {
                    hits++;
                    physicsWorld.DestroyBody(((Circle)physicsElments[0]).getBody());
                    physicsElments.Remove(physicsElments[0]);
                    loadElements();
                }
                else
                    if (leftBox.inside(((Circle)physicsElments[0]).getBody().Position) && !leftBox.isStorable(physicsElments[0]))
                    {
                        faults++;
                        physicsWorld.DestroyBody(((Circle)physicsElments[0]).getBody());
                        physicsElments.Remove(physicsElments[0]);
                        loadElements();
                    }
                    else
                        if (rightBox.inside(((Circle)physicsElments[0]).getBody().Position) && rightBox.isStorable(physicsElments[0]))
                        {
                            hits++;
                            physicsWorld.DestroyBody(((Circle)physicsElments[0]).getBody());
                            physicsElments.Remove(physicsElments[0]);
                            loadElements();
                        }
                        else
                            if (rightBox.inside(((Circle)physicsElments[0]).getBody().Position) && !rightBox.isStorable(physicsElments[0]))
                            {
                                faults++;
                                physicsWorld.DestroyBody(((Circle)physicsElments[0]).getBody());
                                physicsElments.Remove(physicsElments[0]);
                                loadElements();
                            }
            }
        }

        private void controllZone()
        {
            if (jointsIDs != null)
            {
                if (jointsIDs[Microsoft.Kinect.JointType.HandLeft].X < 300 + 100 && jointsIDs[Microsoft.Kinect.JointType.HandLeft].X > bwidth / 4 &&
                    jointsIDs[Microsoft.Kinect.JointType.HandLeft].Y < (200 + 100) && jointsIDs[Microsoft.Kinect.JointType.HandLeft].Y > 200)
                {
                    //Console.WriteLine("izq");
                    ((Circle)physicsElments[0]).getBody().Position = new Vector2(200, bheight - bheight / 2) / PIXELS_TO_METERS;

                }
                else
                    if (jointsIDs[Microsoft.Kinect.JointType.HandRight].X < bwidth * 3 / 4 && jointsIDs[Microsoft.Kinect.JointType.HandRight].X > (bwidth * 3 / 4 - 100) &&
                    jointsIDs[Microsoft.Kinect.JointType.HandRight].Y < 200 + 100 && jointsIDs[Microsoft.Kinect.JointType.HandRight].Y > 200)
                    {
                        //Console.WriteLine("der");
                        ((Circle)physicsElments[0]).getBody().Position = new Vector2((bwidth - 200) / PIXELS_TO_METERS, (bheight - bheight / 2) / PIXELS_TO_METERS);
                    }
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

        //private void verifyInteractionBetweenBodies()
        //{
        //    Contact contact = physicsWorld.GetContactList();
        //    //IContactListener listener = physicsWorld.ContactListener;
        //    //listener.EndContact(contact);            
        //    Body bodyReturn = handContactBall(contact);
        //    timeSpan -= gameTime.ElapsedGameTime;

        //    if (bodyReturn != null)
        //    {
        //        //ElementPhysic e = getPhysicElement(bodyReturn);
        //        //e.incHitNumber();
        //        //e.setColor(Color.Red);
        //        //if (e.getHitNumber().Equals(2))
        //        ////if(e.getColor().Equals(Color.Red))
        //        //{
        //        //    physicsWorld.DestroyBody(bodyReturn);
        //        //    physicsElments.Remove(e);
        //        //    elements--;
        //        //}
        //        //hits++;

        //        ////hit.Play();
        //    }
        //}

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
                //this.Activate(false);
                // pauseScreen = new PauseMenuScreen(content, stage);
                //ScreenManager.AddScreen(pauseScreen, ControllingPlayer);
                PauseMenuScreen pauseMenuScreen = new PauseMenuScreen();
                pauseMenuScreen.CurrentUser = screenManager.getUserIndex();
                pauseMenuScreen.CurrentGame = screenManager.getIndexGame();
                screenManager.AddScreen(pauseMenuScreen, ControllingPlayer);
            }
            else
            {
                //updateBodyJoints(screenManager.Kinect.getJoints());

                //jointsIDs = screenManager.Kinect.getJoints();               
                updateJoints(screenManager.Kinect.getJoints());

                //if (keyboardState.IsKeyDown(Keys.LeftControl))
                //    simulate = !simulate;
                if (seatedMode.Evaluate(input, ControllingPlayer, out player))
                    screenManager.Kinect.setSeatedMode();
                if (defaultMode.Evaluate(input, ControllingPlayer, out player))
                    screenManager.Kinect.setDefaultMode();
                if (!camera && cameraKey.Evaluate(input, ControllingPlayer, out player))
                    camera = true;
                else
                    if (camera && cameraKey.Evaluate(input, ControllingPlayer, out player))
                        camera = false;  

                if (simulate)
                {

                    physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 10, 10);
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

        // crear una clase que implemente IDicttionary??'
        public void updateJoints(IDictionary<Microsoft.Kinect.JointType, Vector2> js)
        {
            jointsIDs = js;
            float deltha = 0.25f;
            Vector2 posNew, posOld, posResult;
            if (js != null)
            {
                ICollection<Microsoft.Kinect.JointType> keys = js.Keys;
                //float x,y;
                for (int i = 0; i < JOINTS_COUNT; i++)
                {
                    //x = js[keys.ElementAt<JointID>(i)].X;
                    //y = js[keys.ElementAt<JointID>(i)].Y;
                    posNew = js[keys.ElementAt<Microsoft.Kinect.JointType>(i)];
                    posOld = jointsIDs[keys.ElementAt<Microsoft.Kinect.JointType>(i)];
                    posResult = posNew - posOld;
                    if (posResult.LengthSquared() > deltha)
                        jointsIDs[keys.ElementAt<Microsoft.Kinect.JointType>(i)] = js[keys.ElementAt<Microsoft.Kinect.JointType>(i)];
                }
                //controll skeleton
                hideBody = false;
                posResult = js[Microsoft.Kinect.JointType.Head] - js[Microsoft.Kinect.JointType.ShoulderCenter];
                Vector2 posResult1 = js[Microsoft.Kinect.JointType.Head] - js[Microsoft.Kinect.JointType.ShoulderCenter];
                //Console.WriteLine("head " + js[JointID.Head]);
                //Console.WriteLine("shoulder " + js[JointID.ShoulderCenter]);
                //if (posResult.LengthSquared() > 10000 || posResult1.LengthSquared() < 2500)
                //{
                //    hideBody = true;
                //}
            }
        }

        //private ElementPhysic getPhysicElement(Body b)
        //{
        //    ElementPhysic p = null;
        //    for (int i = 0; i < physicsElments.Count; i++)
        //        if (((Circle)physicsElments[i]).getBody().Equals(b))
        //            return p = physicsElments[i];
        //    if (p == null)
        //    {
        //        for (int i = 0; i < bodyJoints.Count; i++)
        //            if (((Circle)physicsElments[i]).getBody().Equals(b))
        //                return p = bodyJoints[i];
        //    }
        //    return p;
        //}

        //private Body handContactBall(Contact c)
        //{
        //    Body ret = null;
        //    if (c != null && c.IsTouching())
        //    {
        //        ElementPhysic elementA = getPhysicElement(c.GetFixtureA().GetBody());
        //        ElementPhysic elementB = getPhysicElement(c.GetFixtureB().GetBody());

        //        //if (elementA != null && elementB != null && elementA.getColor().Equals(circleJoint) && elementB.getColor().Equals(circleColor) )
        //        //    ret = c.GetFixtureB().GetBody();
        //        //else if (elementA != null && elementB != null && elementB.getColor().Equals(circleJoint) &&  elementB.getColor().Equals(Color.Red))
        //        //    ret = c.GetFixtureB().GetBody();
        //        //else if (elementA != null && elementB != null && elementB.getColor().Equals(circleJoint) && elementA.getColor().Equals(circleColor))
        //        //    ret = c.GetFixtureA().GetBody();               
        //        //else if (elementA != null && elementB != null && elementB.getColor().Equals(circleJoint) && elementA.getColor().Equals(Color.Red))                   
        //        //    ret = c.GetFixtureA().GetBody();

        //        //if (elementA != null && elementB != null && elementA.getColor().Equals(circleJointColor) && (elementB.getColor().Equals(circleColor) || elementB.getColor().Equals(Color.Red)))
        //        //    ret = c.GetFixtureB().GetBody();
        //        //else if (elementA != null && elementB != null && elementB.getColor().Equals(circleJointColor) && (elementA.getColor().Equals(circleColor) || elementA.getColor().Equals(Color.Red)))
        //        //    ret = c.GetFixtureA().GetBody();
        //    }
        //    return ret;
        //}

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            screenManager.getBasicEffect().CurrentTechnique.Passes[0].Apply();

            //draw elements
            drawElementsInTheWorld(physicsElments);
            //base.drawBodies(spriteBatch, physicsElments, CIRCLERADIUS, 1, circleEdgeColor);
            //draw joints
            //drawBodies(spriteBatch, bodyJoints, JOINTRADIUS, 0.5f);
            // Draw lines.
            //drawSkeleton(spriteBatch);
            if (!hideBody)
            {
                // Draw lines.
                screenManager.Kinect.display(screenManager);
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
                screenManager.Kinect.DrawVideoCam(spriteBatch, new Rectangle(bwidth - 323, 3, 320, 240));
                spriteBatch.End();
            }
            //draw zones
            ElementPolygon rectangleL = new ElementPolygon(zoneL.X, zoneL.Y, zoneL.Width, zoneL.Height, Color.Gray, 0.3f, true);
            ElementPolygon rectangleR = new ElementPolygon(zoneR.X, zoneR.Y, zoneR.Width, zoneR.Height, Color.Gray, 0.3f, true);
            //ElementPolygon rectangleFault = new ElementPolygon(zoneFault.X, zoneFault.Y, zoneFault.Width, zoneFault.Height, Color.Gray, 0.1f, true);
            //rectangleFault.drawPrimitive(screenManager);
            rectangleL.draw(screenManager);
            rectangleR.draw(screenManager);

            //draw boxes
            //ElementPolygon boxLeft = new ElementPolygon(boxL.X, boxL.Y, boxL.Width, boxL.Height, Color.SaddleBrown, 1, true);
            //boxLeft.drawPrimitive(screenManager);
            //ElementPolygon boxRight = new ElementPolygon(boxR.X, boxR.Y, boxR.Width, boxR.Height, Color.SaddleBrown, 1, true);
            //boxLeft.drawPrimitive(screenManager);
            //boxRight.drawPrimitive(screenManager);
            //screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX);            
            spriteBatch.Begin();
            spriteBatch.Draw(screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX), new Vector2(boxL.X, boxL.Y), null, Color.Green, 0, new Vector2(screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX).Width / 3 + 100, screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX).Height / 2), 1, SpriteEffects.None, 0);
            spriteBatch.Draw(screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX), new Vector2(boxR.X, boxR.Y), null, Color.Yellow, 0, new Vector2(screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX).Width / 3 + 100, screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX).Height / 2), 1, SpriteEffects.None, 0);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void drawGroundPrimitive(SpriteBatch spriteBatch)
        {
            quad = new ElementPolygon(0, bheight - 30, bwidth, 30, new Color(124, 107, 70), 1, true);
            quad.draw(screenManager);
            fillQuad = new ElementPolygon(0, bheight - 30, bwidth, 30, new Color(152, 131, 87), 1, false);
            fillQuad.draw(screenManager);
        }
    }
}