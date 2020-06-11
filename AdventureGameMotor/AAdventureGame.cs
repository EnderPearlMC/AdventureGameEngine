using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using ExempleAdventureGameMotor;
using AdvG.Scenes;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace AdvG
{
    abstract class AAdventureGame
    {

        /*
         *  Properties 
         */

        public Main BaseGame { get; private set; }
        public string Title { get; set; }
        public int ScreenWidth { get; private set; }
        public int ScreenHeight { get; private set; }
        public int W { get; private set; }
        public int H { get; private set; }
        public int RW { get; private set; }
        public int RH { get; private set; }
        public WindowMode Mode { get; private set; }
        public string FolderName { get; private set; }

        public ASceneDirector SceneDirector { get; set; }

        public bool ShowSpecialCursor { get; set; }
        public Texture2D CursorToShow { get; set; }

        public ContentManager Content { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        /**
         *  Enums
         */
        public enum WindowMode
        {
            Fixed,
            Resizable,
            Fullscreen
        }

        /*
         *  Constructor
         */

        /**
         * <param type="Main" name="main"> The main file of the monogame project </param>
         * <param type="string" name="title">The title of the window</param>
         * <param type="int" name="w">The width of the window</param>
         * <param type="int" name="h">The height of the window</param>
         * <param type="int" name="rw">The width resolution of the window</param>
         * <param type="int" name="rh">The height resolution of the window</param>
         * <param type="WindowMode" name="mode">The mode the window</param>
         * <param type="string" name="folderName">The height resolution of the window</param>
         */
        public AAdventureGame(Main main, string title, int w, int h, int rw, int rh, WindowMode mode, string folderName)
        {
            BaseGame = main;
            Title = title;
            W = w;
            H = h;
            RW = rw;
            RH = rh;
            Mode = mode;
            FolderName = folderName;

            Content = BaseGame.Content;

            SceneDirector = new ASceneDirector(this);

        }

        /*
         *  Virtual methods 
         */

        /**
         *  You have to call this method on your monogame project's init method  
         */
        public virtual void Initialize()
        {
            LoadWindowMode();
            LoadScenes();
        }

        /**
         *  Here, load all your scenes
         */
        public virtual void LoadScenes()
        {
            Utils.Debug("Loading scenes : ");
        }

        /**
         *  You have to call this method on your monogame project's load method  
         */
        public virtual void Load(SpriteBatch spriteBatch)
        {
            SpriteBatch = spriteBatch;
            InitStateFile();
        }

        /**
         *  You have to call this method on your monogame project's update method
         *  <param type="GameTime" name="gameTime">The game time that will be used to get the delta time</param>
         */
        public virtual void Update(GameTime gameTime)
        {

            // get the size of the window
            if (Mode == WindowMode.Fullscreen)
            {
                // fullscreen mode
                ScreenWidth = BaseGame.graphics.GraphicsDevice.DisplayMode.Width;
                ScreenHeight = BaseGame.graphics.GraphicsDevice.DisplayMode.Height;
            }
            else if (Mode == WindowMode.Fixed || Mode == WindowMode.Resizable)
            {
                // Fixed size mode
                ScreenWidth = BaseGame.Window.ClientBounds.Width;
                ScreenHeight = BaseGame.Window.ClientBounds.Height;
            }

            SceneDirector.Update(gameTime);

        }
        
        /**
         *  You have to call this method on your monogame project's draw method  
         */
        public virtual void Render()
        {

            SpriteBatch.Begin();

            SceneDirector.Render();

            // cursor
            if (ShowSpecialCursor)
            {
                BaseGame.IsMouseVisible = false;

                SpriteBatch.Draw(CursorToShow, new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 70, 70), Color.White);

            }
            else
            {
                BaseGame.IsMouseVisible = true;
            }

            SpriteBatch.End();

        }

        /*
         *  Methods 
         */

        /**
         * 
         * Loads window mode using the title, the WindowMode, the Width and the Heigth
         * 
         */
        public void LoadWindowMode()
        {
            BaseGame.Window.Title = Title;

            if (Mode == WindowMode.Fullscreen)
            {
                // fullscreen mode
                BaseGame.graphics.PreferredBackBufferWidth = RW;
                BaseGame.graphics.PreferredBackBufferHeight = RH;
                BaseGame.graphics.IsFullScreen = true;
            }
            else if (Mode == WindowMode.Fixed)
            {
                // Fixed size mode
                BaseGame.graphics.PreferredBackBufferWidth = W;
                BaseGame.graphics.PreferredBackBufferHeight = H;
            }
            else if (Mode == WindowMode.Resizable)
            {
                // Resizable mode
                BaseGame.graphics.PreferredBackBufferWidth = W;
                BaseGame.graphics.PreferredBackBufferHeight = H;
                BaseGame.Window.AllowUserResizing = true;
            }

            BaseGame.graphics.ApplyChanges();

        }

        /**
         *  Inits the state file
         */
        public void InitStateFile()
        {
            if (!File.Exists(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FolderName), "state.json")))
            {
                SceneDirector.ScenesState = new AState();
                SceneDirector.InitScenesState();
                WriteStateFile();
                ReadStateFile();
                SceneDirector.ChangeScene(SceneDirector.ScenesState.CurrentSceneId);
            }

            else
            {
                ReadStateFile();
                SceneDirector.ChangeScene(SceneDirector.ScenesState.CurrentSceneId);
            }
        }

        /**
         *  Writes state file
         */
        public void WriteStateFile()
        {
            ADatas.WriteFile(FolderName, "state.json", SceneDirector.ScenesState);
        }

        /**
         *  Reads state file
         */
        public void ReadStateFile()
        {
            SceneDirector.ScenesState = ADatas.ReadFile<AState>(FolderName, "state.json");
        }

    }
}
