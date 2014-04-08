using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProyectoSimon.Elements;

namespace ProyectoSimon
{

    /// <summary>
    /// Represents a simple box using the Rectangle class and textures.
    /// </summary>
    public class BoxGame : GameElement
    {
        private Rectangle rec;
        // Represents a value associated to a box element.
        private String property;
        private Color color;

        
        public BoxGame(int p1, int p2, int p3, int p4)
        {
            rec = new Rectangle(p1, p2, p3, p4);
            color = Color.White;
            property = "";
        }

        public void setProperty(String property)
        {
            this.property = property;
        }

        public String getProperty()
        {
            return property;
        }

        /// <summary>
        /// Determines if a point belongs to the box.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>Returns true in the affirmative case.</returns>
        public bool inside(Vector2 pos)
        {
            return rec.Contains((int) pos.X, (int) pos.Y);
        }

        /// <summary>
        /// Determines if an element with the same property is associated or not.
        /// </summary>
        /// <param name="element">Game element</param>
        /// <returns>Returns true in the affirmative case.</returns>
        public bool isStorable(Circle element)
        {
            if (property.Equals(element.getProperty()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add logic if we want to change the object appearance after an specific event occurs.
        /// </summary>
        /// <param name="mColor">Main color.</param>
        /// <param name="sColor">Additional color.</param>
        public override void change(Color mColor, Color sColor)
        {
        }

        /// <summary>
        /// Renderizes a box element using testures.
        /// </summary>
        /// <param name="screenManager">Screen manager used to renderize.</param>
        public override void display(ScreenManager screenManager)
        {
            TextureElement element = new TextureElement(
                GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX),
                new Vector2(rec.X, rec.Y),
                new Vector2(GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Width / 3 + 100,
                GameContentManager.Instance.getTexture(GameContentManager.TEXTURE_GAMEBOX).Height / 2),
                color);
            element.draw(screenManager);
        }

    }

}
