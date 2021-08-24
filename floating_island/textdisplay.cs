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
using System.Text;

namespace floating_island
{
    public class TextDisplay
    {
        public List<string> text { get; private set; }
        public SpriteFont font { get; private set; }
        public List<Texture2D> images { get; private set; }
        public int maxLength { get; private set; }

        public TextDisplay(ContentManager content, SpriteFont font, string path, int maxLength)
        {
            this.images = new List<Texture2D>();
            this.font = font;

            this.maxLength = maxLength;

            using (StreamReader sr = new StreamReader(path))
            {
                text = sr.ReadToEnd().Split('\n').ToList();

                int l = 1;

                for (int i = 0; i < text.Count; i+=l)
                {
                    l = 1;

                    if (text[i][0] == '#')
                    {
                        List<string> tmplist = text[i].Split('#').ToList();

                        if (tmplist[1].Trim('#') == "image")
                        {
                            this.images.Add(content.Load<Texture2D>(tmplist[2].Trim('#').Trim('\n').Trim('\r')));
                        }
                    }
                    else if (text[i].Length >= this.maxLength)
                    {
                        for (int j = 1; j * this.maxLength <= text[i].Length; j++)
                        {
                            text[i] = text[i].Insert(j * this.maxLength, "\n");
                        }
                    }
                }
            }
        }

        public void draw(SpriteBatch spriteBatch, int x, int y, Color color)
        {
            int drawY = 0;
            int j = 0;

            for(int i=0; i<text.Count; i++)
            {
                if (text[i][0] != '#')
                {
                    spriteBatch.DrawString(font, text[i], new Vector2(x, y + drawY), color);
                    drawY += (int)font.MeasureString(text[i]).Y;
                }
                else
                {
                    spriteBatch.Draw(images[j], new Vector2(x, y + drawY), Color.White);
                    drawY += images[j].Height;

                    j++;
                }
            }
        }
    }
}