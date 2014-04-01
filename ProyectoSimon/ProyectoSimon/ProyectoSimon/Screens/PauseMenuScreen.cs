using Microsoft.Xna.Framework;
using ProyectoSimon.Utils;

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
            MenuEntry resumeGameMenuEntry = new MenuEntry(CommonConstants.MENU_ENTRY_RESUME);
            MenuEntry quitGameMenuEntry = new MenuEntry(CommonConstants.MENU_ENTRY_EXIT_GAME);
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
            MainMenuScreen = false;
            base.HandleInput(gameTime, input);
        }

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            string question = CommonConstants.EXIT_GAME_QUESTION;
            string message = CommonConstants.EXIT_GAME_ANSWER_OPTIONS;
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
            mainMenuScreen.CurrentUser = screenManager.getUserIndex();
            mainMenuScreen.CurrentGame = screenManager.getIndexGame();
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }
    }
}