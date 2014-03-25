//-----------------------------------------------------------------------------
// ScreenManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;


namespace ProyectoSimon
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    public class ScreenManager : DrawableGameComponent
    {
        private List<GameScreen> screens = new List<GameScreen>();
        // Analizar si es necesario tener una copia de los GameScreen !!!!
        private List<GameScreen> tempScreensList = new List<GameScreen>();
        private InputState input = new InputState();
        private SpriteBatch spriteBatch;
        private ContentManager contentManager;
        private IDictionary<String, SpriteFont> fonts;
        private IDictionary<String, Texture2D> textures;
        private bool isInitialized;
        // Constants textures.
        
        public static String TEXTURE_MAIN_MENU = "mainmenu";

        public static String TEXTURE_BLANK = "blank";
        public static String TEXTURE_BACKGROUND_MAIN = "mainBackground";
        public static String TEXTURE_BACKGROUND_GAME = "gameBackground";
        public static String TEXTURE_BACKGROUND_GAME2 = "gameBackground2";
        public static String TEXTURE_BACKGROUND_ARROWS = "backgroundArrows";
        public static String TEXTURE_USER_TEMPLATE = "userTemplate";
        public static String TEXTURE_GAMEBOX = "boxempty";
        public static String TEXTURE_ALGO = "algo";
        // Constants fonts.
        public static String GAME_INSTANCE_FONT = "gameInstanceFont";
        public static String USER_MODULE_FONT = "userModuleFont";
        public static String FONT_BEAUTIFULEVERYTIME = "BeautifulEveryTime";
        public static String FONT_BEAUTIFULEVERYTIMEPROJECT = "BeautifulEveryTimeProject";
        public static String FONT_STATISTICSFONT = "StatisticFont";
        // XML parameters. We need a dynamic structure to store the users.
        private List<User> users;
        private GameInstance[] games;
        private StoredDataXML userStoredDataXML, gamePlayStoredDataXML;
        // BasicEffect.
        private BasicEffect basicEffect;
        // User index.
        private int userIndex;
        private int currentGame;
        private KinectSDK kinect;

        /// <summary>
        /// A default SpriteBatch shared by all the screens. This saves
        /// each screen having to bother creating their own local instance.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return spriteBatch; }
        }

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            // Initialize structures.
            fonts = new Dictionary<String, SpriteFont>();
            textures = new Dictionary<String, Texture2D>();
            users = new List<User>();
            userIndex = 0;
            currentGame = 0;
        }

        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            isInitialized = true;
            // Load the SpriteBatch.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // Create a BasicEffect to draw primitives.
            inicializeBasicEffect();
            // Load the store users from XML file.
            loadUsersFromXml();
            // Load instances.
            loadGamesFromXml();
            // Create an instance for the Kinect.
            //kinect = new KinectSDK(GraphicsDevice, GraphicsDeviceManager.DefaultBackBufferWidth, GraphicsDeviceManager.DefaultBackBufferHeight);
            kinect = new KinectSDK(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load content belonging to the screen manager.
            contentManager = Game.Content;
            // Load the fonts;
            loadFonts();
            // Load textures.
            loadTextures();
            // Tell each of the screens to load their content.
            foreach (GameScreen screen in screens)
                screen.Activate(false);
        }

        /// <summary>
        /// Inicializes a BasicEffect to draw primitives in the screens.
        /// </summary>
        private void inicializeBasicEffect()
        {
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter(0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height, 0, 0, 1);
        }

        /// <summary>
        /// Loads textures from the system.
        /// </summary>
        private void loadTextures()
        {
            textures.Add(TEXTURE_BLANK, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BLANK));
            textures.Add(TEXTURE_BACKGROUND_MAIN, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_MAIN));
            textures.Add(TEXTURE_USER_TEMPLATE, contentManager.Load<Texture2D>("Textures/" + TEXTURE_USER_TEMPLATE));
            textures.Add(TEXTURE_BACKGROUND_GAME, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_GAME));
            textures.Add(TEXTURE_BACKGROUND_GAME2, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_GAME2));
            textures.Add(TEXTURE_BACKGROUND_ARROWS, contentManager.Load<Texture2D>("Textures/" + TEXTURE_BACKGROUND_ARROWS));
            textures.Add(TEXTURE_GAMEBOX, contentManager.Load<Texture2D>("Textures/" + TEXTURE_GAMEBOX));
            textures.Add(TEXTURE_ALGO, contentManager.Load<Texture2D>("Textures/" + TEXTURE_ALGO));
            textures.Add(TEXTURE_MAIN_MENU, contentManager.Load<Texture2D>("Textures/" + TEXTURE_MAIN_MENU));
        }

        /// <summary>
        /// Loads fonts from the system.
        /// </summary>
        private void loadFonts()
        {
            fonts.Add(GAME_INSTANCE_FONT, contentManager.Load<SpriteFont>("SpriteFont/" + GAME_INSTANCE_FONT));
            fonts.Add(USER_MODULE_FONT, contentManager.Load<SpriteFont>("SpriteFont/" + USER_MODULE_FONT));
            fonts.Add(FONT_BEAUTIFULEVERYTIME, contentManager.Load<SpriteFont>("SpriteFont/" + FONT_BEAUTIFULEVERYTIME));
            fonts.Add(FONT_BEAUTIFULEVERYTIMEPROJECT, contentManager.Load<SpriteFont>("SpriteFont/" + FONT_BEAUTIFULEVERYTIMEPROJECT));
            fonts.Add(FONT_STATISTICSFONT, contentManager.Load<SpriteFont>("SpriteFont/" + FONT_STATISTICSFONT));
        }

        /// <summary>
        /// Returns the users stored in memory.
        /// </summary>
        public List<User> getUsers()
        {
            return users;
        }

        /// <summary>
        /// Returns the count of users stored in memory.
        /// </summary>
        public int getUsersCount()
        {
            return users.Count;
        }

        /// <summary>
        /// Returns the games stored in memory.
        /// </summary>
        public GameInstance[] getGames()
        {
            return games;
        }

        /// <summary>
        /// Returns the count of games stored in memory.
        /// </summary>
        public int getGamesCount()
        {
            return games.Length;
        }

        /// <summary>
        /// Allows to store the user information, from the XML file, in memory.
        /// </summary>
        private void loadUsersFromXml()
        {
            userStoredDataXML = new StoredDataXML(StoredDataXML.USERS_DATA_PATH, StoredDataXML.USERS_STORED_PLURAL, StoredDataXML.USERS_STORED_SINGULAR);
            // Create nodes to get the information in the xml file.
            XmlNodeList name, surname, picture, age, statistics, nodeList;
            Texture2D photo;
            System.IO.FileStream stream;
            User user;
            // Get the users stored in the xml file.
            nodeList = userStoredDataXML.getStoredElements();

            foreach (XmlElement nodo in nodeList)
            {
                // Gets user info.
                name = nodo.GetElementsByTagName(StoredDataXML.USER_NAME);
                surname = nodo.GetElementsByTagName(StoredDataXML.USER_SURNAME);
                age = nodo.GetElementsByTagName(StoredDataXML.USER_AGE);
                picture = nodo.GetElementsByTagName(StoredDataXML.USER_PICTURE);
                stream = new System.IO.FileStream(@picture[0].InnerText, System.IO.FileMode.Open);
                photo = Texture2D.FromStream(GraphicsDevice, stream);
                stream.Close();
                statistics = nodo.GetElementsByTagName(StoredDataXML.STATISTIC_SINGULAR);
                // Create user in memory.
                user = new User(name[0].InnerText, surname[0].InnerText, picture[0].InnerText, Convert.ToInt32(age[0].InnerText), photo);               
                // Set statistics to the user.
                setStatisticsFromNode(user, statistics);
                users.Add(user);
            }
        }

        /// <summary>
        /// Allows to get statistics from the xml and set them in memory.
        /// </summary>
        private void setStatisticsFromNode(User user, XmlNodeList statistics)
        {
            Statistics statistic = null;
            XmlNodeList gameName, attributes;
            int j = 0;

            foreach (XmlElement nodo in statistics)
            {
                gameName = nodo.GetElementsByTagName(StoredDataXML.STATISTIC_GAMENAME);
                attributes = nodo.GetElementsByTagName(StoredDataXML.STATISTIC_ATTRIBUTE);
                statistic = new Statistics(gameName[0].InnerText);
                setAttributesToStatistic(statistic, attributes);
                user.addStatistic(j, statistic);
                j++;
            }
        }

        /// <summary>
        /// Allows to get attributes from the xml and set them to the statistics.
        /// </summary>
        private void setAttributesToStatistic(Statistics statistic, XmlNodeList attributes)
        {
            XmlNodeList name, value;

            foreach (XmlElement nodo in attributes)
            {
                name = nodo.GetElementsByTagName(StoredDataXML.ATTRIBUTE_NAME);
                value = nodo.GetElementsByTagName(StoredDataXML.ATTRIBUTE_VALUE);
                statistic.setAttribute(name[0].InnerText, Convert.ToInt32(value[0].InnerText));
            }
        }

        /// <summary>
        /// Allows to store the game information, inside XML file, in memory.
        /// </summary>
        private void loadGamesFromXml()
        {
            gamePlayStoredDataXML = new StoredDataXML(GameInstance.XML_FILE_PATH, GameInstance.PLURAL_TAG, GameInstance.SINGULAR_TAG);
            // Create nodes to get the information in the xml file.
            XmlNodeList name, level, nodeList = gamePlayStoredDataXML.getStoredElements();
            GameInstance game;
            int i = 0;
            // Store xml data in memory.
            games = new GameInstance[nodeList.Count];

            foreach (XmlElement nodo in nodeList)
            {
                // Gets game info.
                name = nodo.GetElementsByTagName(GameInstance.NAME_ATTRIBUTE);
                game = new GameInstance(name[0].InnerText);
                level = nodo.GetElementsByTagName(GameInstance.LEVEL_ATTRIBUTE);
                setGameLevels(game, level);
                games[i] = game;
                i++;
            }
        }
        
        /// <summary>
        /// Sets the level information to a specific game to store it in memory. VER LUEGO !!!!!!
        /// </summary>
        private void setGameLevels(GameInstance game, XmlNodeList levels)
        {
            //XmlNodeList gravity, elements, time, movements, attribute;
            Level level = null;

            foreach (XmlElement nodo in levels)
            {
                level = new Level();

                foreach (XmlElement childNodo in nodo.ChildNodes)
                {
                    if (!childNodo.Name.Equals("multiple"))
                        level.addAttribute(childNodo.Name, childNodo.InnerText);
                    else
                        addMultipleAttibutes(childNodo.ChildNodes, level);
                }

                // Get the level information.
                //gravity = nodo.GetElementsByTagName("gravity");
                //elements = nodo.GetElementsByTagName("elements");
                //time = nodo.GetElementsByTagName("time");
                //movements = nodo.GetElementsByTagName("move");

                //level = new Level();
                //level.addAttribute("gravity", Convert.ToInt32(gravity[0].InnerText));
                //level.addAttribute("elements", Convert.ToInt32(elements[0].InnerText));
                //level.addAttribute("time", Convert.ToInt32(time[0].InnerText));

                //for (int i = 0; i < movements.Count;i++ )
                //    level.addAttribute("move"+ i, movements[i].InnerText);

                game.addLevel(level);
            }
        }

        private void addMultipleAttibutes(XmlNodeList multipleNodeList, Level level)
        {
            string attributeName = multipleNodeList[0].Name;
            string[] multipleAttributes = new string[multipleNodeList.Count];

            for (int i = 0; i < multipleNodeList.Count; i++)
            {
                multipleAttributes[i] = multipleNodeList[i].InnerText;
            }

            level.addAttribute(attributeName, multipleAttributes);
        }

        /// <summary>
        /// Store user information from memory to the xml file.
        /// </summary>
        public void storeUsersToXml()
        {
            userStoredDataXML.setStoredElements(users);
        }

        /// <summary>
        /// Add a new user to the memory storage.
        /// </summary>
        public void addNewUser(User newUser)
        {
            users.Add(newUser);
        }

        /// <summary>
        /// Delete a user from the memory storage.
        /// </summary>
        public void deleteUser(int index)
        {
            users.RemoveAt(index);
        }

        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (GameScreen screen in screens)
                screen.Unload();
        }

        /// <summary>
        /// Update the statistics to the user about the last game played.
        /// </summary>
        public void AddStatistic(Statistics stat)
        {
            Statistics aux = users[userIndex].getStatistics(currentGame);
            aux.setAttribute("ganados", (int)aux.getAttribute("ganados") + (int)stat.getAttribute("ganados"));
            aux.setAttribute("perdidos", (int)aux.getAttribute("perdidos") + (int)stat.getAttribute("perdidos"));
            aux.setAttribute("tiempo jugado", (int)aux.getAttribute("tiempo jugado") + (int)stat.getAttribute("tiempo jugado"));
            aux.setAttribute("máximo nivel alcanzado", (int)aux.getAttribute("máximo nivel alcanzado") + (int)stat.getAttribute("máximo nivel alcanzado"));
            storeUsersToXml();
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Read the keyboard and gamepad.
            input.Update();
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            tempScreensList.Clear();

            foreach (GameScreen screen in screens)
                tempScreensList.Add(screen);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (tempScreensList.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                GameScreen screen = tempScreensList[tempScreensList.Count - 1];
                tempScreensList.RemoveAt(tempScreensList.Count - 1);
                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(gameTime, input);
                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }

        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (GameScreen screen in screens)
            {
                if (screen.ScreenState == ScreenState.Hidden)
                    continue;

                screen.Draw(gameTime);
            }
        }

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
        {
            screen.ControllingPlayer = controllingPlayer;
            screen.setScreenManager(this);
            screen.IsExiting = false;

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
                screen.Activate(false);

            screens.Add(screen);
        }

        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(GameScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.Unload();
            }

            screens.Remove(screen);
            tempScreensList.Remove(screen);
        }

        /// <summary>
        /// Expose an array holding all the screens. We return a copy rather
        /// than the real master list, because screens should only ever be added
        /// or removed using the AddScreen and RemoveScreen methods.
        /// </summary>
        public GameScreen[] GetScreens()
        {
            return screens.ToArray();
        }

        /// <summary>
        /// Helper draws a translucent black fullscreen sprite, used for fading
        /// screens in and out, and for darkening the background behind popups.
        /// </summary>
        public void FadeBackBufferToBlack(float alpha, Rectangle box)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(textures[TEXTURE_BLANK], box, Color.Black * alpha);
            spriteBatch.End();
        }

        /// <summary>
        /// Informs the screen manager to serialize its state to disk.
        /// </summary>
        public void Deactivate()
        {
            return;
        }

        /// <summary>
        /// Gets a specific font.
        /// </summary>
        public SpriteFont getFont(String typeFont)
        {
            return fonts[typeFont];
        }

        /// <summary>
        /// Gets a specific texture.
        /// </summary>
        public Texture2D getTexture(String nameTexture)
        {
            return textures[nameTexture];
        }

        /// <summary>
        /// Gets width screen.
        /// </summary>
        public int getWidthScreen()
        {
            return this.Game.Window.ClientBounds.Width;
        }

        /// <summary>
        /// Gets height screen.
        /// </summary>
        public int getHeightScreen()
        {
            return this.Game.Window.ClientBounds.Height;
        }

        /// <summary>
        // Return the basicEffect asociated.
        public BasicEffect getBasicEffect()
        {
            return basicEffect;
        }

        /// <summary>
        /// Return the kinect instance.
        /// </summary>
        public KinectSDK Kinect
        {
            get { return kinect; }
        }

        /// <summary>
        /// Allows to set the current game in the system.
        /// </summary>
        public void setCurrentGame(int i)
        {
            currentGame = i;
        }

        /// <summary>
        /// Return the current game.
        /// </summary>
        public GameInstance getCurrentGame()
        {
            return games[currentGame];
        }

        /// <summary>
        /// Return the index game.
        /// </summary>
        public int getIndexGame()
        {
            return currentGame;
        }

        /// <summary>
        /// Set the user index.
        /// </summary>
        public void setUserIndex(int i)
        {
            userIndex = i;
        }

        /// <summary>
        /// Return the user index.
        /// </summary>
        public int getUserIndex()
        {
            return userIndex;
        }

        /// <summary>
        /// Return the statistics attributes for the current user.
        /// </summary>
        public string[] getCurrentStatisticsItems()
        {
            // Determine if the current user has statistics.
            if (users[userIndex].containStatistic(currentGame))
            {
                Statistics stat = users[userIndex].getStatistics(currentGame);
                string[] keysList = stat.getKeys();
                string[] aux = new string[keysList.Length];
                // Fill the string array with the attributes and values.
                for (int i = 0; i < keysList.Length; i++)
                    aux[i] = keysList[i] + " " + stat.getAttribute(keysList[i]).ToString();

                return aux;
            }
            else
                return new string[0];
        }
    }
}