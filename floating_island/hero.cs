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
    public class hero : map_object
    {
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        public override List<int> what_to_do_with { get; protected set; } = new List<int>();

        public float speed { get; private set; }
        public List<Vector2> path { get; private set; } = new List<Vector2>();
        public override Vector2 hitbox_left { get; protected set; }
        public override Vector2 hitbox_right { get; protected set; }

        List<Texture2D> textures = new List<Texture2D>();
        private int img_phase;

        public string action { get; private set; }
        public string direction { get; protected set; }
        public override bool selected { get; protected set; }

        public override int type { get; protected set; }

        public List<int> action_types { get; protected set; } = new List<int>();
        public List<button> action_buttons { get; protected set; } = new List<button>();

        public hero(ContentManager cm, int type, float x, float y)
        {
            this.speed = 0.001f;
            this.type = type;

            this.x = x;
            this.y = y;

            this.action = "no";
            this.direction = "s";

            for(int i=0; i<4; i++)
            {
                this.action_types.Add(i);

                Texture2D tmptex0 = cm.Load<Texture2D>(i.ToString() + "actbtn0");
                Texture2D tmptex1 = cm.Load<Texture2D>(i.ToString() + "actbtn1");

                int tmpw = tmptex0.Width, tmph = tmptex0.Height;

                this.action_buttons.Add(new button(1, 0, 0, tmpw, tmph, tmptex0, tmptex1));
            }

            this.update_texture(cm, true);
        }
        
        public override void update(ContentManager cm, island my_island, int my_index, bool somethingSelected)
        {
            bool action_changes = false;
            
            string pact = this.action, pdir = this.direction;

            float px = this.x;
            float py = this.y;

            if (this.selected)
            {
                foreach (var current_button in this.action_buttons)
                {
                    bool pac = current_button.pressed;

                    current_button.update();

                    bool cac = current_button.pressed;

                    if (pac != cac)
                    {
                        action_changes = true;
                    }
                }
            }
            else
            {
                var rnd = new Random();

                int rndr = rnd.Next(0, (int)(0.1f / this.speed));

                if (rndr == 0)
                {
                    this.action = "wa";

                    int rndr1 = rnd.Next(0, 4);

                    if (rndr1 == 0)
                    {
                        this.direction = "w";
                    }

                    if (rndr1 == 1)
                    {
                        this.direction = "a";
                    }

                    if (rndr1 == 2)
                    {
                        this.direction = "s";
                    }

                    if (rndr1 == 3)
                    {
                        this.direction = "d";
                    }
                }
                else if (rndr == 1)
                {
                    this.action = "no";
                }
            }

            if (this.action == "wa")
            {
                if (this.direction == "w")
                {
                    this.y -= this.speed;
                }

                if (this.direction == "a")
                {
                    this.x -= this.speed;
                }

                if (this.direction == "s")
                {
                    this.y += this.speed;
                }

                if (this.direction == "d")
                {
                    this.x += this.speed;
                }
            }
            
            //checking if was selected/unselected

            if (my_island.currentState.LeftButton == ButtonState.Released && my_island.oldState.LeftButton == ButtonState.Pressed && my_island.timeSinceLastPress >= 20)
            {   
                float tmpw = this.textures[this.img_phase].Width / 966f / 2f;
                float tmph = this.textures[this.img_phase].Height / 686f / 2f;

                if (!somethingSelected && my_island.mx >= this.x - tmpw && my_island.mx <= this.x + tmpw && my_island.my <= this.y && my_island.my >= this.y - tmph * 2)
                {
                    this.path = null;

                    this.action = "no";

                    this.selected = true;
                }
                else
                {
                    if (!action_changes)
                    {
                        this.selected = false;
                    }
                }
            }

            //checking if coords were changed properly, otherwise rolling them back
            if (!my_island.is_point_free(new Vector2(this.x, this.y), my_index))
            {
                this.x = px;
                this.y = py;
            }

            //updating texture
            if(this.action!=pact||this.direction!=pdir)
            {
                this.update_texture(cm, true);
            }
            else
            {
                this.update_texture(cm, false);
            }
        }

        private void update_texture(ContentManager cm, bool something_changed = false)
        {
            if(something_changed)
            {
                this.img_phase = 0;
                this.textures = new List<Texture2D>();

                while(File.Exists(@"Content\" + this.type.ToString() + "hero" + this.action + this.direction + this.img_phase.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "hero" + this.action + this.direction + this.img_phase.ToString()));

                    this.img_phase++;
                }

                this.img_phase = 0;
            }
            else
            {
                this.img_phase++;

                if(img_phase>=this.textures.Count)
                {
                    img_phase = 0;
                }
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            int sumx = 0;

            foreach (var current_button in this.action_buttons)
            {
                sumx += (int)(current_button.normal_texture.Width*1.1f);
            }

            int dx = x-(int)(sumx/2);

            foreach (var current_button in this.action_buttons)
            {
                current_button.x = dx;
                current_button.y = y - this.textures[this.img_phase].Height - (int)(current_button.normal_texture.Height*1.1);

                dx += (int)(current_button.normal_texture.Width * 1.1f);
            }

            if (this.selected == false)
            {
                spriteBatch.Draw(this.textures[this.img_phase], new Vector2((int)(x - this.textures[this.img_phase].Width / 2), (int)(y - this.textures[this.img_phase].Height)), Color.White);
            }
            else
            {
                spriteBatch.Draw(this.textures[this.img_phase], new Vector2((int)(x - this.textures[this.img_phase].Width / 2), (int)(y - this.textures[this.img_phase].Height)), new Color(new Vector4(255, 0, 0, 255)));
                
                foreach (var current_button in this.action_buttons)
                {
                    current_button.draw(spriteBatch);
                }
            }
        }

        private List<Vector2> find_path(float x, float y, float dx, float dy, float speed, island my_island, int index_to_ignore)
        {
            bool end = false;
            int end_ind = 0, operations=0;

            List<Tuple<Vector2, Vector2>> discovered = new List<Tuple<Vector2, Vector2>>(), to_check = new List<Tuple<Vector2, Vector2>>(), current = new List<Tuple<Vector2, Vector2>>();

            to_check.Add(Tuple.Create(new Vector2(x, y), new Vector2(x, y)));

            while (!end)
            {
                operations++;

                if(operations>=5000)
                {
                    return null;
                }

                discovered.AddRange(to_check);
                
                if (to_check.Count == 0)
                {
                    return null;
                }

                foreach (var current_point in to_check)
                {
                    Vector2 v1, v2, v3, v4;
                    bool b1 = true, b2 = true, b3 = true, b4 = true;

                    v1 = new Vector2(current_point.Item1.X + speed, current_point.Item1.Y);
                    v2 = new Vector2(current_point.Item1.X - speed, current_point.Item1.Y);
                    v3 = new Vector2(current_point.Item1.X, current_point.Item1.Y + speed);
                    v4 = new Vector2(current_point.Item1.X, current_point.Item1.Y - speed);

                    foreach(var current_item in discovered)
                    {
                        var tmpv = current_item.Item1;

                        if (tmpv.X == v1.X && tmpv.Y == v1.Y)
                        {
                            b1 = false;
                        }

                        if (tmpv.X == v2.X && tmpv.Y == v2.Y)
                        {
                            b2 = false;
                        }

                        if (tmpv.X == v3.X && tmpv.Y == v3.Y)
                        {
                            b3 = false;
                        }
                        
                        if (tmpv.X == v4.X && tmpv.Y == v4.Y)
                        {
                            b4 = false;
                        }
                    }

                    if (!my_island.is_point_free(v1, index_to_ignore))
                    {
                        b1 = false;
                    }

                    if (!my_island.is_point_free(v2, index_to_ignore))
                    {
                        b2 = false;
                    }

                    if (!my_island.is_point_free(v3, index_to_ignore))
                    {
                        b3 = false;
                    }

                    if (!my_island.is_point_free(v4, index_to_ignore))
                    {
                        b4 = false;
                    }

                    if (b1)
                    {
                        current.Add(Tuple.Create(v1, current_point.Item1));
                    }

                    if (b2)
                    {
                        current.Add(Tuple.Create(v2, current_point.Item1));
                    }

                    if (b3)
                    {
                        current.Add(Tuple.Create(v3, current_point.Item1));
                    }

                    if (b4)
                    {
                        current.Add(Tuple.Create(v4, current_point.Item1));
                    }
                }

                to_check = new List<Tuple<Vector2, Vector2>>();

                for (int i = 0; i < discovered.Count; i++)
                {
                    if (my_island.get_dist(dx, dy, discovered[i].Item1.X, discovered[i].Item1.Y) < speed)
                    {
                        end_ind = i;

                        end = true;
                    }
                }

                to_check.AddRange(current);
            }

            List<Vector2> ans = new List<Vector2>();

            while (discovered[end_ind].Item1.X != x || discovered[end_ind].Item1.Y != y)
            {
                var current_point = discovered[end_ind].Item1;
                var prev_point = discovered[end_ind].Item2;

                ans.Add(new Vector2(current_point.X - prev_point.X, current_point.Y - prev_point.Y));

                discovered.RemoveAt(end_ind);

                bool flag = true;

                for(int i=0; i<discovered.Count&&flag; i++)
                {
                    if (discovered[i].Item1.X == prev_point.X && discovered[i].Item1.Y == prev_point.Y)
                    {
                        end_ind = i;
                        flag = false;
                    }
                }
            }

            return ans;
        }

        public override List<string> save_list()
        {
            List<string> tmp_list = new List<string>();

            tmp_list.Add("#hero");
            tmp_list.Add(this.x.ToString());
            tmp_list.Add(this.y.ToString());
            tmp_list.Add(this.type.ToString());

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
