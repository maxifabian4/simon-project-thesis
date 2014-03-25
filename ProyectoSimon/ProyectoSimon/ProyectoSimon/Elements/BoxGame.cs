using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProyectoSimon.Elements;


namespace ProyectoSimon
{
    class BoxGame 
    {
        private Rectangle rec;
        private String property;

        public BoxGame(int a1, int a2, int a3, int a4)
        {
            rec = new Rectangle(a1,a2,a3,a4);
            property = "";
        }

        public void setProperty(String s)
        {
            property = s;
        }

        public String getProperty()
        {
            return property;
        }

        public bool inside(Vector2 pos)
        {
            return rec.Contains((int) pos.X, (int) pos.Y);
        }

        public bool isStorable(ElementPhysic elementPhysic)
        {
            // Change !! !!
            if (property.Equals(((Circle)elementPhysic).getKind()))
                return true;
            else
                return false;
        }
    }
}
