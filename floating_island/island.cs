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
    public class island
    {
        public float radius { get; private set; }

        public MouseState currentState { get; private set; }
        public MouseState oldState { get; private set; }

        public float mx { get; private set; }
        public float my { get; private set; }

        public int draw_x=0, draw_y=0;

        public int timeSinceLastPress { get; private set; }

        public List<map_object> map_Objects { get; private set; } = new List<map_object>();
        private List<plant> plant_samples = new List<plant>();
        private List<item> item_samples = new List<item>();
        private List<building> buildingSamples = new List<building>();

        private Texture2D crust;
        private Texture2D attentionDarkness, buildingMenuBackground;

        private button buildingMenuOpen, buildingMenuClose;

        private bool buildingMenuClosed = true;
        private int draw_l;

        private building selectedBuilding = null;

        public int ticks = 0;
        private List<building> buildingRecipeList;

        public island(ContentManager cm, List<plant> plant_samples, List<item> item_samples, List<building> buildingSamples, string path)
        {
            this.plant_samples = plant_samples;
            this.item_samples = item_samples;
            this.buildingSamples = buildingSamples;

            this.buildingRecipeList = new List<building>();

            for(int i=0; i<1; i++)
            {
                this.buildingRecipeList.Add(new building(cm, 0f, 0f, i, buildingSamples[i]));
            }

            this.draw_l = 1;

            this.crust = cm.Load<Texture2D>("island_crust");
            this.attentionDarkness = cm.Load<Texture2D>("attentionDarkness");

            Texture2D tmptex = cm.Load<Texture2D>("w0hidebutton");

            this.buildingMenuOpen = new button(0, 800 - (int)(tmptex.Width / 2), 876 - (int)(tmptex.Height * 1.1f), tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("w1hidebutton"));

            tmptex = cm.Load<Texture2D>("s0hidebutton");

            this.buildingMenuClose = new button(0, 800 - (int)(tmptex.Width / 2), 876 - (int)(tmptex.Height * 1.1f), tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("s1hidebutton"));
            
            this.buildingMenuBackground = cm.Load<Texture2D>("buildingbackground");

            if (Directory.Exists(path))
            {
                if(this.Load(path, cm))
                {
                    return;
                }
            }

            this.oldState = Mouse.GetState();
            this.currentState = Mouse.GetState();

            this.generate(0, cm);

            this.timeSinceLastPress = 0;
        }

        private void generate(int biome, ContentManager cm)
        {
            /*List<string> subdir = Directory.GetDirectories(@"info\global\recipes\").ToList();
            
            int it = 0;

            foreach(var current_dir in subdir)
            {
                string c = current_dir;
                
                if(c[c.Length-1]!='\\')
                {
                    c += '\\';
                }

                if (File.Exists(c + "requirements"))
                {
                    if(File.ReadAllText(c + "requirements")[0]=='0')
                    {
                        tmprecipes.Add(new recipe(cm, it, item_samples));
                    }
                }

                it++;
            }*/

            var rnd = new Random();

            //adding_hero

            int c = 0;

            while (c < 3)
            {
                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();
                
                if (this.add_object(new hero(cm, 0, tmpx, tmpy)))
                {
                    c++;
                }
            }

            int tmp_c = rnd.Next(0, 100);

            int tmp_count, l;

            if (tmp_c >= 50)
            {
                //adding mountain

                bool tmpb = true;

                while (tmpb)
                {
                    float tmpx = (float)rnd.NextDouble();
                    float tmpy = (float)rnd.NextDouble();

                    if (this.add_object(new plant(cm, tmpx, tmpy, 5, 0, plant_samples[5])))
                    {
                        tmpb = false;
                    }
                }
            }
            else
            {
                //adding_hills

                tmp_count = rnd.Next(1, 3);

                l = 1;

                for (int i = 0; i < tmp_count; i += l)
                {
                    l = 1;

                    float tmpx = (float)rnd.NextDouble();
                    float tmpy = (float)rnd.NextDouble();

                    //its really important to add objects using this.add_object() because some necessary actions are done in that method 
                    if (!this.add_object(new plant(cm, tmpx, tmpy, 3, 0, this.plant_samples[3])))
                    {
                        int tmp_rnd = rnd.Next(0, 10);

                        if (tmp_rnd <= 6)
                        {
                            l = 0;
                        }
                    }
                }

                tmp_count = rnd.Next(1, 3);

                for (int i = 0; i < tmp_count; i += l)
                {
                    l = 1;

                    float tmpx = (float)rnd.NextDouble();
                    float tmpy = (float)rnd.NextDouble();

                    //its really important to add objects using this.add_object() because some necessary actions are done in that method 
                    if (!this.add_object(new plant(cm, tmpx, tmpy, 4, 0, this.plant_samples[4])))
                    {
                        int tmp_rnd = rnd.Next(0, 10);

                        if (tmp_rnd <= 6)
                        {
                            l = 0;
                        }
                    }
                }
            }

            //adding trees
            tmp_count = rnd.Next(1, 3);

            int t_c = 0;

            for (int i = 0; i < tmp_count; i += l)
            {
                l = 1;

                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();

                //its really important to add objects using this.add_object() because some necessary actions are done in that method 
                if (!this.add_object(new plant(cm, tmpx, tmpy, 0, 0, this.plant_samples[0])))
                {
                    int tmp_rnd = rnd.Next(0, 10);

                    if (tmp_rnd <= 6)
                    {
                        l = 0;
                    }
                }
                else
                {
                    t_c++;
                }
            }

            tmp_count = rnd.Next(1, 3);

            for (int i = 0; i < tmp_count; i += l)
            {
                l = 1;

                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();

                //its really important to add objects using this.add_object() because some necessary actions are done in that method 
                if (!this.add_object(new plant(cm, tmpx, tmpy, 1, 0, this.plant_samples[1])))
                {
                    int tmp_rnd = rnd.Next(0, 10);

                    if (tmp_rnd <= 6)
                    {
                        l = 0;
                    }
                }
                else
                {
                    t_c++;
                }
            }

            tmp_count = (int)((rnd.NextDouble() + 0.1f) * 3 * t_c);

            for (int i = 0; i < tmp_count; i += l)
            {
                l = 1;

                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();

                //its really important to add objects using this.add_object() because some necessary actions are done in that method 
                if (!this.add_object(new item(cm, tmpx, tmpy, 0, true, 1, item_samples[0])))
                {
                    int tmp_rnd = rnd.Next(0, 10);

                    if (tmp_rnd <= 6)
                    {
                        l = 0;
                    }
                }
            }
        }

        public void save(string path, ContentManager cm)
        {
            //preparing path
            if(path[path.Length-1]!=97&& path[path.Length - 1] != 47)
            {
                path += @"\";
            }

            if(!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //preparing file to save map objects list
            if(!File.Exists(path+"map_objects"))
            {
                var tmpf = File.Create(path + "map_objects");
                tmpf.Close();
            }
            else
            {
                File.Delete(path + "map_objects");
                var tmpf = File.Create(path + "map_objects");
                tmpf.Close();
            }

            using (StreamWriter sr = new StreamWriter(path + "map_objects"))
            {
                foreach (var current_object in map_Objects)
                {
                    var tmp_list = current_object.save_list();

                    foreach(var current_string in tmp_list)
                    {
                        sr.WriteLine(current_string);
                    }
                }
            }
        }

        private bool Load(string path, ContentManager cm)
        {
            if (path[path.Length - 1] != '\\' && path[path.Length - 1] != '/')
            {
                path += @"\";
            }

            if (!File.Exists(path+"map_objects"))
            {
                return false;
            }

            using(StreamReader sr = new StreamReader(path + "map_objects"))
            {
                List<string> tmp_str_list = sr.ReadToEnd().Split('\n').ToList();

                int i = 0;

                while(i<tmp_str_list.Count-1)
                {
                    tmp_str_list[i] = tmp_str_list[i].Trim('\n');
                    tmp_str_list[i] = tmp_str_list[i].Trim('\r');

                    if (tmp_str_list[i] == "#plant")
                    {
                        int tmp_type = Int32.Parse(tmp_str_list[i + 1]);
                        float tmp_x = float.Parse(tmp_str_list[i + 2]);
                        float tmp_y = float.Parse(tmp_str_list[i + 3]);

                        int tmp_grow = Int32.Parse(tmp_str_list[i + 4]);

                        this.add_object(new plant(cm, tmp_x, tmp_y, tmp_type, tmp_grow));

                        i += 5;
                    }
                    else if (tmp_str_list[i] == "#item")
                    {
                        int tmp_type = Int32.Parse(tmp_str_list[i + 1]);
                        float tmp_x = float.Parse(tmp_str_list[i + 2]);
                        float tmp_y = float.Parse(tmp_str_list[i + 3]);
                        bool tmp_bool = bool.Parse(tmp_str_list[i + 4]);
                        int amount = Int32.Parse(tmp_str_list[i + 5]);

                        this.add_object(new item(cm, tmp_x, tmp_y, tmp_type, tmp_bool, amount, item_samples[tmp_type]));

                        i += 6;
                    }
                    else if (tmp_str_list[i] == "#hero")
                    {
                        float tmp_x = float.Parse(tmp_str_list[i + 1]);
                        float tmp_y = float.Parse(tmp_str_list[i + 2]);
                        int tmp_type = Int32.Parse(tmp_str_list[i + 3]);

                        this.add_object(new hero(cm, tmp_type, tmp_x, tmp_y));

                        i += 4;
                    }
                    else if (tmp_str_list[i] == "#building")
                    {
                        int tmp_type = Int32.Parse(tmp_str_list[i + 1]);
                        float tmp_x = float.Parse(tmp_str_list[i + 2]);
                        float tmp_y = float.Parse(tmp_str_list[i + 3]);

                        int tmpn = Int32.Parse(tmp_str_list[i + 4]), z=i+4;

                        List<item> tmpItemList = new List<item>();

                        for(i=z+1; i<z+tmpn*2; i+=2)
                        {
                            int tmp_type1 = Int32.Parse(tmp_str_list[i]);
                            int number = Int32.Parse(tmp_str_list[i+1]);

                            tmpItemList.Add(new item(cm, 0f, 0f, tmp_type1, false, number, this.item_samples[tmp_type1]));
                        }

                        this.add_object(new building(cm, tmp_x, tmp_y, tmp_type, this.buildingSamples[tmp_type], tmpItemList));
                    }
                }
            }

            return true;
        }

        public void update(ContentManager cm)
        {
            this.timeSinceLastPress++;
            this.ticks++;

            if(this.ticks>=1000000)
            {
                this.ticks = 0;
            }

            //getting mouse cursor position and converting it into island coords
            this.currentState = Mouse.GetState();

            this.mx = (float)(this.currentState.X - 316 - this.draw_x) / 966;
            this.my = (float)(this.currentState.Y - 182 - this.draw_y) / 696;

            bool somethingSelected = false;

            if(this.selectedBuilding!=null)
            {
                somethingSelected = true;
            }

            //updating buttons that to open/close building menu
            if (this.selectedBuilding == null)
            {
                if (this.buildingMenuClosed)
                {
                    if (this.currentState.LeftButton == ButtonState.Pressed)
                    {
                        bool f = true;

                        for (int i = 0; i < this.buildingRecipeList.Count && f; i++)
                        {
                            var tmprect = new Rectangle(i * 150 + 24, this.buildingMenuOpen.y + (int)(this.buildingMenuOpen.normal_texture.Height * 1.1f + 24), 134, 134);

                            if (tmprect.Contains(new Vector2(this.currentState.X, this.currentState.Y)))
                            {
                                this.selectedBuilding = new building(cm, mx, my, this.buildingRecipeList[i].type);

                                f = false;
                            }
                        }
                    }

                    this.buildingMenuOpen.update();

                    if (this.buildingMenuOpen.pressed)
                    {
                        somethingSelected = true;

                        this.buildingMenuClosed = false;
                    }
                }
                else
                {
                    if (this.currentState.LeftButton == ButtonState.Pressed)
                    {
                        bool f = true;

                        for (int i = 0; i < this.buildingRecipeList.Count && f; i++)
                        {
                            var tmprect = new Rectangle(i * 150 + 24, this.buildingMenuClose.y + (int)(this.buildingMenuClose.normal_texture.Height * 1.1f + 24), 134, 134);

                            if (tmprect.Contains(new Vector2(this.currentState.X, this.currentState.Y)))
                            {
                                this.selectedBuilding = new building(cm, mx, my, this.buildingRecipeList[i].type);

                                f = false;
                            }
                        }
                    }

                    this.buildingMenuClose.update();

                    if (this.buildingMenuClose.pressed)
                    {
                        somethingSelected = true;

                        this.buildingMenuClosed = true;
                    }
                }
            }

            if (this.selectedBuilding != null)
            {
                this.selectedBuilding.update(cm, this, -1, somethingSelected);
                this.selectedBuilding.changeCoords(new Vector2(this.mx, this.my));

                if(this.currentState.LeftButton == ButtonState.Pressed)
                {
                    if(this.add_object(this.selectedBuilding))
                    {
                        this.selectedBuilding = null;

                        somethingSelected = true;
                    }
                }
            }

            //checking if map objects were selected
            foreach (var currentObject in this.map_Objects)
            {
                if(currentObject.selected)
                {
                    somethingSelected = true;
                }
            }

            //updating all the objects
            int l = 1, pc=this.map_Objects.Count;

            for (int i=0; i<this.map_Objects.Count; i+=l)
            {
                l = 1;

                this.map_Objects[i].update(cm, this, i, somethingSelected);

                if(this.map_Objects.Count<pc)
                {
                    l = 0;
                }
                else if(this.map_Objects[i].selected)
                {
                    somethingSelected = true;
                }

                pc = this.map_Objects.Count;
            }
    
            if (this.currentState.LeftButton == ButtonState.Released && this.oldState.LeftButton == ButtonState.Pressed)
            {
                this.timeSinceLastPress = 0;
            }

            this.oldState = this.currentState;

            //We need to keep our object list sorted by y axis to overlay images properly when drawing
            //so we will sort them here in case if some objects were moved
            this.map_Objects.Sort((a, b) => (a.y).CompareTo(b.y));

            //for building menu appear animation
            if(this.buildingMenuClosed)
            {
                if (this.buildingMenuClose.y < 876 - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f))
                {
                    this.buildingMenuClose.y += 10;
                    this.buildingMenuOpen.y += 10;
                }
            }
            else
            {
                if(this.buildingMenuClose.y > 900 - this.buildingMenuBackground.Height - (int)(this.buildingMenuClose.normal_texture.Height*1.1f))
                {
                    this.buildingMenuClose.y -= 10;
                    this.buildingMenuOpen.y -= 10;
                }
            }

            //can be used for island y moving
            //still unfinished
           /* if (this.ticks % 15 == 0)
            {
                this.draw_y += this.draw_l;

                if (this.draw_y < 0 || this.draw_y > 3)
                {
                    this.draw_l *= -1;
                }
            }*/
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(crust, new Vector2(this.draw_x, this.draw_y), Color.White);

            //drawing map objects
            foreach (var current_object in this.map_Objects)
            {
                int draw_x = (int)(316 + this.draw_x + current_object.x * 966);
                int draw_y = (int)(183 + this.draw_y + current_object.y * 686);

                current_object.draw(spriteBatch, draw_x, draw_y);
            }

            if (this.selectedBuilding != null)
            {
                this.selectedBuilding.draw(spriteBatch, (int)(316 + this.draw_x + this.selectedBuilding.x * 966), (int)(183 + this.draw_y + this.selectedBuilding.y * 686));
            }

            //drawing some effects
            spriteBatch.Draw(attentionDarkness, new Vector2(0, 0), Color.White);

            //drawing buildings on building panel
            if (this.selectedBuilding == null)
            {
                if (this.buildingMenuClosed)
                {
                    this.buildingMenuOpen.draw(spriteBatch);

                    spriteBatch.Draw(this.buildingMenuBackground, new Vector2(0, this.buildingMenuOpen.y + (int)(this.buildingMenuOpen.normal_texture.Height * 1.1f)), Color.White);

                    for (int i = 0; i < this.buildingRecipeList.Count; i++)
                    {
                        this.buildingRecipeList[i].drawAsRecipe(spriteBatch, i * 150 + 24, this.buildingMenuOpen.y + (int)(this.buildingMenuOpen.normal_texture.Height * 1.1f + 24));
                    }
                }
                else
                {
                    this.buildingMenuClose.draw(spriteBatch);

                    spriteBatch.Draw(this.buildingMenuBackground, new Vector2(0, this.buildingMenuClose.y + (int)(this.buildingMenuClose.normal_texture.Height * 1.1f)), Color.White);

                    for (int i = 0; i < this.buildingRecipeList.Count; i++)
                    {
                        this.buildingRecipeList[i].drawAsRecipe(spriteBatch, i * 150 + 24, this.buildingMenuClose.y + (int)(this.buildingMenuClose.normal_texture.Height * 1.1f + 24));
                    }
                }
            }
        }
        
        public bool add_object(map_object object_to_add)
        {
            //we need to check if some of our hitbox points cant be where they want to be
            //cos U CAN'T JUST DO WHAT YOU WANT

            bool b1 = this.is_point_free(new Vector2(object_to_add.x + object_to_add.hitbox_left.X, object_to_add.y + object_to_add.hitbox_left.Y), new List<int>());
            bool b2 = this.is_point_free(new Vector2(object_to_add.x + object_to_add.hitbox_left.X, object_to_add.y + object_to_add.hitbox_right.Y), new List<int>());
            bool b3 = this.is_point_free(new Vector2(object_to_add.x + object_to_add.hitbox_right.X, object_to_add.y + object_to_add.hitbox_left.Y), new List<int>());
            bool b4 = this.is_point_free(new Vector2(object_to_add.x + object_to_add.hitbox_right.X, object_to_add.y + object_to_add.hitbox_right.Y), new List<int>());

            if (!b1 || !b2 || !b3 || !b4)
            {
                return false;
            }

            this.map_Objects.Add(object_to_add);

            return true;
        }

        public bool delete_object(int indexToDeleteAt)
        {
            if(indexToDeleteAt>=0&&indexToDeleteAt<this.map_Objects.Count)
            {
                map_Objects.RemoveAt(indexToDeleteAt);

                return true;
            }

            return false;
        }

        public float get_dist(float x, float y, float x1, float y1)
        {
            //finding distance using Pifagorean theorem
            return (float)Math.Sqrt(Math.Abs(x - x1) * Math.Abs(x - x1) + Math.Abs(y - y1) * Math.Abs(y - y1));
        }

        public bool is_point_free(Vector2 point, List<int> indexes_to_ignore)
        {
            float d1 = this.get_dist(point.X, point.Y, 0.5f, 0.5f);

            //checking if point is out of island

            if(d1>=0.5f)
            {
                return false;
            }


            //checking hitboxes

            for(int i=0; i<this.map_Objects.Count; i++)
            {
                if(!indexes_to_ignore.Contains(i))
                {
                    if(this.map_Objects[i].contains_point(point))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool is_point_free(Vector2 point, int index_to_ignore)
        {
            float d1 = this.get_dist(point.X, point.Y, 0.5f, 0.5f);

            //checking if point is out of island

            if (d1 >= 0.5f)
            {
                return false;
            }


            //checking hitboxes

            for (int i = 0; i < this.map_Objects.Count; i++)
            {
                if (index_to_ignore!=i)
                {
                    if (this.map_Objects[i].contains_point(point))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}