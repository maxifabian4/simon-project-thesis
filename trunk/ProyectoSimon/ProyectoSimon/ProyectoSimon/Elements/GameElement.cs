using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon.Elements
{

    /// <summary>
    /// Abstract class created to represent a simple entity in the game.
    /// Basically, we can create circles, boxes, polygons, and so on.
    /// </summary>
    public abstract class GameElement
    {
        /// <summary>
        /// Displays the GameElement object depending on the instance.
        /// </summary>
        /// <param name="screenManager">Current screen manager.</param>
        public abstract void display(SpriteBatch spriteBatch, BasicEffect basicEffect);

        /// <summary>
        /// Changes its own properties after external events.
        /// </summary>
        /// <param name="mColor">Main color.</param>
        /// <param name="sColor">Second or background color.</param>
        public abstract void change(Color mColor, Color sColor);
    }

}
