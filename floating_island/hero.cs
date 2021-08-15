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
        public float speed { get; private set; }
        public string path { get; private set; } = "";
        public override Vector2 hitbox_left { get; protected set; }
        public override Vector2 hitbox_right { get; protected set; }

        private Vector2 target=new Vector2(0, 0);

        List<Texture2D> textures = new List<Texture2D>();
        private int img_phase;

        public string action { get; private set; }
        public string direction { get; protected set; }

        public override int type { get; protected set; }

        public List<int> action_types { get; protected set; } = new List<int>();
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

            this.update_texture(cm, true);
        }

        public override void update(ContentManager cm, island my_island, int my_index)
        {
            if ((target.X != 0 || target.Y != 0) && (this.path == "" || this.path == null))
            {
                this.path = this.find_path(this.x, this.y, target.X, target.Y, speed, my_island, my_index, -1);
            }

            bool action_changes = false;

            string pact = this.action, pdir = this.direction;

            float px = this.x;
            float py = this.y;

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

            int l = 1;

            //checking if shit in hand must not EXIST AT ALL
            if (this.itemInHand != null && this.itemInHand.number <= 0)
            {
                this.itemInHand = null;
            }

            //item delivery logic
            if (this.itemInHand == null)
            {
                if (this.path == null || this.path.Length <= 0)
                {
                    //We need this list, just trust me
                    //I'm just to lazy to write why
                    List<item> neededItems = new List<item>();

                    l = 1;

                    //searching for buildings
                    for (int i = 0; i < my_island.map_Objects.Count; i += l)
                    {
                        map_object currentObject = my_island.map_Objects[i];

                        //if object is building then we add needed items
                        if (currentObject.save_list()[0] == "#building")
                        {
                            neededItems.AddRange(((building)currentObject).itemsToComplete);
                        }
                    }


                    l = 1;

                    float minDist = 1.1f;
                    int minInd = -1;

                    bool end = false;

                    for (int i = 0; i < my_island.map_Objects.Count && !end; i += l)
                    {
                        map_object currentObject = my_island.map_Objects[i];

                        if (currentObject.save_list()[0] == "#item")
                        {
                            foreach(var currentItem in neededItems)
                            {
                                if (currentItem.type == currentObject.type)
                                {
                                    float tmpDist = my_island.get_dist(this.x, this.y, currentObject.x, currentObject.y);

                                    if (tmpDist <= this.speed)
                                    {
                                        this.itemInHand = (item)currentObject;
                                        my_island.delete_object(i);

                                        end = true;

                                        minInd = -1;

                                        break;
                                    }
                                    else if(minDist>tmpDist)
                                    {
                                        minDist = tmpDist;
                                        minInd = i;
                                    }
                                }
                            }
                        }
                    }

                    if (minInd != -1)
                    {
                        this.path = find_path(this.x, this.y, my_island.map_Objects[minInd].x, my_island.map_Objects[minInd].y, this.speed, my_island, my_index, minInd);
                    }
                }
            }
            else if(this.path == null || this.path.Length <= 0)
            {
                float minDist = 1.1f;
                int minInd = -1;

                bool end1 = false;

                for (int i = 0; i < my_island.map_Objects.Count && !end1; i++)
                {
                    map_object currentObject = my_island.map_Objects[i];

                    if(currentObject.save_list()[0]=="#building")
                    {
                        building currentBuilding = (building)currentObject;

                        bool end = false;

                        for (int j = 0; j < currentBuilding.itemsToComplete.Count && !end; j++)
                        {
                            if (currentBuilding.itemsToComplete[j].type == this.itemInHand.type)
                            {
                                float tmpDist = currentBuilding.getSmallestDist(this.x, this.y);

                                if (tmpDist <= this.speed)
                                {
                                    currentBuilding.addItem(this.itemInHand, cm);

                                    this.itemInHand = null;

                                    float tmpx = 0;
                                    float tmpy = 0;

                                    while(!my_island.is_point_free(new Vector2(this.x+tmpx, this.y+tmpy), my_index))
                                    {
                                        int rndr1 = rnd.Next(0, 2);

                                        if (rndr1 == 0)
                                        {
                                            tmpx = currentBuilding.hitbox_left.X * (float)rnd.NextDouble() - this.speed;
                                        }
                                        else
                                        {
                                            tmpx = currentBuilding.hitbox_right.X * (float)rnd.NextDouble() + this.speed;
                                        }

                                        rndr1 = rnd.Next(0, 2);

                                        if (rndr1 == 0)
                                        {
                                            tmpy = currentBuilding.hitbox_left.Y * (float)rnd.NextDouble() - this.speed;
                                        }
                                        else
                                        {
                                            tmpy = currentBuilding.hitbox_right.Y * (float)rnd.NextDouble() + this.speed;
                                        }
                                    }

                                    this.x += tmpx;
                                    this.y += tmpy;

                                    end = true;
                                    end1 = true;

                                    minInd = -1;
                                }
                                else if (minDist > tmpDist)
                                {
                                    minDist = tmpDist;
                                    minInd = i;
                                }
                            }
                        }
                    }
                }

                if (minInd != -1)
                {
                    this.path = find_path(this.x, this.y, my_island.map_Objects[minInd].x, my_island.map_Objects[minInd].y, this.speed, my_island, my_index, minInd);
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

            //checking if coords were changed properly, otherwise rolling them back
            if (!my_island.is_point_free(new Vector2(this.x, this.y), my_index))
            {
                this.x = px;
                this.y = py;
            }

            //updating texture
            if (this.action != pact || this.direction != pdir)
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
            if (something_changed)
            {
                this.img_phase = 0;
                this.textures = new List<Texture2D>();

                while (File.Exists(@"Content\" + this.type.ToString() + "hero" + this.action + this.direction + this.img_phase.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "hero" + this.action + this.direction + this.img_phase.ToString()));

                    this.img_phase++;
                }

                this.img_phase = 0;
            }
            else
            {
                this.img_phase++;

                if (img_phase >= this.textures.Count)
                {
                    img_phase = 0;
                }
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(this.textures[this.img_phase], new Vector2((int)(x - this.textures[this.img_phase].Width / 2), (int)(y - this.textures[this.img_phase].Height)), Color.White);

            if (this.itemInHand != null)
            {
                this.itemInHand.draw(spriteBatch, (int)(x - this.textures[this.img_phase].Width / 2), (int)(y - this.textures[this.img_phase].Height * 1.1f));
            }
        }

        //!!! TOO SLOW, MUST BE CHANGED !!!
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

            if (this.itemInHand == null)
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