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

        public Statistics(String name)
        {
            gameName = name;
            attributes = new Dictionary<string, object>();

            attributes.Add("ganados", 0);
            attributes.Add("perdidos", 0);
            attributes.Add("tiempo jugado", 0);
            attributes.Add("máximo nivel alcanzado", 0);
        }

        public void incWon()
        {
            int value = (int)attributes["ganados"];
            attributes["ganados"] = ++value;
        }

        public void incLost()
        {
            int value = (int)attributes["perdidos"];
            attributes["perdidos"] = ++value;
        }
        
        public void incTime(int time)
        {
            int value = (int)attributes["tiempo jugado"];
            attributes["tiempo jugado"] = value + time; 
        }

        public void setLevel(int level)
        {
            attributes["máximo nivel alcanzado"] = level;
        }

        public object getAttribute(string key)
        {
            return attributes[key];
        }

        public void setAttribute(string key, object value)
        {
            attributes[key] = value;
        }

        public void addAttribute(string key, object value)
        {
            attributes.Add(key, value);
        }

        public String getGameName()
        {
            return gameName;
        }

        public string[] getKeys()
        {
            return attributes.Keys.ToList().ToArray();
        }
    }
}
