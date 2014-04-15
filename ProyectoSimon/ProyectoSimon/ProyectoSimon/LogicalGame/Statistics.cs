using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyectoSimon
{
    public class Statistics
    {
        private IDictionary<string, object> attributes;
        private String gameName;
        /// <summary>
        /// Class constructor.
        /// </summary>
        public Statistics(String name)
        {
            gameName = name;
            attributes = new Dictionary<string, object>();

            attributes.Add("ganados", 0);
            attributes.Add("perdidos", 0);
            attributes.Add("tiempo jugado", 0);
            attributes.Add("máximo nivel alcanzado", 0);
        }
        /// <summary>
        /// Increases won count.
        /// </summary>
        public void incWon()
        {
            int value = (int) attributes["ganados"];
            attributes["ganados"] = ++value;
        }
        /// <summary>
        /// Increases lost count.
        /// </summary>
        public void incLost()
        {
            int value = (int) attributes["perdidos"];
            attributes["perdidos"] = ++value;
        }
        /// <summary>
        /// Increases time count.
        /// </summary>
        public void incTime(int time)
        {
            int value = (int) attributes["tiempo jugado"];
            attributes["tiempo jugado"] = value + time;
        }
        /// <summary>
        /// Sets level max.
        /// </summary>
        public void setLevel(int level)
        {
            attributes["máximo nivel alcanzado"] = level;
        }
        /// <summary>
        /// Retunrs an attribute found by a key.
        /// </summary>
        public object getAttribute(string key)
        {
            return attributes[key];
        }
        /// <summary>
        /// Sets an attribute in a collection.
        /// </summary>
        public void setAttribute(string key, object value)
        {
            attributes[key] = value;
        }
        /// <summary>
        /// Adds an attribute in a collection.
        /// </summary>
        public void addAttribute(string key, object value)
        {
            attributes.Add(key, value);
        }
        /// <summary>
        /// Returns a game name.
        /// </summary>
        public String getGameName()
        {
            return gameName;
        }
        /// <summary>
        /// Returns a key collection about attributes structure contain.
        /// </summary>
        public string[] getKeys()
        {
            return attributes.Keys.ToList().ToArray();
        }
    }
}
