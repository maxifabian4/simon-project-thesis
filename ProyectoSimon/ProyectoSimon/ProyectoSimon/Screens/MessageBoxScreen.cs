//-----------------------------------------------------------------------------
// MessageBoxScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using C3.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProyectoSimon
{
    /// <summary>
    /// A popup message box screen, used to display "are you sure?"
    /// confirmation messages.
    /// </summary>
    class MessageBoxScreen : GameScreen
    {
        private string titleText;
        private string messageText;
        private InputAction menuSelect;
        private InputAction menuCancel;
        private Color colorMessageText;
        private Color panelColor;

        public event EventHandler<PlayerIndexEventArgs> Accepted;
        public event EventHandler<PlayerIndexEventArgs> Cancelled;

        /// <summary>
        /// Constructor automatically includes the standard "A=ok, B=cancel"
        /// usage text prompt.
        /// </summary>
        public MessageBoxScreen(string questionText, string message)
            : this(questionText, message, true)
        {
            //colorTitleText = new Color(191, 82, 28);
            colorMessageText = Color.White;
            panelColor = new Color(201, 47, 15);
        }

        /// <summary>
        /// Constructor lets the caller specify whether to include the standard
        /// "ACEPTAR=enter, CANCELAR=esc" usage text prompt.
        /// </summary>
        public MessageBoxScreen(string qtText, string msg, bool includeUsageText)
        {
            titleText = qtText;
            messageText = msg;
            IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(0.2);
            TransitionOffTime = TimeSpan.FromSeconds(0.2);

            menuSelect = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Enter },
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
            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                // Raise the accepted event, then exit the message box.
                if (Accepted != null)
                    Accepted(this, new PlayerIndexEventArgs(playerIndex));

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
            int wBox = screenManager.getWidthScreen();
            int hBox = screenManager.getHeightScreen();
            int x = 0;
            int y = 0;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            SpriteFont menuFont = screenManager.getFont(ScreenManager.USER_MODULE_FONT);

            // Make a background.
            drawMessageBox(spriteBatch, x, y, wBox, hBox);
            // Draw title.
            drawTextInBox(spriteBatch, menuFont, titleText, colorMessageText, x, wBox, hBox / 2 + 10);
            // Draw message.
            drawTextInBox(spriteBatch, menuFont, messageText, colorMessageText, x, wBox, hBox / 2 + (int)menuFont.MeasureString(titleText).Y + 20);
            base.Draw(gameTime);
        }

        /// <summary>
        /// Draws a rectagle to simulate a window.
        /// </summary>
        private void drawMessageBox(SpriteBatch spriteBatch, int x, int y, int wBox, int hBox)
        {
            Rectangle backgroundRectangle = new Rectangle(x, y, wBox, hBox);
            // Darken down any other screens that were drawn beneath the popup.
            screenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3, backgroundRectangle);
            ElementPolygon frame = new ElementPolygon(x, hBox / 2 - 20, wBox, 130, panelColor * TransitionAlpha, 1, true);
            frame.draw(screenManager);
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