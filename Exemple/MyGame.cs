using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExempleAdventureGameMotor
{
    class MyGame : AdvG.AAdventureGame
    {

        private Texture2D Cursor;

        public MyGame(Main main) : base(main, "Horror Adventure Game", 1280, 720, 1920, 1080, WindowMode.Resizable, "AdvGameExemple")
        {

        }

        public override void LoadScenes()
        {
            base.LoadScenes();

            SceneDirector.AddScene(new SceneExemple(this));
            SceneDirector.AddScene(new SceneExemple2(this));

        }

        public override void Load(SpriteBatch spriteBatch)
        {

            

            Cursor = Content.Load<Texture2D>("cursor");

            base.Load(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {

            ShowSpecialCursor = true;
            CursorToShow = Cursor;

            base.Update(gameTime);
        }

    }
}
