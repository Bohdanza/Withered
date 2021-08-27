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

namespace floating_island 
{
    public class game_world
    {
        private string path;
        island mainIsland; 
        List<plant> sample_plant_list = new List<plant>();
        List<item> sample_item_list = new List<item>();
        List<building> sampleBuildings = new List<building>();
        List<monster> sampleMonsters = new List<monster>();
        private int timeSinceLastWave, capacity;
        private Texture2D background;

        public game_world(ContentManager cm, string path)
        {
            //TODO:
            //later I must add one more shit here
            //which would control if the whole world must be [re]generated

            this.background = cm.Load<Texture2D>("back");

            this.path = path;

            if (this.path[this.path.Length - 1] != 97 && this.path[this.path.Length - 1] != 47)
            {
                this.path += @"/";
            }

            try
            {
                using (StreamReader sr = new StreamReader(this.path + "main_info"))
                {
                    List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                    this.capacity = Int32.Parse(tmplist[0]);
                    this.timeSinceLastWave = Int32.Parse(tmplist[1]);
                }
            }
            catch
            {
                this.capacity = 1000;
                this.timeSinceLastWave = 0;
            }

            for (int i=0; i<6; i++)
            {
                this.sample_plant_list.Add(new plant(cm, 0, 0, i, 0));
            }
            
            for (int i = 0; i < 4; i++)
            {
                this.sample_item_list.Add(new item(cm, 0, 0, i, true, 0));
            }

            for (int i = 0; i < 9; i++)
            {
                this.sampleBuildings.Add(new building(cm, 0f, 0f, i)); 
            }

            for (int i = 0; i < 2; i++)
            {
                this.sampleMonsters.Add(new monster(cm, i, 0f, 0f));
            }

            this.mainIsland = new island(cm, this.sample_plant_list, this.sample_item_list, this.sampleBuildings, this.sampleMonsters, this.path + @"islands/0");
        }

        public void update(ContentManager cm)
        {
            this.timeSinceLastWave++;

            var rnd = new Random();

            bool waveSummon = rnd.Next(0, 10000) <= this.timeSinceLastWave;

            if (!waveSummon)
            {
                this.mainIsland.update(cm);
            }
            else
            {
                this.timeSinceLastWave = 0;

                this.capacity -= rnd.Next(50, 150);
            }
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.background, new Vector2(0, 0), Color.White);

            this.mainIsland.draw(spriteBatch);
        }

        public void save(ContentManager cm)
        {
            //this shit must be fixed later

            this.mainIsland.save(this.path+@"islands/0", cm);
        }
    }
}