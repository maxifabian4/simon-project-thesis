//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;

namespace ProyectoSimon
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen()
            : base()
        {
            // Create our menu entries.
            MenuEntry resumeGameMenuEntry = new MenuEntry("reanudar");
            MenuEntry quitGameMenuEntry = new MenuEntry("salir del juego");
            // Hook up menu event handlers.
            resumeGameMenuEntry.Selected += OnCancel;
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
            // Add entries to the menu.
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(quitGameMenuEntry);
        }

        /// <summary>
        /// Allows to handle the input.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            mainMenuScreen = false;
            base.HandleInput(gameTime, input);
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            string question = "Desea salir del juego?";
            string message = "ACEPTAR = enter CANCELAR = esc";
            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(question, message);
            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
            screenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.setCurrentUser(screenManager.getUserIndex());
            mainMenuScreen.setCurrentGame(screenManager.getIndexGame());
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }
    }
}