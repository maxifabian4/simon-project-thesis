using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyectoSimon
{
    public class GameInstance
    {
        public static string NAME_ATTRIBUTE = "gamename";
        public static string LEVEL_ATTRIBUTE = "level";
        public static string SINGULAR_TAG = "game";
        public static string PLURAL_TAG = "games";
        public static string XML_FILE_PATH = "Data//games.xml";
        private IList<Level> levels;
        private String name;
        /// <summary>
        /// Class constructor.
        /// </summary>
        public GameInstance(String n)
        {
            name = n;
            levels = new List<Level>();
        }
        /// <summary>
        /// Adds a new level in levels list.
        /// </summary>
        public void addLevel(Level l)
        {
            levels.Add(l);
        }
        /// <summary>
        /// Returns a level collection.
        /// </summary>
        public IList<Level> getLevels()
        {
            return levels;
        }
        /// <summary>
        /// Return a game level name.
        /// </summary>
        public String getName()
        {
            return name;
        }
    }
}