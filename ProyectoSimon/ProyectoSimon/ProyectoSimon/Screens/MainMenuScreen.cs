//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

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
            MenuEntry playGameMenuEntry = new MenuEntry("jugar");
            MenuEntry statisticsMenuEntry = new MenuEntry("estadísticas");
            MenuEntry newUserMenuEntry = new MenuEntry("nuevo usuario");
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
            if (screenManager.Kinect.isConected())
            {
                if (screenManager.getIndexGame() == 0)
                {
                    GamePlayScreenCirculos circleGame = new GamePlayScreenCirculos(screenManager.getWidthScreen(), screenManager.getHeightScreen(), screenManager.getGames()[currentGame].getLevels());
                    LoadingScreen.Load(screenManager, true, e.PlayerIndex, new BackgroundScreen(ScreenManager.TEXTURE_BACKGROUND_GAME2), circleGame);
                }
                else if (screenManager.getIndexGame() == 1)
                {
                    GamePlayScreenSeleccionador selectorGame = new GamePlayScreenSeleccionador(screenManager.getWidthScreen(), screenManager.getHeightScreen(), screenManager.getGames()[currentGame].getLevels());
                    LoadingScreen.Load(screenManager, true, e.PlayerIndex, new BackgroundScreen(ScreenManager.TEXTURE_BACKGROUND_GAME), selectorGame);
                }
                else if (screenManager.getIndexGame() == 2)
                {
                    GamePlayScreenFlechas arrowsGame = new GamePlayScreenFlechas(screenManager.getWidthScreen(), screenManager.getHeightScreen(), screenManager.getGames()[currentGame].getLevels());
                    LoadingScreen.Load(screenManager, true, e.PlayerIndex, new BackgroundScreen(ScreenManager.TEXTURE_BACKGROUND_ARROWS), arrowsGame);
                }
                else if (screenManager.getIndexGame() == 3)
                {
                    GamePlayScreenLibre freeGame = new GamePlayScreenLibre(screenManager.getWidthScreen(), screenManager.getHeightScreen(), screenManager.getGames()[currentGame].getLevels());
                    LoadingScreen.Load(screenManager, true, e.PlayerIndex, new BackgroundScreen(ScreenManager.TEXTURE_BACKGROUND_GAME2), freeGame);
                }                
            }
            else 
            {
                showMessageToUser();
            }
        }

        private void showMessageToUser()
        {
            MessageBoxScreen kinectState = new MessageBoxScreen("Dispositivo Kinect desconectado !!", "Presione Enter para regresar al menú principal.");
            kinectState.Accepted += ConfirmExitMessageKinectAccepted;
            screenManager.AddScreen(kinectState, null);
        }

        /// <summary>
        /// Listener implemented when the user accept the message box.
        /// </summary>
        void ConfirmExitMessageKinectAccepted(object sender, PlayerIndexEventArgs e)
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.setCurrentUser(screenManager.getUserIndex());
            mainMenuScreen.setCurrentGame(screenManager.getIndexGame());
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Listener implemented when the user select the "estadísticas" entry.
        /// </summary>
        void StatisticsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            string[] statisticsItems = screenManager.getCurrentStatisticsItems();
            int availableAreaX0, availableAreaX1;
            availableAreaX0 = MARGIN_LEFT_MENU_PANEL + screenManager.getWidthScreen() / 4;
            availableAreaX1 = screenManager.getWidthScreen();
            StatisticsMenuScreen statisticsMenuScreen = new StatisticsMenuScreen(statisticsItems, availableAreaX0, availableAreaX1, screenManager.getWidthScreen(), screenManager.getHeightScreen());
            statisticsMenuScreen.setCurrentUser(screenManager.getUserIndex());
            statisticsMenuScreen.setCurrentGame(screenManager.getIndexGame());
            screenManager.AddScreen(statisticsMenuScreen, e.PlayerIndex);
        }

        /// <summary>
        /// Listener implemented when the user select the "nuevo usuario" entry.
        /// </summary>
        void NewUserMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            int availableAreaX0, availableAreaX1;
            availableAreaX0 = MARGIN_LEFT_MENU_PANEL + screenManager.getWidthScreen() / 4;
            availableAreaX1 = screenManager.getWidthScreen();
            UserFormMenuScreen userFormMenuScreen = new UserFormMenuScreen(availableAreaX0, availableAreaX1, screenManager.getWidthScreen(), screenManager.getHeightScreen());
            userFormMenuScreen.setCurrentUser(-1);
            userFormMenuScreen.setCurrentGame(screenManager.getIndexGame());
            screenManager.AddScreen(userFormMenuScreen, e.PlayerIndex);
        }

        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            string question = "Desea salir del sistema?";
            string message = "aceptar (enter)       cancelar (esc)";
            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(question, message);
            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
            screenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "Desea salir de la aplicación?" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            screenManager.Game.Exit();
        }
    }
}