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
        private Texture2D edge;
        public List<researchRecipe> researchRecipes { get; private set; }
        public int width { get; private set; }
        public int height { get; private set; }
        public List<researchRecipe> lastResearches { get; private set; } = new List<researchRecipe>();

        public researchTree(List<researchRecipe> researchRecipes, ContentManager cm)
        {
            this.edge = cm.Load<Texture2D>("edge");

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
            this.height = maxpx;

            int tmpz = 0;

            for (int i = 0; i < this.researchRecipes.Count; i++)
            {
                this.researchRecipes[i].y = maxpx - depth[i] * 87 * 2;

                bool tmpb = true;

                for (int j = 0; j < this.researchRecipes.Count; j++)
                {
                    if (this.researchRecipes[j].parentType == this.researchRecipes[i].type)
                    {
                        tmpb = false;
                    }
                }

                if (tmpb)
                {
                    leaves.Add(i);
                    tmplist[i] = 1;

                    this.researchRecipes[i].x = tmpz * 87 * 2;

                    tmpz++;
                }
            }

            this.width = leaves.Count * 174;

            //for other
            while (currentDepth >= 0)
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
                    if (depth[j] == currentDepth)
                    {
                        this.researchRecipes[j].x /= tmplist[j];
                        leaves.Add(j);
                    }
                }
            }
        }

        public void draw(SpriteBatch spriteBatch, int x, int y)
        {
            foreach (var currentRecipe in this.researchRecipes)
            {
                foreach (var currentRecipe1 in this.researchRecipes)
                {
                    if (currentRecipe1.type == currentRecipe.parentType)
                    {
                        int tmpw = Math.Abs(currentRecipe.x - currentRecipe1.x);
                        int tmph = Math.Abs(currentRecipe.y - currentRecipe1.y);

                        int length = (int)Math.Sqrt(tmpw * tmpw + tmph * tmph);

                        float rot = (float)Math.Atan((float)tmph / tmpw);

                        // rot = (float)rot / 360;

                        if (currentRecipe1.x < currentRecipe.x)
                        {
                            rot = (float)(Math.PI - rot);
                        }

                        spriteBatch.Draw(this.edge, new Vector2(currentRecipe.x + x + 43, currentRecipe.y + y + 43), null, Color.White, rot, new Vector2(0, 0), new Vector2(length / this.edge.Width, 1), SpriteEffects.None, 0);
                    }
                }
            }

            foreach (var currentRecipe in this.researchRecipes)
            {
                currentRecipe.draw(spriteBatch, x, y);
            }
        }

        public void update(ContentManager cm, List<ResearchPoint> researchPoints, int x, int y)
        {
            this.lastResearches = new List<researchRecipe>();

            foreach (var currentRecipe in this.researchRecipes)
            {
                foreach (var currentRecipe1 in this.researchRecipes)
                {
                    if (currentRecipe1.type == currentRecipe.parentType && currentRecipe1.researched)
                    {
                        if (currentRecipe.canBeResearched(researchPoints, x, y))
                        {
                            currentRecipe.researched = true;
                            this.lastResearches.Add(currentRecipe);
                        }
                    }
                }

                currentRecipe.update(cm);
            }
        } 
    }
}
