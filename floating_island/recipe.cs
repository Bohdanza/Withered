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
    public class recipe
    {
        public int type { get; private set; }
        public List<item> craftItems { get; private set; } = new List<item>();
        public List<bool> usage_list { get; private set; } = new List<bool>();
        public List<item> resultItems { get; private set; } = new List<item>();
        public Texture2D frame_texture { get; private set; }

        public recipe(ContentManager cm, int type, List<item> item_samples)
        {
            this.type = type;
            this.frame_texture = cm.Load<Texture2D>("frame");

            using(StreamReader sr = new StreamReader(@"info/global\recipes/" + this.type.ToString() + @"/main_info"))
            {
                List<string> tmp_str_list = sr.ReadToEnd().Split('\n').ToList();

                int tmp_c=Int32.Parse(tmp_str_list[0]);
                int current_str=1;

                for(int i=0; i<tmp_c; i++)
                {
                    int tmp_type = Int32.Parse(tmp_str_list[current_str]);
                    int amount = Int32.Parse(tmp_str_list[current_str + 1]);
                    bool b = bool.Parse(tmp_str_list[current_str + 2]);

                    this.craftItems.Add(new item(cm, 0, 0, tmp_type, false, amount, item_samples[tmp_type]));
                    this.usage_list.Add(b);

                    current_str += 3;
                }

                tmp_c = Int32.Parse(tmp_str_list[current_str]);
                current_str++;

                for(int i=0; i<tmp_c; i++)
                {
                    int tmp_type = Int32.Parse(tmp_str_list[current_str]);
                    int amount = Int32.Parse(tmp_str_list[current_str + 1]);

                    this.resultItems.Add(new item(cm, 0, 0, tmp_type, false, amount, item_samples[tmp_type]));
                }
            }
        }

        public void update(ContentManager cm)
        {
            foreach(var current_item in this.craftItems)
            {
                current_item.update(cm);
            }

            foreach (var current_item in this.resultItems)
            {
                current_item.update(cm);
            }
        }

        public void draw_result(SpriteBatch spriteBatch, int x, int y)
        {
            int current_coord = x;

            foreach(var current_item in this.resultItems)
            {
                spriteBatch.Draw(frame_texture, new Vector2(current_coord, y), Color.White);

                current_item.draw(spriteBatch, current_coord, y);

                current_coord += (int)(this.frame_texture.Width*1.1f);
            }
        }

        public void draw_crafting(SpriteBatch spriteBatch, int x, int y)
        {
            int current_coord = x;

            foreach (var current_item in this.craftItems)
            {
                spriteBatch.Draw(frame_texture, new Vector2(current_coord, y), Color.White);

                current_item.draw(spriteBatch, current_coord, y);

                current_coord += (int)(this.frame_texture.Width * 1.1f);
            }
        }
    }
}