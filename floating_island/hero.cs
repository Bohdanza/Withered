﻿using Microsoft.VisualBasic;
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
        public string path { get; private set; } = "";
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
        public item itemInHand { get; protected set; }

        public hero(ContentManager cm, int type, float x, float y, item handItem)
        {
            this.itemInHand = handItem;

            this.speed = 0.005f;
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

            int l = 1;

            //checking if shit in hand must not EXIST AT ALL
            if (this.itemInHand!=null && this.itemInHand.number <= 0)
            {
                this.itemInHand = null;
            }

            //We need this list, just trust me
            ///I'm just to lazy to write why
            List<item> discoveredItems = new List<item>();
            List<item> neededItems = new List<item>();

            //checking map objects
            for (int i=0; i<my_island.map_Objects.Count; i+=l)
            {
                l = 1;

                if (my_island.map_Objects[i].save_list()[0] == "#item")
                {
                    if (my_island.get_dist(my_island.map_Objects[i].x, my_island.map_Objects[i].y, this.x, this.y) <= this.speed * 3f && this.itemInHand == null)
                    {
                        this.itemInHand = (item)my_island.map_Objects[i];

                        my_island.delete_object(i);
                        l = 0;
                    }
                    else
                    {
                        discoveredItems.Add((item)my_island.map_Objects[i]);                       
                    }
                }
                else if(my_island.map_Objects[i].save_list()[0]=="#building")
                {
                    if (this.itemInHand != null && ((building)my_island.map_Objects[i]).ItemCanBeAdded(itemInHand))
                    {
                        if (my_island.map_Objects[i].contains_point(new Vector2(this.x + this.speed, this.y)) || my_island.map_Objects[i].contains_point(new Vector2(this.x - this.speed, this.y)) || my_island.map_Objects[i].contains_point(new Vector2(this.x, this.y + this.speed)) || my_island.map_Objects[i].contains_point(new Vector2(this.x, this.y - this.speed)))
                        {
                            ((building)my_island.map_Objects[i]).addItem(this.itemInHand, cm);
                        }
                        else if (this.path == null || this.path.Length <= 0)
                        {
                            this.path = this.find_path(this.x, this.y, my_island.map_Objects[i].x, my_island.map_Objects[i].y, this.speed, my_island, my_index, i);
                        }
                    }

                    neededItems.AddRange(((building)my_island.map_Objects[i]).itemsToComplete);
                }
            }

            //for items
            if (this.itemInHand == null && (this.path == null || this.path == ""))
            {
                bool flag = true;

                for (int i = 0; i < neededItems.Count && flag; i++)
                {
                    foreach(var currentItem in discoveredItems)
                    {
                        if(currentItem.type==neededItems[i].type)
                        {
                            this.path = this.find_path(this.x, this.y, currentItem.x, currentItem.y, this.speed, my_island, my_index, -1);
                            
                            if(this.path!=null)
                            {
                                flag = false;
                            }
                        }
                    }
                }
            }

            //checking if path is unempty
            if (this.path != null && this.path.Length > 0)
            {
                this.action = "wa";

                this.direction = this.path[0].ToString();

                this.path = this.path.Remove(0, 1);
            }

            //walking update
            if (this.action == "wa" && !this.selected)
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

            if (this.itemInHand != null)
            {
                this.itemInHand.draw(spriteBatch, (int)(x - this.textures[this.img_phase].Width / 2), (int)(y - this.textures[this.img_phase].Height * 1.1f));
            }
        }

        //!!! TO SLOW, MUST BE CHANGED !!!
        private string find_path(float x, float y, float dx, float dy, float speed, island my_island, int index_to_ignore, int indexToIgnore2)
        {
            List<Tuple<Vector2, string>> current = new List<Tuple<Vector2, string>>();
            List<List<int>> mainList = new List<List<int>>();
            List<int> ignored = new List<int>();

            ignored.Add(indexToIgnore2);
            ignored.Add(index_to_ignore);

            for (float i = (float)x % speed; i < 1f; i += speed)
            {
                List<int> tmpList = new List<int>();

                for (float j = (float)y % speed; j < 1f; j += speed)
                {
                    if (my_island.is_point_free(new Vector2(i, j), ignored))
                    {
                        tmpList.Add(1);
                    }
                    else
                    {
                        tmpList.Add(0);
                    }
                }

                mainList.Add(tmpList);
            }

            current.Add(new Tuple<Vector2, string>(new Vector2((int)(x / speed), (int)(y / speed)), ""));

            dx = (int)(dx / speed);
            dy = (int)(dy / speed);

            bool end = false;

            while (current.Count > 0 && !end)
            {
                List<Tuple<Vector2, string>> queue = new List<Tuple<Vector2, string>>();

                foreach (var currentTuple in current)
                {
                    mainList[(int)(currentTuple.Item1.X)][(int)(currentTuple.Item1.Y)] = 0;

                    Tuple<Vector2, string> t1, t2, t3, t4;

                    t1 = new Tuple<Vector2, string>(new Vector2(currentTuple.Item1.X + 1, currentTuple.Item1.Y), currentTuple.Item2 + "d");
                    t2 = new Tuple<Vector2, string>(new Vector2(currentTuple.Item1.X - 1, currentTuple.Item1.Y), currentTuple.Item2 + "a");
                    t3 = new Tuple<Vector2, string>(new Vector2(currentTuple.Item1.X, currentTuple.Item1.Y + 1), currentTuple.Item2 + "s");
                    t4 = new Tuple<Vector2, string>(new Vector2(currentTuple.Item1.X, currentTuple.Item1.Y - 1), currentTuple.Item2 + "w");
                    
                    if (t1.Item1.X >= 0 && t1.Item1.X < mainList.Count && t1.Item1.Y >= 0 && t1.Item1.Y < mainList[(int)(t1.Item1.X)].Count && mainList[(int)t1.Item1.X][(int)t1.Item1.Y] == 1)
                    {
                        queue.Add(t1);
                        mainList[(int)(t1.Item1.X)][(int)(t1.Item1.Y)] = 0;

                        //   File.AppendAllText("log.txt", t1.Item1.X.ToString() + " " + t1.Item1.Y.ToString() + "\n");

                        if (Math.Abs(t1.Item1.Y - dy) < 1 && Math.Abs(t1.Item1.X - dx) < 1)
                        {
                            return t1.Item2;
                        }
                    }

                    if (t2.Item1.X >= 0 && t2.Item1.X < mainList.Count && t2.Item1.Y >= 0 && t2.Item1.Y < mainList[(int)(t2.Item1.X)].Count && mainList[(int)t2.Item1.X][(int)t2.Item1.Y] == 1)
                    {
                        queue.Add(t2);
                        mainList[(int)(t2.Item1.X)][(int)(t2.Item1.Y)] = 0;

                        //   File.AppendAllText("log.txt", t2.Item1.X.ToString() + " " + t2.Item1.Y.ToString() + "\n");

                        if (Math.Abs(t2.Item1.Y - dy) < 1 && Math.Abs(t2.Item1.X - dx) < 1)
                        {
                            return t2.Item2;
                        }
                    }

                    if (t3.Item1.X >= 0 && t3.Item1.X < mainList.Count && t3.Item1.Y >= 0 && t3.Item1.Y < mainList[(int)(t3.Item1.X)].Count && mainList[(int)t3.Item1.X][(int)t3.Item1.Y] == 1)
                    {
                        queue.Add(t3);
                        mainList[(int)(t3.Item1.X)][(int)(t3.Item1.Y)] = 0;

                        //     File.AppendAllText("log.txt", t3.Item1.X.ToString() + " " + t3.Item1.Y.ToString() + "\n");

                        if (Math.Abs(t3.Item1.Y - dy) < 1 && Math.Abs(t3.Item1.X - dx) < 1)
                        {
                            return t3.Item2;
                        }
                    }

                    if (t4.Item1.X >= 0 && t4.Item1.X < mainList.Count && t4.Item1.Y >= 0 && t4.Item1.Y < mainList[(int)(t4.Item1.X)].Count && mainList[(int)t4.Item1.X][(int)t4.Item1.Y] == 1)
                    {
                        queue.Add(t4);
                        mainList[(int)(t4.Item1.X)][(int)(t4.Item1.Y)] = 0;

                        //    File.AppendAllText("log.txt", t4.Item1.X.ToString() + " " + t4.Item1.Y.ToString() + "\n");

                        if (Math.Abs(t4.Item1.Y - dy) < 1 && Math.Abs(t4.Item1.X - dx) < 1)
                        {
                            return t4.Item2;
                        }
                    }
                }

                current = queue;
            }

            return null;
        }

        public override List<string> save_list()
        {
            List<string> tmp_list = new List<string>();

            tmp_list.Add("#hero");
            tmp_list.Add(this.x.ToString());
            tmp_list.Add(this.y.ToString());
            tmp_list.Add(this.type.ToString());
            
            if(this.itemInHand==null)
            {
                tmp_list.Add("null");
            }
            else
            {
                tmp_list.AddRange(this.itemInHand.save_list());
            }

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
