using System;
using System.Collections.Generic;
using System.Linq;
using C3.XNA;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Kinect;

namespace ProyectoSimon
{
    public class KinectSDK
    {
        private static KinectSDK instance;
        // Kinect parameters.
        private IDictionary<JointType, Vector2> jointTypes;
        private KinectSensor kinectSensor;
        // Screen parameters.
        private int bbwidth;
        private int bbheight;
        // Parameters to draw.
        Texture2D kinectRGBVideo;
        string connectedStatus = "Not connected";
        GraphicsDevice graphics;

        /// <summary>
        /// Constructor.
        /// </summary>        
        private KinectSDK()
        {
            KinectSensor.KinectSensors.StatusChanged += new EventHandler<StatusChangedEventArgs>(KinectSensors_StatusChanged);
            DiscoverKinectSensor();
        }
        /// <summary>
        /// Returns an KinectSDK Instance.
        /// </summary>  
        public static KinectSDK Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new KinectSDK();
                }
                return instance;
            }
        }
        /// <summary>
        /// Sets hight, wide sizes and Graphic Device.
        /// </summary>  
        public void setGraphicDevice(GraphicsDevice gd, int w, int h)
        {
            bbwidth = w;
            bbheight = h;
            graphics = gd;
        }
        /// <summary>
        /// This is a part of the KinectSensor library – making it possible to go through
        /// all connected Kinect-devices and check their status. By listening to this event
        /// handler, you can run code once the status changes on any of the connected devices.
        /// </summary>        
        void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            if (this.kinectSensor == e.Sensor)
            {
                if (e.Status == KinectStatus.Disconnected ||
                    e.Status == KinectStatus.NotPowered)
                {
                    this.kinectSensor = null;
                    this.DiscoverKinectSensor();
                }
            }
        }

        /// <summary>
        /// This function will go through all connected Kinect-devices and when found a device, 
        /// use it to set our kinectSensor instance, update a message the user can read and in
        /// the end Initialize it if connected.
        /// </summary>       
        private void DiscoverKinectSensor()
        {
            foreach (KinectSensor sensor in KinectSensor.KinectSensors)
            {
                if (sensor.Status == KinectStatus.Connected)
                {
                    // Found one, set our sensor to this
                    kinectSensor = sensor;
                    break;
                }
            }

            if (this.kinectSensor == null)
            {
                connectedStatus = "Found none Kinect Sensors connected to USB";
                return;
            }

            // You can use the kinectSensor.Status to check for status
            // and give the user some kind of feedback
            switch (kinectSensor.Status)
            {
                case KinectStatus.Connected:
                    {
                        connectedStatus = "Status: Connected";
                        break;
                    }
                case KinectStatus.Disconnected:
                    {
                        connectedStatus = "Status: Disconnected";
                        break;
                    }
                case KinectStatus.NotPowered:
                    {
                        connectedStatus = "Status: Connect the power";
                        break;
                    }
                default:
                    {
                        connectedStatus = "Status: Error";
                        break;
                    }
            }

            // Init the found and connected device
            if (kinectSensor.Status == KinectStatus.Connected)
            {
                InitializeKinect();
            }
        }

        /// <summary>
        /// We want out application to simply render what the Kinect can see (images from the RGB Camera).
        /// To do this, we tell the kinectSensor that we should enable the ColorStream, and what Format we 
        /// want out. We also listen for the ColorFrameReady event whom will notify us if the Kinect got a
        /// new image ready for us and render a cursor/ball in the hands of the player.
        /// </summary>
        private bool InitializeKinect()
        {
            // Color stream
            kinectSensor.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);

            // Skeleton Stream
            kinectSensor.SkeletonStream.Enable(new TransformSmoothParameters()
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            });
            kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSensor_SkeletonFrameReady);

            try
            {
                kinectSensor.Start();
            }
            catch
            {
                connectedStatus = "Unable to start the Kinect Sensor";
                return false;
            }
            return true;
        }

        /// <summary>
        /// First you get all the skeletons in the returned collection. Then we find all the skeletons that belong
        /// to the player currently being tracked (this is automatic), and find the joint of the Right Hand. This 
        /// joint includes a position that you simply can use to render your object at.
        /// The formula I created when rendering the object is not very accurate but it get’s the job done. This is 
        /// because I simply just use the X and Y of a 3D point, leaving the Z. This means that we bypass the depth 
        /// of the scene so at point’s it will be wrong as we didn’t implement depth.
        /// The Kinect returns a position between –1 (left) and 1 (right). I use this to convert the number to the 
        /// range is 0 to 1 since the resolution of the scene is from 0 to 640 and 0 to 480. I then multiply the 
        /// position with the resolution so the cursor can move around on the entire scene.
        /// </summary>
        void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    Skeleton[] skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletonData);
                    Skeleton playerSkeleton = (from s in skeletonData
                                               where s.TrackingState == SkeletonTrackingState.Tracked
                                               select s).FirstOrDefault();

                    if (playerSkeleton != null)
                    {
                        jointTypes = new Dictionary<JointType, Vector2>();
                        Joint scaledJoint;
                        foreach (Joint joint in playerSkeleton.Joints)
                        {
                            scaledJoint = scaleTo(joint, bbwidth, bbheight);
                            jointTypes.Add(joint.JointType, new Vector2(
                                //joint.Position.X * bbwidth * 0.5f + (0.75f * bbwidth),
                                //joint.Position.Y * bbheight * -1 * 0.65f + (0.9f * bbheight)));                                

                            scaledJoint.Position.X, scaledJoint.Position.Y));

                        }
                    }
                }
            }
        }

        /// <summary>
        /// It captures the image from the Kinect sensor, creates a Color array, fills it with the
        /// data from the captures image for each pixel, and then finally stores it in a Texture2d object.
        /// </summary>
        void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame colorImageFrame = e.OpenColorImageFrame())
            {
                if (colorImageFrame != null)
                {
                    byte[] pixelsFromFrame = new byte[colorImageFrame.PixelDataLength];

                    colorImageFrame.CopyPixelDataTo(pixelsFromFrame);

                    Color[] color = new Color[colorImageFrame.Height * colorImageFrame.Width];
                    kinectRGBVideo = new Texture2D(graphics, colorImageFrame.Width, colorImageFrame.Height);

                    // Go through each pixel and set the bytes correctly
                    // Remember, each pixel got a Rad, Green and Blue
                    int index = 0;
                    for (int y = 0; y < colorImageFrame.Height; y++)
                    {
                        for (int x = 0; x < colorImageFrame.Width; x++, index += 4)
                        {
                            color[y * colorImageFrame.Width + x] = new Color(pixelsFromFrame[index + 2], pixelsFromFrame[index + 1], pixelsFromFrame[index + 0]);
                        }
                    }

                    // Set pixeldata from the ColorImageFrame to a Texture2D
                    kinectRGBVideo.SetData(color);
                }
            }
        }
        /// <summary>
        /// Returns the joint types with their 2D positions.
        /// </summary>
        public IDictionary<JointType, Vector2> getJoints()
        {
            return jointTypes;
        }
        /// <summary>
        /// Returns the 2D position for an specific joint type.
        /// </summary>
        public Vector2 getJointPosition(JointType id)
        {
            if (jointTypes != null)
                return jointTypes[id];
            else
                return Vector2.Zero;
        }
        /// <summary>
        /// Returns a boolean, It is true if a player is an enought distance. 
        /// </summary>
        public bool isInRange()
        {
            if (jointTypes != null)
            {
                Vector2 posResult = jointTypes[Microsoft.Kinect.JointType.Head] - jointTypes[Microsoft.Kinect.JointType.ShoulderCenter];
                return !(posResult.LengthSquared() > 10000 || posResult.LengthSquared() < 2500);
            }
            else
                return false;
        }
        /// <summary>
        /// It draws a color video detected from Kinect.
        /// </summary>        
        public void DrawVideoCam(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch, Rectangle area)
        {
            spriteBatch.Draw(kinectRGBVideo, area, Color.White);
        }
        /// <summary>
        /// It takes a picture from a color video.
        /// </summary>    
        public Texture2D getCapture()
        {
            return kinectRGBVideo;
        }
        /// <summary>
        /// It checks Kinect status. 
        /// </summary>    
        public bool isConected()
        {
            return connectedStatus.Equals("Status: Connected");
        }
        /// <summary>
        /// It enables Kinect seated mode.
        /// </summary>   
        public void setSeatedMode()
        {
            kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
        }
        /// <summary>
        /// It enables Kinect default mode.
        /// </summary>  
        public void setDefaultMode()
        {
            kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
        }
        /// <summary>
        /// It returns Kinect mode (default or seated).
        /// </summary>  
        public string getTrackingMode()
        {
            return kinectSensor.SkeletonStream.TrackingMode.ToString();
        }
        /// <summary>
        /// It scales skeleton joints from real world to screen using scaleTo(Joint, int, int, float, float).
        /// </summary>          
        private Joint scaleTo(Joint joint, int width, int height)
        {
            return scaleTo(joint, width, height, 1.0f, 1.0f);
        }
        /// <summary>
        /// It scales skeleton joints from real world to screen using scale(int, float , float).
        /// </summary> 
        private Joint scaleTo(Joint joint, int width, int height, float skeletonMaxX, float skeletonMaxY)
        {
            Microsoft.Kinect.SkeletonPoint pos = new SkeletonPoint()
            {
                X = scale(width, skeletonMaxX, joint.Position.X),
                Y = scale(height, skeletonMaxY, -joint.Position.Y),
                Z = joint.Position.Z
            };

            joint.Position = pos;

            return joint;
        }
        /// <summary>
        /// It scales skeleton joints from real world to screen.
        /// </summary> 
        private float scale(int maxPixel, float maxSkeleton, float position)
        {
            float value = ((((maxPixel / maxSkeleton) / 2) * position) + (maxPixel / 2));
            if (value > maxPixel)
                return maxPixel;
            if (value < 0)
                return 0;
            return value;
        }
        /// <summary>
        /// It draws skeleton joints on screen.
        /// </summary> 
        public void display(SpriteBatch spriteBatch, BasicEffect basicEffect)
        {
            float radius = 10;
            float alpha = 1;
            Color colorSkeleton = new Color(206, 103, 0);
            if (jointTypes != null && jointTypes.Count > 0)
            {

                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], jointTypes[Microsoft.Kinect.JointType.Head]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.Head], radius, alpha);
                //drawLine(screenManager, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.Head], jointTypes[Microsoft.Kinect.JointType.ShoulderCenter]);

                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], jointTypes[Microsoft.Kinect.JointType.ShoulderLeft]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.ShoulderLeft], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderLeft], jointTypes[Microsoft.Kinect.JointType.ElbowLeft]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.ElbowLeft], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ElbowLeft], jointTypes[Microsoft.Kinect.JointType.WristLeft]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.WristLeft], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.WristLeft], jointTypes[Microsoft.Kinect.JointType.HandLeft]);

                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.HandLeft], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderCenter], jointTypes[Microsoft.Kinect.JointType.ShoulderRight]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.ShoulderRight], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ShoulderRight], jointTypes[Microsoft.Kinect.JointType.ElbowRight]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.ElbowRight], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.ElbowRight], jointTypes[Microsoft.Kinect.JointType.WristRight]);
                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.WristRight], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.WristRight], jointTypes[Microsoft.Kinect.JointType.HandRight]);

                drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.HandRight], radius, alpha);
                drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipCenter], jointTypes[Microsoft.Kinect.JointType.HipLeft]);

                if (KinectSDK.Instance.getTrackingMode().Equals(SkeletonTrackingMode.Default.ToString()))
                {
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.HipCenter], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipCenter], jointTypes[Microsoft.Kinect.JointType.Spine]);

                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.Spine], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.Spine], jointTypes[Microsoft.Kinect.JointType.ShoulderCenter]);

                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.FootLeft], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipCenter], jointTypes[Microsoft.Kinect.JointType.HipRight]);
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.HipRight], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipRight], jointTypes[Microsoft.Kinect.JointType.KneeRight]);
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.KneeRight], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.KneeRight], jointTypes[Microsoft.Kinect.JointType.AnkleRight]);
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.AnkleRight], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.AnkleRight], jointTypes[Microsoft.Kinect.JointType.FootRight]);
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.FootRight], radius, alpha);

                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.HipLeft], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.HipLeft], jointTypes[Microsoft.Kinect.JointType.KneeLeft]);
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.KneeLeft], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.KneeLeft], jointTypes[Microsoft.Kinect.JointType.AnkleLeft]);
                    drawCircleJoint(spriteBatch, basicEffect, jointTypes[Microsoft.Kinect.JointType.AnkleLeft], radius, alpha);
                    drawLine(spriteBatch, colorSkeleton, alpha, jointTypes[Microsoft.Kinect.JointType.AnkleLeft], jointTypes[Microsoft.Kinect.JointType.FootLeft]);
                }
            }
        }
        /// <summary>
        /// Draw a simple circle primitive.
        /// </summary>         
        private void drawCircleJoint(SpriteBatch spriteBatch, BasicEffect basicEffect, Vector2 vector2, float radius, float alpha)
        {
            Color colorSkeleton = new Color(206, 103, 0); // Change!!
            ElementCircle circle = new ElementCircle(radius, vector2, colorSkeleton, colorSkeleton, alpha);
            circle.drawBorderWeigth(spriteBatch, basicEffect, new Color(64, 64, 64), 1);
            circle.draw(spriteBatch, basicEffect);
        }
        /// <summary>
        /// Draw a simple line primitive.
        /// </summary>           
        private void drawLine(SpriteBatch spriteBatch, Color colorLine, float alpha, Vector2 begin, Vector2 end)
        {
            spriteBatch.DrawLine(begin, end, colorLine, 9, 0.5f);
        }
    }
}
