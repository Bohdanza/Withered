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
    public class Recipe
    {
        public int type { get; private set; }
        public List<item> neededItems { get; private set; }
        public List<item> currentNeededItems { get; private set; }
        public List<item> results { get; private set; }
        private ContentManager contentManager;

        /// <summary>
        /// initializing with file reading
        /// </summary>
        /// <param name="type"></param>
        /// <param name=""></param>
        public Recipe(int type, ContentManager contentManager)
        {
            this.contentManager = contentManager;

            currentNeededItems = new List<item>();
            results = new List<item>();
            neededItems = new List<item>();

            this.type = type;

            using (StreamReader sr = new StreamReader(@"info\global\recipes\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmp_list = sr.ReadToEnd().Split('\n').ToList();

                int current_line, tmpn = Int32.Parse(tmp_list[0]);

                for (current_line = 1; current_line <= tmpn * 2; current_line += 2)
                {
                    neededItems.Add(new item(contentManager, 0f, 0f, Int32.Parse(tmp_list[current_line]), false, Int32.Parse(tmp_list[current_line + 1])));
                }

                tmpn = Int32.Parse(tmp_list[current_line]) * 2 + current_line;

                for (current_line++; current_line < tmpn; current_line += 2)
                {
                    results.Add(new item(contentManager, 0f, 0f, Int32.Parse(tmp_list[current_line]), false, Int32.Parse(tmp_list[current_line + 1])));
                }
            }
        }

        /// <summary>
        /// initialing with file reading, item samples must be given
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contentManager"></param>
        public Recipe(int type, ContentManager contentManager, List<item> sampleItems)
        {
            this.contentManager = contentManager;

            currentNeededItems = new List<item>();
            results = new List<item>();
            neededItems = new List<item>();

            this.type = type;

            using (StreamReader sr = new StreamReader(@"info\global\recipes\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmp_list = sr.ReadToEnd().Split('\n').ToList();

                int current_line, tmpn = Int32.Parse(tmp_list[0]);

                for (current_line = 1; current_line <= tmpn * 2; current_line += 2)
                {
                    int tmptype = Int32.Parse(tmp_list[current_line]);

                    neededItems.Add(new item(contentManager, 0f, 0f, tmptype, false, Int32.Parse(tmp_list[current_line + 1]), sampleItems[tmptype]));
                }

                tmpn = Int32.Parse(tmp_list[current_line]) * 2 + current_line;

                for (current_line++; current_line < tmpn; current_line += 2)
                {
                    int tmptype = Int32.Parse(tmp_list[current_line]);

                    results.Add(new item(contentManager, 0f, 0f, tmptype, false, Int32.Parse(tmp_list[current_line + 1]), sampleItems[tmptype]));
                }
            }
        }

        /// <summary>
        /// initializing with sample 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contentManager"></param>
        /// <param name="sampleRecipe"></param>
        public Recipe(int type, ContentManager contentManager, Recipe sampleRecipe)
        {
            this.contentManager = contentManager;

            this.type = type;

            this.neededItems = sampleRecipe.getNeeded(contentManager);
            this.results = sampleRecipe.getResults(contentManager);

            this.currentNeededItems = new List<item>();
        }

        public bool addItem(item itemToAdd)
        {
            //Normal system with content managers must be developed
            item tmpitem = new item(contentManager, 0f, 0f, itemToAdd.type, false, itemToAdd.number, itemToAdd);

            int l = 1;
            bool f = false;

            for (int i = 0; i < this.currentNeededItems.Count; i+=l)
            {
                l = 1;

                if (currentNeededItems[i].type == tmpitem.type)
                {
                    f = true;

                    int tmpnumber = currentNeededItems[i].number;

                    currentNeededItems[i].number -= tmpitem.number;

                    tmpitem.number -= tmpnumber;

                    if (tmpitem.number <= 0)
                    {
                        currentNeededItems.RemoveAt(i);
                        l = 0;
                    }

                    if (tmpnumber <= 0)
                    {
                        return true;
                    }
                }
            }

            return f;
        }

        public bool itemCanBeAdded(item itemToAdd)
        {
            foreach(var currentItem in currentNeededItems)
            {
                if (currentItem.type == itemToAdd.type)
                {
                    return true;
                }
            }

            return false;
        }

        public void resetCurrent(ContentManager cm)
        {
            currentNeededItems = new List<item>();

            foreach (var currentItem in neededItems)
            {
                currentNeededItems.Add(new item(cm, 0f, 0f, currentItem.type, false, currentItem.number, currentItem));
            }
        }

        public List<item> getNeeded(ContentManager contentManager)
        {
            List<item> tmplist = new List<item>();

            foreach(var currentItem in neededItems)
            {
                tmplist.Add(new item(contentManager, 0f, 0f, currentItem.type, false, currentItem.number, currentItem));
            }

            return tmplist;
        }

        public List<item> getResults(ContentManager contentManager)
        {
            List<item> tmplist = new List<item>();

            foreach (var currentItem in results)
            {
                tmplist.Add(new item(contentManager, 0f, 0f, currentItem.type, false, currentItem.number, currentItem));
            }

            return tmplist;
        }
    }
}