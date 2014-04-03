using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using System;
using ProyectoSimon.Utils;

namespace ProyectoSimon
{
    /// <summary>
    /// This class allows display all statistical data stored in the system.
    /// </summary>
    class StatisticsMenuScreen : MenuScreen
    {
        private int availableAreaX0, availableAreaX1;
        private int x, y, w, h;
        private int wScreen;
        private int hScreen;
        private MenuEntry salirEntry;
        private Color statisticsFrameColor;
        private Color statisticsDataColor;
        private Color titleColor;

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
            salirEntry = new MenuEntry(CommonConstants.MENU_ENTRY_EXIT);
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
            mainMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
            mainMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Draws the frame for the Statistic screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteFont titleFont = GameContentManager.Instance.getFont(GameContentManager.GAME_INSTANCE_FONT);
            SpriteFont dataFont = GameContentManager.Instance.getFont(GameContentManager.USER_MODULE_FONT);
            string title = CommonConstants.STATISTIC_SCREEN_TITLE;
            int horizontalValue = (int) titleFont.MeasureString(title).X;
            int verticalValue = (int) titleFont.MeasureString(title).Y;
            // Draw statistics frame.
            drawStatisticsFrame(verticalValue);
            // Draw tittle.
            drawTitleFrame(title, screenManager.SpriteBatch, titleFont, horizontalValue);
            // Draw statistics data.
            drawStatisticsData(screenManager.SpriteBatch, dataFont, verticalValue, DataManager.Instance.getCurrentStatisticsItems());
        }

        /// <summary>
        /// Draws the statistics' attributes.
        /// </summary>
        private void drawStatisticsData(SpriteBatch spriteBatch, SpriteFont statisticsFont, int verticalMeasure, string[] data)
        {
            int xIni, yIni, xFin;
            string name, value;
            string[] currentInformation;
            xIni = x + 20;
            xFin = x + w - 30;
            yIni = y + verticalMeasure + 30;
            spriteBatch.Begin();

            for (int i = 0; i < data.Length; i++)
            {
                currentInformation = data[i].Split(CommonConstants.STATISTICAL_MEASURES_SEPARATOR);
                name = CommonUtilMethods.GetStatisticsName(currentInformation);
                value = CommonUtilMethods.GetStatisticsValue(currentInformation);
                xFin -= (int) statisticsFont.MeasureString(value).X;
                spriteBatch.DrawString(statisticsFont, name, new Vector2(xIni, yIni + i * 30), statisticsDataColor * TransitionAlpha);
                spriteBatch.DrawString(statisticsFont, value, new Vector2(xFin, yIni + i * 30), statisticsDataColor * TransitionAlpha);
                xFin = x + w - 30;
            }

            spriteBatch.End();
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
            ElementPolygon frame = new ElementPolygon(x, hScreen / 4, w, hScreen / 4 + 50, statisticsFrameColor * TransitionAlpha, 1, true);
            frame.draw(screenManager);
            ElementPolygon titleFrame = new ElementPolygon(x, hScreen / 4, w, verticalValue + 10, MenuPanelColor * TransitionAlpha, 1, true);
            titleFrame.draw(screenManager);
        }
    }
}
