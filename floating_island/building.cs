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
    public class building : map_object
    {
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        public override Vector2 hitbox_right { get; protected set; }
        public override Vector2 hitbox_left { get; protected set; }
        public override int type { get; protected set; }

        private int imgPhase, recipeImgPhase;
        List<Texture2D> textures = new List<Texture2D>();
        List<Texture2D> recipeTextures = new List<Texture2D>();
        public List<item> itemsToComplete { get; protected set; } = new List<item>();
        public List<Tuple<int, int>> researchPointsAdded { get; protected set; } = new List<Tuple<int, int>>();

        /// <summary>
        /// initializing with file reading
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        public building(ContentManager cm, float x, float y, int type)
        {
            this.x = x;
            this.y = y;

            this.type = type;

            using (StreamReader sr = new StreamReader(@"info\global\buildings\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmp_list = sr.ReadToEnd().Split('\n').ToList();

                int tmpint = Int32.Parse(tmp_list[0]), currentInd;

                for(currentInd=1; currentInd<=tmpint*2; currentInd+=2)
                {
                    this.itemsToComplete.Add(new item(cm, 0f, 0f, Int32.Parse(tmp_list[currentInd]), false, Int32.Parse(tmp_list[currentInd + 1])));
                }

                this.hitbox_left = new Vector2(float.Parse(tmp_list[currentInd]), float.Parse(tmp_list[currentInd+1]));
                this.hitbox_right = new Vector2(float.Parse(tmp_list[currentInd+2]), float.Parse(tmp_list[currentInd+3]));

                currentInd += 4;

                tmpint = Int32.Parse(tmp_list[currentInd]);

                tmpint = currentInd + tmpint * 2;

                //reading researchpoints
                //first is type, second is amount
                for(currentInd = currentInd+1; currentInd<tmpint; currentInd+=2)
                {
                    this.researchPointsAdded.Add(new Tuple<int, int>(Int32.Parse(tmp_list[currentInd]), Int32.Parse(tmp_list[currentInd + 1])));
                }
            }

            this.update_texture(cm, true);
        }

        /// <summary>
        /// initializing with file reading, but without it for items
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="itemSamples"></param>
        public building(ContentManager cm, float x, float y, int type, List<item> itemSamples)
        {
            this.x = x;
            this.y = y;

            this.type = type;

            using (StreamReader sr = new StreamReader(@"info\global\buildings\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmp_list = sr.ReadToEnd().Split('\n').ToList();

                int tmpint = Int32.Parse(tmp_list[0]), currentInd;

                for (currentInd = 1; currentInd <= tmpint * 2; currentInd += 2)
                {
                    int tmpType = Int32.Parse(tmp_list[currentInd]);

                    this.itemsToComplete.Add(new item(cm, 0f, 0f, tmpType, false, Int32.Parse(tmp_list[currentInd + 1]), itemSamples[tmpType]));
                }

                this.hitbox_left = new Vector2(float.Parse(tmp_list[currentInd + 1]), float.Parse(tmp_list[currentInd + 2]));
                this.hitbox_right = new Vector2(float.Parse(tmp_list[currentInd + 3]), float.Parse(tmp_list[currentInd + 4]));

                currentInd += 4;

                tmpint = Int32.Parse(tmp_list[currentInd]);

                tmpint = currentInd + tmpint * 2;

                //reading researchpoints
                //first is type, second is amount
                for (currentInd = currentInd + 1; currentInd < tmpint; currentInd += 2)
                {
                    this.researchPointsAdded.Add(new Tuple<int, int>(Int32.Parse(tmp_list[currentInd]), Int32.Parse(tmp_list[currentInd + 1])));
                }
            }

            this.update_texture(cm, true);
        }

        /// <summary>
        /// using sample
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="sampleBuilding"></param>
        public building(ContentManager cm, float x, float y, int type, building sampleBuilding)
        {
            this.x = x;
            this.y = y;

            this.type = type;

            this.hitbox_left = sampleBuilding.hitbox_left;
            this.hitbox_right = sampleBuilding.hitbox_right;

            this.itemsToComplete = new List<item>();

            foreach(var currentItem in sampleBuilding.itemsToComplete)
            {
                this.itemsToComplete.Add(new item(cm, 0f, 0f, currentItem.type, false, currentItem.number, currentItem));
            }

            this.researchPointsAdded = sampleBuilding.researchPointsAdded;

            this.update_texture(cm, true);
        }
        
        /// <summary>
        /// using sample and items to complete
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="type"></param>
        /// <param name="sampleBuilding"></param>
        public building(ContentManager cm, float x, float y, int type, building sampleBuilding, List<item> itemsToComplete)
        {
            this.x = x;
            this.y = y;

            this.type = type;

            this.hitbox_left = sampleBuilding.hitbox_left;
            this.hitbox_right = sampleBuilding.hitbox_right;

            this.itemsToComplete = new List<item>();

            foreach (var currentItem in itemsToComplete)
            {
                this.itemsToComplete.Add(new item(cm, 0f, 0f, currentItem.type, false, currentItem.number, currentItem));
            }

            this.researchPointsAdded = sampleBuilding.researchPointsAdded;

            this.update_texture(cm, true);
        }

        private void update_texture(ContentManager cm, bool something_changed = false)
        {
            if (something_changed == false)
            {
                this.imgPhase++;
                this.recipeImgPhase++;

                if (this.imgPhase >= this.textures.Count)
                {
                    this.imgPhase = 0;
                }

                if (this.recipeImgPhase >= this.recipeTextures.Count)
                {
                    this.recipeImgPhase = 0;
                }
            }
            else
            {
                this.textures = new List<Texture2D>();
                this.imgPhase = 0;

                bool tmpf = this.itemsToComplete.Count<=0;

                while (File.Exists(@"Content\" + this.type.ToString() + "building" + this.imgPhase.ToString() + tmpf.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "building" + this.imgPhase.ToString() + tmpf.ToString()));

                    this.imgPhase++;
                }

                this.imgPhase = 0;

                //=====================
                this.recipeTextures = new List<Texture2D>();
                this.recipeImgPhase = 0;

                while (File.Exists(@"Content\" + this.type.ToString() + "building" + this.recipeImgPhase.ToString() + "recipe" + ".xnb"))
                {
                    this.recipeTextures.Add(cm.Load<Texture2D>(this.type.ToString() + "building" + this.recipeImgPhase.ToString() + "recipe"));

                    this.recipeImgPhase++;
                }

                this.recipeImgPhase = 0;
            }
        }

        public override void update(ContentManager cm, island my_island, int my_index)
        {
            this.update_texture(cm, false);
        }
        
        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            int tmpw = this.textures[this.imgPhase].Width;
            int tmph = this.textures[this.imgPhase].Height;

            spriteBatch.Draw(this.textures[this.imgPhase], new Vector2(x - tmpw / 2, y - tmph), Color.White);
        }

        public void drawAsRecipe(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(this.recipeTextures[this.recipeImgPhase], new Vector2(x, y), Color.White);
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

        public void addItem(item itemToAdd, ContentManager cm)
        {
            int l = 1;

            for(int i=0; i<this.itemsToComplete.Count; i+=l)
            {
                l = 1;

                if(this.itemsToComplete[i].type==itemToAdd.type)
                {
                    int tmpc = this.itemsToComplete[i].number;

                    this.itemsToComplete[i].number = Math.Max(0, this.itemsToComplete[i].number - itemToAdd.number);

                    itemToAdd.number = Math.Max(0, itemToAdd.number + this.itemsToComplete[i].number - tmpc);
                }

                if(this.itemsToComplete[i].number<=0)
                {
                    this.itemsToComplete.RemoveAt(i);

                    l = 0;
                }
            }

            this.update_texture(cm, true);
        }

        public override List<string> save_list()
        {
            List<string> tmp_list = new List<string>();

            tmp_list.Add("#building");
            tmp_list.Add(this.type.ToString());
            tmp_list.Add(this.x.ToString());
            tmp_list.Add(this.y.ToString());
            tmp_list.Add(this.itemsToComplete.Count.ToString());
            
            foreach(var currentItem in this.itemsToComplete)
            {
                tmp_list.Add(currentItem.type.ToString());
                tmp_list.Add(currentItem.number.ToString());
            }

            return tmp_list;
        }

        public bool ItemCanBeAdded(item itemToAdd)
        {
            foreach(var currentItem in this.itemsToComplete)
            {
                if(currentItem.type == itemToAdd.type && currentItem.number>0)
                {
                    return true;
                }
            }

            return false;
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