using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace floating_island
{
    public class ResearchPoint
    {
        public int type { get; private set; }
        public int amount;

        public SpriteFont font;
        public Texture2D texture { get; private set; }
        private Texture2D background;

        public ResearchPoint(ContentManager cm, int type, int amount, SpriteFont font)
        {
            this.type = type;
            this.amount = amount;

            if (font != null)
            {
                this.font = font;
            }
            else
            {
                this.font = cm.Load<SpriteFont>("pointsFont");
            }

            this.texture = cm.Load<Texture2D>(this.type.ToString() + "research_point");
            this.background = cm.Load<Texture2D>("pointsBackground");
        }

        //I actually don't know why I need this, but
        //I have a feeling that I need
        public void update()
        {
            ;
        }

        public void draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(this.background, new Vector2(x, y + (int)((this.texture.Height - this.background.Height) / 2)), Color.White);
            spriteBatch.Draw(this.texture, new Vector2(x - (int)(this.texture.Width / 2), y), Color.White);

            spriteBatch.DrawString(this.font, this.amount.ToString(), new Vector2(x + (int)(this.texture.Width * 0.6f), y + (int)((this.texture.Height - this.background.Height) / 2) + (int)((this.background.Height - this.font.MeasureString(this.amount.ToString()).Y) / 2)), Color.White);
        }

        //must be fixed and integrated in normal draw funktion
        public void drawForRecipe(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(this.texture, new Vector2(x, y - (this.texture.Height - this.font.MeasureString(this.amount.ToString()).Y) / 2), Color.White) ;
            spriteBatch.DrawString(this.font, this.amount.ToString(), new Vector2(x + (int)(this.texture.Width * 1.1f), y), Color.White);
        }

        public Vector2 getDrawRect()
        {
            return new Vector2(this.background.Width + (int)(this.texture.Width / 2), Math.Max(this.texture.Height, Math.Max(this.background.Height, this.font.MeasureString(this.amount.ToString()).Y)));
        }
    }
}
