using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;

namespace ProyectoSimon.Elements
{
    class Skeleton : GameElement
    {
        private List<Circle> circleJoints;
        //private List<MouseJoint> mouseJoints;
        private static int JOINTS_COUNT = 20;
        private IDictionary<Microsoft.Kinect.JointType, Vector2> jointTypes;
        private Vector2 vector;

        public Skeleton(World physicsWorld)
        {
            circleJoints = new List<Circle>();

            if (physicsWorld != null)
            {
                vector = Vector2.Zero;
                makeMouseJoints(physicsWorld);
            }

            jointTypes = new Dictionary<Microsoft.Kinect.JointType, Vector2>();
        }

        private void makeMouseJoints(World physicsWorld)
        {
            Circle circle;

            for (int i = 0; i < JOINTS_COUNT; i++)
            {
                circle = new Circle(physicsWorld, new Vector2(50, 50), 10, true);
                
                circle.setType(BodyType.Static);
                circle.setFriction(.0f);
                
                circle.getBody().SetUserData(this);
                //// Add elements to the lists.
                circleJoints.Add(circle);
                //mouseJoints.Add(mouseJoint);
            }
        }

        public void update(IDictionary<Microsoft.Kinect.JointType, Vector2> js)
        {
            if (js != null && circleJoints.Count > 0)
            {
                ICollection<Microsoft.Kinect.JointType> keys = js.Keys;

                for (int i = 0; i < JOINTS_COUNT; i++)
                    //mouseJoints[i].SetTarget(js[keys.ElementAt<JointID>(i)] / PIXELS_TO_METERS);
                    circleJoints[i].getBody().Position = js[keys.ElementAt<Microsoft.Kinect.JointType>(i)] / PIXELS_TO_METERS;
            }

            jointTypes = js;
        }

        public override void change(Color mColor, Color sColor) { }

        public void traslateSkeleton(Vector2 vect) {
            vector = vect;
        }
        public override void display(ScreenManager screenManager)
        {
            screenManager.Kinect.display(screenManager);
        }
        //public override void display(ScreenManager screenManager)
        //{
        //    float radius = 10;
        //    float alpha = 1;
        //    Color colorSkeleton = new Color(206, 103, 0);

        //    if (jointTypes != null && jointTypes.Count > 0)
        //    {
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.HipCenter], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipCenter], jointTypes[Microsoft.Kinect.JointType.Spine]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.Spine], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.Spine], jointTypes[Microsoft.Kinect.JointType.ShoulderCenter]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], jointTypes[Microsoft.Kinect.JointType.Head]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.Head], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.Head], jointTypes[Microsoft.Kinect.JointType.ShoulderCenter]);

        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], jointTypes[Microsoft.Kinect.JointType.ShoulderLeft]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.ShoulderLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderLeft], jointTypes[Microsoft.Kinect.JointType.ElbowLeft]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.ElbowLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ElbowLeft], jointTypes[Microsoft.Kinect.JointType.WristLeft]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.WristLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.WristLeft], jointTypes[Microsoft.Kinect.JointType.HandLeft]);

        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.HandLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], jointTypes[Microsoft.Kinect.JointType.ShoulderRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.ShoulderRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderRight], jointTypes[Microsoft.Kinect.JointType.ElbowRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.ElbowRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ElbowRight], jointTypes[Microsoft.Kinect.JointType.WristRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.WristRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.WristRight], jointTypes[Microsoft.Kinect.JointType.HandRight]);

        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.HandRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipCenter], jointTypes[Microsoft.Kinect.JointType.HipLeft]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.HipLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipLeft], jointTypes[Microsoft.Kinect.JointType.KneeLeft]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.KneeLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.KneeLeft], jointTypes[Microsoft.Kinect.JointType.AnkleLeft]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.AnkleLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.AnkleLeft], jointTypes[Microsoft.Kinect.JointType.FootLeft]);

        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.FootLeft], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipCenter], jointTypes[Microsoft.Kinect.JointType.HipRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.HipRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipRight], jointTypes[Microsoft.Kinect.JointType.KneeRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.KneeRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.KneeRight], jointTypes[Microsoft.Kinect.JointType.AnkleRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.AnkleRight], radius, alpha);
        //        drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.AnkleRight], jointTypes[Microsoft.Kinect.JointType.FootRight]);
        //        drawCircleJoint(screenManager, jointTypes[Microsoft.Kinect.JointType.FootRight], radius, alpha);
        //    }
        //}

        //// Draw a simple circle primitive.
        //private void drawCircleJoint(ScreenManager screenManager, Vector2 vector2, float radius, float alpha)
        //{
        //    Color colorSkeleton = new Color(206, 103, 0); // Change!!
        //    ElementCircle circle = new ElementCircle(radius, vector2 + vector, colorSkeleton, colorSkeleton, alpha);
        //    circle.drawBorderWeigth(screenManager, new Color(64, 64, 64), 1);
        //    circle.drawPrimitive(screenManager);
        //}

        //// Draw a simple line primitive.
        //private void drawLine(ScreenManager screenManager, Color colorLine, float alpha, Vector2 begin, Vector2 end)
        //{
        //    Box2D.XNA.FixedArray8<Vector2> vertexs = new Box2D.XNA.FixedArray8<Vector2>();
        //    vertexs[0] = new Vector2(begin.X, begin.Y) + vector;
        //    vertexs[1] = new Vector2(end.X, end.Y) + vector;

        //    ElementPolygon line = new ElementPolygon(vertexs, colorLine, 1, false, 2);
        //    line.drawPrimitive(screenManager);
        //}

        public Vector2 getJointPosition(Microsoft.Kinect.JointType jointType)
        {
            if (jointTypes != null && jointTypes.Count > 0)
                return jointTypes[jointType];
            else
                return new Vector2(-1, -1);
        }
    }
}
