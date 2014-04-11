using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProyectoSimon.Elements
{

    /// <summary>
    /// This class represents the skeleton (Kinect representation), using Cicles elements.
    /// </summary>
    public class Skeleton : GameElement
    {
        // Number of skeleton joints.
        //private static int JOINTS_COUNT = 20;

        // Physics circles used to represent the skeleton.
        private List<Circle> circleJoints;

        // In order to track an specific JointJoint type collection. 
        // AnkleLeft, AnkleRight, ElbowLeft, ElbowRight, FootLeft, FootRight, HandLeft, HandRight, Head,
        // HipCenter, HipLeft, HipRight, KneeLeft, KneeRight, ShoulderCenter, ShoulderLeft, ShoulderRight, 
        // Spine, WristLeft, WristRight.
        private IDictionary<Microsoft.Kinect.JointType, Vector2> jointTypes;


        public Skeleton(World physicsWorld)
        {
            circleJoints = new List<Circle>();
            if (physicsWorld != null)
            {
                makeJoints(physicsWorld);
            }

            jointTypes = new Dictionary<Microsoft.Kinect.JointType, Vector2>();
        }

        private void makeJoints(World physicsWorld)
        {
            Circle circle;
            // Create a physics circle for each skeleton joints that make up a tracked skeleton.
            for (int i = 0; i < CommonConstants.JOINTS_COUNT; i++)
            {
                circle = new Circle(physicsWorld, new Vector2(50, 50), CommonConstants.JOINTRADIUS, true);

                // Set physics properties.
                circle.setBodyType(BodyType.Static);
                circle.setFriction(.0f);
                circle.getBody().SetUserData(this);

                // Add elements to the lists.
                circleJoints.Add(circle);
            }
        }

        /// <summary>
        /// Updates each new skeleton position.
        /// </summary>
        /// <param name="js">Skeleton joints that make up a tracked skeleton.</param>
        public void update(IDictionary<Microsoft.Kinect.JointType, Vector2> js)
        {
            ICollection<Microsoft.Kinect.JointType> keys;

            if (js != null && circleJoints.Count > 0)
            {
                keys = js.Keys;

                for (int i = 0; i < CommonConstants.JOINTS_COUNT; i++)
                {
                    circleJoints[i].getBody().Position = js[keys.ElementAt<Microsoft.Kinect.JointType>(i)] / CommonConstants.PIXELS_TO_METERS;
                }
            }

            jointTypes = js;
        }

        /// <summary>
        /// Changes its own properties after external events.
        /// </summary>
        /// <param name="mainColor">Main color.</param>
        /// <param name="secondColor">Second color.</param>
        public override void change(Color mainColor, Color secondColor)
        {
        }

        /// <summary>
        /// Displays the Kinect skeleton.
        /// </summary>
        /// <param name="screenManager">Main manager system.</param>
        public override void display(SpriteBatch spriteBatch, BasicEffect basicEffect)
        {
            KinectSDK.Instance.display(spriteBatch, basicEffect);
        }

        /// <summary>
        /// Retrieves a physics joint position.
        /// </summary>
        /// <param name="jointType">Joint type.</param>
        /// <returns>Joint position</returns>
        public Vector2 getJointPosition(Microsoft.Kinect.JointType jointType)
        {
            if (jointTypes != null && jointTypes.Count > 0)
            {
                return jointTypes[jointType];
            }
            else
            {
                return new Vector2(-1, -1);
            }
        }

    }

}
