using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace ProyectoSimon.Elements
{
    class Ground : GameElement
    {
        private Vector2 v1, v2;

        public Ground(World world, Vector2 v1, Vector2 v2) {
            this.v1 = v1 / PIXELS_TO_METERS;
            this.v2 = v2 / PIXELS_TO_METERS;
            makeGround(world);
        }

        private void makeGround(World world)
        {
            // Create a ground.
            Body groundBody = world.CreateBody(new BodyDef());
            PolygonShape groundEdge = new PolygonShape();
            groundEdge.SetAsEdge(v1, v2);
            FixtureDef fdGroundBox = new FixtureDef();
            fdGroundBox.shape = groundEdge;
            groundBody.CreateFixture(groundEdge, 0.0f);
            groundBody.SetUserData(this);
        }

        public override void display(ScreenManager screenManager) {
            //quad = new ElementPolygon(0, bheight - 30, bwidth, 30, new Color(124, 107, 70), 1, true);
            //quad.drawPrimitive(screenManager);
            //fillQuad = new ElementPolygon(0, bheight - 30, bwidth, 30, new Color(152, 131, 87), 1, false);
            //fillQuad.drawPrimitive(screenManager);
        }

        public override void change(Color mColor, Color sColor) { }
    }
}
