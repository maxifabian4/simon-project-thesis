using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace ProyectoSimon.Elements
{
    class CirclesContactListener : IContactListener
    {
        private Color mainColorKicked;
        private Color secondColorKicked;

        public void BeginContact(Contact contact) {
            Random r = new Random();
            Object objectA = contact.GetFixtureA().GetBody().GetUserData();
            Object objectB = contact.GetFixtureB().GetBody().GetUserData();
            Circle circle;

            mainColorKicked = new Color(166, 184, 162);
            secondColorKicked = new Color(185, 205, 181);
            
            if (objectA != null && objectB != null)
            {       
                if (objectA.ToString().Contains("Ground") && objectB.ToString().Contains("Circle"))
                {
                    contact.GetFixtureB().GetBody().SetLinearVelocity(new Vector2(r.Next(-15, 15), 25));
                }
                else
                    if (objectB.ToString().Contains("Ground") && objectA.ToString().Contains("Circle"))
                    {
                        contact.GetFixtureA().GetBody().SetLinearVelocity(new Vector2(r.Next(-5, 5), 25));
                    }
                
                if (objectA.ToString().Contains("Circle") && objectB.ToString().Contains("Skeleton"))
                {
                    circle = (Circle) objectA;
                    
                    if (!circle.isKicked())
                    {
                        circle.change(mainColorKicked, secondColorKicked);
                        circle.getBody().ApplyImpulse(new Vector2(0, 25), circle.getBody().Position);
                    }
                }
                else
                    if (objectB.ToString().Contains("Circle") && objectA.ToString().Contains("Skeleton"))
                    {
                        circle = (Circle) objectB;

                        if (circle.isKicked())
                        {
                            circle.change(mainColorKicked, secondColorKicked);
                            circle.getBody().ApplyImpulse(new Vector2(0, 25), circle.getBody().Position);
                        }
                    }
                }
        }

        public void EndContact(Contact contact) { }

        public void PreSolve(Contact contact, ref Manifold oldManifold) { }

        public void PostSolve(Contact contact, ref ContactImpulse impulse) { }
    }
}

