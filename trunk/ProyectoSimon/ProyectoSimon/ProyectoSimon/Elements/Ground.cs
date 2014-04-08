using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace ProyectoSimon.Elements
{

    /// <summary>
    /// Represents a physics ground element in Box2D.
    /// </summary>
    public class Ground : GameElement
    {
        // Initial (v1) and end point (v2) used to define a ground.
        private Vector2 v1;
        private Vector2 v2;

        public Ground(World world, Vector2 v1, Vector2 v2)
        {
            this.v1 = v1 / PIXELS_TO_METERS;
            this.v2 = v2 / PIXELS_TO_METERS;
            // Create the physics world.
            makeGround(world);
        }

        /// <summary>
        /// Creates a physics ground based on a previous defined world.
        /// </summary>
        /// <param name="world">Box2D world.</param>
        private void makeGround(World world)
        {
            // Create a ground.
            Body groundBody = world.CreateBody(new BodyDef());
            PolygonShape groundEdge = new PolygonShape();
            FixtureDef fdGroundBox = new FixtureDef();

            groundEdge.SetAsEdge(v1, v2);
            fdGroundBox.shape = groundEdge;
            groundBody.CreateFixture(groundEdge, 0.0f);
            groundBody.SetUserData(this);
        }

        /// <summary>
        /// Renderizes a box element using some logic. We can draw it using primitives or textures.
        /// </summary>
        /// <param name="screenManager">Main system manager.</param>
        public override void display(ScreenManager screenManager)
        {
        }

        /// <summary>
        /// Add logic if we want to change the object appearance after an specific event occurs.
        /// </summary>
        /// <param name="mColor">Main color.</param>
        /// <param name="sColor">Additional color.</param>
        public override void change(Color mColor, Color sColor)
        {
        }

    }

}
