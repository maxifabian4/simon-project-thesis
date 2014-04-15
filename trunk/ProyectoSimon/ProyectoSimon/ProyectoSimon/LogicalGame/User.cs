using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon
{
    public class User
    {
        private string name, surname, path;
        private int age;
        private Texture2D picture;
        private IDictionary<int, Statistics> gameStatistics;
        /// <summary>
        /// Class constructor.
        /// </summary>
        public User(string n, string s, string p, int a, Texture2D t)
        {
            name = n;
            surname = s;
            path = p;
            age = a;
            picture = t;
            gameStatistics = new Dictionary<int, Statistics>();
        }
        /// <summary>
        /// Returns user name.
        /// </summary>
        public string getName()
        {
            return name;
        }
        /// <summary>
        /// Returns user surname.
        /// </summary>
        public string getSurname()
        {
            return surname;
        }
        /// <summary>
        /// Returns user picture path.
        /// </summary>
        public string getPath()
        {
            return path;
        }
        /// <summary>
        /// Returns user age.
        /// </summary>
        public int getAge()
        {
            return age;
        }
        /// <summary>
        /// Returns user picture.
        /// </summary>
        public Texture2D getPicture()
        {
            return picture;
        }
        /// <summary>
        /// Returns user statistics by game.
        /// </summary>
        public Statistics getStatistics(int index)
        {
            return gameStatistics[index];
        }
        /// <summary>
        /// Adds user statistics.
        /// </summary>
        public void addStatistic(int indexGame, Statistics stat)
        {
            if (gameStatistics.ContainsKey(indexGame))
                gameStatistics[indexGame] = stat;
            else
                gameStatistics.Add(indexGame, stat);
        }
        /// <summary>
        /// Returns user statistics count.
        /// </summary>
        public int getStatisticsCount()
        {
            return gameStatistics.Count;
        }
        /// <summary>
        /// Returns true if a game statistics is contain in gameStatistics collection.
        /// </summary>
        internal bool containStatistic(int currentGame)
        {
            return gameStatistics.ContainsKey(currentGame);
        }
    }
}
