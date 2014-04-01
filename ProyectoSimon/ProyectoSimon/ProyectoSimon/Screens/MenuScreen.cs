using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using Box2D.XNA;
using ProyectoSimon.Utils;

namespace ProyectoSimon
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        // Menu entry list.
        private List<MenuEntry> menuEntries;
        // Used to track the current entry in the menu.
        private int selectedEntry;
        // Global positions for slide format.
        private float valueTransition;
        // Input parameters.
        private InputAction menuLeft;
        private InputAction menuRight;
        private InputAction menuUp;
        private InputAction menuDown;
        private InputAction menuSelect;
        private InputAction menuCancel;
        private InputAction menuCtrl;
        private InputAction menuDel;
        // Current game play instance.
        private int currentGame;
        // Current user.
        private int currentUser;
        // Determines if we are in main menu screen.
        private bool mainMenuScreen;
        // Determines the menu panel color.
        private Color menuPanelColor;

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get
            {
                return menuEntries;
            }
        }

        /// <summary>
        /// Gets the input action associated to the left key movement.
        /// </summary>
        protected InputAction MenuLeft
        {
            get
            {
                return menuLeft;
            }
        }

        /// <summary>
        /// Gets the input action associated to the right key movement.
        /// </summary>
        protected InputAction MenuRight
        {
            get
            {
                return menuRight;
            }
        }

        /// <summary>
        /// Gets the main menu value flag.
        /// </summary>
        protected bool MainMenuScreen
        {
            get
            {
                return mainMenuScreen;
            }
            set
            {
                mainMenuScreen = value;
            }
        }

        /// <summary>
        /// Gets the current user index.
        /// </summary>
        public int CurrentUser
        {
            get
            {
                return currentUser;
            }
            set
            {
                currentUser = value;
            }
        }

        /// <summary>
        /// Gets the current game index.
        /// </summary>
        public int CurrentGame
        {
            get
            {
                return currentGame;
            }
            set
            {
                currentGame = value;
            }
        }

        /// <summary>
        /// Gets the menu panel color.
        /// </summary>
        public Color MenuPanelColor
        {
            get
            {
                return menuPanelColor;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen()
        {
            menuEntries = new List<MenuEntry>();
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            menuUp = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Up },
                true);
            menuDown = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Down },
                true);
            menuSelect = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Enter },
                true);
            menuCancel = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Escape },
                true);
            menuLeft = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Left },
                true);
            menuRight = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Right },
                true);
            menuCtrl = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.LeftControl, Keys.RightControl },
                true);
            menuDel = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Delete },
                true);

            // Initialize the first entry.
            selectedEntry = 0;
            // Initialize the current user.
            CurrentUser = 0;
            // Initialize the current instance game.
            CurrentGame = 0;
            // We are in main menu screen.
            mainMenuScreen = true;
            // Initialize the main color for the menu panel.
            menuPanelColor = new Color(201, 47, 15);
        }

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            // For input tests we pass in our ControllingPlayer, which may
            // either be null (to accept input from any player) or a specific index.
            // If we pass a null controlling player, the InputState helper returns to
            // us which player actually provided the input. We pass that through to
            // OnSelectEntry and OnCancel, so they can tell which player triggered them.
            PlayerIndex playerIndex;

            // Move to the previous menu entry?
            if (menuUp.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry--;

                if (selectedEntry < 0)
                {
                    selectedEntry = menuEntries.Count - 1;
                }
            }

            // Move to the next menu entry?
            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                {
                    selectedEntry = 0;
                }
            }

            // An specific entry has been selected? ...
            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            // ... or cancel the selection?
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
            else if (mainMenuScreen && (input.IsKeyPressed(Keys.LeftControl, ControllingPlayer, out playerIndex)
                || input.IsKeyPressed(Keys.RightControl, ControllingPlayer, out playerIndex)))
            {
                // has been (Ctrl + ->) combination pressed? If so, change to the next game.
                if (menuRight.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    if (CurrentGame == screenManager.getGames().Length - 1)
                        CurrentGame = 0;
                    else
                        CurrentGame++;
                    screenManager.setCurrentGame(CurrentGame);
                }
                // has been (Ctrl + <-) combination pressed? If so, change to the previous game.
                else if (menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    if (CurrentGame == 0)
                        CurrentGame = screenManager.getGames().Length - 1;
                    else
                        CurrentGame--;
                    screenManager.setCurrentGame(CurrentGame);
                }
            }
            // if right arrow key has been pressed, change to the previous user.
            else if (mainMenuScreen && menuRight.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (CurrentUser == screenManager.getUsers().Count - 1)
                    CurrentUser = 0;
                else
                    CurrentUser++;
                screenManager.setUserIndex(CurrentUser);
            }
            // if left arrow key has been pressed, change to the previous user.
            else if (mainMenuScreen && menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (CurrentUser == 0)
                    CurrentUser = screenManager.getUsersCount() - 1;
                else
                    CurrentUser--;
                screenManager.setUserIndex(CurrentUser);
            }
            // Remove a selected user from the system. We need to display a message in order to confirm the transaction.
            else if (mainMenuScreen && menuDel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                screenManager.deleteUser(CurrentUser);
                screenManager.storeUsersToXml();
                CurrentUser = 0;
            }
        }

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex, PlayerIndex playerIndex)
        {
            menuEntries[entryIndex].OnSelectEntry(playerIndex);
        }

        /// <summary>
        /// Handler for when the user has cancelled the menu.
        /// </summary>
        protected virtual void OnCancel(PlayerIndex playerIndex)
        {
            ExitScreen();
        }

        /// <summary>
        /// Helper overload makes it easy to use OnCancel as a MenuEntry event handler.
        /// </summary>
        protected void OnCancel(object sender, PlayerIndexEventArgs e)
        {
            OnCancel(e.PlayerIndex);
        }

        /// <summary>
        /// Allows the screen the chance to position the menu entries. By default
        /// all menu entries are lined up in a vertical list, centered on the screen.
        /// </summary>
        protected virtual void UpdateMenuEntryLocations(int widthScreen, int heightScreen)
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            SpriteFont sansSerif15 = screenManager.getFont(ScreenManager.USER_MODULE_FONT);
            MenuEntry menuEntry;
            Vector2 position = new Vector2(0, heightScreen - CommonConstants.MARGIN_DOWN_MENU_ENTRIES);
            float transitionOffset = (float) Math.Pow(TransitionPosition, 2);

            // Update each menu entry's location in turn
            for (int i = menuEntries.Count - 1; i >= 0; i--)
            {
                menuEntry = menuEntries[i];
                // Each entry is to be centered horizontally inside de panel menu.
                position.X = (widthScreen / 8 - (int) sansSerif15.MeasureString(menuEntry.Text).X / 2) + CommonConstants.MARGIN_LEFT_MENU_PANEL;

                if (ScreenState == ScreenState.TransitionOn)
                {
                    position.X -= transitionOffset * (widthScreen / 8);
                    valueTransition = (-1) * transitionOffset * (widthScreen / 8);
                }
                else
                {
                    position.X += transitionOffset * (widthScreen / 8);
                    valueTransition = transitionOffset * (widthScreen / 8);
                }

                // Set the entry's position.
                menuEntry.Position = position;
                // Move up for the next entry the size of this entry.
                position.Y -= (int) sansSerif15.MeasureString(menuEntry.Text).Y + CommonConstants.MARGIN_ALIGN_MENU_ENTRIES;
            }
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            bool isSelected;

            // Update each nested MenuEntry object.
            for (int i = menuEntries.Count - 1; i >= 0; i--)
            {
                isSelected = IsActive && (i == selectedEntry);
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // Width value of screen.
            int widthScreen = screenManager.getWidthScreen();
            // Height value of screen.
            int heightScreen = screenManager.getHeightScreen();
            GraphicsDevice graphics = screenManager.GraphicsDevice;
            SpriteBatch spriteBatch = screenManager.SpriteBatch;
            // Gets fonts.
            SpriteFont gameInstanceFont = screenManager.getFont(ScreenManager.GAME_INSTANCE_FONT);
            SpriteFont userModuleFont = screenManager.getFont(ScreenManager.USER_MODULE_FONT);
            SpriteFont fontBeautiful = screenManager.getFont(ScreenManager.FONT_BEAUTIFULEVERYTIME);
            SpriteFont fontBeautifulProject = screenManager.getFont(ScreenManager.FONT_BEAUTIFULEVERYTIMEPROJECT);
            // Make sure our entries are in the right place before we draw them.
            UpdateMenuEntryLocations(widthScreen, heightScreen);
            // Draw background.
            DrawBackground(spriteBatch, widthScreen, heightScreen);
            // Create an rectangle on the left.
            DrawMenuPanel(screenManager, widthScreen, heightScreen);
            // Draw game instance name.
            DrawGameInstanceModule(spriteBatch, gameInstanceFont, widthScreen);
            // Draw user module.
            DrawUserModule(spriteBatch, userModuleFont, heightScreen);
            // Draw each menu entry in turn.
            DrawMenuEntries(spriteBatch, gameTime, widthScreen, userModuleFont);
            // Draw proyect name.
            DrawProjectName(spriteBatch, fontBeautiful, fontBeautifulProject, widthScreen);
        }

        /// <summary>
        /// Draws the menu entries.
        /// </summary>
        private void DrawMenuEntries(SpriteBatch spriteBatch, GameTime gameTime, int widthScreen, SpriteFont font)
        {
            MenuEntry menuEntry;

            for (int i = menuEntries.Count - 1; i >= 0; i--)
            {
                menuEntry = menuEntries[i];
                menuEntry.Draw(
                    this,
                    IsActive && (i == selectedEntry),
                    gameTime,
                    font,
                    CommonConstants.MARGIN_LEFT_MENU_PANEL,
                    widthScreen / 4,
                    valueTransition);
            }
        }

        /// <summary>
        /// Draws a background drawing a picture from the Content. We use the spriteBatch to draw the background because, we want it remain in all screens.
        /// </summary>
        private void DrawBackground(SpriteBatch spriteBatch, int widthScreen, int heightScreen)
        {
            // Create an background rectangle.
            Rectangle backgroundRectangleBack = new Rectangle(0, 0, widthScreen, heightScreen);

            spriteBatch.Begin();
            // Draw rectangle on the background.
            spriteBatch.Draw(screenManager.getTexture(ScreenManager.TEXTURE_MAIN_MENU), backgroundRectangleBack, Color.White * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws a rectangle on the left to show the main menu.
        /// </summary>
        private void DrawMenuPanel(ScreenManager screenManager, int widthScreen, int heightScreen)
        {
            ElementPolygon panel = new ElementPolygon(
                CommonConstants.MARGIN_LEFT_MENU_PANEL,
                CommonConstants.MARGIN_UP_MENU_PANEL,
                widthScreen / 4,
                heightScreen - 2 * CommonConstants.MARGIN_UP_MENU_PANEL,
                MenuPanelColor * TransitionAlpha,
                1,
                true);
            panel.draw(screenManager);
        }

        /// <summary>
        /// Draws the user module on the main menu.
        /// </summary>
        private void DrawUserModule(SpriteBatch spriteBatch, SpriteFont userModuleFont, int heightScreen)
        {
            String CurrentUserName, CurrentUserSurname;
            Texture2D image;
            int age;

            if (CurrentUser.Equals(CommonConstants.NEW_USER_INDEX))
            {
                // Draw the user information regarding to the default information.
                CurrentUserName = CommonConstants.DEFAULT_USER_NAME;
                CurrentUserSurname = CommonConstants.DEFAULT_USER_LASTNAME;
                age = CommonConstants.DEFAULT_USER_AGE_VALUE;
                image = screenManager.getTexture(ScreenManager.TEXTURE_USER_TEMPLATE);
                DrawUserInfo(spriteBatch, userModuleFont, heightScreen, CurrentUserName, CurrentUserSurname, age, image);
            }
            else
            {
                // Draw the user information regarding to the current selection.
                CurrentUserName = screenManager.getUsers()[CurrentUser].getName();
                CurrentUserSurname = screenManager.getUsers()[CurrentUser].getSurname();
                age = screenManager.getUsers()[CurrentUser].getAge();
                image = screenManager.getUsers()[CurrentUser].getPicture();
                DrawUserInfo(spriteBatch, userModuleFont, heightScreen, CurrentUserName, CurrentUserSurname, age, image);
            }
        }

        /// <summary>
        /// Draws the user information in the user module.
        /// </summary>
        private void DrawUserInfo(SpriteBatch spriteBatch, SpriteFont userModuleFont, int heightScreen, string CurrentUserName, string CurrentUserSurname,
            int age, Texture2D image)
        {
            // Create an rectangle for user avatar.
            int xAvatarRect = CommonConstants.MARGIN_LEFT_MENU_PANEL + 20, xTextsUser;
            xAvatarRect += (int) valueTransition;
            Rectangle avatarRectangle = new Rectangle(xAvatarRect, heightScreen / 2 - 10, 70, 70);
            Rectangle originRectangle = new Rectangle(image.Bounds.Width / 2 - image.Bounds.Height / 2, 0, image.Bounds.Height, image.Bounds.Height);

            spriteBatch.Begin();
            spriteBatch.Draw(image, avatarRectangle, originRectangle, Color.White * TransitionAlpha);
            // Draw user data.
            xTextsUser = xAvatarRect + 70;
            xTextsUser += (int) valueTransition;
            DrawUserData(spriteBatch, userModuleFont, xTextsUser, heightScreen / 2 - 10, CurrentUserName, CurrentUserSurname, age);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the name, surname and age for an specific user.
        /// </summary>
        private void DrawUserData(SpriteBatch spriteBatch, SpriteFont userModuleFont, int initialPosX, int initialPosY,
                                    string name, string surname, int age)
        {
            // Draw name and surname.
            spriteBatch.DrawString(userModuleFont, name, new Vector2(initialPosX + 10, initialPosY), Color.White * TransitionAlpha);
            spriteBatch.DrawString(userModuleFont, surname, new Vector2(initialPosX + 10, initialPosY + 20), Color.White * TransitionAlpha);
            // Draw age.
            spriteBatch.DrawString(
                userModuleFont,
                String.Format(CommonConstants.DEFAULT_USER_AGE_STRING, age),
                new Vector2(initialPosX + 10, initialPosY + 45),
                Color.White * TransitionAlpha);
        }

        /// <summary>
        /// Draws the current game name at the top of the main menu.
        /// </summary>
        private void DrawGameInstanceModule(SpriteBatch spriteBatch, SpriteFont font, int widthScreen)
        {
            String textToDisplay;
            float gameAndNumberY, gameAndNumberX;

            textToDisplay = screenManager.getCurrentGame().getName();
            gameAndNumberY = CommonConstants.MARGIN_UP_GAME_INSTANCE;
            gameAndNumberX = gameAndNumberX = (widthScreen / 4) - (int) font.MeasureString(textToDisplay).X - CommonConstants.MARGIN_RIGHT_ELEMENTS_MENU_PANEL;
            gameAndNumberX += valueTransition;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, textToDisplay, new Vector2(gameAndNumberX, gameAndNumberY), Color.White * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the project name at the right top.
        /// </summary>
        private void DrawProjectName(SpriteBatch spriteBatch, SpriteFont fontS, SpriteFont fontP, int widthScreen)
        {
            int simonXPos = widthScreen - (int) fontS.MeasureString(CommonConstants.PROJECT_NAME).X - 10;
            int simonYPos = -5;
            spriteBatch.Begin();
            spriteBatch.DrawString(fontS, CommonConstants.PROJECT_NAME, new Vector2(simonXPos, simonYPos), Color.White * TransitionAlpha);
            spriteBatch.DrawString(fontP, CommonConstants.PROJECT_DESCRIPTION, new Vector2(simonXPos, simonYPos + 48), Color.White * TransitionAlpha);
            spriteBatch.End();
        }

    }

}
