using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace ProyectoSimon
{
    public class ElementPolygon : ElementShape
    {
        private const int MAX_VERTEX_POLYGON = 100;
        private int vertexCount;
        private FixedArray8<Vector2> vertexs;
        private float alpha;

        public ElementPolygon(FixedArray8<Vector2> vert, PrimitiveType type, Color backGroundColor, float a, int n)
        {
            vertexs = vert;
            vertexData = new VertexPositionColor[MAX_VERTEX_POLYGON];
            primitiveCount = 0;
            vertexOffset = 0;
            vertexCount = n;
            alpha = a;
            primitiveType = type;

            if (type == PrimitiveType.TriangleList)
                createSolidPolygon(vertexs, backGroundColor * alpha);
            else
                createPolygon(vertexs, backGroundColor * alpha);
            //}
            //else
            //{
            //    primitiveType = PrimitiveType.LineList;
            //    createPolygon(vertexs, backGroundColor * alpha);
            //}
        }

        public ElementPolygon(float x, float y, float width, float height, Color backGroundColor, float alpha, Boolean isSolid)
        {
            FixedArray8<Vector2> vertexs = new FixedArray8<Vector2>();

            vertexData = new VertexPositionColor[MAX_VERTEX_POLYGON];
            primitiveCount = 0;
            vertexOffset = 0;
            vertexCount = 4;

            vertexs[0] = new Vector2(x, y);
            vertexs[1] = new Vector2(x + width, y);
            vertexs[2] = new Vector2(x + width, y + height);
            vertexs[3] = new Vector2(x, y + height);

            if (isSolid)
            {
                primitiveType = PrimitiveType.TriangleList;
                createSolidPolygon(vertexs, backGroundColor * alpha);
            }
            else
            {
                primitiveType = PrimitiveType.LineList;
                createPolygon(vertexs, backGroundColor * alpha);
            }
        }

        private void createSolidPolygon(FixedArray8<Vector2> vertexs, Color colorFill)
        {
            primitiveCount = 0;

            for (int i = 1; i < vertexCount - 1; i++)
            {
                vertexData[primitiveCount * 3].Position = new Vector3(vertexs[0], 0.0f);
                vertexData[primitiveCount * 3].Color = colorFill;
                //vertexData[primitiveCount * 3].Color = Color.LightGray * .5f;

                vertexData[primitiveCount * 3 + 1].Position = new Vector3(vertexs[i], 0.0f);
                vertexData[primitiveCount * 3 + 1].Color = colorFill;

                vertexData[primitiveCount * 3 + 2].Position = new Vector3(vertexs[i + 1], 0.0f);
                vertexData[primitiveCount * 3 + 2].Color = colorFill;

                primitiveCount++;
            }
        }

        private void createPolygon(FixedArray8<Vector2> vertexs, Color color)
        {
            primitiveCount = 0;

            for (int i = 0; i < vertexCount - 1; i++)
            {
                vertexData[primitiveCount * 2].Position = new Vector3(vertexs[i], 0.0f);
                vertexData[primitiveCount * 2].Color = color;
                vertexData[primitiveCount * 2 + 1].Position = new Vector3(vertexs[i + 1], 0.0f);
                vertexData[primitiveCount * 2 + 1].Color = color;
                primitiveCount++;
            }

            vertexData[primitiveCount * 2].Position = new Vector3(vertexs[vertexCount - 1], 0.0f);
            vertexData[primitiveCount * 2].Color = color;
            vertexData[primitiveCount * 2 + 1].Position = new Vector3(vertexs[0], 0.0f);
            vertexData[primitiveCount * 2 + 1].Color = color;
            primitiveCount++;
        }

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
