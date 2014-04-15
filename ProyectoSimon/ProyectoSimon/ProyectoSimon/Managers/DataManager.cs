using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon
{
    public class DataManager
    {
        private static DataManager instance;
        private GraphicsDevice graphicDevice;
        private List<User> users;
        private GameInstance[] games;
        private StoredDataXML userStoredDataXML, gamePlayStoredDataXML;
        private int userIndex;
        private int currentGame;
        /// <summary>
        /// Class constructor.
        /// </summary>
        private DataManager()
        {
            users = new List<User>();
            userIndex = 0;
            currentGame = 0;
        }
        /// <summary>
        /// It returns a class instance.
        /// </summary>
        public static DataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataManager();
                }
                return instance;
            }
        }
        /// <summary>
        /// It loads users and games from xml files.
        /// </summary>
        public void initialize()
        {
            // Load the store users from XML file.
            loadUsersFromXml();
            // Load instances.
            loadGamesFromXml();
        }
        /// <summary>
        /// Sets a game Graphic Device.
        /// </summary>
        public void setGraphicDevice(GraphicsDevice gd)
        {
            graphicDevice = gd;
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
                photo = Texture2D.FromStream(graphicDevice, stream);
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
                game.addLevel(level);
            }
        }
        /// <summary>
        /// It adds a multiple attributes.
        /// </summary>
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
        /// Update the statistics to the user about the last game played.
        /// </summary>
        public void AddStatistic(Statistics stat)
        {
            Statistics aux = users[userIndex].getStatistics(currentGame);
            aux.setAttribute("ganados", (int) aux.getAttribute("ganados") + (int) stat.getAttribute("ganados"));
            aux.setAttribute("perdidos", (int) aux.getAttribute("perdidos") + (int) stat.getAttribute("perdidos"));
            aux.setAttribute("tiempo jugado", (int) aux.getAttribute("tiempo jugado") + (int) stat.getAttribute("tiempo jugado"));
            aux.setAttribute("máximo nivel alcanzado", (int) aux.getAttribute("máximo nivel alcanzado") + (int) stat.getAttribute("máximo nivel alcanzado"));
            storeUsersToXml();
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
    }
}