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
        // Physics world parameters.
        private World physicsWorld;
        // Logic game parameters.
        private bool simulate, camera, video, wasItAHit;
        private int elements, hits;
        private GameTime gameTime;
        private int change, arrowsIndex;
        private float alpha, scale;
        private Circle arrowCircle;
        private Rectangle rectCenter, rectUp, rectDown, rectLeft, rectRight;

        /// <summary>
        /// Class constructor where initializes all dinamic game structures .
        /// </summary>
        public GamePlayScreenArrows(int w, int h, IList<Level> l)
        {
            // Create the statistics.
            currentStatistics = new Statistics("flechas");
            // Rectangle width center.
            int wCenter = 75;
            int wBox = 40;

            generateInput();
            simulate = true;
            wasItAHit = false;
            camera = false;
            video = true;
            currentLevel = 0;
            levels = l;
            elements = Convert.ToInt16(levels[currentLevel].getAttribute("elements"));
            hits = 0;
            width = w;
            height = h;
            alpha = 1.0f;
            scale = .25f;
            arrowsIndex = 0;
            change = getIndexAngle();
            // Center box.
            rectCenter = new Rectangle(w / 2 - wCenter + w / 8, h / 2 - wCenter, wCenter * 2, wCenter * 2);
            // Control boxes.
            rectUp = new Rectangle(w / 4 - wBox / 2, h / 4 - wBox / 2 - 20, wBox * 2, wBox * 2);
            rectDown = new Rectangle(w / 4 - wBox / 2, h * 3 / 4 - wBox / 2 + 20, wBox * 2, wBox * 2);
            rectLeft = new Rectangle(w / 8 - wBox / 2 - 20, h / 2 - wBox / 2, wBox * 2, wBox * 2);
            rectRight = new Rectangle(w * 3 / 8 - wBox / 2 + 20, h / 2 - wBox / 2, wBox * 2, wBox * 2);

            loadWorld();
        }
        /// <summary>
        /// It initializes a physic world, elements and player skeleton.
        /// </summary>
        private void loadWorld()
        {
            // Create physic world with a specific gravity.
            physicsWorld = new World(new Vector2(0, Convert.ToInt32(levels[currentLevel].getAttribute("gravity"))), true);
            simulate = true;
            arrowsIndex = 0;
            hits = 0;
            wasItAHit = false;
            change = getIndexAngle();
            arrowCircle = makePhysicElement();
            // Loads Kinect's elements, null parameter is used beacause in this case we don't need interation with elements.
            skeleton = new Skeleton(null);
        }
        /// <summary>
        /// It makes a physic element with a linear velocity.
        /// </summary>
        private Circle makePhysicElement()
        {
            Circle aux = new Circle(physicsWorld, new Vector2(width, height / 2), CommonConstants.CIRCLERADIUS, false);
            aux.setLinearVelocity(new Vector2(-10, 0));
            return aux;
        }
        /// <summary>
        /// It returns player state which indicates if a player won the game.
        /// </summary>
        public override int getPlayerState()
        {
            // Playing state by default.
            int state = 0;

            if (arrowsIndex == elements)
            {
                if (minimumCompleted())
                    state = 1;
                else
                    state = -1;
            }

            return state;
        }
        /// <summary>
        /// It verifies if a player has a minium of elements to win the game level.
        /// </summary>
        private bool minimumCompleted()
        {
            return elements == hits;
        }
        /// <summary>
        /// It sets statistics game on a string array to show it after finalize the game level. 
        /// </summary>
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
                    if (isArrowInArea())
                    {
                        change = getIndexAngle();
                        arrowCircle = makePhysicElement();
                        arrowsIndex++;
                        alpha = 1.0f;
                        scale = .25f;
                        wasItAHit = false;
                    }
                    else if (arrowCircle.getBody().Position.X * 30 >= width * 3 / 8 &&
                        arrowCircle.getBody().Position.X * 30 < width / 2 + width / 8)
                    {
                        alpha -= 0.04f;
                        if (isThereAHit())
                            scale += 0.02f;
                    }
                }

                if (arrowsIndex == elements)
                    verifyGameStatus();
            }
        }
        /// <summary>
        /// It checks if a player did a gesture like an arrow marks. 
        /// </summary>
        private bool isThereAHit()
        {
            int xA, yA;
            xA = Convert.ToInt16(arrowCircle.getBody().Position.X) * 30;
            yA = Convert.ToInt16(arrowCircle.getBody().Position.Y) * 30;

            Vector2 jointHandLeft = skeleton.getJointPosition(Microsoft.Kinect.JointType.HandLeft);
            Vector2 jointHandRight = skeleton.getJointPosition(Microsoft.Kinect.JointType.HandRight);

            return rectUp.Contains((int) jointHandLeft.X, (int) jointHandLeft.Y) && rectUp.Contains((int) jointHandRight.X, (int) jointHandRight.Y) && rectCenter.Contains(xA, yA) && change == 1
                || rectDown.Contains((int) jointHandLeft.X, (int) jointHandLeft.Y) && rectUp.Contains((int) jointHandRight.X, (int) jointHandRight.Y) && rectCenter.Contains(xA, yA) && change == 3
                || rectLeft.Contains((int) jointHandLeft.X, (int) jointHandLeft.Y) && rectCenter.Contains(xA, yA) && change == 0
                || rectRight.Contains((int) jointHandRight.X, (int) jointHandRight.Y) && rectCenter.Contains(xA, yA) && change == 2;
        }
        /// <summary>
        /// It checks if an arrow is in the area. 
        /// </summary>
        private bool isArrowInArea()
        {
            return arrowCircle.getBody().Position.X * 30 < width * 3 / 8 + width / 8;
        }
        /// <summary>
        /// It reloads the physic world and elements to replay the game level. 
        /// </summary>
        public override void restartStage()
        {
            loadWorld();
            elements = Convert.ToInt16(levels[currentLevel].getAttribute("elements"));
        }
        /// <summary>
        /// It loads next game level to play.
        /// </summary>
        public override void nextLevel()
        {
            if (levels.Count > currentLevel + 1)
                currentLevel++;
        }
        /// <summary>
        /// It recibes all player input from kinect, keyboard, etc.
        /// </summary>
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
        /// <summary>
        /// It displays player skeleton, kinect video, game statistics panel and game elements.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Apply effects to draw primitives.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Draw statistics panel.
            drawStatisticsPanel(spriteBatch, new String[] { hits + "/" + elements + " aciertos", (elements - arrowsIndex) + " restantes" });

            //Show camera if it is active.
            if (camera && video)
            {
                spriteBatch.Begin();
                KinectSDK.Instance.DrawVideoCam(spriteBatch, new Rectangle(width - 323, 3, 320, 240));
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

            skeleton.display(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);

        }
        /// <summary>
        /// It displays a rectangle on game play screen.
        /// </summary>
        private void drawRectangle(Rectangle rect, Color fillColor, Color edgeColor, int border, float alpha)
        {
            ElementPolygon rectPrimitive = new ElementPolygon(rect.Center.X - rect.Width / 2, rect.Center.Y - rect.Height / 2, rect.Width, rect.Height, fillColor, alpha, true);
            rectPrimitive.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            ElementPolygon rectEdgePrimitive = new ElementPolygon(rect.Center.X - rect.Width / 2, rect.Center.Y - rect.Height / 2, rect.Width, rect.Height, edgeColor, alpha, false);
            rectEdgePrimitive.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
        }
        /// <summary>
        /// It returns what kind of arrow appears on game play screen.
        /// </summary>
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
                String[] movements = (String[]) levels[currentLevel].getAttribute("move");

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
    }
}