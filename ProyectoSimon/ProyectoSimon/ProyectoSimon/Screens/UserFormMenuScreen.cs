//-----------------------------------------------------------------------------
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

namespace ProyectoSimon
{
    /// <summary>
    /// This class represents a new user form in order to create a account in the system.
    /// The user can move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    class UserFormMenuScreen : MenuScreen
    {
        private const int MAX_FIELDS = 3;
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
            nameField = surnameField = ageField = "";
            // Create our menu entries.
            cancelEntry = new MenuEntry("cancelar");
            addEntry = new MenuEntry("agregar");
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
            mainMenuScreen = false;
            base.HandleInput(gameTime, input);
            PlayerIndex playerIndex;

            if (gameTime.TotalGameTime.Seconds % 2 == 0)
                blink = true;
            else
                blink = false;

            if (tabSelected.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (indexField < MAX_FIELDS)
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
                        nameField = nameField.Insert(cursorIndex, " ");
                    else
                        nameField = nameField.Insert(cursorIndex, auxKey.Substring(auxKey.Length - 1, 1));
                }
                else if (isFieldSelected[1])
                {
                    auxKey = input.CurrentKeyboardStates[0].GetPressedKeys()[0].ToString();
                    if (auxKey.Equals("Space"))
                        surnameField = surnameField.Insert(cursorIndex, " ");
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
            else if (menuRight.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (cursorIndex < getCurrentText().Length)
                    cursorIndex++;
            }
            else if (menuLeft.Evaluate(input, ControllingPlayer, out playerIndex))
            {
                if (cursorIndex > 0)
                    cursorIndex--;
            }
            else if (isFieldSelected[3] && captureKinect.Evaluate(input, ControllingPlayer, out playerIndex) && screenManager.Kinect.isConected())
                video = screenManager.Kinect.getCapture();
        }

        /// <summary>
        /// Return the current value from the input field.
        /// </summary>
        private string getCurrentText()
        {
            string auxText = "";

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
            mainMenuScreen.setCurrentUser(screenManager.getUserIndex());
            mainMenuScreen.setCurrentGame(screenManager.getIndexGame());
            LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
        }

        /// <summary>
        /// Event handler for when the Add menu entry is selected.
        /// </summary>
        void AddEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            if (!nameField.Equals("") && !surnameField.Equals("") && !ageField.Equals("") && video != null)
            {
                string photoPath = "Data//" + nameField.ToLower() + "_" + surnameField.ToLower() + ".jpg";
                System.IO.FileStream stream = new System.IO.FileStream(@photoPath, System.IO.FileMode.Create);
                video.SaveAsJpeg(stream, video.Width, video.Height);
                User user = new User(nameField.ToLower(), surnameField.ToLower(), photoPath, Convert.ToInt32(ageField), video);
                //Creating statistic fields
                Statistics game1 = new Statistics("círculos");
                user.addStatistic(0, game1);
                Statistics game2 = new Statistics("clasificador");
                user.addStatistic(1, game2);
                Statistics game3 = new Statistics("flechas");
                user.addStatistic(2, game3);
                screenManager.addNewUser(user);
                // Store users in a XML file
                screenManager.storeUsersToXml();

                // Modularizar!!!!!!!!!
                MainMenuScreen mainMenuScreen = new MainMenuScreen();
                mainMenuScreen.setCurrentUser(screenManager.getUsersCount() - 1);
                mainMenuScreen.setCurrentGame(screenManager.getIndexGame());
                LoadingScreen.Load(screenManager, false, null, mainMenuScreen);
            }
        }

        /// <summary>
        /// Draw event for the user form menu screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            SpriteFont comandsFont = screenManager.getFont(ScreenManager.USER_MODULE_FONT);
            SpriteFont titleFont = screenManager.getFont(ScreenManager.GAME_INSTANCE_FONT);
            string title = "nuevo usuario";
            int horizontalValue = (int) comandsFont.MeasureString(title).X;
            int verticalValue = (int) comandsFont.MeasureString(title).Y;

            // Draw statistics frame.
            drawUserFormFrame(verticalValue);
            // Draw tittle.
            drawTitleFrame(title, screenManager.SpriteBatch, titleFont, horizontalValue);
            // Draw header line.
            //drawLine(screenManager.SpriteBatch, new Vector2(x + 10, y + verticalValue + 15), new Vector2(x + w - 10, y + verticalValue + 15));
            //// Draw footer line.
            //drawLine(screenManager.SpriteBatch, new Vector2(x + 10, y + h - 20 + 10), new Vector2(x + w - 10, y + h - 20 + 10));
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
            drawLabelAndField(spriteBatch, menuFont, posY + marginUp, "nombre", isFieldSelected[0], nameField.ToLower());
            drawLabelAndField(spriteBatch, menuFont, posY + marginUp + 40, "apellido", isFieldSelected[1], surnameField.ToLower());
            drawLabelAndField(spriteBatch, menuFont, posY + marginUp + 80, "edad", isFieldSelected[2], ageField.ToLower());
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
            textLabel = "foto";
            wText = (int) menuFont.MeasureString(textLabel).X;
            posX = x + w / 2 - 60 - wText;

            if (isSelected)
            {
                cameraFrame = new ElementPolygon(posX + blankSpace + wText, posY + 80, frameSize, frameSize, new Color(55, 55, 55), .8f, true);
                edgeCameraFrame = new ElementPolygon(posX + blankSpace + wText, posY + 80, frameSize, frameSize, new Color(227, 117, 64), .8f, false);
                captureFrame = new ElementPolygon(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize, new Color(55, 55, 55), .8f, true);
                edgeCaptureFrame = new ElementPolygon(posX + blankSpace + wText + frameSize + 40, posY + 80, frameSize, frameSize, new Color(227, 117, 64), .8f, false);

                if (screenManager.Kinect.isConected())
                {
                    spriteBatch.Begin();
                    screenManager.Kinect.DrawVideoCam(spriteBatch, new Rectangle(posX + blankSpace + wText, posY + 80, frameSize, frameSize));
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
                    textField = textField.Insert(cursorIndex, "|");
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
        /// Draw a line between v1 and v2. ABSTRAERRRRRR  !!!!!!!
        /// </summary>
        //private void drawLine2(SpriteBatch spriteBatch, Vector2 v1, Vector2 v2)
        //{
        //    FixedArray8<Vector2> vertexs = new FixedArray8<Vector2>();
        //    vertexs[0] = new Vector2(v1.X, v1.Y);
        //    vertexs[1] = new Vector2(v2.X, v2.Y);

        //    ElementPolygon line = new ElementPolygon(vertexs, Color.White * TransitionAlpha, .6f, false, 2);
        //    line.drawPrimitive(screenManager);
        //}



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
            ElementPolygon titleFrame = new ElementPolygon(x, hScreen / 4, w, verticalValue + 12, panelColor * TransitionAlpha, 1, true);
            titleFrame.draw(screenManager);
        }
    }
}
