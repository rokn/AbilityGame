using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PowerOfOne
{
    public class Main : Microsoft.Xna.Framework.Game
    {

        private const int TileSetsCount = 43;
        public const bool inEditMode = false;
        public const bool showBoundingBoxes = true;
        private const int EnemiesCount = 7;
        private const int levelWidth1 = 100;
        private const int levelHeight1 = 60;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static Texture2D mouseTexture;
        public static MouseCursor mouse;
        public static KeysInput keyboard;
        public static int width, height;
        private bool exit;
        public static ContentManager content;
        public static List<Projectile> Projectiles;
        public static Camera camera;
        public static TileMap tilemap;
        private Player player;
        public static List<Rectangle> blockRects;
        public static List<Entity> Entities;
        public GameTime gameTimeRef;
        public static Texture2D BoundingBox;
        public static List<Entity> removeEntities;
        public static SpriteFont Font;

        //Variables for edit mode :
        public static int currTileset;

        public static string SavePath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PowerOfOne");
            }
        }

        public static string SaveName
        {
            get
            {
                return SavePath + @"\Level1";
            }
        }

        public Main()
        {
            Components.Add(new FrameRateCounter(this));
            graphics = new GraphicsDeviceManager(this);
            content = Content;
            content.RootDirectory = "Content";
            GoFullscreenBorderless();
        }

        protected override void Initialize()
        {
            mouse = new MouseCursor(width, height, 150);
            keyboard = new KeysInput();
            exit = false;
            camera = new Camera();
            tilemap = new TileMap(Vector2.Zero,levelWidth1, levelHeight1);
            TileSet.SpriteSheet = new List<Texture2D>();
            TileSet.tileHeight = 32;
            TileSet.tileWidth = 32;
            blockRects = new List<Rectangle>();

            if (!inEditMode)
            {
                Projectiles = new List<Projectile>();
                Entities = new List<Entity>();
                removeEntities = new List<Entity>();
                player = new Player(new Vector2(370, 1612));
                Entities.Add(player);
                Random rand = new Random();
                Entities.Add(new Enemy(new Vector2(500, 800), rand.Next(EnemiesCount)));
                Entities.Add(new Enemy(new Vector2(600, 800), rand.Next(EnemiesCount)));
                Entities.Add(new Enemy(new Vector2(700, 800), rand.Next(EnemiesCount)));
                Entities.Add(new Enemy(new Vector2(800, 800), rand.Next(EnemiesCount)));
                Entities.Add(new Enemy(new Vector2(900, 800), rand.Next(EnemiesCount)));
                Entities.Add(new Enemy(new Vector2(1000, 800), rand.Next(EnemiesCount)));
                Entities.Add(new Enemy(new Vector2(1100, 800), rand.Next(EnemiesCount)));
            }
            else {
                currTileset = 0;
            }
            LoadLevel();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            mouseTexture = Scripts.LoadTexture("Mouse");
            BoundingBox = Scripts.LoadTexture("WhitePixel");
            Font = Content.Load<SpriteFont>("Font");

            for (int i = 0; i < TileSetsCount; i++)
            {
                TileSet.SpriteSheet.Add(Scripts.LoadTexture(@"TileSets\Tileset_ (" + i.ToString()+")"));
            }

            if (!inEditMode)
            {
                Projectiles.ForEach(LoadObject);
                Entities.ForEach(LoadObject);
            }
            else
            {
                EditorGUI.Initialize();
                EditorGUI.Load();
            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (this.IsActive)
            {
                UpdateInput(gameTime);

                if (keyboard.JustPressed(Keys.Escape))
                {
                    exit = true;
                }

                if (!inEditMode)
                {
                    NormalUpdate(gameTime);
                }
                else
                {
                    EditUpdate();
                    UpdateCamera();
                }

                //if (Main.keyboard.IsHeld(Keys.LeftControl))
                //{
                //    if (Main.keyboard.JustPressed(Keys.L))
                //    {
                //        LoadLevel();
                //    }
                //}
            }

            if (exit)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkSlateGray);

            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, camera.GetTransformation(graphics.GraphicsDevice));

            if (!inEditMode)
            {
                Projectiles.ForEach(DrawObject);
                Entities.ForEach(DrawEntity);
            }
            else
            {
                EditorGUI.Draw(spriteBatch,false);
            }

            DrawObject(tilemap);

            if (showBoundingBoxes)
            {
                foreach (var rect in blockRects)
                {
                    spriteBatch.Draw(BoundingBox, rect, rect, Color.Black * 0.3f, 0, new Vector2(), SpriteEffects.None, 0.9f);
                }
            }

            spriteBatch.End();


            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);

            DrawMouse();

            if (inEditMode)
            {
                EditorGUI.Draw(spriteBatch,true);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region HelperMethods

        private void DrawMouse()
        {
            spriteBatch.Draw(mouseTexture, mouse.Position, null, Color.Red, 0, new Vector2(), 1f, SpriteEffects.None, 1f);
        }

        private void GoFullscreenBorderless()
        {
            IntPtr hWnd = this.Window.Handle;
            var control = System.Windows.Forms.Control.FromHandle(hWnd);
            var form = control.FindForm();
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        }

        private void UpdateInput(GameTime gameTime)
        {
            keyboard.Update(gameTime);
            mouse.UpdateMouse(gameTime);
        }

        private void LoadObject(dynamic obj)
        {
            obj.Load();
        }

        private void UpdateObject(dynamic obj)
        {
            obj.Update(gameTimeRef);
        }


        private void UpdateCamera()
        {
            if (Scripts.KeyIsPressed(Keys.NumPad4))
            {
                camera.Move(new Vector2(-8, 0));
            }

            if (Scripts.KeyIsPressed(Keys.NumPad6))
            {
                camera.Move(new Vector2(8, 0));
            }

            if (Scripts.KeyIsPressed(Keys.NumPad8))
            {
                camera.Move(new Vector2(0, -8));
            }

            if (Scripts.KeyIsPressed(Keys.NumPad2))
            {
                camera.Move(new Vector2(0, 8));
            }
        }

        private void NormalUpdate(GameTime gameTime)
        {
            if (gameTimeRef == null)
            {
                gameTimeRef = gameTime;
            }

            Projectiles.ForEach(UpdateObject);
            Entities.ForEach(UpdateObject);
            RemoveEntities();
        }


        private void RemoveEntities()
        {
            if (removeEntities.Count > 0)
            {
                removeEntities.ForEach(Entities.VoidRemove);
                removeEntities.Clear();
            }
        }

        private void EditUpdate()
        {
            EditorGUI.Update();

            if (keyboard.JustPressed(Keys.Add))
            {
                if (currTileset < TileSetsCount - 1)
                {
                    currTileset++;
                }
                else
                {
                    currTileset = 0;
                }
                EditorGUI.Initialize();
            }

            if (keyboard.JustPressed(Keys.Subtract))
            {
                if (currTileset > 0)
                {
                    currTileset--;
                }
                else
                {
                    currTileset = TileSetsCount - 1;
                }
                EditorGUI.Initialize();
            }
        }

        private static void LoadLevel()
        {
            if (File.Exists(SaveName + ".bin"))
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(SaveName + ".bin", FileMode.Open, FileAccess.Read, FileShare.Read);

                for (int i = 0; i < Main.tilemap.Width; i++)
                {

                    for (int b = 0; b < Main.tilemap.Height; b++)
                    {
                        try
                        {
                            tilemap.tileMap[i, b] = (TileCell)formatter.Deserialize(stream);
                        }
                        catch (SerializationException)
                        {
                            break;
                        }
                    }

                }

                stream.Close();

                Stream secondStream = new FileStream(SaveName + "Rects.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                blockRects = (List<Rectangle>)formatter.Deserialize(secondStream);
                stream.Close();
            }
        }

        private void DrawObject(dynamic obj)
        {
            obj.Draw(spriteBatch);
        }

        private void DrawEntity(Entity ent)
        {
            Rectangle screenRect = new Rectangle((int)camera.Position.X, (int)camera.Position.Y, width, height);

            if (ent.rect.Intersects(screenRect))
            {
                ent.Draw(spriteBatch);
            }
        }
        #endregion
    }
}
