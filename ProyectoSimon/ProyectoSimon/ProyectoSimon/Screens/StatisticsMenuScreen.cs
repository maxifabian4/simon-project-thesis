#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using System;
#endregion

namespace ProyectoSimon
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class StatisticsMenuScreen : MenuScreen
    {
        private int availableAreaX0, availableAreaX1;
        private int x, y, w, h;
        private int wScreen, hScreen;
        MenuEntry salirEntry;
        private Color statisticsFrameColor, statisticsDataColor, titleColor;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StatisticsMenuScreen(string[] dt, int a0, int b1, int wS, int hS)
            : base()
        {
            availableAreaX0 = a0;
            availableAreaX1 = b1;
            x = (availableAreaX1 - availableAreaX0) / 4 + availableAreaX0;
            y = hS / 4;
            w = (availableAreaX1 - availableAreaX0) / 2;
            h = 2 * y;
            wScreen = wS;
            hScreen = hS;
            statisticsFrameColor = titleColor = Color.White;
            statisticsDataColor = new Color(64, 64, 64);
            // Create our menu entries.
            salirEntry = new MenuEntry("Salir");
            // Hook up menu event handlers.
            salirEntry.Selected += SalirEntrySelected;
            // Add entries to the menu.
            MenuEntries.Add(salirEntry);
        }

        /// <summary>
        /// Event handler for when the Ungulate menu entry is selected.
        /// </summary>
        void SalirEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.setCurrentUser(screenManager.getUserIndex());
            mainMenuScreen.setCurrentGame(screenManager.getIndexGame());
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Draws the frame for the Statistic screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteFont titleFont = screenManager.getFont(ScreenManager.GAME_INSTANCE_FONT);
            SpriteFont dataFont = screenManager.getFont(ScreenManager.USER_MODULE_FONT);
            string title = "estadísticas";
            int horizontalValue = (int) titleFont.MeasureString(title).X;
            int verticalValue = (int) titleFont.MeasureString(title).Y;
            // Draw statistics frame.
            drawStatisticsFrame(verticalValue);
            // Draw tittle.
            drawTitleFrame(title, screenManager.SpriteBatch, titleFont, horizontalValue);
            // Draw statistics data.
            drawStatisticsData(screenManager.SpriteBatch, dataFont, verticalValue, screenManager.getCurrentStatisticsItems());
        }

        /// <summary>
        /// Draws the statistics' attributes.
        /// </summary>
        private void drawStatisticsData(SpriteBatch spriteBatch, SpriteFont statisticsFont, int verticalMeasure, string[] data)
        {
            int xIni, yIni, xFin;
            string name, value;
            xIni = x + 20;
            xFin = x + w - 30;
            yIni = y + verticalMeasure + 30;
            spriteBatch.Begin();

            for (int i = 0; i < data.Length; i++)
            {
                name = getNameStatistics(data[i].Split(' '));
                value = getValueStatistics(data[i].Split(' '));
                xFin -= (int) statisticsFont.MeasureString(value).X;
                spriteBatch.DrawString(statisticsFont, name, new Vector2(xIni, yIni + i * 30), statisticsDataColor * TransitionAlpha);
                spriteBatch.DrawString(statisticsFont, value, new Vector2(xFin, yIni + i * 30), statisticsDataColor * TransitionAlpha);
                xFin = x + w - 30;
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Return the value of an attribute.
        /// </summary>
        private string getValueStatistics(string[] p)
        {
            return p[p.Length - 1];
        }

        /// <summary>
        /// Return the name for an attribute.
        /// </summary>
        private string getNameStatistics(string[] p)
        {
            string name = p[0];

            for (int i = 1; i < p.Length; i++)
                if (i != p.Length - 1)
                    name += " " + p[i];

            return name;
        }

        /// <summary>
        /// Draws the title for the frame.
        /// </summary>
        private void drawTitleFrame(string title, SpriteBatch spriteBatch, SpriteFont statisticsFont, int horizontalValue)
        {
            int middleValue = horizontalValue / 2;
            spriteBatch.Begin();
            spriteBatch.DrawString(statisticsFont, title, new Vector2(x + w / 2 - middleValue, hScreen / 4 + 5), titleColor * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws a simple frame for the Statistics screen.
        /// </summary>
        private void drawStatisticsFrame(int verticalValue)
        {
            //ElementPolygon frameShadow = new ElementPolygon(x + 5, hScreen / 4 + 5, w, hScreen / 4 + 50, new Color(33, 33, 33) * TransitionAlpha, .25f, true);
            //frameShadow.drawPrimitive(screenManager);
            ElementPolygon frame = new ElementPolygon(x, hScreen / 4, w, hScreen / 4 + 50, statisticsFrameColor * TransitionAlpha, 1, true);
            frame.drawPrimitive(screenManager);
            ElementPolygon titleFrame = new ElementPolygon(x, hScreen / 4, w, verticalValue + 10, panelColor * TransitionAlpha, 1, true);
            titleFrame.drawPrimitive(screenManager);
        }
    }
}
