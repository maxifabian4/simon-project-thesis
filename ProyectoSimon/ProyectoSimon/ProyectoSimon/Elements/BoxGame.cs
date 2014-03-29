using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using ProyectoSimon.Elements;


namespace ProyectoSimon
{
    class BoxGame : GameElement
    {
        private Rectangle rec;
        private String property;
        private Color color;

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

        public bool isStorable(GameElement element)
        {
            // Change !! !!
            if (property.Equals(((Circle)element).getKind()))
                return true;
            else
                return false;
        }

        public override void change(Color mColor, Color sColor) { }

        public override void display(ScreenManager screenManager) {
            TextureElement element = new TextureElement(screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX),new Vector2(rec.X, rec.Y),
                                        new Vector2(screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX).Width / 3 + 100, 
                                        screenManager.getTexture(ScreenManager.TEXTURE_GAMEBOX).Height / 2),color);
            element.draw(screenManager);            
        }
       
        
    }
}
