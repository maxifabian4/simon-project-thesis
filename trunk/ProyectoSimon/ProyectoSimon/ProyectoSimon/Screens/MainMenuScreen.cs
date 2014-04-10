using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using ProyectoSimon.Utils;

namespace ProyectoSimon
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen()
            : base()
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry(CommonConstants.MENU_ENTRY_PLAY);
            MenuEntry statisticsMenuEntry = new MenuEntry(CommonConstants.MENU_ENTRY_STATISTICS);
            MenuEntry newUserMenuEntry = new MenuEntry(CommonConstants.MENU_ENTRY_NEW_USER);
            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            statisticsMenuEntry.Selected += StatisticsMenuEntrySelected;
            newUserMenuEntry.Selected += NewUserMenuEntrySelected;
            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(statisticsMenuEntry);
            MenuEntries.Add(newUserMenuEntry);
        }

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (KinectSDK.Instance.isConected())
            {
                if (DataManager.Instance.getIndexGame() == 0)
                {
                    GamePlayScreenCircles circleGame = new GamePlayScreenCircles(
                        ScreenManager.getWidthScreen(),
                        ScreenManager.getHeightScreen(),
                        DataManager.Instance.getGames()[CurrentGame].getLevels());
                    LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new BackgroundScreen(GameContentManager.TEXTURE_BACKGROUND_GAME2), circleGame);
                }
                else if (DataManager.Instance.getIndexGame() == 1)
                {
                    GamePlayScreenChooser selectorGame = new GamePlayScreenChooser(
                        ScreenManager.getWidthScreen(),
                        ScreenManager.getHeightScreen(),
                        DataManager.Instance.getGames()[CurrentGame].getLevels());
                    LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new BackgroundScreen(GameContentManager.TEXTURE_BACKGROUND_GAME), selectorGame);
                }
                else if (DataManager.Instance.getIndexGame() == 2)
                {
                    GamePlayScreenArrows arrowsGame = new GamePlayScreenArrows(
                        ScreenManager.getWidthScreen(),
                        ScreenManager.getHeightScreen(),
                        DataManager.Instance.getGames()[CurrentGame].getLevels());
                    LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new BackgroundScreen(GameContentManager.TEXTURE_BACKGROUND_ARROWS), arrowsGame);
                }
                else if (DataManager.Instance.getIndexGame() == 3)
                {
                    GamePlayScreenFree freeGame = new GamePlayScreenFree(
                        ScreenManager.getWidthScreen(),
                        ScreenManager.getHeightScreen(),
                        DataManager.Instance.getGames()[CurrentGame].getLevels());
                    LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new BackgroundScreen(GameContentManager.TEXTURE_BACKGROUND_GAME2), freeGame);
                }
            }
            else
            {
                showMessageToUser();
            }
        }

        private void showMessageToUser()
        {
            MessageBoxScreen kinectState = new MessageBoxScreen(CommonConstants.TITLE_MESSAGE_UNPLUGGED_KINECT, CommonConstants.DESCRIPTION_MESSAGE_UNPLUGGED_KINECT);
            kinectState.Accepted += ConfirmExitMessageKinectAccepted;
            ScreenManager.AddScreen(kinectState, null);
        }

        /// <summary>
        /// Listener implemented when the user accept the message box.
        /// </summary>
        void ConfirmExitMessageKinectAccepted(object sender, PlayerIndexEventArgs e)
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
            mainMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
            LoadingScreen.Load(ScreenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Listener implemented when the user select the "estadísticas" entry.
        /// </summary>
        void StatisticsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            string[] statisticsItems = DataManager.Instance.getCurrentStatisticsItems();
            int availableAreaX0, availableAreaX1;
            availableAreaX0 = CommonConstants.MARGIN_LEFT_MENU_PANEL + ScreenManager.getWidthScreen() / 4;
            availableAreaX1 = ScreenManager.getWidthScreen();
            StatisticsMenuScreen statisticsMenuScreen = new StatisticsMenuScreen(
                statisticsItems,
                availableAreaX0,
                availableAreaX1,
                ScreenManager.getWidthScreen(),
                ScreenManager.getHeightScreen());
            statisticsMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
            statisticsMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
            ScreenManager.AddScreen(statisticsMenuScreen, e.PlayerIndex);
        }

        /// <summary>
        /// Listener implemented when the user select the "nuevo usuario" entry.
        /// </summary>
        void NewUserMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int availableAreaX0, availableAreaX1;
            availableAreaX0 = CommonConstants.MARGIN_LEFT_MENU_PANEL + ScreenManager.getWidthScreen() / 4;
            availableAreaX1 = ScreenManager.getWidthScreen();
            UserFormMenuScreen userFormMenuScreen = new UserFormMenuScreen(
                availableAreaX0,
                availableAreaX1,
                ScreenManager.getWidthScreen(),
                ScreenManager.getHeightScreen());
            userFormMenuScreen.CurrentUser = CommonConstants.NEW_USER_INDEX;
            userFormMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
            ScreenManager.AddScreen(userFormMenuScreen, e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            string question = CommonConstants.EXIT_SYSTEM_QUESTION;
            string message = CommonConstants.EXIT_SYSTEM_ANSWER_OPTIONS;
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(question, message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "Desea salir de la aplicación?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }
    }
}