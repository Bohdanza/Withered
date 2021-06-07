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

            int current = -1;

            for (int i = 0; i < this.researchRecipes.Count && current == -1; i++)
            {
                if (this.researchRecipes[i].parentType == -1)
                {
                    current = i;
                }
            }

            List<int> leaves = new List<int>();
            List<int> tmplist = new List<int>();
            List<int> depth = new List<int>();
            int currentDepth = 0;
            int maxpx = 0;

            //initializing depth, leaves etc.
            for (int i = 0; i < this.researchRecipes.Count; i++)
            {
                int currentInd = i;

                depth.Add(0);
                tmplist.Add(0);

                while (this.researchRecipes[currentInd].parentType != -1)
                {
                    depth[i]++;

                    for (int j = 0; j < this.researchRecipes.Count; j++)
                    {
                        if (this.researchRecipes[j].type == this.researchRecipes[currentInd].parentType)
                        {
                            currentInd = j;
                        }
                    }
                }

                currentDepth = Math.Max(currentDepth, depth[i]);
            }

            maxpx = currentDepth * 87 * 2;

            int tmpz = 0;

            for (int i = 0; i < this.researchRecipes.Count; i++)
            {
                this.researchRecipes[i].y = maxpx - depth[i] * 87 * 2;

                bool tmpb = true;

                for (int j = 0; j < this.researchRecipes.Count; j++)
                {
                    if(this.researchRecipes[j].parentType == this.researchRecipes[i].type)
                    {
                        tmpb = false;
                    }
                }

                if(tmpb)
                {
                    leaves.Add(i);
                    tmplist[i] = 1;

                    this.researchRecipes[i].x = tmpz * 87 * 2;

                    tmpz++;
                }
            }

            //for other
            while(currentDepth>=0)
            {
                for (int i = 0; i < leaves.Count; i++)
                {
                    if (depth[leaves[i]] == currentDepth)
                    {
                        for (int j = 0; j < this.researchRecipes.Count; j++)
                        {
                            if (this.researchRecipes[j].type == this.researchRecipes[leaves[i]].parentType)
                            {
                                this.researchRecipes[j].x += this.researchRecipes[leaves[i]].x;
                                tmplist[j]++;
                            }
                        }
                    }
                }

                currentDepth--;

                for (int j = 0; j < this.researchRecipes.Count; j++)
                {
                    if(depth[j]==currentDepth)
                    {
                        this.researchRecipes[j].x /= tmplist[j];
                        leaves.Add(j);
                    }
                }
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
