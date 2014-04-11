using System.Threading;
using Microsoft.Xna.Framework.Input;
using Box2D.XNA;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProyectoSimon.Elements;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ProyectoSimon
{
    class GamePlayScreenArrows : GameplayScreen
    {
        // Kinect parameters.
        Skeleton skeleton;
        //  Input parameters.
    //    protected InputAction cameraKey;
        // Physics world parameters.
        private World physicsWorld;
        // Logic game parameters.
        private bool simulate, camera, video, wasItAHit;
        //, camera, video;
        private int bwidth, bheight, elements, hits;
        private GameTime gameTime;
        private int change, arrowsIndex;
        private float alpha, scale;
        private Circle arrowCircle;
        private Rectangle rectCenter, rectUp, rectDown, rectLeft, rectRight;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GamePlayScreenArrows(int w, int h, IList<Level> l)
        {
            // Create the statistics.
            currentStatistics = new Statistics("flechas");

    //        TransitionOnTime = TimeSpan.FromSeconds(1.5);
    //        TransitionOffTime = TimeSpan.FromSeconds(0.5);
            //        cameraKey = new InputAction(
            //            new Buttons[] { },
            //            new Keys[] { Keys.RightShift,Keys.LeftShift },
            //            true);
            //        camera = false;
            //        video = true;

            // Rectangle width center.
            int wCenter = 75;
            int wBox = 40;

            //pauseAction = new InputAction(
            //    new Buttons[] { },
            //    new Keys[] { Keys.Escape },
            //    true);
            generateInput();
            simulate = true;
            wasItAHit = false;
            camera = false;
            video = true;
            currentLevel = 0;
            levels = l;
            elements = Convert.ToInt16(levels[currentLevel].getAttribute("elements"));
            hits = 0;
            bwidth = w;
            bheight = h;
            alpha = 1.0f;
            scale = .25f;
            arrowsIndex = 0;
            change = getIndexAngle();
            // Center box.
            rectCenter = new Rectangle(w/2 - wCenter + w/8 , h/2 - wCenter, wCenter*2, wCenter*2);
            // Control boxes.
            rectUp = new Rectangle(w / 4 - wBox/2, h / 4 - wBox/2 - 20, wBox*2, wBox*2);
            rectDown = new Rectangle(w / 4 - wBox / 2, h * 3/4 - wBox / 2 + 20, wBox * 2, wBox * 2);
            rectLeft = new Rectangle(w / 8 - wBox / 2 - 20, h / 2 - wBox / 2, wBox * 2, wBox * 2);
            rectRight = new Rectangle(w * 3 / 8 - wBox / 2 + 20, h / 2 - wBox / 2, wBox * 2, wBox * 2);

            loadWorld();
        }

        private void loadWorld()
        {
            // Create physic world with a specific gravity. NO TENER ATRIBUTOS Q NO USAMOS EN EL XML !!!!
            physicsWorld = new World(new Vector2(0, Convert.ToInt32(levels[currentLevel].getAttribute("gravity"))), true);
            simulate = true;
            arrowsIndex = 0;
            hits = 0;
            wasItAHit = false;
            change = getIndexAngle();
            arrowCircle = makePhysicElement();
            // Load Kinect's elements.
            skeleton = new Skeleton(null);
        }

        private Circle makePhysicElement()
        {
            Circle aux = new Circle(physicsWorld, new Vector2(bwidth, bheight / 2), CommonConstants.CIRCLERADIUS, false);
            aux.setLinearVelocity(new Vector2(-10, 0)); // atributo desde xml !!

            return aux;
        }

        public override int getPlayerState()
        {
            // Playing state by default.
            int state = 0;

            if (arrowsIndex == elements)
            {
                if (cumplioMinimo())
                    state = 1;
                else 
                    state = -1;
            }

            return state;
        }

        private bool cumplioMinimo()
        {
            return elements == hits;
        }

        public override string[] setCurrentStatistics()
        {
            return new string[] { 
                    "nivel|" + currentLevel, 
                    "totales|" + elements,
                    "acertados|" + hits,
                    "no acertados|" + (elements - hits)};
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
                simulate = true;
                this.gameTime = gameTime;

                if (arrowsIndex < elements)
                {
                    if (isThereAHit() && !wasItAHit)
                    {
                        hits++;
                        wasItAHit = true;
                    }

                    // usar una sólo método y pregunta por las áreas, más legible!
                    if (isArrowInArea())
                    {
                        change = getIndexAngle();
                        arrowCircle = makePhysicElement();
                        arrowsIndex++;
                        alpha = 1.0f;
                        scale = .25f;
                        wasItAHit = false;
                    }
                    else if (arrowCircle.getBody().Position.X * 30 >= bwidth * 3 / 8  &&
                        arrowCircle.getBody().Position.X * 30 < bwidth / 2 + bwidth / 8)
                    {
                        alpha -= 0.04f;
                        if (isThereAHit())
                            scale += 0.02f;
                    }
                }

                //if (ScreenManager.Kinect.isInRange())
                //{
                if (arrowsIndex == elements)
                    verifyGameStatus();
                //}
                //else simulate = false;
            }
        }

        private bool isThereAHit()
        {
            int xA, yA;
            xA = Convert.ToInt16(arrowCircle.getBody().Position.X) * 30;
            yA = Convert.ToInt16(arrowCircle.getBody().Position.Y) * 30;

            Vector2 jointHandLeft = skeleton.getJointPosition(Microsoft.Kinect.JointType.HandLeft);// +new Vector2(-200, 0);
            Vector2 jointHandRight = skeleton.getJointPosition(Microsoft.Kinect.JointType.HandRight);// +new Vector2(-200, 0);

            return rectUp.Contains((int)jointHandLeft.X, (int)jointHandLeft.Y) && rectUp.Contains((int)jointHandRight.X, (int)jointHandRight.Y) && rectCenter.Contains(xA, yA) && change == 1
                || rectDown.Contains((int)jointHandLeft.X, (int)jointHandLeft.Y) && rectUp.Contains((int)jointHandRight.X, (int)jointHandRight.Y) && rectCenter.Contains(xA, yA) && change == 3
                || rectLeft.Contains((int)jointHandLeft.X, (int)jointHandLeft.Y) && rectCenter.Contains(xA, yA) && change == 0
                || rectRight.Contains((int)jointHandRight.X, (int)jointHandRight.Y) && rectCenter.Contains(xA, yA) && change == 2;

            //return rectUp.Contains(Mouse.GetState().X, Mouse.GetState().Y) && rectCenter.Contains(xA, yA) && change == 1
            //    || rectDown.Contains(Mouse.GetState().X, Mouse.GetState().Y) && rectCenter.Contains(xA, yA) && change == 3
            //    || rectLeft.Contains(Mouse.GetState().X, Mouse.GetState().Y) && rectCenter.Contains(xA, yA) && change == 0
            //    || rectRight.Contains(Mouse.GetState().X, Mouse.GetState().Y) && rectCenter.Contains(xA, yA) && change == 2;
        }

        private bool isArrowInArea()
        {
            return arrowCircle.getBody().Position.X * 30 < bwidth * 3 / 8 + bwidth / 8;
        }

        public override void restartStage()
        {
            loadWorld();
            elements = Convert.ToInt16(levels[currentLevel].getAttribute("elements"));
        }

        public override void nextLevel()
        {
            if (levels.Count > currentLevel + 1)
                currentLevel++;
        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {
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
                    physicsWorld.Step(1.0f / 60.0f, 8, 3);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            // Apply effects to draw primitives.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

    //        if (getPlayerState() == 0)
    //        {
    //            ScreenManager.getBasicEffect().CurrentTechnique.Passes[0].Apply();
    //            // Draw each physics elements in the world.
                //drawElementsInTheWorld(physicsElements, change);
    //            // Draw skeleton if it isn't hidden.
            
    //        }

            // Draw statistics panel.
            drawStatisticsPanel(spriteBatch, new String[] { hits + "/" + elements + " aciertos", (elements - arrowsIndex) + " restantes" });
            
    //        // Show camera if it is active.
            if (camera && video)
            {
                spriteBatch.Begin();
                KinectSDK.Instance.DrawVideoCam(spriteBatch, new Rectangle(bwidth - 323, 3, 320, 240));
                spriteBatch.End();
            }
            
            drawRectangle(rectCenter, Color.LightBlue, Color.White, 3, .5f);
            drawRectangle(rectUp, Color.LightCoral, Color.White, 3, .5f);
            drawRectangle(rectDown, Color.LightCoral, Color.White, 3, .5f);
            drawRectangle(rectLeft, Color.LightCoral, Color.White, 3, .5f);
            drawRectangle(rectRight, Color.LightCoral, Color.White, 3, .5f);

            if (arrowsIndex < elements)
            {
                Texture2D texture = GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_ALGO);
                Rectangle rect = new Rectangle(0, 0, texture.Width, texture.Height);
                Vector2 vector = new Vector2(texture.Width, texture.Height) / 2;

                ScreenManager.SpriteBatch.Begin();

                ScreenManager.SpriteBatch.Draw(texture, arrowCircle.getBody().Position * 30, rect, Color.White * alpha, (90 * change) * MathHelper.Pi / 180, vector, scale, SpriteEffects.None, 1);

                ScreenManager.SpriteBatch.End();
            }

            //if (ScreenManager.Kinect.isInRange())
            //{
                //skeleton.traslateSkeleton(new Vector2(-200, 0));
            skeleton.display(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            //}
            //drawMouseCursor();
        }

        //private void drawMouseCursor()
        //{
        //    drawRectangle(new Rectangle(Mouse.GetState().X - 5, Mouse.GetState().Y - 5, 10, 10), Color.OrangeRed, Color.White, 3, 1);
        //}

        private void drawRectangle(Rectangle rect, Color fillColor, Color edgeColor, int border, float alpha)
        {
            // Cambiar !!!
            ElementPolygon rectPrimitive = new ElementPolygon(rect.Center.X - rect.Width / 2, rect.Center.Y - rect.Height / 2, rect.Width, rect.Height, fillColor, alpha, true);
            rectPrimitive.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            ElementPolygon rectEdgePrimitive = new ElementPolygon(rect.Center.X - rect.Width / 2, rect.Center.Y - rect.Height / 2, rect.Width, rect.Height, edgeColor, alpha, false);
            rectEdgePrimitive.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
        }

        private int getIndexAngle()
        {
            int index = 0;

            if (!levels[currentLevel].exist("move"))
            {
                Random next = new Random();
                index = next.Next(0, 3);
            }
            else
            {
                String[] movements = (String[])levels[currentLevel].getAttribute("move");

                if (movements[arrowsIndex].Equals("left"))
                    index = 0;
                else if (movements[arrowsIndex].Equals("up"))
                    index = 1;
                else if (movements[arrowsIndex].Equals("right"))
                    index = 2;
                else if (movements[arrowsIndex].Equals("down"))
                    index = 3;
            }

            return index;
        }

        // Draw a simple line primitive. SACAR DESPUES !!!
        //private void drawLine(ScreenManager ScreenManager, Color colorLine, float alpha, Vector2 begin, Vector2 end)
        //{
        //    Box2D.XNA.FixedArray8<Vector2> vertexs = new Box2D.XNA.FixedArray8<Vector2>();
        //    vertexs[0] = new Vector2(begin.X, begin.Y);
        //    vertexs[1] = new Vector2(end.X, end.Y);

        //    ElementPolygon line = new ElementPolygon(vertexs, colorLine, 1, false, 2);
        //    line.drawPrimitive(ScreenManager);
        //}
    }
}