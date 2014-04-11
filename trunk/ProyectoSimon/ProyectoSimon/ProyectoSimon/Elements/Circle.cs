using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Box2D.XNA;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon.Elements
{

    /// <summary>
    /// Represents a physics circle in Box2D.
    /// </summary>
    public class Circle : GameElement
    {
        // Main and second color are used to simulate a gradient color effect.
        // EdgeColor used for the edge effect.
        private Color mainColor;
        private Color secondColor;
        private Color edgeColor;

        // Transparency effect.
        private float alpha;

        // Box2D base objects.
        private Body body;
        private Vector2 position;
        private float radius;

        // Game-logic properties.
        private Boolean kicked;
        private string property;

        // Physics world associated.
        private World physicsWorld;

        public Circle(World physicsWorld, Vector2 position, float radius, Boolean skeleton)
        {
            // Initialize local parameters by default.
            this.position = position;
            this.radius = radius;
            this.alpha = 1.0f;
            this.kicked = false;
            this.mainColor = new Color(193, 82, 28);
            this.secondColor = new Color(227, 117, 64);
            this.edgeColor = new Color(84, 84, 84);
            this.physicsWorld = physicsWorld;

            // Apply physic properties depending on the usage.
            if (skeleton)
            {
                makeCircleBodySkeleton(physicsWorld);
            }
            else
            {
                makeCircleBody(physicsWorld);
            }
        }

        /// <summary>
        /// Kill Body instance from Box2D world.
        /// </summary>
        public void destroy()
        {
            physicsWorld.DestroyBody(body);
        }

        // ---- Change local properties externally ----
        public void changeMainColor(Color c)
        {
            mainColor = c;
        }

        public void changeSecondColor(Color c)
        {
            secondColor = c;
        }

        public void changeEdgeColor(Color c)
        {
            edgeColor = c;
        }

        public void setLinearVelocity(Vector2 v)
        {
            body.SetLinearVelocity(v);
        }

        public void setBodyType(BodyType bt)
        {
            body.SetType(bt);
        }

        public void setUserData(Object obj)
        {
            body.SetUserData(obj);
        }

        public void setFriction(float f)
        {
            body.GetFixtureList().SetFriction(f);
            body.GetFixtureList().SetRestitution(0);
            body.GetFixtureList().SetDensity(0);
        }

        public void setProperty(string property)
        {
            this.property = property;
        }
        // ----  ----

        // ---- Retrieves local properties ----
        public Body getBody()
        {
            return body;
        }

        public string getProperty()
        {
            return property;
        }
        // ----  ----

        /// <summary>
        /// Asks is the body has been kicked or not.
        /// </summary>
        /// <returns>Boolean</returns>
        public Boolean isKicked()
        {
            return kicked;
        }

        /// <summary>
        /// Changes its own properties after external events.
        /// </summary>
        /// <param name="mainColor">Main color.</param>
        /// <param name="secondColor">Second color.</param>
        public override void change(Color mainColor, Color secondColor)
        {
            this.mainColor = mainColor;
            this.secondColor = secondColor;
            this.kicked = true;
        }

        /// <summary>
        /// Displays the circle object using primitives.
        /// </summary>
        /// <param name="screenManager">Main manager system.</param>
        public override void display(SpriteBatch spriteBatch, BasicEffect basicEffect)
        {
            ElementCircle solidCircle = new ElementCircle(radius, body.Position * CommonConstants.PIXELS_TO_METERS, mainColor, secondColor, alpha);
            solidCircle.draw(spriteBatch, basicEffect);
            solidCircle.drawBorderWeigth(spriteBatch, basicEffect, edgeColor, 1.5f);
        }

        /// <summary>
        /// Makes a physics circle used in the skeleton set.
        /// </summary>
        /// <param name="physicsWorld">Box2D world</param>
        private void makeCircleBodySkeleton(World physicsWorld)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;
            bodyDef.position = position / CommonConstants.PIXELS_TO_METERS;

            body = physicsWorld.CreateBody(bodyDef);

            CircleShape dynamicCircle = new CircleShape();
            dynamicCircle._radius = radius / CommonConstants.PIXELS_TO_METERS;

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = dynamicCircle;
            fixtureDef.restitution = .7f;
            fixtureDef.friction = .5f;
            fixtureDef.density = 1.0f;

            body.CreateFixture(fixtureDef);

            Random r = new Random();
            body.SetAngularVelocity((float) r.Next(-10, 10));

            body.SetUserData(this);
        }

        /// <summary>
        /// Makes a simple physics.
        /// </summary>
        /// <param name="physicsWorld">Box2D world</param>
        private void makeCircleBody(World physicsWorld)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;
            bodyDef.position = position / CommonConstants.PIXELS_TO_METERS;

            body = physicsWorld.CreateBody(bodyDef);

            CircleShape dynamicCircle = new CircleShape();
            dynamicCircle._radius = radius / CommonConstants.PIXELS_TO_METERS;

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = dynamicCircle;
            fixtureDef.restitution = 1.0f;
            fixtureDef.friction = .3f;
            fixtureDef.density = 1.0f;

            body.CreateFixture(fixtureDef);

            Random r = new Random();
            body.SetAngularVelocity((float) r.Next(-5, 0));
            body.SetLinearVelocity(new Vector2(0, 10));
            body.SetUserData(this);
        }

    }

}
