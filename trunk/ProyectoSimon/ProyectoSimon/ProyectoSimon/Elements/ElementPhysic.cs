using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace ProyectoSimon.Elements
{
    abstract class ElementPhysic
    {
        protected const int PIXELS_TO_METERS = 30;

        public abstract void display(ScreenManager screenManager);
        public abstract void change(Color mColor, Color sColor);
    }
}
