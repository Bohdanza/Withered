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
    public class game_world
    {
        private string path;
        island mainIsland; 
        List<plant> sample_plant_list = new List<plant>();
        List<item> sample_item_list = new List<item>();
        List<building> sampleBuildings = new List<building>();

        public game_world(ContentManager cm, string path)
        {
            //TODO:
            //later I must add one more shit here
            //which would control if the whole world must be [re]generated
            
            this.path = path;

            if (this.path[this.path.Length - 1] != 97 && this.path[this.path.Length - 1] != 47)
            {
                this.path += @"\";
            }

            for (int i=0; i<6; i++)
            {
                this.sample_plant_list.Add(new plant(cm, 0, 0, i, 0));
            }

            for (int i = 0; i < 2; i++)
            {
                this.sample_item_list.Add(new item(cm, 0, 0, i, true, 0));
            }

            for (int i = 0; i < 1; i++)
            {
                this.sampleBuildings.Add(new building(cm, 0f, 0f, i)); 
            }

            this.mainIsland = new island(cm, this.sample_plant_list, this.sample_item_list, this.sampleBuildings, this.path + @"islands\0");
        }

        public void update(ContentManager cm)
        {
            this.mainIsland.update(cm);   
        }

        public void draw(SpriteBatch spriteBatch)
        {
            this.mainIsland.draw(spriteBatch);
        }

        public void save(ContentManager cm)
        {
            //this shit must be fixed later

            this.mainIsland.save(this.path+@"islands\0", cm);
        }
    }
}