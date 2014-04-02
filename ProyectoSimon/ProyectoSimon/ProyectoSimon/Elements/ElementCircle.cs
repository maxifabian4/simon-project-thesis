using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProyectoSimon
{
    /// <summary>
    /// Represents a geometric object. In this case it represents simple circle.
    /// </summary>
    public class ElementCircle : ElementShape
    {
        private const int MAX_VERTEX_POLYGON = 1000;
        private const int SEGMENTS = 100;
        private float radius;
        private Vector2 center;

        /// <summary>
        /// This constructor is used to create a new circle with a fill color and with a transparence.
        /// </summary>
        public ElementCircle(float r, Vector2 c, Color mainColor, Color secondColor, float alpha)
        {
            initializeAttributes(r, c);
            PrimitiveType = PrimitiveType.TriangleList;
            createSolidCircle(mainColor * alpha, secondColor * alpha);
        }

        /// <summary>
        /// This constructor is used to create a new circle without a fill color and with a transparence.
        /// </summary>
        public ElementCircle(float r, Vector2 c, Color mainColor, float alpha)
        {
            initializeAttributes(r, c);
            PrimitiveType = PrimitiveType.LineList;
            createCircle(mainColor * alpha);
        }

        /// <summary>
        /// Initializes the circle attributes by default.
        /// </summary>
        private void initializeAttributes(float r, Vector2 c)
        {
            radius = r;
            center = c;
            VertexData = new VertexPositionColor[MAX_VERTEX_POLYGON];
            PrimitiveCount = 0;
            VertexOffset = 0;
        }

        /// <summary>
        /// Creates a geometric circle using primitives.
        /// </summary>
        private void createCircle(Color color)
        {
            Vector2 v1, v2;
            double increment = Math.PI * 2.0 / (double) SEGMENTS;
            double theta = 0.0;
            PrimitiveCount = 0;

            for (int i = 0; i < SEGMENTS; i++)
            {
                v1 = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                v2 = center + radius * new Vector2((float) Math.Cos(theta + increment), (float) Math.Sin(theta + increment));

                VertexData[PrimitiveCount * 2].Position = new Vector3(v1, 0.0f);
                VertexData[PrimitiveCount * 2].Color = color;
                VertexData[PrimitiveCount * 2 + 1].Position = new Vector3(v2, 0.0f);
                VertexData[PrimitiveCount * 2 + 1].Color = color;
                PrimitiveCount++;
                theta += increment;
            }
        }

        /// <summary>
        /// Creates a geometric circle using primitives and a fill color.
        /// </summary>
        private void createSolidCircle(Color mainColor, Color secondColor)
        {
            Vector2 v1, v2;
            double increment = Math.PI * 2.0 / (double) SEGMENTS;
            double theta = 0.0;
            PrimitiveCount = 0;

            Vector2 v0 = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < SEGMENTS - 1; i++)
            {
                v1 = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                v2 = center + radius * new Vector2((float) Math.Cos(theta + increment), (float) Math.Sin(theta + increment));

                VertexData[PrimitiveCount * 3].Position = new Vector3(v0, 0.0f);
                VertexData[PrimitiveCount * 3].Color = secondColor;

                VertexData[PrimitiveCount * 3 + 1].Position = new Vector3(v1, 0.0f);
                VertexData[PrimitiveCount * 3 + 1].Color = mainColor;

                VertexData[PrimitiveCount * 3 + 2].Position = new Vector3(v2, 0.0f);
                VertexData[PrimitiveCount * 3 + 2].Color = mainColor;

                PrimitiveCount++;

                theta += increment;
            }
        }

        /// <summary>
        /// Draws a shape border specifying a size.
        /// </summary>
        public override void drawBorderWeigth(ScreenManager sManager, Color color, float size)
        {
            if (size > 0)
            {
                ElementCircle circleBorder;

                for (float i = 0; i < size; i += .5f)
                {
                    circleBorder = new ElementCircle(radius + i, center, color, 1);
                    circleBorder.draw(sManager);
                }
            }
        }

    }

}
