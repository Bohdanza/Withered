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
    public class plant : map_object
    {
        List<Texture2D> textures = new List<Texture2D>();
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        public override int type { get; protected set; }
        private int img_phase, grow_stage;
        public int max_grow { get; private set; }
        public string action { get; private set; }

        //!IMPORTANT!
        //hitbox points are stored as values that must be ADDED (even for the left hitbox corner) to get the real hitbox
        public override Vector2 hitbox_left { get; protected set; }
        public override Vector2 hitbox_right { get; protected set; }

        public plant(ContentManager cm, float x, float y, int type, int grow_stage)
        {
            this.x = x;
            this.y = y;

            this.action = "no";

            this.type = type;

            this.grow_stage = grow_stage;

            using(StreamReader sr = new StreamReader(@"info\global\plants\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmp_list = sr.ReadToEnd().Split('\n').ToList();

                this.max_grow = Int32.Parse(tmp_list[0]);

                this.hitbox_left = new Vector2(float.Parse(tmp_list[1]), float.Parse(tmp_list[2]));
                this.hitbox_right = new Vector2(float.Parse(tmp_list[3]), float.Parse(tmp_list[4]));
            }

            this.update_texture(cm, true);
        }

        public plant(ContentManager cm, float x, float y, int type, int grow_stage, plant sample_plant)
        {
            this.x = x;
            this.y = y;

            this.action = "no";

            this.type = type;

            this.max_grow = sample_plant.max_grow;
            
            this.grow_stage = grow_stage;

            this.hitbox_left = new Vector2(sample_plant.hitbox_left.X, sample_plant.hitbox_left.Y);
            this.hitbox_right = new Vector2(sample_plant.hitbox_right.X, sample_plant.hitbox_right.Y);

            this.update_texture(cm, true);
        }

        private void update_texture(ContentManager cm, bool something_changed = false)
        {
            if (something_changed == false)
            {
                this.img_phase++;
                
                if(this.img_phase>=this.textures.Count)
                {
                    this.img_phase = 0;
                }
            }
            else
            {
                this.textures = new List<Texture2D>();
                this.img_phase = 0;

                while(File.Exists(@"Content\" + this.type.ToString() + "plant" + this.img_phase.ToString() + this.action + this.grow_stage.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "plant" + this.img_phase.ToString() + this.action + this.grow_stage.ToString()));

                    this.img_phase++;
                }

                this.img_phase = 0;
            }
        }

        public override void update(ContentManager cm, island my_island, int my_index)
        {
            bool action_changes = false;

            this.update_texture(cm);
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            int tmpw = this.textures[this.img_phase].Width;
            int tmph = this.textures[this.img_phase].Height;

            spriteBatch.Draw(this.textures[this.img_phase], new Vector2(x - tmpw / 2, y - tmph), Color.White);
        }

        public override bool contains_point(Vector2 point)
        {
            if (point.X >= this.x + hitbox_left.X && point.Y >= this.y + hitbox_left.Y && point.X <= this.x + hitbox_right.X && point.Y <= this.y + hitbox_right.Y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override List<string> save_list()
        {
            List<string> tmp_list = new List<string>();

            tmp_list.Add("#plant");
            tmp_list.Add(this.type.ToString());
            tmp_list.Add(this.x.ToString());
            tmp_list.Add(this.y.ToString());
            tmp_list.Add(this.grow_stage.ToString());

            return tmp_list;
        }

        //later i'll add defence against stupid here 
        public override bool changeCoords(Vector2 newCoords)
        {
            this.x = newCoords.X;
            this.y = newCoords.Y;

            return true;
        }
    }
}