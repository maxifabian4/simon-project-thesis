using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProyectoSimon
{
    public class Level
    {
        private IDictionary<String, Object> attributes;
        /// <summary>
        /// Class constructor.
        /// </summary>
        public Level()
        {
            attributes = new Dictionary<String, Object>();
        }
        /// <summary>
        /// It adds an attribute (name,key).
        /// </summary>
        public void addAttribute(String name, Object value)
        {
            attributes.Add(name, value);
        }
        /// <summary>
        /// It returns a class instance.
        /// </summary>
        public Object getAttribute(String name)
        {
            if (attributes.Keys.Contains(name))
                return attributes[name];
            else
                return null;
        }
        /// <summary>
        /// It sets an attribute found by name.
        /// </summary>
        public void setAttribute(String name, Object value)
        {
            attributes[name] = value;
        }
        /// <summary>
        /// It returns true if an attribute exist.
        /// </summary>
        public bool exist(string attributeName)
        {
            return attributes.Keys.Contains(attributeName);
        }
    }
}
