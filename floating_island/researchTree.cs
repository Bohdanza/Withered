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
    public class researchTree
    {
        public List<researchRecipe> researchRecipes { get; private set; }

        public researchTree(List<researchRecipe> researchRecipes)
        {
            this.researchRecipes = researchRecipes;

            int current=-1;

            for (int i = 0; i < this.researchRecipes.Count && current == -1; i++)
            {
                if(this.researchRecipes[i].parentType==-1)
                {
                    current = i;
                }
            }

            List<int> tmplist = new List<int>();
            List<int> depth = new List<int>();
            int currentDepth = 0;

            for (int i = 0; i < this.researchRecipes.Count; i++)
            {
                int currentInd = i;

                depth.Add(0);
                tmplist.Add(0);

                while(this.researchRecipes[currentInd].parentType!=-1)
                {
                    depth[i]++;

                    for (int j = 0; j < this.researchRecipes.Count; j++)
                    {
                        if(this.researchRecipes[j].type==this.researchRecipes[currentInd].parentType)
                        {
                            currentInd = j;
                        }
                    }
                }

                currentDepth = Math.Max(currentDepth, depth[i]);
            }

            int tmpz = 0;

            for (int i = 0; i < this.researchRecipes.Count; i++)
            {
                if (depth[i] == currentDepth)
                {
                    tmplist[i] = 1;

                    this.researchRecipes[i].x = tmpz * 174;
                    this.researchRecipes[i].y = depth[i] * 174;

                    tmpz++;
                }
            }

         //   currentDepth--;

            while (currentDepth>=0)
            {
                for (int i = 0; i < this.researchRecipes.Count; i++)
                {
                    if (depth[i] == currentDepth)
                    {
                        this.researchRecipes[i].x /= tmplist[i];
                        this.researchRecipes[i].y = depth[i] * 174;

                        for (int j = 0; j < this.researchRecipes.Count; j++)
                        {
                            if(this.researchRecipes[j].type==this.researchRecipes[i].parentType)
                            {
                                this.researchRecipes[j].x += this.researchRecipes[i].x;
                                tmplist[j]++;
                            }
                        }
                    }
                }

                currentDepth--;
            }
        }

        public void draw(SpriteBatch spriteBatch, int x, int y)
        {
            foreach(var currentRecipe in this.researchRecipes)
            {
                currentRecipe.draw(spriteBatch, x, y);
            }
        }
    }
}
