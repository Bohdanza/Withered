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
    public class item : map_object
    {
        List<Texture2D> textures = new List<Texture2D>();
        public override List<int> what_to_do_with { get; protected set; }
        public bool on_the_ground { get; private set; } = false;
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        public override int type { get; protected set; }
        private int img_phase;

        //!IMPORTANT!
        //hitbox points are stored as values that must be ADDED (even for the left hitbox corner) to get the real hitbox
        public override Vector2 hitbox_left { get; protected set; }
        public override Vector2 hitbox_right { get; protected set; }

        public int max_number { get; private set; }
        public int number { get; set; }

        public item(ContentManager cm, float x, float y, int type, bool on_the_ground, int number)
        {
            this.type = type;
            this.x = x;
            this.y = y;

            this.on_the_ground = on_the_ground;

            this.hitbox_left = new Vector2(0, 0);
            this.hitbox_right = new Vector2(0, 0);

            this.number = number;

            using (StreamReader sr = new StreamReader(@"info\global\items\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmp_string_list = sr.ReadToEnd().Split('\n').ToList();

                this.max_number = Int32.Parse(tmp_string_list[0]);
            }

            update_texture(cm, true);
        }

        public item(ContentManager cm, float x, float y, int type, bool on_the_ground, int number, item sample_item)
        {
            this.type = type;
            this.x = x;
            this.y = y;

            this.on_the_ground = on_the_ground;

            this.hitbox_left = new Vector2(0, 0);
            this.hitbox_right = new Vector2(0, 0);

            this.number = number;
            this.max_number = sample_item.max_number;

            update_texture(cm, true);
        }

        private void update_texture(ContentManager cm, bool something_changed = false)
        {
            if (something_changed == false)
            {
                this.img_phase++;

                if (this.img_phase >= this.textures.Count)
                {
                    this.img_phase = 0;
                }
            }
            else
            {
                this.textures = new List<Texture2D>();
                this.img_phase = 0;

                while (File.Exists(@"Content\" + this.type.ToString() + "item" + this.img_phase.ToString() + this.on_the_ground.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "item" + this.img_phase.ToString() + this.on_the_ground.ToString()));

                    this.img_phase++;
                }

                this.img_phase = 0;
            }
        }

        public override void update(ContentManager cm, island my_island, int my_index, bool somethingSeleted)
        {
            this.update_texture(cm);
        }

        //to  update when in the inventory/recipe etc.
        public void update(ContentManager cm)
        {
            this.update_texture(cm);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            if (!this.on_the_ground)
            {
                spriteBatch.Draw(this.textures[this.img_phase], new Vector2(x, y), Color.White);
            }
            else
            {
                spriteBatch.Draw(this.textures[this.img_phase], new Vector2(x-(int)this.textures[this.img_phase].Width/2, y-this.textures[this.img_phase].Height), Color.White);
            }
        }

        public override List<string> save_list()
        {
            List<string> tmp_list = new List<string>();

            tmp_list.Add("#item");
            tmp_list.Add(this.type.ToString());
            tmp_list.Add(this.x.ToString());
            tmp_list.Add(this.y.ToString());
            tmp_list.Add(this.on_the_ground.ToString());
            tmp_list.Add(this.number.ToString());

            return tmp_list;
        }
    }
}