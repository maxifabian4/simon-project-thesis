using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

/**
 * 
 * ELIMINAR OBSOLETO!!!!
 * 
 */

namespace ProyectoSimon
{
    public abstract class StoredData
   { 
        private string path;

        public string getPath()
        {
            return path;
        }

        public void setPath(string p)
        {
            path = p;
        }

        public abstract XmlNodeList getStoredElements();

        public abstract void setStoredElements(List<Object> elems);
    }
}
