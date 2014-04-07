﻿//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using System;
using Microsoft.Xna.Framework.Input;
using ProyectoSimon.Utils;

namespace ProyectoSimon
{
    /// <summary>
    /// This class represents a new user form in order to create a account in the system.
    /// The user can move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    class UserFormMenuScreen : MenuScreen
    {
        private int availableAreaX0, availableAreaX1;
        private int x, y, w, h;
        private int wScreen, hScreen;
        private int indexField;
        private Boolean[] isFieldSelected;
        private MenuEntry cancelEntry;
        private MenuEntry addEntry;
        private InputAction tabSelected, backSelect, deleteSelect, numbersSelected, keysSelected, captureKinect, spaceSelected;
        private string nameField;
        private string surnameField;
        private string ageField;
        private int cursorIndex;
        private Texture2D video;
        private bool blink;
        private Color userDataColor, userFrameColor, titleColor;

        /// <summary>
        /// Constructor.
        /// </summary>
        public UserFormMenuScreen(int a0, int b1, int wS, int hS)
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
            indexField = 0;
            cursorIndex = 0;
            userDataColor = new Color(64, 64, 64);
            userFrameColor = titleColor = Color.White;
            isFieldSelected = new Boolean[] { true, false, false, false };
            nameField = surnameField = ageField = String.Empty;
            // Create our menu entries.
            cancelEntry = new MenuEntry(CommonConstants.MENU_ENTRY_CANCEL);
            addEntry = new MenuEntry(CommonConstants.MENU_ENTRY_ADD);
            // Hook up menu event handlers.
            cancelEntry.Selected += CancelEntrySelected;
            addEntry.Selected += AddEntrySelected;
            // Add entries to the menu.
            MenuEntries.Add(cancelEntry);
            MenuEntries.Add(addEntry);
            blink = false;

            spaceSelected = new InputAction(
                null,
                new Keys[] { Keys.Space },
                true);
            tabSelected = new InputAction(
                null,
                new Keys[] { Keys.Tab },
                true);
            deleteSelect = new InputAction(
                null,
                new Keys[] { Keys.Delete },
                true);
            backSelect = new InputAction(
                null,
                new Keys[] { Keys.Back },
                true);
            numbersSelected = new InputAction(
                null,
                new Keys[] { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, 
                    Keys.D7, Keys.D8, Keys.D9},
                true);
            keysSelected = new InputAction(
                null,
                new Keys[] { Keys.A, Keys.B, Keys.C, Keys.D, Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.J, Keys.K, Keys.L, 
                    Keys.M, Keys.N, Keys.O, Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T, Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z},
                true);
            captureKinect = new InputAction(
                null,
                new Keys[] { Keys.RightShift, Keys.LeftShift },
                true);
        }

        /// <summary>
        /// Responds to user input, accepting or cancelling the message box.
        /// </summary>
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            MainMenuScreen = false;
            base.HandleInput(gameTime, input);
            PlayerIndex playerIndex;

            if (gameTime.TotalGameTime.Seconds % 2 == 0)
                blink = true;
            else
                blink = false;

            if (tabSelected.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (indexField < CommonConstants.USER_FORM_MAX_FIELDS)
                    indexField++;
                else
                    indexField = 0;
                isFieldSelected = new Boolean[] { false, false, false, false };
                isFieldSelected[indexField] = true;

                cursorIndex = getCurrentText().Length;
            }
            else if (deleteSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (cursorIndex != getCurrentText().Length)
                {
                    if (isFieldSelected[0])
                        nameField = nameField.Remove(cursorIndex, 1);
                    else if (isFieldSelected[1])
                        surnameField = surnameField.Remove(cursorIndex, 1);
                    else if (isFieldSelected[2])
                        ageField = ageField.Remove(cursorIndex, 1);
                }

            }
            else if (backSelect.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (cursorIndex != 0)
                {
                    if (isFieldSelected[0])
                        nameField = nameField.Remove(cursorIndex - 1, 1);
                    else if (isFieldSelected[1])
                        surnameField = surnameField.Remove(cursorIndex - 1, 1);
                    else if (isFieldSelected[2])
                        ageField = ageField.Remove(cursorIndex - 1, 1);

                    cursorIndex--;
                }
            }
            else if (numbersSelected.Evaluate(input, ControllingPlayer, out playerIndex) || keysSelected.Evaluate(input, ControllingPlayer, out playerIndex) ||
                spaceSelected.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                string auxKey;

                if (isFieldSelected[0])
                {
                    auxKey = input.CurrentKeyboardStates[0].GetPressedKeys()[0].ToString();
                    if (auxKey.Equals("Space"))
                        nameField = nameField.Insert(cursorIndex, CommonConstants.STRING_BLANK_SPACE);
                    else
                        nameField = nameField.Insert(cursorIndex, auxKey.Substring(auxKey.Length - 1, 1));
                }
                else if (isFieldSelected[1])
                {
                    auxKey = input.CurrentKeyboardStates[0].GetPressedKeys()[0].ToString();
                    if (auxKey.Equals("Space"))
                        surnameField = surnameField.Insert(cursorIndex, CommonConstants.STRING_BLANK_SPACE);
                    else
                        surnameField = surnameField.Insert(cursorIndex, auxKey.Substring(auxKey.Length - 1, 1));
                }
                else if (isFieldSelected[2])
                {
                    auxKey = input.CurrentKeyboardStates[0].GetPressedKeys()[0].ToString();
                    ageField = ageField.Insert(cursorIndex, auxKey.Substring(auxKey.Length - 1, 1));
                }

                cursorIndex++;
            }
            else if (MenuRight.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (cursorIndex < getCurrentText().Length)
                    cursorIndex++;
            }
            else if (MenuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (cursorIndex > 0)
                    cursorIndex--;
            }
            else if (isFieldSelected[3] && captureKinect.Evaluate(input, ControllingPlayer, out playerIndex) && KinectSDK.Instance.isConected())
                video = KinectSDK.Instance.getCapture();
        }

        /// <summary>
        /// Return the current value from the input field.
        /// </summary>
        private string getCurrentText()
        {
            string auxText = String.Empty;

            if (isFieldSelected[0])
                auxText = nameField;
            else if (isFieldSelected[1])
                auxText = surnameField;
            else if (isFieldSelected[2])
                auxText = ageField;

            return auxText.ToLower();
        }

        /// <summary>
        /// Event handler for when the Cancel menu entry is selected.
        /// </summary>
        void CancelEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            MainMenuScreen mainMenuScreen = new MainMenuScreen();
            mainMenuScreen.CurrentUser = DataManager.Instance.getUserIndex();
            mainMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Event handler for when the Add menu entry is selected.
        /// </summary>
        void AddEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (!nameField.Equals(String.Empty) && !surnameField.Equals(String.Empty) && !ageField.Equals(String.Empty) && video != null)
            {
                string photoPath = CommonUtilMethods.GenerateCapturePath(nameField, surnameField, CommonUtilMethods.JPG);
                System.IO.FileStream stream = new System.IO.FileStream(@photoPath, System.IO.FileMode.Create);
                video.SaveAsJpeg(stream, video.Width, video.Height);

                User user = new User(nameField.ToLower(), surnameField.ToLower(), photoPath, Convert.ToInt32(ageField), video);
                //Creating statistic fields
                Statistics circlesGame = new Statistics(CommonConstants.CIRCLES_GAME_NAME);
                user.addStatistic(0, circlesGame);
                Statistics chooserGame = new Statistics(CommonConstants.CHOOSER_GAME_NAME);
                user.addStatistic(1, chooserGame);
                Statistics arrowsGame = new Statistics(CommonConstants.ARROWS_GAME_NAME);
                user.addStatistic(2, arrowsGame);
                DataManager.Instance.addNewUser(user);
                // Store users in a XML file
                DataManager.Instance.storeUsersToXml();

                // Modularizar!!!!!!!!!
                MainMenuScreen mainMenuScreen = new MainMenuScreen();
                mainMenuScreen.CurrentUser = DataManager.Instance.getUsersCount() - 1;
                mainMenuScreen.CurrentGame = DataManager.Instance.getIndexGame();
                LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
            }
        }

        /// <summary>
        /// Draw event for the user form menu screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteFont comandsFont = GameContentManager.Instance.getFont(GameContentManager.USER_MODULE_FONT);
            SpriteFont titleFont = GameContentManager.Instance.getFont(GameContentManager.GAME_INSTANCE_FONT);
            string title = CommonConstants.NEWFORM_SCREEN_TITLE;
            int horizontalValue = (int) comandsFont.MeasureString(title).X;
            int verticalValue = (int) comandsFont.MeasureString(title).Y;

            // Draw statistics frame.
            drawUserFormFrame(verticalValue);
            // Draw tittle.
            drawTitleFrame(title, screenManager.SpriteBatch, titleFont, horizontalValue);
            // Draw user input comands.
            drawUserComands(screenManager.SpriteBatch, comandsFont, verticalValue + y);
        }

        /// <summary>
        /// Draws the commands to get the input from the user.
        /// </summary>
        private void drawUserComands(SpriteBatch spriteBatch, SpriteFont menuFont, int posY)
        {
            int marginUp = 45;
            // Draw labels and fields.
            drawLabelAndField(spriteBatch, menuFont, posY + marginUp, CommonConstants.DEFAULT_USER_NAME, isFieldSelected[0], nameField.ToLower());
            drawLabelAndField(spriteBatch, menuFont, posY + marginUp + 40, CommonConstants.DEFAULT_USER_LASTNAME, isFieldSelected[1], surnameField.ToLower());
            drawLabelAndField(spriteBatch, menuFont, posY + marginUp + 80, CommonConstants.DEFAULT_USER_AGE_WORD, isFieldSelected[2], ageField.ToLower());
            // Draw capture photo module.
            drawCapturePhotoModule(spriteBatch, menuFont, posY + marginUp + 80, isFieldSelected[3]);
        }

        /// <summary>
        /// Draws the video stream mode from the Kinect sensor.
        /// </summary>
        private void drawCapturePhotoModule(SpriteBatch spriteBatch, SpriteFont menuFont, int posY, Boolean isSelected)
        {
            ElementPolygon cameraFrame, captureFrame, edgeCaptureFrame, edgeCameraFrame;
            int wText, posX, blankSpace, frameSize;
            string textLabel;

            blankSpace = 50;
            frameSize = 80;
            textLabel = CommonConstants.DEFAULT_USER_CAPTURE;
            wText = (int) menuFont.MeasureString(textLabel).X;
            posX = x + w / 2 - 60 - wText;

            if (isSelected)
            {
                cameraFrame = new ElementPolygon(posX + blankSpace + wText, posY + 80, frameSize, frameSize, new Color(55, 55, 55), .8f, true);
                edgeCameraFrame = new ElementPolygon(posX + blankSpace + wText, posY + 80, frameSize, frameSize, new Color(227, 117, 64), .8f, false);
                captureFrame = new ElementPolygon(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize, new Color(55, 55, 55), .8f, true);
                edgeCaptureFrame = new ElementPolygon(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize, new Color(227, 117, 64), .8f, false);

                if (KinectSDK.Instance.isConected())
                {
                    spriteBatch.Begin();
                   KinectSDK.Instance.DrawVideoCam(spriteBatch, new Rectangle(posX + blankSpace + wText, posY + 80, frameSize, frameSize));
                    spriteBatch.End();
                }
            }
            else
            {
                cameraFrame = new ElementPolygon(posX + blankSpace + wText, posY + 80, frameSize, frameSize, Color.Black, .8f, true);
                edgeCameraFrame = new ElementPolygon(posX + blankSpace + wText, posY + 80, frameSize, frameSize, Color.White, .8f, false);
                captureFrame = new ElementPolygon(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize, Color.Black, .8f, true);
                edgeCaptureFrame = new ElementPolygon(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize, Color.White, .8f, false);
            }

            spriteBatch.Begin();
            if (video != null)
                spriteBatch.Draw(video, new Rectangle(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize), Color.White);
            spriteBatch.End();

            // draw photo label.
            spriteBatch.Begin();
            spriteBatch.DrawString(menuFont, textLabel, new Vector2(posX, posY + 80), userDataColor * TransitionAlpha);
            spriteBatch.End();

            // Draw capture frame.
            if (video == null)
                captureFrame.draw(screenManager);

            edgeCaptureFrame.draw(screenManager);

            if (!isSelected)
                cameraFrame.draw(screenManager);

            edgeCameraFrame.draw(screenManager);
        }

        /// <summary>
        /// Draws a label and a text field.
        /// </summary>
        private void drawLabelAndField(SpriteBatch spriteBatch, SpriteFont menuFont, int posY, string textLabel, Boolean isSelected, string textField)
        {
            ElementPolygon namefield, edgeNamefield;
            int wText, wField, hField, posX, blankSpace;

            blankSpace = 50;
            wField = 150;
            hField = 30;
            wText = (int) menuFont.MeasureString(textLabel).X;
            posX = x + w / 2 - 60 - wText;

            if (isSelected)
            {
                namefield = new ElementPolygon(posX + blankSpace + wText, posY, wField, hField, new Color(55, 55, 55), .8f, true);
                edgeNamefield = new ElementPolygon(posX + blankSpace + wText, posY, wField, hField, new Color(227, 117, 64), .8f, false);
                if (blink)
                    textField = textField.Insert(cursorIndex, CommonConstants.DEFAULT_TEXT_CURSOR);
            }
            else
            {
                namefield = new ElementPolygon(posX + blankSpace + wText, posY, wField, hField, Color.Black, .8f, true);
                edgeNamefield = new ElementPolygon(posX + blankSpace + wText, posY, wField, hField, Color.White, .8f, false);
            }

            // Draw label.
            spriteBatch.Begin();
            spriteBatch.DrawString(menuFont, textLabel, new Vector2(posX, posY), userDataColor * TransitionAlpha);
            spriteBatch.End();
            // Draw field.
            namefield.draw(screenManager);
            edgeNamefield.draw(screenManager);
            // Draw current keys.
            spriteBatch.Begin();
            spriteBatch.DrawString(menuFont, textField, new Vector2(posX + blankSpace + wText + 5, posY + 2), Color.White * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws the title for the frame.
        /// </summary>
        private void drawTitleFrame(string title, SpriteBatch spriteBatch, SpriteFont statisticsFont, int horizontalValue)
        {
            int middleValue = horizontalValue / 2;
            spriteBatch.Begin();
            spriteBatch.DrawString(statisticsFont, title, new Vector2(x + w / 2 - middleValue, hScreen / 4), titleColor * TransitionAlpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws a simple frame for the User Form screen.
        /// </summary>
        private void drawUserFormFrame(int verticalValue)
        {
            ElementPolygon frame = new ElementPolygon(x, hScreen / 4, w, hScreen / 4 * 2 + 10, userFrameColor * TransitionAlpha, 1, true);
            frame.draw(screenManager);
            ElementPolygon titleFrame = new ElementPolygon(x, hScreen / 4, w, verticalValue + 12, MenuPanelColor * TransitionAlpha, 1, true);
            titleFrame.draw(screenManager);
        }
    }
}
