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

        public User(string n, string s, string p,int a, Texture2D t)
        {
            name = n;
            surname = s;
            path = p;
            age = a;
            picture = t;
            gameStatistics = new Dictionary<int, Statistics>();
        }

        public string getName()
        {
            return name;
        }

        public string getSurname()
        {
            return surname;
        }

        public string getPath()
        {
            return path;
        }

        public int getAge()
        {
            return age;
        }

        public Texture2D getPicture()
        {
            return picture;
        }

        public Statistics getStatistics(int index)
        {
            return gameStatistics[index];
        }

        public void addStatistic(int indexGame, Statistics stat)
        {
            if (gameStatistics.ContainsKey(indexGame))
                gameStatistics[indexGame] = stat;
            else
                gameStatistics.Add(indexGame, stat);
        }

        public int getStatisticsCount()
        {
            return gameStatistics.Count;
        }

        internal bool containStatistic(int currentGame)
        {
            return gameStatistics.ContainsKey(currentGame);
        }
    }
}
