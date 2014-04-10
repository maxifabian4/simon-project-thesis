//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2D.XNA;
using System.Collections.Generic;
using ProyectoSimon.Elements;
using Microsoft.Kinect;

namespace ProyectoSimon
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    abstract class GameplayScreen : GameScreen
    {
        protected float pauseAlpha;
        protected InputAction pauseAction;
        protected IList<Level> levels;
        protected IDictionary<Microsoft.Kinect.JointType, Vector2> jointsIDs;
        protected Statistics currentStatistics;
        protected TimeSpan timeSpan;
        protected int currentLevel;
        protected InputAction cameraKey, seatedMode, defaultMode;

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        /// 

        protected void generateInput()
        {
            pauseAction = new InputAction(
               new Buttons[] { },
               new Keys[] { Keys.Escape },
               true);
            cameraKey = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.RightShift, Keys.LeftShift },
                true);
            seatedMode = new InputAction(
               new Buttons[] { },
               new Keys[] { Keys.LeftAlt, Keys.RightAlt },
               true);

            defaultMode = new InputAction(
               new Buttons[] { },
               new Keys[] { Keys.LeftControl, Keys.RightControl },
               true);

        }
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
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                // TODO: this game isn't very fun! You could probably improve
                // it by inserting something more interesting in this space :-)
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");
          
            PlayerIndex player;
            if (pauseAction.Evaluate(input, ControllingPlayer, out player))
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
        }

        /// <summary>
        /// Create a Circle body an put it in the Box2D world.
        /// </summary>
        protected Body createCircle(Vector2 position, float radius, World physicsWorld)
        {
            var circle = new CircleShape();
            circle._radius = radius;

            var fd = new FixtureDef();
            fd.shape = circle;
            fd.restitution = 1.0f;
            fd.friction = 1.0f;
            fd.density = 10.0f;

            BodyDef bd = new BodyDef();
            bd.type = BodyType.Dynamic;
            bd.position = position;

            var body = physicsWorld.CreateBody(bd);
            body.CreateFixture(fd);
            MassData mass = new MassData();
            mass.center = new Vector2(0, 0);
            mass.mass = 20;
            mass.i = 100;
            body.SetMassData(ref mass);
            body.SetAngularDamping(0.0f);
            body.SetLinearDamping(0.0f);

            return body;
        }

        /// <summary>
        /// Draws the elements in the Box2D world.
        /// </summary>
        protected void drawElementsInTheWorld(List<GameElement> elements)
        {
            for (int i = 0; i < elements.Count; i++)
                elements[i].display(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
        }

        /// <summary>
        /// Prints a message box to the user.
        /// </summary>
        protected void showMessageState(bool won)
        {
            MessageStateScreen finishState = new MessageStateScreen(won, setCurrentStatistics());
            finishState.Cancelled += ConfirmExitMessageStateAccepted;
            finishState.AcceptedAgain += ConfirmExitMessageBoxCancelled;

            if (won)
                finishState.AcceptedNext += ConfirmNextMessageBoxAcepted;

            ScreenManager.AddScreen(finishState, null);
        }

        /// <summary>
        /// Listener implemented when the user accept the message box.
        /// </summary>
        void ConfirmExitMessageStateAccepted(object sender, PlayerIndexEventArgs e)
        {
            //stage.stopSong();
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
            mainMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
            LoadingScreen.Load(ScreenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Listener implemented when the user cancel the message box.
        /// </summary>
        void ConfirmExitMessageBoxCancelled(object sender, PlayerIndexEventArgs e)
        {
            ////stage.stopSong();
            restartStage();
            //playSong();
        }

        void ConfirmNextMessageBoxAcepted(object sender, PlayerIndexEventArgs e)
        {
            ////stage.stopSong();
            nextLevel();
            restartStage();
            //playSong();
        }

        /// <summary>
        /// Verifies the game status when the instance is updated.
        /// </summary>
        protected void verifyGameStatus()
        {
            // Cambiar a verify...
            int state = getPlayerState();

            if (state == 1)
            {
                currentStatistics.incWon();
                currentStatistics.setLevel(currentLevel);
                currentStatistics.incTime(timeSpan.Seconds);
                showMessageState(true);
            }
            else
                if (state == -1)
                {
                    currentStatistics.incLost();
                    if (levels[currentLevel].exist("time"))
                        currentStatistics.incTime(Convert.ToInt32(levels[currentLevel].getAttribute("time")) / 1000);
                    showMessageState(false);
                }

            if (state != 0)
                DataManager.Instance.AddStatistic(currentStatistics);
        }

        /// <summary>
        /// Provate method to draw a panel with the current statistics.
        /// </summary>
        protected void drawStatisticsPanel(SpriteBatch spriteBatch, String[] items) { 
            // Load font to draw.
            SpriteFont statisticsFont = GameContentManager.Instance.getFont(GameContentManager.FONT_STATISTICSFONT);
            Vector2 generalPosition = new Vector2(15, 10);
            // Draw panel.
            //quad = new ElementPolygon(5, 10, 170, 110, Color.Black, 0.5f, true);
            //quad.drawPrimitive(ScreenManager);
            //fillQuad = new ElementPolygon(5, 10, 170, 110, new Color(33, 33, 33), 1, false);
            //fillQuad.drawPrimitive(ScreenManager);

            spriteBatch.Begin();
            for (int i = 0; i < items.Length; i++) {
                // Draw text panel.
                spriteBatch.DrawString(statisticsFont, items[i], generalPosition + new Vector2(2, 2), Color.Black);
                spriteBatch.DrawString(statisticsFont, items[i], generalPosition, Color.White);
                generalPosition += new Vector2(0, 25);
            }
            spriteBatch.End();
        }

        /// <summary>
        /// Abstract methods that depends on the game instance.
        /// </summary>
        public abstract void restartStage();

        abstract public int getPlayerState();

        public abstract void nextLevel();

        public abstract string[] setCurrentStatistics();
    }
}
