// Microbes 1.0
// Copyright 2006 Michael Anderson
// December 20, 2006

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion


namespace Microbes
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MicrobesGame game = new MicrobesGame())
            {
                game.Run();
            }
        }
    }




    /// <summary>
    /// Holds state for a disc
    /// </summary>
    class Disc
    {
        public Vector2 pos;
        public float radius = 0.04f;
        public float blurThreshold = 0.9f;
        public Color color = Color.White;
        public int charge = 0;


        public Disc(float x, float y)
        {
            pos.X = x;
            pos.Y = y;
        }


        public Matrix WorldMatrix()
        {
            return Matrix.CreateScale(radius) * Matrix.CreateTranslation(pos.X, pos.Y, 0);
        }
    }




    /// <summary>
    /// Handles drawing a list of discs
    /// </summary>
    class DiscManager
    {
        GraphicsDevice device;
        Effect effect;
        EffectParameter wvpMatrixParameter;
        EffectParameter colorParameter;
        EffectParameter blurThresholdParameter;
        VertexBuffer vb;
        IndexBuffer ib;
        VertexDeclaration vdecl;
        int numVertices;
        int numIndices;
        int numPrimitives;
        int bytesPerVertex;


        public void Init(ContentManager content, GraphicsDevice device)
        {
            this.device = device;
            effect = content.Load<Effect>("Microbes");
            wvpMatrixParameter = effect.Parameters["worldViewProj"];
            colorParameter = effect.Parameters["color"];
            blurThresholdParameter = effect.Parameters["blurThreshold"];

            CreateDiscMesh();
        }


        private void CreateDiscMesh()
        {
            int numWedges = 72;
            numVertices = numWedges + 1;
            bytesPerVertex = VertexPositionTexture.SizeInBytes;
            VertexPositionTexture[] tri = new VertexPositionTexture[numVertices];

            for (int i = 0; i < numVertices; i++)
            {
                float x;
                float y;
                float theta;
                float radius;
                if (i == 0)
                {
                    x = 0;
                    y = 0;
                    theta = 0;
                    radius = 0;
                }
                else
                {
                    theta = (float)(i - 1) / numWedges * 2 * (float)Math.PI;
                    radius = 1.0f;
                    x = (float)Math.Cos(theta);
                    y = (float)Math.Sin(theta);
                }
                tri[i] = new VertexPositionTexture(new Vector3(x, y, 0), new Vector2(theta, radius));
            }
            vb = new VertexBuffer(device, numVertices * bytesPerVertex, ResourceUsage.None, ResourceManagementMode.Automatic);
            vb.SetData<VertexPositionTexture>(tri);
            vdecl = new VertexDeclaration(device, VertexPositionTexture.VertexElements);
            numPrimitives = numWedges;
            numIndices = 3 * numPrimitives;
            ib = new IndexBuffer(device, numIndices * 2, ResourceUsage.None, IndexElementSize.SixteenBits);
            short[] indices = new short[numIndices];
            int iIndex = 0;
            for (int iPrim = 0; iPrim < numPrimitives; iPrim++)
            {
                indices[iIndex++] = 0;
                indices[iIndex++] = (short)(iPrim + 1);
                if (iPrim == numPrimitives - 1)
                    indices[iIndex++] = 1;
                else
                    indices[iIndex++] = (short)(iPrim + 2);
            }
            ib.SetData<short>(indices);
        }


        public void Draw(List<Disc> discList, Matrix viewMatrix, Matrix projMatrix)
        {
            Matrix world;
            Matrix worldViewProj;

            effect.CurrentTechnique = effect.Techniques[0];
            effect.Begin();
            EffectPass pass = effect.CurrentTechnique.Passes[0];
            device.VertexDeclaration = vdecl;
            device.Vertices[0].SetSource(vb, 0, bytesPerVertex);
            device.Indices = ib;

            pass.Begin();
            foreach (Disc disc in discList)
            {
                world = disc.WorldMatrix();
                worldViewProj = world * viewMatrix * projMatrix;
                wvpMatrixParameter.SetValue(worldViewProj);
                colorParameter.SetValue(disc.color.ToVector4());
                blurThresholdParameter.SetValue(disc.blurThreshold);
                effect.CommitChanges();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, numVertices, 0, numPrimitives);
            }
            pass.End();
            effect.End();
        }
    }




    /// <summary>
    /// The main type for the game
    /// </summary>
    public class MicrobesGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        Matrix viewMatrix = Matrix.Identity;
        Matrix projMatrix;
        Random random = new Random();

        List<Disc> backgroundDiscList = new List<Disc>();
        List<Disc> microbeDiscList = new List<Disc>();
        Disc playerDisc;
        DiscManager discManager = new DiscManager();


        public MicrobesGame()
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ContentManager(Services);

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            //graphics.IsFullScreen = true;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// </summary>
        protected override void Initialize()
        {
            Disc backgroundDisc = new Disc(0, 0);
            backgroundDisc.radius = 1.0f;
            backgroundDisc.color = Color.Black;
            backgroundDisc.blurThreshold = 0.98f;
            backgroundDiscList.Add(backgroundDisc);

            // Create a bunch of random microbes
            int numDiscs = 75;
            for (int i = 0; i < numDiscs; i++)
            {
                float theta = (float)random.NextDouble() * MathHelper.TwoPi;
                float rho = (float)random.NextDouble();
                float x = rho * (float)Math.Cos(theta);
                float y = rho * (float)Math.Sin(theta);
                Disc disc = new Disc(x, y);
                if (i % 2 == 1)
                {
                    disc.color = Color.Tomato;
                    disc.charge = +1;
                }
                else
                {
                    disc.color = Color.LawnGreen;
                    disc.charge = -1;
                }
                disc.radius = (float)random.NextDouble() / 30.0f + 0.01f;

                // Compute a reasonable blurThreshold from the radius.  Ideally this
                // should take the disc's size in screenspace into account.  For now:
                //     radius 0.01 --> 0.75 blurThreshold
                //     radius 0.10 --> 1.00 blurThreshold
                disc.blurThreshold = (disc.radius - 0.01f) / (0.10f - 0.01f) * 0.25f + 0.75f;

                microbeDiscList.Add(disc);
            }

            // Add the player-controlled disc to the list last (so it draws on top)
            playerDisc = new Disc(0, 0);
            playerDisc.color = Color.White;
            microbeDiscList.Add(playerDisc);

            base.Initialize();
        }


        /// <summary>
        /// Load graphics content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                discManager.Init(content, graphics.GraphicsDevice);
            }

            graphics.GraphicsDevice.RenderState.CullMode = CullMode.None;

            Create2DProjectionMatrix();
        }


        /// <summary>
        /// Unload graphics content.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent == true)
                content.Unload();
        }


        public void Create2DProjectionMatrix()
        {
            // Projection matrix ignores Z and just squishes X or Y to balance the upcoming viewport stretch
            float projScaleX;
            float projScaleY;
            float width = graphics.GraphicsDevice.Viewport.Width;
            float height = graphics.GraphicsDevice.Viewport.Height;
            if (width > height)
            {
                // Wide window
                projScaleX = height / width;
                projScaleY = 1.0f;
            }
            else
            {
                // Tall window
                projScaleX = 1.0f;
                projScaleY = width / height;
            }
            projMatrix = Matrix.CreateScale(projScaleX, projScaleY, 0.0f);
            projMatrix.M43 = 0.5f;
        }




        protected void UpdateMicrobeDiscs()
        {
            foreach (Disc disc in microbeDiscList)
            {
                foreach (Disc discNeighbor in microbeDiscList)
                {
                    if (discNeighbor == disc)
                        continue;
                    float d = Vector2.Distance(disc.pos, discNeighbor.pos);
                    Vector2 deltaVector = Vector2.Normalize(discNeighbor.pos - disc.pos);
                    float minDistance = disc.radius + discNeighbor.radius;

                    // Compute attraction / repulsion force
                    float magnitude = 0;
                    if (disc.charge != 0 && discNeighbor.charge != 0)
                    {
                        if (disc.charge == discNeighbor.charge)
                            magnitude = -1 / d * 0.0005f; // repel 
                        else
                            magnitude = 1 / d * 0.0005f; // attraction
                    }
                    else
                    {
                        if (discNeighbor.color == Color.Blue)
                            magnitude = 1 / d * 0.0005f; // attraction
                        else if (discNeighbor.color == Color.Yellow)
                            magnitude = -1 / d * 0.0005f; // repel 
                    }

                    magnitude = MathHelper.Clamp(magnitude, -d, d);

                    // Prevent discs from getting too close to each other
                    Vector2 testPos = disc.pos + (magnitude * deltaVector);
                    float newD = Vector2.Distance(testPos, discNeighbor.pos);
                    if (newD <= minDistance)
                        magnitude = d - minDistance;

                    disc.pos = disc.pos + (magnitude * deltaVector);
                }

                // Prevent discs from leaving the large circular area
                Vector2 vCenter = new Vector2(0, 0);
                float distanceFromCenter = Vector2.Distance(disc.pos, vCenter);
                if (distanceFromCenter > 1 - disc.radius)
                {
                    Vector2 deltaVector = Vector2.Normalize(vCenter - disc.pos);

                    float magnitude = ((1 - disc.radius) - distanceFromCenter);
                    disc.pos = disc.pos - (magnitude * deltaVector);
                }
            }
        }


        /// <summary>
        /// Gather input and update everything.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            KeyboardState keyboardState = Keyboard.GetState();
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

            if (gamePadState.Buttons.Back == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            playerDisc.color = Color.White;
            if (gamePadState.Buttons.A == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.A))
            {
                playerDisc.color = Color.Yellow;
            }
            if (gamePadState.Buttons.B == ButtonState.Pressed ||
                keyboardState.IsKeyDown(Keys.B))
            {
                playerDisc.color = Color.Blue;
            }
            
            float leftX = gamePadState.ThumbSticks.Left.X;
            if (keyboardState.IsKeyDown(Keys.Left))
                leftX -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Right))
                leftX += 1.0f;

            float leftY = -gamePadState.ThumbSticks.Left.Y;
            if (keyboardState.IsKeyDown(Keys.Up))
                leftY -= 1.0f;
            if (keyboardState.IsKeyDown(Keys.Down))
                leftY += 1.0f;

            playerDisc.pos.X += leftX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            playerDisc.pos.Y -= leftY * (float)gameTime.ElapsedGameTime.TotalSeconds;

            UpdateMicrobeDiscs();

            base.Update(gameTime);
        }


        /// <summary>
        /// Draw everything.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            discManager.Draw(backgroundDiscList, viewMatrix, projMatrix);
            discManager.Draw(microbeDiscList, viewMatrix, projMatrix);

            base.Draw(gameTime);
        }
    }
}