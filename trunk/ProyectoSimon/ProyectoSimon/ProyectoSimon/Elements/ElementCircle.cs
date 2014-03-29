using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace ProyectoSimon
{
    public class ElementCircle : ElementShape
    {
        private const int MAX_VERTEX_POLYGON = 1000;
        private float radius;
        private int segments;
        private Vector2 center;

        public ElementCircle(float r, Vector2 c, Color mainColor, Color secondColor, float alpha)
        {
            initializeAttributes(r, c);
            primitiveType = PrimitiveType.TriangleList;
            createSolidCircle(mainColor * alpha, secondColor * alpha);
        }

        private void initializeAttributes(float r, Vector2 c)
        {
            radius = r;
            center = c;
            segments = 100;
            vertexData = new VertexPositionColor[MAX_VERTEX_POLYGON];
            primitiveCount = 0;
            vertexOffset = 0;
        }

        public ElementCircle(float r, Vector2 c, Color mainColor, float alpha)
        {
            initializeAttributes(r, c);
            primitiveType = PrimitiveType.LineList;
            createCircle(mainColor * alpha);
        }

        private void createCircle(Color color)
        {
            Vector2 v1, v2;
            double increment = Math.PI * 2.0 / (double) segments;
            double theta = 0.0;
            primitiveCount = 0;

            for (int i = 0; i < segments; i++)
            {
                v1 = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                v2 = center + radius * new Vector2((float) Math.Cos(theta + increment), (float) Math.Sin(theta + increment));

                vertexData[primitiveCount * 2].Position = new Vector3(v1, 0.0f);
                vertexData[primitiveCount * 2].Color = color;
                vertexData[primitiveCount * 2 + 1].Position = new Vector3(v2, 0.0f);
                vertexData[primitiveCount * 2 + 1].Color = color;
                primitiveCount++;
                theta += increment;
            }
        }

        private void createSolidCircle(Color mainColor, Color secondColor)
        {
            Vector2 v1, v2;
            double increment = Math.PI * 2.0 / (double) segments;
            double theta = 0.0;
            primitiveCount = 0;

            Vector2 v0 = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
            theta += increment;

            for (int i = 1; i < segments - 1; i++)
            {
                v1 = center + radius * new Vector2((float) Math.Cos(theta), (float) Math.Sin(theta));
                v2 = center + radius * new Vector2((float) Math.Cos(theta + increment), (float) Math.Sin(theta + increment));

                vertexData[primitiveCount * 3].Position = new Vector3(v0, 0.0f);
                vertexData[primitiveCount * 3].Color = secondColor; // new Color(227, 117, 64);

                vertexData[primitiveCount * 3 + 1].Position = new Vector3(v1, 0.0f);
                vertexData[primitiveCount * 3 + 1].Color = mainColor;

                vertexData[primitiveCount * 3 + 2].Position = new Vector3(v2, 0.0f);
                vertexData[primitiveCount * 3 + 2].Color = mainColor;

                primitiveCount++;

                theta += increment;
            }
        }

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

