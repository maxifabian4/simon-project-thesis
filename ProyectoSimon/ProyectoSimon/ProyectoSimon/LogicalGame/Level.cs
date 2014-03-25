using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyectoSimon
{
    public class Level
    {
        private IDictionary<String, Object> attributes;

        public Level() {
            attributes = new Dictionary<String, Object>();
        }

        public void addAttribute(String name, Object value) {
            attributes.Add(name, value);
        }

        public Object getAttribute(String name) {
            if (attributes.Keys.Contains(name))
                return attributes[name];
            else 
                return null;
        }

        public void setAttribute(String name, Object value)
        {
            attributes[name] = value;
        }

        public bool exist(string attributeName)
        {
            return attributes.Keys.Contains(attributeName);
        }
    }
}
