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

        public GameInstance(String n)
        {
            name = n;
            levels = new List<Level>();
        }
        public void addLevel(Level l)
        {
            levels.Add(l);
        }

        public IList<Level> getLevels()
        {
            return levels;
        }

        public String getName()
        {
            return name;
        }
    }
}