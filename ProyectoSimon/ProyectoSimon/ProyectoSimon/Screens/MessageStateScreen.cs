//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Box2D.XNA;

namespace ProyectoSimon
{
    /// <summary>
    /// A popup message state screen, used to display information about game state.
    /// </summary>
    class MessageStateScreen : GameScreen
    {
        private InputAction menuNext;
        private InputAction menuAgain;
        private InputAction menuCancel;
        private Color colorTitleText, colorFrameText;
        private Color colorTitleBox, colorFrameBox;
        private string[] finalStatistics;
        private bool won;

        public event EventHandler<PlayerIndexEventArgs> AcceptedNext;
        public event EventHandler<PlayerIndexEventArgs> AcceptedAgain;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        /// <summary>
        /// Constructor automatically includes the standard "A = continuar, B = volver a jugar, C = menú principal"
        /// usage text prompt.
        /// </summary>
        public MessageStateScreen(bool won, string[] fStat)
        {
            finalStatistics = fStat;
            this.won = won;

            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);
            colorTitleBox = new Color(201, 47, 15);
            colorFrameBox = Color.White;
            colorTitleText = Color.White;
            colorFrameText = new Color(64, 64, 64);

            menuNext = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Enter },
                true);
            menuAgain = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Back },
                true);
            menuCancel = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Escape },
                true);
        }

        /// <summary>
        /// Loads graphics content for this screen. This uses the shared ContentManager
        /// provided by the Game class, so the content will remain loaded forever.
        /// Whenever a subsequent MessageBoxScreen tries to load this same content,
        /// it will just get back another reference to the already loaded data.
        /// </summary>
        public override void Activate(bool instancePreserved) { }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            PlayerIndex playerIndex;

            // We pass in our ControllingPlayer, which may either be null (to
            // accept input from any player) or a specific index. If we pass a null
            // controlling player, the InputState helper returns to us which player
            // actually provided the input. We pass that through to our Accepted and
            // Cancelled events, so they can tell which player triggered them.
            if (menuNext.Evaluate(input, ControllingPlayer, out playerIndex) && won)
            {
                // Raise the accepted event (next), then exit the message box.
                if (AcceptedNext != null)
                    AcceptedNext(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (menuAgain.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event (again), then exit the message box.
                if (AcceptedAgain != null)
                    AcceptedAgain(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Raise the cancelled event, then exit the message box.
                if (Cancelled != null)
                    Cancelled(this, new PlayerIndexEventArgs(playerIndex));

                ExitScreen();
            }
        }

        /// <summary>
        /// Draws the message box.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Initialize parameters.
            int wBox = 400;
            int hBox = 300;
            int x = (ScreenManager.getWidthScreen() - wBox) / 2;
            int y = (ScreenManager.getHeightScreen() - hBox) / 2;
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont menuFont = GameContentManager.Instance.getFont(GameContentManager.FONT_STATISTICSFONT);
            string titleText = "Resultados del juego";
            int verticalValue = (int)menuFont.MeasureString(titleText).Y;
            
            // Make a background.
            drawMessageBox(x, y, wBox, hBox, verticalValue);
            // Draw title.
            drawTextInBox(spriteBatch, menuFont, titleText, colorTitleText, x, x + wBox, y + 10);
            // Draw header line.
            //drawLine(ScreenManager.SpriteBatch, new Vector2(x + 10, y + verticalValue + 15), new Vector2(x + wBox - 10, y + verticalValue + 15));
            // Draw footer line.
            //drawLine(ScreenManager.SpriteBatch, new Vector2(x + 10, y + hBox - 50), new Vector2(x + wBox - 10, y + hBox - 50));
            // Draw statistics elements.
            drawStatisticsData(ScreenManager.SpriteBatch, menuFont, verticalValue, x, y, wBox);
            // Draw "buttons" in the box.
            drawButtonsInBox(spriteBatch, menuFont, colorFrameText, x + wBox, y + hBox - 40);
        }

        private void drawButtonsInBox(SpriteBatch spriteBatch, SpriteFont menuFont, Color colorText, int xFin, int y)
        {
            colorText *= TransitionAlpha;
            string buttonTextNext = "> (Enter)";
            string buttonTextAgain = "@ (Return)";
            string buttonTextCancel = "< (Esc)";

            float textNextSizeX = menuFont.MeasureString(buttonTextNext).X / 2;
            float textAgainSizeX = menuFont.MeasureString(buttonTextAgain + buttonTextNext).X / 2;
            float textCancelSizeX = menuFont.MeasureString(buttonTextCancel + buttonTextAgain + buttonTextNext).X / 2;

            Vector2 textPositionNext = new Vector2(xFin - textNextSizeX - 60, y);
            Vector2 textPositionAgain = new Vector2(textPositionNext.X - textAgainSizeX - 30, y);
            Vector2 textPositionCancel = new Vector2(textPositionAgain.X - textCancelSizeX + 30, y);

            spriteBatch.Begin();
            
            if (!won)
                spriteBatch.DrawString(menuFont, buttonTextNext, textPositionNext, new Color(188, 188, 188));
            else
                spriteBatch.DrawString(menuFont, buttonTextNext, textPositionNext, colorText);

            spriteBatch.DrawString(menuFont, buttonTextAgain, textPositionAgain, colorText);
            spriteBatch.DrawString(menuFont, buttonTextCancel, textPositionCancel, colorText);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the statistics' attributes.
        /// </summary>
        private void drawStatisticsData(SpriteBatch spriteBatch, SpriteFont statisticsFont, int verticalMeasure, int x, int y, int wBox)
        {
            int xIni, yIni, xFin;
            string name, value;
            xIni = x + 20;
            xFin = x + wBox - 35;
            yIni = y + verticalMeasure + 30;
            spriteBatch.Begin();

            for (int i = 0; i < finalStatistics.Length; i++)
            {
                name = finalStatistics[i].Split('|')[0];
                value = finalStatistics[i].Split('|')[1];
                xFin -= (int)statisticsFont.MeasureString(value).X;
                spriteBatch.DrawString(statisticsFont, name, new Vector2(xIni, yIni + i * 30), colorFrameText * TransitionAlpha);
                spriteBatch.DrawString(statisticsFont, value, new Vector2(xFin, yIni + i * 30), colorFrameText * TransitionAlpha);
                xFin = x + wBox - 35;
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draw a line between v1 and v2. METERLO EN UNA CLASE ESTATICA ????
        /// </summary>
        //private void drawLine(SpriteBatch spriteBatch, Vector2 v1, Vector2 v2)
        //{
        //    FixedArray8<Vector2> vertexs = new FixedArray8<Vector2>();
        //    vertexs[0] = new Vector2(v1.X, v1.Y);
        //    vertexs[1] = new Vector2(v2.X, v2.Y);

        //    ElementPolygon line = new ElementPolygon(vertexs, Color.White * TransitionAlpha, .6f, false, 2);
        //    line.drawPrimitive(ScreenManager);
        //}

        /// <summary>
        /// Draws a rectagle to simulate a window.
        /// </summary>
        private void drawMessageBox(int x, int y, int wBox, int hBox, int verticalValue)
        {
            /// ???????????? spriteBatch
            Rectangle backgroundRectangle = new Rectangle(0, 0, ScreenManager.getWidthScreen(), ScreenManager.getHeightScreen());
            // Darken down any other screens that were drawn beneath the popup.
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3, backgroundRectangle);
            ElementPolygon frame = new ElementPolygon(x, y, wBox, hBox, colorFrameBox * TransitionAlpha, 1, true);
            frame.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
            ElementPolygon titleFrame = new ElementPolygon(x, y, wBox, verticalValue + 20, colorTitleBox * TransitionAlpha, 1, true);
            titleFrame.draw(ScreenManager.SpriteBatch, ScreenManager.BasicEffect);
        }

        /// <summary>
        /// Draws the message in the message box.
        /// </summary>
        private void drawTextInBox(SpriteBatch spriteBatch, SpriteFont menuFont, string text, Color color, int xIni, int xFin, int y)
        {
            color *= TransitionAlpha;
            float textSizeX = menuFont.MeasureString(text).X / 2;
            Vector2 textPosition = new Vector2(xIni + (xFin - xIni) / 2 - textSizeX, y);

            spriteBatch.Begin();
            // Draw the message box text.
            spriteBatch.DrawString(menuFont, text, textPosition, color);
            spriteBatch.End();
        }
    }
}