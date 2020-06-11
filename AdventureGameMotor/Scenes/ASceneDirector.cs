using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvG.Scenes
{
    class ASceneDirector
    {

        /*
         *  Properties 
         */
        
        public List<AScene> Scenes { get; set; }

        public AScene CurrentScene { get; set; }
        
        public AState ScenesState { get; set; }

        public AAdventureGame Game { get; set; }

        /*
         *  Constructor
         */
        
        public ASceneDirector(AAdventureGame game)
        {

            Scenes = new List<AScene>();

            Game = game;

        }

        /*
         *  Methods 
         */

        /**
         *  Adds a AScene to the game
         *  <param name="scene">The scene to add to the game</param>
         */
        public void AddScene(AScene scene)
        {
            Utils.Debug("Scene \"" + scene.Id + "\" loaded.");
            Scenes.Add(scene);
        }

        public void ChangeScene(string id)
        {

            bool found = false;

            foreach(AScene s in Scenes)
            {
                if (s.Id == id && !found)
                {
                    // scene found
                    found = true;

                    CurrentScene = s;



                    if (CurrentScene != null)
                    {
                        CurrentScene.UnLoad();
                    }

                    CurrentScene.Load(this);

                    ScenesState.CurrentSceneId = CurrentScene.Id;
                    Game.WriteStateFile();

                }
            }

        }

        public void Update(GameTime gameTime)
        {
            // update the current scene
            if (CurrentScene != null)
            {
                CurrentScene.Update(gameTime);
            }
        }

        public void Render()
        {
            // render the current scene
            if (CurrentScene != null)
            {
                CurrentScene.Render(Game.SpriteBatch);
            }
        }

        /**
         *  Fills the scenes state
         */
        public void InitScenesState()
        {

            ScenesState.CurrentSceneId = "exemple";

            foreach (AScene s in Scenes)
            {
                if (s != CurrentScene)
                    s.Load(this);

                Dictionary<string, object> d = new Dictionary<string, object>();

                foreach (Dictionary<string, object> oe in s.ObjectsElements)
                {
                    Dictionary<string, object> d2 = new Dictionary<string, object>()
                    {
                        { "visible", oe["visible"] }
                    };

                    d.Add((string) oe["element_name"], d2);

                }

                ScenesState.ScenesState.Add(s.Id, d);

                if (s != CurrentScene)
                    s.UnLoad();
            }
        }

        /**
         *  <returns type="Dictionary<string, object>">The state of a scene</returns>
         */
        public Dictionary<string, object> GetSceneState(string id)
        {
            return ((JObject) ScenesState.ScenesState[id]).ToObject<Dictionary<string, object>>();
        }

    }
}
