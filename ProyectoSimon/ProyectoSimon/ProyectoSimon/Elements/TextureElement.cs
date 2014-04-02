using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon
{
    /// <summary>
    /// Represents a texture object in the system, capable of store a texture to be represented.
    /// </summary>
    class TextureElement : DrawableElement
    {
        private Texture2D texture;
        private Vector2 position, origin;
        private Color textureColor;

        public TextureElement(Texture2D texture, Vector2 pos, Vector2 origin, Color tColor)
        {
            this.texture = texture;
            this.position = pos;
            this.origin = origin;
            this.textureColor = tColor;
        }

        /// <summary>
        /// Ability to draw itself.
        /// </summary>
        public override void draw(ScreenManager screenManager)
        {
            screenManager.SpriteBatch.Begin();
            screenManager.SpriteBatch.Draw(texture, position, null, textureColor, 0, origin, 1, SpriteEffects.None, 0);
            screenManager.SpriteBatch.End();
        }

    }

}
