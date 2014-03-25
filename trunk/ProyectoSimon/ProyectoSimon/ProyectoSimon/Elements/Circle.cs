using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Box2D.XNA;

namespace ProyectoSimon.Elements
{
    class Circle : ElementPhysic
    {
        private Body body;
        private Color mainColor, secondColor, edgeColor;
        private Boolean kicked;
        private float radius;
        private Vector2 position;
        private float alpha;
        private World physicsWorld;
        private string kind;

        public Circle(World physicsWorld, Vector2 p, float r, Boolean skeleton)
        {
            position = p;
            radius = r;
            alpha = 1;
            kicked = false;
            mainColor = new Color(193, 82, 28); 
            secondColor = new Color(227, 117, 64);

            //mainColor = ;

            edgeColor = new Color(84, 84, 84);
            this.physicsWorld = physicsWorld;

            if(skeleton)
                makeCircleBodySkeleton(physicsWorld);
            else
                makeCircleBody(physicsWorld);
        }

        public void destroy() {
            physicsWorld.DestroyBody(body);
        }

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

        public Boolean isKicked() {
            return kicked;
        }

        public void setLinearVelocity(Vector2 v)
        {
            body.SetLinearVelocity(v);
        }

        public void setType(BodyType bt) {
            body.SetType(bt);
        }

        public void setUserData(Object obj) {
            body.SetUserData(obj);
        }

        public void setFriction(float f)
        {
            body.GetFixtureList().SetFriction(f);
            body.GetFixtureList().SetRestitution(0);
            body.GetFixtureList().SetDensity(0);
        }

        private void makeCircleBody(World physicsWorld)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;
            bodyDef.position = position / PIXELS_TO_METERS;

            body = physicsWorld.CreateBody(bodyDef);

            CircleShape dynamicCircle = new CircleShape();
            dynamicCircle._radius = radius / PIXELS_TO_METERS;

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = dynamicCircle;
            fixtureDef.restitution = 1.0f;
            fixtureDef.friction = .3f;
            fixtureDef.density = 1.0f;
            //fixtureDef.restitution = .8f;
            //fixtureDef.friction = .5f;
            //fixtureDef.density = 1.0f;
            body.CreateFixture(fixtureDef);

            Random r = new Random();
            body.SetAngularVelocity((float)r.Next(-5, 0));
            body.SetLinearVelocity(new Vector2(0, 10));
            body.SetUserData(this);
        }

        private void makeCircleBodySkeleton(World physicsWorld)
        {
            BodyDef bodyDef = new BodyDef();
            bodyDef.type = BodyType.Dynamic;
            bodyDef.position = position / PIXELS_TO_METERS;

            body = physicsWorld.CreateBody(bodyDef);

            CircleShape dynamicCircle = new CircleShape();
            dynamicCircle._radius = radius / PIXELS_TO_METERS;

            FixtureDef fixtureDef = new FixtureDef();
            fixtureDef.shape = dynamicCircle;
            fixtureDef.restitution = .7f;
            fixtureDef.friction = .5f;
            fixtureDef.density = 1.0f;

            body.CreateFixture(fixtureDef);

            Random r = new Random();
            body.SetAngularVelocity((float)r.Next(-10, 10));

            body.SetUserData(this);
        }

        public override void change(Color mColor, Color sColor) {
            mainColor = mColor;
            secondColor = sColor;
            kicked = true;
        }

        public override void display(ScreenManager screenManager)
        {
            ElementCircle solidCircle = new ElementCircle(radius, body.Position * PIXELS_TO_METERS, mainColor, secondColor, alpha);
            solidCircle.drawPrimitive(screenManager);
            solidCircle.drawBorderWeigth(screenManager, edgeColor, 1.5f);
        }

        public Body getBody() {
            return body;
        }

        public void setKind(string p)
        {
            kind = p;
        }

        public string getKind()
        {
            return kind;
        }
    }
}
