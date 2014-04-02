using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProyectoSimon
{
    /// <summary>
    /// Represents a geometry object. It can take different shapes.
    /// </summary>
    public abstract class ElementShape : DrawableElement
    {
        private VertexPositionColor[] vertexData;
        private PrimitiveType primitiveType;
        private int primitiveCount;
        private int vertexOffset;

        /// <summary>
        /// Describes a custom vertex format structure that contains position and color
        /// information.
        /// </summary>
        protected VertexPositionColor[] VertexData
        {
            get
            {
                return vertexData;
            }

            set
            {
                vertexData = value;
            }
        }

        /// <summary>
        /// Defines how vertex data is ordered (TriangleList - TriangleStrip - LineList - LineStrip).
        /// </summary>
        protected PrimitiveType PrimitiveType
        {
            get
            {
                return primitiveType;
            }
            
            set
            {
                primitiveType = value;
            }
        }

        /// <summary>
        /// Number of primitives to render.
        /// </summary>
        protected int PrimitiveCount
        {
            get
            {
                return primitiveCount;
            }

            set
            {
                primitiveCount = value;
            }
        }

        /// <summary>
        /// Offset (in vertices) from the biginning of the buffer to start reading data.
        /// </summary>
        protected int VertexOffset
        {
            get
            {
                return vertexOffset;
            }

            set
            {
                vertexOffset = value;
            }
        }

        /// <summary>
        /// Draws a shape border specifying a size.
        /// </summary>
        public abstract void drawBorderWeigth(ScreenManager sManager, Color color, float size);

        /// <summary>
        /// Ability to draw itself.
        /// </summary>
        public override void draw(ScreenManager screenManager)
        {
            screenManager.getBasicEffect().CurrentTechnique.Passes[0].Apply();

            screenManager.SpriteBatch.Begin();
            screenManager.SpriteBatch.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                PrimitiveType,
                VertexData,
                VertexOffset,
                PrimitiveCount);
            screenManager.SpriteBatch.End();
        }

    }

}
