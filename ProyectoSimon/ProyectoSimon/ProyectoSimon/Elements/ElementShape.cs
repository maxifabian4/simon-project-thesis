using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProyectoSimon
{
    public abstract class ElementShape : DrawableElement
    {
        protected VertexPositionColor[] vertexData;
        protected PrimitiveType primitiveType;
        protected int primitiveCount;
        protected int vertexOffset;

        public override void draw(ScreenManager screenManager)
        {
            screenManager.getBasicEffect().CurrentTechnique.Passes[0].Apply();

            screenManager.SpriteBatch.Begin();
            screenManager.SpriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(primitiveType,
                                                                                   vertexData,
                                                                                   vertexOffset,
                                                                                   primitiveCount);
            screenManager.SpriteBatch.End();
        }

        public abstract void drawBorderWeigth(ScreenManager sManager, Color color, float size);
    }
}
