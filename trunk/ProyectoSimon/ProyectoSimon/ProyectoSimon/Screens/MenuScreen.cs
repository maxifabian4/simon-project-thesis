//-----------------------------------------------------------------------------
// MenuScreen.cs
//
// XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

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

namespace ProyectoSimon
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    abstract class MenuScreen : GameScreen
    {
        // Margins of menu panel.
        protected const int MARGIN_LEFT_MENU_PANEL = 20;
        private const int MARGIN_UP_MENU_PANEL = 10;
        private const int MARGIN_DOWN_MENU_ENTRIES = 50;
        private const int MARGIN_ALIGN_MENU_ENTRIES = 20;
        private const int MARGIN_UP_GAME_INSTANCE = 30;
        private const int MARGIN_RIGHT_ELEMENTS_MENU_PANEL = 10;
        // Menu entry list.
        private List<MenuEntry> menuEntries = new List<MenuEntry>();
        private int selectedEntry = 0;
        // Global positions for slide format.
        private float valueTransition;
        // Input parameters.
        private InputAction menuUp, menuDown;
        protected InputAction menuLeft, menuRight;
        private InputAction menuSelect, menuCancel;
        private InputAction menuCtrl, menuDel;
        // Current game play instance.
        protected int currentGame;
        // Current user.
        protected int currentUser;
        // Determine if we are in main menu screen.
        protected bool mainMenuScreen;
        // Design parameters.
        protected Color panelColor;

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get { return menuEntries; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public MenuScreen()
        {
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
                new Keys[] { Keys.LeftControl , Keys.RightControl},
                true);
            menuDel = new InputAction(
                new Buttons[] { },
                new Keys[] { Keys.Delete },
                true);

            // Initialize the current user.
            currentUser = 0;
            // Initialize the current instance game.
            currentGame = 0;
            // We are in main menu screen.
            mainMenuScreen = true;
            // Inicialize the panel color.
            panelColor = new Color(201, 47, 15);
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
                    selectedEntry = menuEntries.Count - 1;
            }
            // Move to the next menu entry?
            if (menuDown.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
            }

            if (menuSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnSelectEntry(selectedEntry, playerIndex);
            }
            else if (menuCancel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                OnCancel(playerIndex);
            }
            else if (mainMenuScreen && (input.IsKeyPressed(Keys.LeftControl, ControllingPlayer, out playerIndex) || input.IsKeyPressed(Keys.RightControl, ControllingPlayer, out playerIndex)))
            {
                if (menuRight.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    if (currentGame == screenManager.getGames().Length - 1)
                        currentGame = 0;
                    else
                        currentGame++;
                    screenManager.setCurrentGame(currentGame);
                }
                else if (menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
                {
                    if (currentGame == 0)
                        currentGame = screenManager.getGames().Length - 1;
                    else
                        currentGame--;
                    screenManager.setCurrentGame(currentGame);
                }
            }
            else if (mainMenuScreen && menuRight.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (currentUser == screenManager.getUsers().Count - 1)
                    currentUser = 0;
                else
                    currentUser++;
                screenManager.setUserIndex(currentUser);
            }
            else if (mainMenuScreen && menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (currentUser == 0)
                    currentUser = screenManager.getUsersCount() - 1;
                else
                    currentUser--;
                screenManager.setUserIndex(currentUser);
            }
            else if (mainMenuScreen && menuDel.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                screenManager.deleteUser(currentUser);
                screenManager.storeUsersToXml();
                currentUser = 0;
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
        protected virtual void updateMenuEntryLocations(int widthScreen, int heightScreen)
        {
            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            SpriteFont sansSerif15 = screenManager.getFont(ScreenManager.USER_MODULE_FONT);
            MenuEntry menuEntry;
            Vector2 position = new Vector2(0, heightScreen - MARGIN_DOWN_MENU_ENTRIES);
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            // Update each menu entry's location in turn
            for (int i = menuEntries.Count - 1; i >= 0; i--)
            {
                menuEntry = menuEntries[i];
                // Each entry is to be centered horizontally inside de panel menu.
                position.X = (widthScreen / 8 - (int)sansSerif15.MeasureString(menuEntry.Text).X / 2) + MARGIN_LEFT_MENU_PANEL;

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
                position.Y -= (int)sansSerif15.MeasureString(menuEntry.Text).Y + MARGIN_ALIGN_MENU_ENTRIES;
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
            updateMenuEntryLocations(widthScreen, heightScreen);
            // Draw background.
            drawBackground(spriteBatch, widthScreen, heightScreen);
            // Create an rectangle on the left.
            drawMenuPanel(screenManager, widthScreen, heightScreen);
            // Draw game instance name.
            drawGameInstanceModule(spriteBatch, gameInstanceFont, widthScreen);
            // Draw user module.
            drawUserModule(spriteBatch, userModuleFont, heightScreen);
            // Draw each menu entry in turn.
            drawMenuEntries(spriteBatch, gameTime, widthScreen, userModuleFont);
            // Draw proyect name.
            drawProjectName(spriteBatch, fontBeautiful, fontBeautifulProject, widthScreen);
        }

        /// <summary>
        /// Draws the menu entries.
        /// </summary>
        private void drawMenuEntries(SpriteBatch spriteBatch, GameTime gameTime, int widthScreen, SpriteFont font)
        {
            MenuEntry menuEntry;
            for (int i = menuEntries.Count - 1; i >= 0; i--)
            {
                menuEntry = menuEntries[i];
                menuEntry.Draw(this, IsActive && (i == selectedEntry), gameTime, font, MARGIN_LEFT_MENU_PANEL, widthScreen / 4, valueTransition);
            }
        }

        /// <summary>
        /// Draws a background drawing a picture from the Content. We use the spriteBatch to draw the background because, we want it remain in all screens.
        /// </summary>
        private void drawBackground(SpriteBatch spriteBatch, int widthScreen, int heightScreen)
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
        private void drawMenuPanel(ScreenManager screenManager, int widthScreen, int heightScreen)
        {
            ElementPolygon panel = new ElementPolygon(MARGIN_LEFT_MENU_PANEL, MARGIN_UP_MENU_PANEL, widthScreen / 4, heightScreen - 2 * MARGIN_UP_MENU_PANEL, panelColor * TransitionAlpha, 1, true);
            panel.drawPrimitive(screenManager);
            //ElementPolygon edgePanel = new ElementPolygon(MARGIN_LEFT_MENU_PANEL, MARGIN_UP_MENU_PANEL, widthScreen / 4, heightScreen - 2 * MARGIN_UP_MENU_PANEL, new Color(33, 33, 33), 1, false);
            //edgePanel.drawPrimitive(screenManager);
        }

        /// <summary>
        /// Draws the user module on the main menu.
        /// </summary>
        private void drawUserModule(SpriteBatch spriteBatch, SpriteFont userModuleFont, int heightScreen)
        {
            String currentUserName, currentUserSurname;
            Texture2D image;
            int age;

            if (currentUser == -1)
            {
                currentUserName = "nombre";
                currentUserSurname = "apellido";
                age = 0;
                image = screenManager.getTexture(ScreenManager.TEXTURE_USER_TEMPLATE);
                drawUserInfo(spriteBatch, userModuleFont, heightScreen, currentUserName, currentUserSurname, age, image);
            }
            else
            {
                currentUserName = screenManager.getUsers()[currentUser].getName();
                currentUserSurname = screenManager.getUsers()[currentUser].getSurname();
                age = screenManager.getUsers()[currentUser].getAge();
                //age = 8;
                image = screenManager.getUsers()[currentUser].getPicture();
                drawUserInfo(spriteBatch, userModuleFont, heightScreen, currentUserName, currentUserSurname, age, image);
            }
        }

        /// <summary>
        /// Draws the user information in the user module.
        /// </summary>
        private void drawUserInfo(SpriteBatch spriteBatch, SpriteFont userModuleFont, int heightScreen, string currentUserName, string currentUserSurname,
            int age, Texture2D image)
        {
            // Create an rectangle for user avatar.
            int xAvatarRect = MARGIN_LEFT_MENU_PANEL + 20, xTextsUser;
            xAvatarRect += (int)valueTransition;
            Rectangle avatarRectangle = new Rectangle(xAvatarRect, heightScreen / 2 - 10, 70, 70);
            Rectangle originRectangle = new Rectangle(image.Bounds.Width / 2 - image.Bounds.Height / 2, 0, image.Bounds.Height, image.Bounds.Height);
            //ElementPolygon avatarEdge = new ElementPolygon(xAvatarRect, heightScreen / 2 - 10, 70, 70, new Color(150, 150, 150), TransitionAlpha, false);

            spriteBatch.Begin();
            spriteBatch.Draw(image, avatarRectangle, originRectangle, Color.White * TransitionAlpha);
            // Draw user data.
            xTextsUser = xAvatarRect + 70;
            xTextsUser += (int)valueTransition;
            drawUserData(spriteBatch, userModuleFont, xTextsUser, heightScreen / 2 - 10, currentUserName, currentUserSurname, age);
            spriteBatch.End();

            // Draw edge primitive for the image.
            //avatarEdge.drawPrimitive(screenManager);
        }

        /// <summary>
        /// Draws the name, surname and age for an specific user.
        /// </summary>
        private void drawUserData(SpriteBatch spriteBatch, SpriteFont userModuleFont, int initialPosX, int initialPosY,
                                    string name, string surname, int age)
        {
            // Draw name and surname.
            spriteBatch.DrawString(userModuleFont, name, new Vector2(initialPosX + 10, initialPosY), Color.White * TransitionAlpha);
            spriteBatch.DrawString(userModuleFont, surname, new Vector2(initialPosX + 10, initialPosY + 20), Color.White * TransitionAlpha);
            // Draw age.
            spriteBatch.DrawString(userModuleFont, age + " años", new Vector2(initialPosX + 10, initialPosY + 45), Color.White * TransitionAlpha);
        }

        /// <summary>
        /// Draws the current game name at the top of the main menu.
        /// </summary>
        private void drawGameInstanceModule(SpriteBatch spriteBatch, SpriteFont font, int widthScreen)
        {
            String textToDisplay;
            float gameAndNumberY, gameAndNumberX;

            //textToDisplay = screenManager.getCurrentGame().getName() + " | " + (currentGame + 1);
            textToDisplay = screenManager.getCurrentGame().getName();
            gameAndNumberY = MARGIN_UP_GAME_INSTANCE;
            gameAndNumberX = gameAndNumberX = (widthScreen / 4) - (int)font.MeasureString(textToDisplay).X - MARGIN_RIGHT_ELEMENTS_MENU_PANEL;
            gameAndNumberX += valueTransition;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, textToDisplay, new Vector2(gameAndNumberX, gameAndNumberY), Color.White * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the project name at the right top.
        /// </summary>
        private void drawProjectName(SpriteBatch spriteBatch, SpriteFont fontS, SpriteFont fontP, int widthScreen)
        {
            int simonXPos = widthScreen - (int)fontS.MeasureString("simon").X - 10;
            int simonYPos = -5;

            //ElementCircle circleFill = new ElementCircle(75, new Vector2(widthScreen-50, 10), Color.Black, .5f, true);
            //circleFill.drawPrimitive(screenManager);
            //ElementCircle circleEdge = new ElementCircle(75, new Vector2(widthScreen-50, 10), new Color(33, 33, 33), 1, false);
            //circleEdge.drawPrimitive(screenManager);

            spriteBatch.Begin();
            spriteBatch.DrawString(fontS, "simon", new Vector2(simonXPos, simonYPos), Color.White * TransitionAlpha);
            spriteBatch.DrawString(fontP, "PROYECTO ACADEMICO", new Vector2(simonXPos, simonYPos + 48), Color.White * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Set the current user in memory.
        /// </summary>
        public void setCurrentUser(int ind)
        {
            currentUser = ind;
        }

        /// <summary>
        /// Set the current game in memory.
        /// </summary>
        public void setCurrentGame(int ind)
        {
            currentGame = ind;
        }
    }
}