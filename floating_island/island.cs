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
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace floating_island
{
    public class island
    {
        public const int crustWidth = 760;
        public const int crustHeight = 517;
        
        public const int crustXPos = 410;
        public const int crustYPos = 323;

        public const int waveDelay = 2000;

        public int timeSinceLastWave { get; private set; }
        public int waveNumber { get; private set; }

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
        private List<monster> monsterSamples = new List<monster>();

        private Texture2D crust;
        private Texture2D attentionDarkness, buildingMenuBackground, researchBackground;

        private button buildingMenuOpen, buildingMenuClose, researchMenuOpen, researchMenuClose, cancelButton, tutorialMenuOpen, tutorialMenuClose;

        private TextDisplay tutorialText;

        private bool buildingMenuClosed = true, researchMenuClosed = true, tutorialMenuClosed = true, tutorialTextShown = false;
        private int draw_l, researchMenuPos = -900, tutorialMenuPos = -900, tutorialTextPos=901;

        private building selectedBuilding = null;

        public int ticks = 0;
        private List<building> buildingRecipeList;

        private List<ResearchPoint> researchPoints = new List<ResearchPoint>();
        private researchTree mainResearchTree;
        private SpriteFont wavesFont;

        public SoundEffect waveSound { get; private set; }

        public island(ContentManager cm, List<plant> plant_samples, List<item> item_samples, List<building> buildingSamples, List<monster> monsterSamples, string path)
        {
            this.timeSinceLastWave = 256;
            this.waveNumber = 0;

            this.plant_samples = plant_samples;
            this.item_samples = item_samples;
            this.buildingSamples = buildingSamples;
            this.monsterSamples = monsterSamples;

            this.draw_l = 1;

            this.waveSound = cm.Load<SoundEffect>("drumBeat");

            this.wavesFont = cm.Load<SpriteFont>("metalFont");

            this.crust = cm.Load<Texture2D>("island_crust");
            this.attentionDarkness = cm.Load<Texture2D>("attentionDarkness");

            Texture2D tmptex = cm.Load<Texture2D>("w0hidebutton");

            this.buildingMenuOpen = new button(0, 800 - (int)(tmptex.Width / 2), 876 - (int)(tmptex.Height * 1.1f), tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("w1hidebutton"));

            tmptex = cm.Load<Texture2D>("s0hidebutton");

            this.buildingMenuClose = new button(0, 800 - (int)(tmptex.Width / 2), 876 - (int)(tmptex.Height * 1.1f), tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("s1hidebutton"));


            tmptex = cm.Load<Texture2D>("recmenuopen0");

            this.researchMenuOpen = new button(0, 15, this.buildingMenuOpen.y - (int)(tmptex.Height*1.1f), tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("recmenuopen1"));

            tmptex = cm.Load<Texture2D>("cross0");

            this.researchMenuClose = new button(0, 1452, 19, tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("cross1"));


            tmptex = cm.Load<Texture2D>("tutorialmenuopen0");

            this.tutorialMenuOpen = new button(0, 15, this.researchMenuOpen.y - (int)(tmptex.Height * 1.1f), tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("tutorialmenuopen1"));

            tmptex = cm.Load<Texture2D>("cross0");

            this.tutorialMenuClose = new button(0, 1452, 19, tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("cross1"));


            tmptex = cm.Load<Texture2D>("backbutton0");

            this.cancelButton = new button(0, 33, 33, tmptex.Width, tmptex.Height, tmptex, cm.Load<Texture2D>("backbutton1"));

            this.buildingMenuBackground = cm.Load<Texture2D>("buildingbackground");
            this.researchBackground = cm.Load<Texture2D>("evomenu");

            this.tutorialText = new TextDisplay(cm, cm.Load<SpriteFont>("menu_font"), @"info\global\tutorial", 40);

            this.timeSinceLastPress = 0;

            this.oldState = Mouse.GetState();
            this.currentState = Mouse.GetState();

            if (Directory.Exists(path))
            {
                if(this.Load(path, cm))
                {
                    return;
                }
            }

            this.generate(0, cm);
        }

        private void generate(int biome, ContentManager cm)
        {
            this.buildingRecipeList = new List<building>();
            this.map_Objects = new List<map_object>();
            this.buildingRecipeList = new List<building>();
            this.researchPoints = new List<ResearchPoint>();

            this.waveNumber = 0;

            for (int i = 0; i < 1; i++)
            {
                this.buildingRecipeList.Add(new building(cm, 0f, 0f, i, buildingSamples[i], buildingSamples[i].maxhp));
            }

            //initializing recipe tree
            using (StreamReader sr = new StreamReader(@"info\global\recipes\tree_info"))
            {
                List<researchRecipe> tmpres = new List<researchRecipe>();

                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                int n = Int32.Parse(tmplist[0]);

                for(int i=1; i<n*2; i+=2)
                {
                    bool tmpbool = false;

                    if(tmplist[i + 1].Trim('\n').Trim('\r')=="-1")
                    {
                        tmpbool = true;
                    }

                    tmpres.Add(new researchRecipe(cm, Int32.Parse(tmplist[i]), tmpbool, Int32.Parse(tmplist[i + 1])));
                }

                this.mainResearchTree = new researchTree(tmpres, cm);
            }

            //------
            var tmpfont = cm.Load<SpriteFont>("pointsFont");

            for(int i=0; i<1; i++)
            {
                this.researchPoints.Add(new ResearchPoint(cm, i, 0, tmpfont));
            }
            
            this.add_object(new building(cm, 0.5f, 0.5f, 7, this.buildingSamples[7], new List<item>(), buildingSamples[7].maxhp));
            
            var rnd = new Random();

            //adding items
            int tmp_c = rnd.Next(5, 7);
            int c = 0;
            
            while (c < tmp_c)
            {
                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();

                if (this.add_object(new item(cm, tmpx, tmpy, 0, true, 1, item_samples[0])))
                {
                    c++;
                }
            }

            tmp_c = rnd.Next(3, 6);
            c = 0;

            while (c < tmp_c)
            {
                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();

                if (this.add_object(new item(cm, tmpx, tmpy, 1, true, 1, item_samples[1])))
                {
                    c++;
                }
            }

            int tmp_count, l;

            //adding heroes
            c = 0;

            while (c < 1)
            {
                float tmpx = (float)rnd.NextDouble();
                float tmpy = (float)rnd.NextDouble();

                if (this.add_object(new hero(cm, 0, tmpx, tmpy, null)))
                {
                    c++;
                }
            }

            //TEST
            c = 0;

            while (c < 2)
            {
                float tmpx = (float)rnd.NextDouble() * 0.1f + 0.8f;
                float tmpy = (float)rnd.NextDouble() * 0.1f + 0.8f;

                if (this.add_object(new monster(cm, tmpx, tmpy, this.monsterSamples[0].hp, this.monsterSamples[0])))
                {
                    c++;
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

            //preparing points and researches file
            if (!File.Exists(path + "researches"))
            {
                var tmpf = File.Create(path + "researches");
                tmpf.Close();
            }
            else
            {
                File.Delete(path + "researches");
                var tmpf = File.Create(path + "researches");
                tmpf.Close();
            }
            
            //preparing wave_info file
            if (!File.Exists(path + "wave_info"))
            {
                var tmpf = File.Create(path + "wave_info");
                tmpf.Close();
            }
            else
            {
                File.Delete(path + "wave_info");
                var tmpf = File.Create(path + "wave_info");
                tmpf.Close();
            }

            using (StreamWriter sr = new StreamWriter(path + "map_objects"))
            {
                sr.Flush();

                foreach (var current_object in map_Objects)
                {
                    var tmp_list = current_object.save_list();

                    foreach(var current_string in tmp_list)
                    {
                        sr.WriteLine(current_string);
                    }
                }
            }

            using (StreamWriter sr = new StreamWriter(path + "researches"))
            {
                sr.WriteLine(this.researchPoints.Count);

                for (int i = 0; i < this.researchPoints.Count; i++)
                {
                    sr.WriteLine(this.researchPoints[i].amount);
                }

                sr.WriteLine(this.mainResearchTree.researchRecipes.Count.ToString());

                for (int i = 0; i < this.mainResearchTree.researchRecipes.Count; i++)
                {
                    sr.WriteLine(this.mainResearchTree.researchRecipes[i].type.ToString());
                    sr.WriteLine(this.mainResearchTree.researchRecipes[i].researched.ToString());
                    sr.WriteLine(this.mainResearchTree.researchRecipes[i].parentType.ToString());
                }
            }

            using (StreamWriter sr = new StreamWriter(path + "building_recipes"))
            {
                sr.WriteLine(this.buildingRecipeList.Count.ToString());

                foreach(var currentBuilding in this.buildingRecipeList)
                {
                    sr.WriteLine(currentBuilding.type.ToString());
                }
            }

            using (StreamWriter sr = new StreamWriter(path + "wave_info"))
            {
                sr.WriteLine(this.waveNumber.ToString());
            }
        }

        private bool Load(string path, ContentManager cm)
        {
            try
            {
                if (path[path.Length - 1] != '\\' && path[path.Length - 1] != '/')
                {
                    path += @"\";
                }

                if (!File.Exists(path + "researches") || !File.Exists(path + "map_objects"))
                {
                    return false;
                }

                using (StreamReader sr = new StreamReader(path + "map_objects"))
                {
                    List<string> tmp_str_list = sr.ReadToEnd().Split('\n').ToList();

                    int i = 0;

                    while (i < tmp_str_list.Count - 1)
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

                            item tmpitem;

                            if (tmp_str_list[i + 4].Trim('\r') == "null" || tmp_str_list[i + 4].Trim('\n') == "null")
                            {
                                tmpitem = null;

                                i++;
                            }
                            else
                            {
                                int tmp_type1 = Int32.Parse(tmp_str_list[i + 5]);
                                float tmp_x1 = float.Parse(tmp_str_list[i + 6]);
                                float tmp_y1 = float.Parse(tmp_str_list[i + 7]);
                                bool tmp_bool1 = bool.Parse(tmp_str_list[i + 8]);
                                int amount1 = Int32.Parse(tmp_str_list[i + 9]);

                                tmpitem = new item(cm, tmp_x1, tmp_y1, tmp_type1, tmp_bool1, amount1, item_samples[tmp_type1]);

                                i += 6;
                            }

                            this.add_object(new hero(cm, tmp_type, tmp_x, tmp_y, tmpitem));

                            i += 4;
                        }
                        else if (tmp_str_list[i] == "#building")
                        {
                            int tmp_type = Int32.Parse(tmp_str_list[i + 1]);
                            float tmp_x = float.Parse(tmp_str_list[i + 2]);
                            float tmp_y = float.Parse(tmp_str_list[i + 3]);
                            int tmp_hp = Int32.Parse(tmp_str_list[i + 4]);
                                
                            int tmpn = Int32.Parse(tmp_str_list[i + 5]), z = i + 5;

                            List<item> tmpItemList = new List<item>();

                            for (i = z + 1; i < z + tmpn * 2; i += 2)
                            {
                                int tmp_type1 = Int32.Parse(tmp_str_list[i]);
                                int number = Int32.Parse(tmp_str_list[i + 1]);

                                tmpItemList.Add(new item(cm, 0f, 0f, tmp_type1, false, number, this.item_samples[tmp_type1]));
                            }

                            this.add_object(new building(cm, tmp_x, tmp_y, tmp_type, this.buildingSamples[tmp_type], tmpItemList, tmp_hp));
                        }
                        else if(tmp_str_list[i] == "#monster")
                        {
                            int tmp_type = Int32.Parse(tmp_str_list[i + 1]);
                            float tmp_x = float.Parse(tmp_str_list[i + 2]);
                            float tmp_y = float.Parse(tmp_str_list[i + 3]);
                            int tmpHp = Int32.Parse(tmp_str_list[i + 4]);

                            this.add_object(new monster(cm, tmp_x, tmp_y, tmpHp, this.monsterSamples[tmp_type]));

                            i += 5;
                        }
                        else if (tmp_str_list[i] == "#bullet")
                        {
                            int tmp_type = Int32.Parse(tmp_str_list[i + 1]);
                            float tmp_x = float.Parse(tmp_str_list[i + 2]);
                            float tmp_y = float.Parse(tmp_str_list[i + 3]);
                            float tmpdir = float.Parse(tmp_str_list[i + 4]);

                            this.add_object(new bullet(cm, tmp_type, tmp_x, tmp_y, tmpdir));

                            i += 5;
                        }
                        else
                        {
                            i++;
                        }
                    }
                }

                using (StreamReader sr = new StreamReader(path + "researches"))
                {
                    int n = Int32.Parse(sr.ReadLine().Trim('\n').Trim('\r'));

                    for (int i = 0; i < n; i++)
                    {
                        this.researchPoints.Add(new ResearchPoint(cm, i, Int32.Parse(sr.ReadLine().Trim('\n').Trim('\r')), null));
                    }

                    n = Int32.Parse(sr.ReadLine().Trim('\n').Trim('\r'));

                    List<researchRecipe> tmpResList = new List<researchRecipe>();

                    for (int i = 0; i < n; i++)
                    {
                        int tmpType = Int32.Parse(sr.ReadLine().Trim('\n').Trim('\r'));
                        bool researched = bool.Parse(sr.ReadLine().Trim('\n').Trim('\r'));
                        int tmpParentType = Int32.Parse(sr.ReadLine().Trim('\n').Trim('\r'));

                        tmpResList.Add(new researchRecipe(cm, tmpType, researched, tmpParentType));
                    }

                    this.mainResearchTree = new researchTree(tmpResList, cm);
                }
                
                using (StreamReader sr = new StreamReader(path + "building_recipes"))
                {
                    this.buildingRecipeList = new List<building>();

                    List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                    int n = Int32.Parse(tmplist[0]);

                    for (int i = 1; i <= n; i++)
                    {
                        int tmptype = Int32.Parse(tmplist[i]);

                        this.buildingRecipeList.Add(new building(cm, 0f, 0f, tmptype, this.buildingSamples[tmptype], buildingSamples[i].maxhp));
                    }
                }

                using (StreamReader sr = new StreamReader(path + "wave_info"))
                {
                    List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                    this.waveNumber = Int32.Parse(tmplist[0]);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        
        public void update(ContentManager cm)
        {
            this.oldState = this.currentState; 
            this.currentState = Mouse.GetState();

            this.timeSinceLastPress++;
            this.ticks++;

            if(this.ticks>=1000000)
            {
                this.ticks = 0;
            }

            if (this.researchMenuClosed && this.tutorialMenuClosed)
            {
                this.timeSinceLastWave++;
                
                if (this.timeSinceLastWave == waveDelay)
                {
                    this.timeSinceLastWave = 0;
                    this.waveNumber++;

                    this.waveSound.Play();

                    var rnd = new Random();

                    int tmpc1 = 0;
                    int tmpc = 5 + (this.waveNumber - 1) * 3;

                    while (tmpc1 < tmpc)
                    {
                        float tmpx = (float)rnd.NextDouble();
                        float tmpy = (float)rnd.NextDouble();

                        if (this.add_object(new monster(cm, tmpx, tmpy, this.monsterSamples[0].hp, this.monsterSamples[0])))
                        {
                            tmpc1++;
                        }
                    }
                    
                    tmpc1 = 0;
                    tmpc =  (int)(0.25*this.waveNumber*this.waveNumber);

                    while (tmpc1 < tmpc)
                    {
                        float tmpx = (float)rnd.NextDouble();
                        float tmpy = (float)rnd.NextDouble();

                        if (this.add_object(new monster(cm, tmpx, tmpy, this.monsterSamples[1].hp, this.monsterSamples[1])))
                        {
                            tmpc1++;
                        }
                    }
                }
            }

            //Just for testing
            /*if(this.ticks%300 == 0)
            {
                this.add_object(new bullet(cm, 0, 0.6f, 0.6f, 90));
            }*/
            
            //getting mouse cursor position and converting it into island coords
            this.mx = (float)(this.currentState.X - crustXPos - this.draw_x) / crustWidth;
            this.my = (float)(this.currentState.Y - crustYPos - this.draw_y) / crustHeight;

            if (this.selectedBuilding == null && this.researchMenuClosed && this.tutorialMenuClosed)
            {
                this.researchMenuOpen.update();

                this.researchMenuOpen.y = this.buildingMenuOpen.y - (int)(this.researchMenuOpen.normal_texture.Height * 1.1f);

                if (this.researchMenuOpen.pressed)
                {
                    this.researchMenuClosed = false;
                }

                this.tutorialMenuOpen.update();

                this.tutorialMenuOpen.y = this.researchMenuOpen.y - (int)(this.tutorialMenuOpen.normal_texture.Height * 1.1f);

                if(this.tutorialMenuOpen.pressed)
                {
                    this.tutorialMenuClosed = false;
                }
            }

            //updating buttons
            if (this.researchMenuClosed && this.tutorialMenuClosed)
            {
                //rolling rec and tutorial menus up
                if (this.researchMenuPos > -900)
                {
                    this.researchMenuPos -= 50;

                    this.researchMenuClose.y = this.researchMenuPos + (int)(this.researchMenuClose.normal_texture.Height * 0.7f);

                    if (this.researchMenuPos < -900)
                    {
                        this.researchMenuPos = -900;
                    }
                }

                if (this.tutorialMenuPos > -900)
                {
                    this.tutorialMenuPos -= 50;

                    this.tutorialMenuClose.y = this.tutorialMenuPos + (int)(this.tutorialMenuClose.normal_texture.Height * 0.7f);

                    if (this.tutorialMenuPos <= -900)
                    {
                        this.tutorialMenuPos = -900;
                        this.tutorialTextPos = 901;
                    }
                }

                if(this.tutorialTextPos<901)
                {
                    this.tutorialTextPos += 50;

                    if(this.tutorialTextPos>901)
                    {
                        this.tutorialTextPos = 901;
                    }
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
                            this.buildingMenuClosed = true;
                        }
                    }
                }

                if (this.selectedBuilding != null)
                {
                    this.selectedBuilding.update(cm, this, -1);
                    this.selectedBuilding.changeCoords(new Vector2(this.mx, this.my));

                    this.cancelButton.update();

                    if (this.currentState.LeftButton == ButtonState.Pressed)
                    {
                        if (this.add_object(this.selectedBuilding))
                        {
                            this.selectedBuilding = null;
                        }
                    }

                    if (this.cancelButton.pressed)
                    {
                        this.selectedBuilding = null;
                    }
                }

                //updating all the objects
                int l = 1, pc = this.map_Objects.Count;

                List<Tuple<building, bool>> completedList = new List<Tuple<building, bool>>();

                for (int i = 0; i < this.map_Objects.Count; i += l)
                {
                    if (this.map_Objects[i].save_list()[0] == "#building")
                    {
                        building tmpbuilding = (building)this.map_Objects[i];

                        completedList.Add(new Tuple<building, bool>(tmpbuilding, tmpbuilding.itemsToComplete.Count <= 0));
                    }
                }

                for (int i = 0; i < this.map_Objects.Count; i += l)
                {
                    try
                    {
                        l = 1;

                        this.map_Objects[i].update(cm, this, i);

                        if (!this.map_Objects[i].alive)
                        {
                            this.delete_object(i);
                            l = 0;
                        }

                        if (this.map_Objects.Count < pc)
                        {
                            l = 0;
                        }

                        pc = this.map_Objects.Count;
                    }
                    catch
                    {
                        break;
                    }
                }

                l = 1;

                foreach (var currentBuilding in completedList)
                {
                    if ((currentBuilding.Item1.itemsToComplete.Count <= 0) != currentBuilding.Item2)
                    {
                        this.addResearchPoints(currentBuilding.Item1.researchPointsAdded);
                    }
                }

                if (this.currentState.LeftButton == ButtonState.Released && this.oldState.LeftButton == ButtonState.Pressed)
                {
                    this.timeSinceLastPress = 0;
                }

                //We need to keep our object list sorted by y axis to overlay images properly when drawing
                //so we will sort them here in case if some objects were moved
                this.map_Objects.Sort((a, b) => (a.y).CompareTo(b.y));

                //for building menu appear animation
                if (this.buildingMenuClosed)
                {
                    //880 insted of 900 to show part of building menu
                    if (this.buildingMenuClose.y < 880 - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f))
                    {
                        this.buildingMenuClose.y += 10;
                        this.buildingMenuOpen.y += 10;

                        if (this.buildingMenuOpen.y > 880 - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f))
                        {
                            this.buildingMenuOpen.y = 880 - (int)(this.buildingMenuOpen.normal_texture.Height * 1.1f);
                            this.buildingMenuClose.y = 880 - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f);
                        }
                    }
                }
                else
                {
                    if (this.buildingMenuClose.y > 900 - this.buildingMenuBackground.Height - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f))
                    {
                        this.buildingMenuClose.y -= 10;
                        this.buildingMenuOpen.y -= 10;

                        if (this.buildingMenuClose.y < 900 - this.buildingMenuBackground.Height - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f))
                        {
                            this.buildingMenuOpen.y = 900 - this.buildingMenuBackground.Height - (int)(this.buildingMenuOpen.normal_texture.Height * 1.1f);
                            this.buildingMenuClose.y = 900 - this.buildingMenuBackground.Height - (int)(this.buildingMenuClose.normal_texture.Height * 1.1f);
                        }
                    }
                }
            }
            
            if (this.tutorialMenuClosed && !this.researchMenuClosed)
            {     
                if (this.researchMenuPos < 0)
                {
                    this.researchMenuClose.y = this.researchMenuPos + (int)(this.researchMenuClose.normal_texture.Height * 0.7f);

                    this.researchMenuPos += 50;

                    if (this.researchMenuPos > 0)
                    {
                        this.researchMenuPos = 0;
                    }
                }
                    
                this.mainResearchTree.update(cm, this.researchPoints, 800 - this.mainResearchTree.width / 2, 50);

                foreach (var currentRecipe in this.mainResearchTree.lastResearches)
                {
                    foreach(var currentBuilding in currentRecipe.addedBuildings)
                    {
                        this.buildingRecipeList.Add(currentBuilding);
                    }
                }

                this.researchMenuClose.update();

                if(this.researchMenuClose.pressed)
                {
                    this.researchMenuClosed = true;
                }
            }

            //for tutorial appear animation
            if (this.researchMenuClosed && !this.tutorialMenuClosed)
            {
                if (this.tutorialMenuPos < 0)
                {
                    this.tutorialMenuClose.y = this.tutorialMenuPos + (int)(this.tutorialMenuClose.normal_texture.Height * 0.7f);

                    this.tutorialMenuPos += 50;

                    if (this.tutorialMenuPos > 0)
                    {
                        this.tutorialMenuPos = 0;
                    }
                }

                if (!this.tutorialTextShown)
                {
                    if (this.tutorialTextPos > 0)
                    {
                        this.tutorialTextPos -= 50;

                        if (this.tutorialTextPos < 0)
                        {
                            this.tutorialTextPos = 0;

                            this.tutorialTextShown = true;
                        }
                    }
                }
                else
                {
                    if (this.oldState.ScrollWheelValue > currentState.ScrollWheelValue)
                    {
                        this.tutorialTextPos -= 100;
                    }
                    else if (this.oldState.ScrollWheelValue < currentState.ScrollWheelValue)
                    {
                        this.tutorialTextPos += 100;

                        if (this.tutorialTextPos > 0)
                        {
                            this.tutorialTextPos = 0;
                        }
                    }
                }

                this.tutorialMenuClose.update();

                if(this.tutorialMenuClose.pressed)
                {
                    this.tutorialMenuClosed = true;
                    this.tutorialTextShown = false;
                }
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(crust, new Vector2(this.draw_x, this.draw_y), Color.White);

            //drawing first layer map objects
            foreach (var current_object in this.map_Objects)
            {
                if (current_object.drawUnderOther)
                {
                    int draw_x = (int)(crustXPos + this.draw_x + current_object.x * crustWidth);
                    int draw_y = (int)(crustYPos + this.draw_y + current_object.y * crustHeight);

                    current_object.draw(spriteBatch, draw_x, draw_y);
                }
            }

            //drawing second layer map objects
            foreach (var current_object in this.map_Objects)
            {
                if (!current_object.drawUnderOther)
                {
                    int draw_x = (int)(crustXPos + this.draw_x + current_object.x * crustWidth);
                    int draw_y = (int)(crustYPos + this.draw_y + current_object.y * crustHeight);

                    current_object.draw(spriteBatch, draw_x, draw_y);
                }
            }

            //drawing building that is selected and could be builded
            if (this.selectedBuilding != null)
            {
                this.selectedBuilding.draw(spriteBatch, (int)(crustXPos + this.draw_x + this.selectedBuilding.x * crustWidth), (int)(crustYPos + this.draw_y + this.selectedBuilding.y * crustHeight));
            }

            //drawing some effects
            spriteBatch.Draw(attentionDarkness, new Vector2(0, 0), Color.White);

            //drawing res. points
            int start = 1600;

            for (int i = 0; i < this.researchPoints.Count; i++)
            {
                start -= (int)(this.researchPoints[i].getDrawRect().X * 1.1f);

                this.researchPoints[i].draw(spriteBatch, start, (int)(this.researchPoints[i].getDrawRect().Y * 0.1f));
            }

            //drawing Wave N sign
            if (this.timeSinceLastWave <= 255)
            {
                Vector2 vector = this.wavesFont.MeasureString("Wave " + this.waveNumber.ToString());

                spriteBatch.DrawString(this.wavesFont, "Wave " + this.waveNumber.ToString(), new Vector2(800 - vector.X/2, 60), new Color(0, 0, 0, 255-this.timeSinceLastWave));
            }

            //drawing buildings on building panel, buttons etc.
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

                if (this.researchMenuClosed && this.tutorialMenuClosed && this.researchMenuPos <= -900 && this.tutorialMenuPos <= -900)
                {
                    this.researchMenuOpen.draw(spriteBatch);
                    this.tutorialMenuOpen.draw(spriteBatch);
                }
                else if (this.tutorialMenuClosed && (!this.researchMenuClosed || this.researchMenuPos > -900))
                {
                    spriteBatch.Draw(this.researchBackground, new Vector2(0, this.researchMenuPos), Color.White);
                    this.researchMenuClose.draw(spriteBatch);

                    this.mainResearchTree.draw(spriteBatch, 800 - this.mainResearchTree.width / 2, 50 + this.researchMenuPos);
                }
                else if (this.researchMenuClosed && (!this.tutorialMenuClosed || this.tutorialMenuPos > -900))
                {
                    spriteBatch.Draw(this.researchBackground, new Vector2(0, this.tutorialMenuPos), Color.White);
                    this.tutorialMenuClose.draw(spriteBatch);

                    this.tutorialText.draw(spriteBatch, 200, tutorialTextPos, Color.Black);
                }
            }
            else
            {
                this.cancelButton.draw(spriteBatch);
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
                if (!(this.map_Objects[i].save_list()[0] == "#building" && ((building)this.map_Objects[i]).itemsToComplete.Count > 0))
                {
                    if (index_to_ignore != i)
                    {
                        if (this.map_Objects[i].contains_point(point))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public void addResearchPoints(Tuple<int, int> points)
        {
            if(points.Item1<this.researchPoints.Count)
            {
                this.researchPoints[points.Item1].amount += points.Item2;
            }
        }

        public void addResearchPoints(List<Tuple<int, int>> points)
        {
            foreach(var currentTuple in points)
            {
                if (currentTuple.Item1 < this.researchPoints.Count)
                {
                    this.researchPoints[currentTuple.Item1].amount += currentTuple.Item2;
                }
            }
        }
        
        /// <summary>
        /// Getting the closest object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="indexToIgnore"></param>
        /// <returns>Closest object</returns>
        public map_object getClosestObject(Vector2 point, int indexToIgnore)
        {
            float d = 100f;
            int ind = -1;

            for(int i=0; i<this.map_Objects.Count; i++)
            {
                if (i != indexToIgnore)
                {
                    float d1 = this.get_dist(point.X, point.Y, this.map_Objects[i].x, this.map_Objects[i].y);

                    if (d > d1)
                    {
                        d = d1;
                        ind = i;
                    }
                }
            }

            try
            {
                return this.map_Objects[ind];
            }
            catch 
            {
                return null;
            }
        }

        /// <summary>
        /// Getting the closest object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="indexToIgnore"></param>
        /// <returns>Closest object</returns>
        public map_object getClosestObject(Vector2 point, List<int> indexesToIgnore)
        {
            float d = 100f;
            int ind = -1;

            for (int i = 0; i < this.map_Objects.Count; i++)
            {
                if (!indexesToIgnore.Contains(i))
                {
                    float d1 = this.get_dist(point.X, point.Y, this.map_Objects[i].x, this.map_Objects[i].y);

                    if (d > d1)
                    {
                        d = d1;
                        ind = i;
                    }
                }
            }

            try
            {
                return this.map_Objects[ind];
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Getting the closest object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="indexesToIgnore"></param>
        /// <param name="objectTypes">First words of objects-to-check save lists</param>
        /// <returns>Closest object</returns>
        public map_object getClosestObject(Vector2 point, List<int> indexesToIgnore, List<string> objectTypes)
        {
            float d = 100f;
            int ind = -1;

            for (int i = 0; i < this.map_Objects.Count; i++)
            {
                if (!indexesToIgnore.Contains(i) && objectTypes.Contains(this.map_Objects[i].save_list()[0]))
                {
                    float d1 = this.get_dist(point.X, point.Y, this.map_Objects[i].x, this.map_Objects[i].y);

                    if (d > d1)
                    {
                        d = d1;
                        ind = i;
                    }
                }
            }

            try
            {
                return this.map_Objects[ind];
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Getting the closest object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="indexesToIgnore"></param>
        /// <param name="objectTypes">First words of objects-to-check save lists</param>
        /// <returns>Closest object</returns>
        public map_object getClosestObject(Vector2 point, int indexToIgnore, List<string> objectTypes)
        {
            float d = 100f;
            int ind = -1;

            for (int i = 0; i < this.map_Objects.Count; i++)
            {
                if (indexToIgnore != i && objectTypes.Contains(this.map_Objects[i].save_list()[0]))
                {
                    float d1 = this.get_dist(point.X, point.Y, this.map_Objects[i].x, this.map_Objects[i].y);

                    if (d > d1)
                    {
                        d = d1;
                        ind = i;
                    }
                }
            }

            try
            {
                return this.map_Objects[ind];
            }
            catch
            {
                return null;
            }
        }
        /// <summary>
        /// Getting the closest object
        /// </summary>
        /// <param name="point"></param>
        /// <param name="indexesToIgnore"></param>
        /// <param name="objectType">First word of objects-to-check save lists</param>
        /// <returns>Closest object</returns>
        public map_object getClosestObject(Vector2 point, int indexToIgnore, string objectType)
        {
            float d = 100f;
            int ind = -1;

            for (int i = 0; i < this.map_Objects.Count; i++)
            {
                if (indexToIgnore != i && objectType==map_Objects[i].save_list()[0])
                {
                    float d1 = this.get_dist(point.X, point.Y, this.map_Objects[i].x, this.map_Objects[i].y);

                    if (d > d1)
                    {
                        d = d1;
                        ind = i;
                    }
                }
            }

            try
            {
                return this.map_Objects[ind];
            }
            catch
            {
                return null;
            }
        }
    }
}