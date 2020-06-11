using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvG.Scenes
{
    class AScene
    {

        /*
         *  Properties 
         */
        public string Id { get; set; }
        public string PathOfFile { get; set; }
        public AAdventureGame Game { get; set; }
        public ASceneDirector SceneDirector { get; set; }

        public float AlphaFade { get; set; }

        // elements
        public List<Dictionary<string, object>> ImageElements { get; private set; }
        public List<Dictionary<string, object>> TravelZonesElements { get; private set; }
        public List<Dictionary<string, object>> ObjectsElements { get; private set; }

        private MouseState oldMouseState;

        /*
         *  Constructor
         */

        /**
         *  <param type="string" name="id">The id of the scene</param>
         *  <param type="string" name="pathOfFile">The path of the scene file</param>
         *  <param type="AAdventureGame" name="game">The game of the scene</param>
         */
        public AScene(string id, string pathOfFile, AAdventureGame game)
        {
            Id = id;
            PathOfFile = pathOfFile;
            Game = game;

            // elements
            ImageElements = new List<Dictionary<string, object>>();
            TravelZonesElements = new List<Dictionary<string, object>>();
            ObjectsElements = new List<Dictionary<string, object>>();

        }

        /*
         *  Methods 
         */
        private void ParseFile(string text)
        {

            Dictionary<string, object> elements = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);

            foreach(KeyValuePair<string, object> e in elements)
            {

                // check what kind of element it is

                Dictionary<string, object> element = ((JObject)e.Value).ToObject<Dictionary<string, object>>();

                switch (((Dictionary<string, object>)element)["type"])
                {

                    // image elements
                    case "image":

                        Dictionary<string, object> obj = ((JObject)e.Value).ToObject<Dictionary<string, object>>();
                        obj["element_name"] = e.Key;

                        ImageElements.Add(obj);

                        break;

                    // travel zones elements
                    case "travel_zone":

                        Dictionary<string, object> obj1 = ((JObject)e.Value).ToObject<Dictionary<string, object>>();
                        obj1["element_name"] = e.Key;

                        TravelZonesElements.Add(obj1);

                        break;

                    // objects elements
                    case "object":

                        Dictionary<string, object> obj2 = ((JObject)e.Value).ToObject<Dictionary<string, object>>();
                        obj2["element_name"] = e.Key;

                        ObjectsElements.Add(obj2);

                        break;

                    default:
                        break;
                }

            }

        }

        /*
         *  Virtual Methods 
         */

        public virtual void Load(ASceneDirector sceneDirector)
        {

            SceneDirector = sceneDirector;

            oldMouseState = Mouse.GetState();

            AlphaFade = 1.0f;

            // We try to read the scene file
            if (PathOfFile.EndsWith(".json"))
            {
                //try
                //{
                    string text = System.IO.File.ReadAllText(PathOfFile);
                    ParseFile(text);
                //}
                //catch (Exception e)
                //{
                //    throw new Exception("Error during reading or parsing the scene file for scene : \"" + Id + "\". The path of the scene file is maybe incorrect.");
                //}
            }
            else
            {
                throw new Exception("The scene file for scene : \"" + Id + "\" must be .json file!");
            }

            foreach (Dictionary<string, object> ie in ImageElements)
            {
                // load texture
                Texture2D texture = Game.Content.Load<Texture2D>((string) ie["name"]);

                ie.Add("texture", texture);

            }

            foreach (Dictionary<string, object> te in TravelZonesElements)
            {
                // load cursor texture
                Texture2D texture = Game.Content.Load<Texture2D>((string)te["cursor"]);

                te.Add("cursor_texture", texture);

            }

            foreach (Dictionary<string, object> oe in ObjectsElements)
            {
                // load object texture
                Texture2D texture = Game.Content.Load<Texture2D>((string)oe["img"]);
                
                oe.Add("texture", texture);

            }

        }

        public virtual void UnLoad()
        {
            ImageElements.RemoveAll(item => true);
            TravelZonesElements.RemoveAll(item => true);
            ObjectsElements.RemoveAll(item => true);
        }

        public virtual void Update(GameTime gameTime)
        {

            MouseState newMouseState = Mouse.GetState();

            // update fade transition
            if (AlphaFade > 0)
            {
                AlphaFade -= (float) gameTime.ElapsedGameTime.TotalSeconds * 1;
            }

            // Travel Zones Elements
            foreach (Dictionary<string, object> te in TravelZonesElements)
            {

                if ((bool)te["visible"])
                {
                    Rectangle rect = new Rectangle(0, 0, 0, 0);

                    if ((string)te["x_mode"] == "pixel")
                    {
                        rect.X = Convert.ToInt32(te["x"]);
                    }

                    if ((string)te["y_mode"] == "pixel")
                    {
                        rect.Y = Convert.ToInt32(te["y"]);
                    }

                    if ((string)te["x_mode"] == "screen")
                    {
                        rect.X = (int)(Game.ScreenWidth / Convert.ToDecimal(te["x"]));
                    }

                    if ((string)te["y_mode"] == "screen")
                    {
                        rect.Y = (int)(Game.ScreenHeight / Convert.ToDecimal(te["y"]));
                    }

                    rect.Width = (int)(Game.ScreenWidth / Convert.ToDecimal(te["w"]));
                    rect.Height = (int)(Game.ScreenHeight / Convert.ToDecimal(te["h"]));

                    if (rect.Contains(newMouseState.Position))
                    {
                        Game.ShowSpecialCursor = true;
                        Game.CursorToShow = (Texture2D)te["cursor_texture"];

                        // detect if the zone is clicked
                        if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                        {
                            Game.SceneDirector.ChangeScene((string)te["id_to_go"]);
                        }

                    }
                }
            }

            foreach (Dictionary<string, object> oe in ObjectsElements)
            {

                if ((bool)oe["visible"])
                {
                    Rectangle rect = new Rectangle(0, 0, 0, 0);

                    if ((string)oe["x_mode"] == "pixel")
                    {
                        rect.X = Convert.ToInt32(oe["x"]);
                    }

                    if ((string)oe["y_mode"] == "pixel")
                    {
                        rect.Y = Convert.ToInt32(oe["y"]);
                    }

                    if ((string)oe["x_mode"] == "screen")
                    {
                        rect.X = (int)(Game.ScreenWidth / Convert.ToDecimal(oe["x"]));
                    }

                    if ((string)oe["y_mode"] == "screen")
                    {
                        rect.Y = (int)(Game.ScreenHeight / Convert.ToDecimal(oe["y"]));
                    }

                    rect.Width = (int)(Game.ScreenWidth / Convert.ToDecimal(oe["w"]));
                    rect.Height = (int)(Game.ScreenHeight / Convert.ToDecimal(oe["h"]));

                    if (rect.Contains(newMouseState.Position))
                    {

                        // detect if the zone is clicked
                        if (newMouseState.LeftButton == ButtonState.Pressed && oldMouseState.LeftButton == ButtonState.Released)
                        {
                            
                        }

                    }

                }



                Dictionary<string, object> state = ((JObject)SceneDirector.GetSceneState(Id)[(string) oe["element_name"]]).ToObject<Dictionary<string, object>>();

                oe["visible"] = state["visible"];

            }

            oldMouseState = newMouseState;

        }

        public virtual void Render(SpriteBatch spriteBatch)
        {
            RenderSceneFile(spriteBatch);

            // fade transition
            Texture2D rect = new Texture2D(Game.BaseGame.GraphicsDevice, 1, 1);
            rect.SetData(new Color[] { Color.Black });

            spriteBatch.Draw(rect, new Rectangle(0, 0, Game.ScreenWidth, Game.ScreenHeight), Color.White * AlphaFade);

        }

        private void RenderSceneFile(SpriteBatch spriteBatch)
        {

            // Image Elements
            foreach(Dictionary<string, object> ie in ImageElements)
            {

                if ((bool)ie["visible"])
                {
                    Rectangle rect = new Rectangle(0, 0, 0, 0);

                    if ((string)ie["x_mode"] == "pixel")
                    {
                        rect.X = Convert.ToInt32(ie["x"]);
                    }

                    if ((string)ie["y_mode"] == "pixel")
                    {
                        rect.Y = Convert.ToInt32(ie["y"]);
                    }

                    if ((string)ie["x_mode"] == "screen")
                    {
                        rect.X = (int)(Game.ScreenWidth / Convert.ToDecimal(ie["x"]));
                    }

                    if ((string)ie["y_mode"] == "screen")
                    {
                        rect.Y = (int)(Game.ScreenHeight / Convert.ToDecimal(ie["y"]));
                    }

                    rect.Width = (int)(Game.ScreenWidth / Convert.ToDecimal(ie["w"]));
                    rect.Height = (int)(Game.ScreenHeight / Convert.ToDecimal(ie["h"]));

                    spriteBatch.Draw((Texture2D)ie["texture"], rect, Color.White);
                }

            }

            // objects Elements
            foreach (Dictionary<string, object> oe in ObjectsElements)
            {

                if ((bool)oe["visible"])
                {
                    Rectangle rect = new Rectangle(0, 0, 0, 0);

                    if ((string)oe["x_mode"] == "pixel")
                    {
                        rect.X = Convert.ToInt32(oe["x"]);
                    }

                    if ((string)oe["y_mode"] == "pixel")
                    {
                        rect.Y = Convert.ToInt32(oe["y"]);
                    }

                    if ((string)oe["x_mode"] == "screen")
                    {
                        rect.X = (int)(Game.ScreenWidth / Convert.ToDecimal(oe["x"]));
                    }

                    if ((string)oe["y_mode"] == "screen")
                    {
                        rect.Y = (int)(Game.ScreenHeight / Convert.ToDecimal(oe["y"]));
                    }

                    rect.Width = (int)(Game.ScreenWidth / Convert.ToDecimal(oe["w"]));
                    rect.Height = (int)(Game.ScreenHeight / Convert.ToDecimal(oe["h"]));

                    spriteBatch.Draw((Texture2D)oe["texture"], rect, Color.White);
                }

            }

        }

    }
}
