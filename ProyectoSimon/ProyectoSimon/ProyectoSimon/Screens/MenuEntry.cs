//-----------------------------------------------------------------------------
// MenuEntry.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    class MenuEntry
    {
        /// <summary>
        /// The text rendered for this entry.
        /// </summary>
        private string text;
        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float selectionFade;
        /// <summary>
        /// The position at which the entry is drawn. This is set by the MenuScreen
        /// each frame in Update.
        /// </summary>
        private Vector2 position;
        // Selected box color.
        private Color selectedBoxColor;

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        /// <summary>
        /// Gets or sets the position at which to draw this menu entry.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<PlayerIndexEventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry(PlayerIndex playerIndex)
        {
            if (Selected != null)
                Selected(this, new PlayerIndexEventArgs(playerIndex));
        }

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text)
        {
            this.text = text;
            selectedBoxColor = new Color(249, 88, 57);
        }

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(MenuScreen screen, bool isSelected, GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }

        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(MenuScreen screen, bool isSelected, GameTime gameTime, SpriteFont font, int originX, int destineX, float valueTransition)
        {
            // Draw text, centered on the middle of each line.
            ScreenManager screenManager = screen.getScreenManager();
            SpriteBatch spriteBatch = screenManager.SpriteBatch;

            // Draw the box.
            if (isSelected)
                drawBoxRectangle(spriteBatch, screenManager, originX, destineX, font, valueTransition, screen);
            // Draw the menu entry.
            drawTextEntry(spriteBatch, isSelected, font, screen.TransitionAlpha, gameTime);
        }

        private void drawTextEntry(SpriteBatch spriteBatch, bool isSelected, SpriteFont font, float transitionAlpha, GameTime gameTime)
        {
            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;
            float pulsate = (float)Math.Sin(time * 6) + 1;
            float scale = 1 + pulsate * 0.05f * selectionFade;
            Vector2 originText = new Vector2(0, font.LineSpacing / 2);
            Color colorText = isSelected ? Color.White : new Color(150, 150, 150);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, text, position, colorText * transitionAlpha, 0, originText, scale, SpriteEffects.None, 0);
            spriteBatch.End();
        }

        private void drawBoxRectangle(SpriteBatch spriteBatch, ScreenManager screenManager, int originX, int destineX, SpriteFont font, float valueTransition, MenuScreen screen)
        {
            // Rectangle box.
            int originY, destineY;
            originY = (int)(position.Y - font.MeasureString(text).Y / 2);
            destineY = (int)(position.Y + font.MeasureString(text).Y / 2);
            ElementPolygon box = new ElementPolygon(originX, originY, destineX, destineY - originY + 5, selectedBoxColor, 1, true);
            box.drawPrimitive(screenManager);
            //ElementPolygon boxEdge = new ElementPolygon(originX + 20, originY, destineX - 40, destineY - originY + 5, Color.White, 0.5f, false);
            //boxEdge.drawPrimitive(screenManager);
        }

        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(MenuScreen screen)
        {
            return screen.getScreenManager().getFont(ScreenManager.USER_MODULE_FONT).LineSpacing;
        }

        /// <summary>
        /// Queries how wide the entry is, used for centering on the screen.
        /// </summary>
        public virtual int GetWidth(MenuScreen screen)
        {
            return (int)screen.getScreenManager().getFont(ScreenManager.USER_MODULE_FONT).MeasureString(Text).X;
        }
    }
}