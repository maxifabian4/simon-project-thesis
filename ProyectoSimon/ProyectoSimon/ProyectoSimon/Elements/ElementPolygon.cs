using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace ProyectoSimon
{
    /// <summary>
    /// Represents a geometric object. In this case it represents simple circle.
    /// </summary>
    public class ElementPolygon : ElementShape
    {
        private const int MAX_VERTEX_POLYGON = 100;
        private int vertexCount;
        private FixedArray8<Vector2> vertexs;
        private float alpha;

        /// <summary>
        /// This constructor is used to create a new Polygon with a fill color and with a transparence.
        /// </summary>
        public ElementPolygon(FixedArray8<Vector2> vert, PrimitiveType type, Color backGroundColor, float a, int n)
        {
            vertexs = vert;
            VertexData = new VertexPositionColor[MAX_VERTEX_POLYGON];
            PrimitiveCount = 0;
            VertexOffset = 0;
            vertexCount = n;
            alpha = a;
            PrimitiveType = type;

            if (type == PrimitiveType.TriangleList)
            {
                createSolidPolygon(vertexs, backGroundColor * alpha);
            }
            else
            {
                createPolygon(vertexs, backGroundColor * alpha);
            }
        }

        /// <summary>
        /// This constructor is used to create a new Polygon with a transparence and without a fill color.
        /// </summary>
        public ElementPolygon(float x, float y, float width, float height, Color backGroundColor, float alpha, Boolean isSolid)
        {
            FixedArray8<Vector2> vertexs = new FixedArray8<Vector2>();

            VertexData = new VertexPositionColor[MAX_VERTEX_POLYGON];
            PrimitiveCount = 0;
            VertexOffset = 0;
            vertexCount = 4;

            vertexs[0] = new Vector2(x, y);
            vertexs[1] = new Vector2(x + width, y);
            vertexs[2] = new Vector2(x + width, y + height);
            vertexs[3] = new Vector2(x, y + height);

            if (isSolid)
            {
                PrimitiveType = PrimitiveType.TriangleList;
                createSolidPolygon(vertexs, backGroundColor * alpha);
            }
            else
            {
                PrimitiveType = PrimitiveType.LineList;
                createPolygon(vertexs, backGroundColor * alpha);
            }
        }
        /// <summary>
        /// Creates a geometric polygon using primitives and a fill color.
        /// </summary>
        private void createSolidPolygon(FixedArray8<Vector2> vertexs, Color colorFill)
        {
            PrimitiveCount = 0;

            for (int i = 1; i < vertexCount - 1; i++)
            {
                VertexData[PrimitiveCount * 3].Position = new Vector3(vertexs[0], 0.0f);
                VertexData[PrimitiveCount * 3].Color = colorFill;

                VertexData[PrimitiveCount * 3 + 1].Position = new Vector3(vertexs[i], 0.0f);
                VertexData[PrimitiveCount * 3 + 1].Color = colorFill;

                VertexData[PrimitiveCount * 3 + 2].Position = new Vector3(vertexs[i + 1], 0.0f);
                VertexData[PrimitiveCount * 3 + 2].Color = colorFill;

                PrimitiveCount++;
            }
        }

        /// <summary>
        /// Creates a geometric circle using primitives.
        /// </summary>
        private void createPolygon(FixedArray8<Vector2> vertexs, Color color)
        {
            PrimitiveCount = 0;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                VertexData[PrimitiveCount * 2].Position = new Vector3(vertexs[i], 0.0f);
                VertexData[PrimitiveCount * 2].Color = color;
                VertexData[PrimitiveCount * 2 + 1].Position = new Vector3(vertexs[i + 1], 0.0f);
                VertexData[PrimitiveCount * 2 + 1].Color = color;
                PrimitiveCount++;
            }

            VertexData[PrimitiveCount * 2].Position = new Vector3(vertexs[vertexCount - 1], 0.0f);
            VertexData[PrimitiveCount * 2].Color = color;
            VertexData[PrimitiveCount * 2 + 1].Position = new Vector3(vertexs[0], 0.0f);
            VertexData[PrimitiveCount * 2 + 1].Color = color;
            PrimitiveCount++;
        }

        /// <summary>
        /// Draws a shape border specifying a size.
        /// </summary>
        public override void drawBorderWeigth(ScreenManager sManager, Color color, float size)
        {
            if (size > 0)
            {
                ElementPolygon polygonBorder;

                for (float i = 0; i < size; i += .05f)
                {
                    for (int j = 0; j < vertexCount; j++)
                        vertexs[j] += new Vector2(i, i);

                    polygonBorder = new ElementPolygon(vertexs, PrimitiveType.LineList, color, alpha, vertexCount);
                    polygonBorder.draw(sManager);
                }
            }
        }

    }

}
