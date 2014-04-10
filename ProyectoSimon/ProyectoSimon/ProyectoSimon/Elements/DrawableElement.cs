using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon
{
    /// <summary>
    /// Abstract class developed in order represent a drawable object with the 
    /// ability to be displayed by itself.
    /// </summary>
    public abstract class DrawableElement
    {
        /// <summary>
        /// Ability to draw itself.
        /// </summary>
        public abstract void draw(SpriteBatch spriteBatch, BasicEffect basicEffect);
    }

}
