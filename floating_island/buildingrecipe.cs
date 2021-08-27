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
    //very very very bad class
    //BUT
    ///using best words
    //I'll fix it later ©
    public class researchRecipe
    {
        public List<building> addedBuildings { get; private set; } = new List<building>();
        public int type { get; private set; }
        public int parentType { get; private set; }
        private Texture2D background;
        private Texture2D texture;
        public bool researched;
        public List<ResearchPoint> researchPointsNeeded = new List<ResearchPoint>();
        public int x = 0, y = 0;
        private MouseState oldState = Mouse.GetState();
        private int pointDrawCoords=0;

        public researchRecipe(ContentManager cm, int type, bool researched, int parentType)
        { 
            this.type = type;

            this.background = cm.Load<Texture2D>("buildingrecipebackground");
            this.texture = cm.Load<Texture2D>(this.type.ToString() + "rec");

            this.researched = researched;
            this.parentType = parentType;

            using(StreamReader sr = new StreamReader(@"info/global/recipes/" + this.type.ToString() + @"/main_info"))
            {
                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                int tmpn = Int32.Parse(tmplist[0].Trim('\n').Trim('\r'));

                for(int i=1; i<tmpn*2; i++)
                {
                    int tmptype = Int32.Parse(tmplist[i].Trim('\n').Trim('\r'));
                    int tmpcount = Int32.Parse(tmplist[i + 1].Trim('\n').Trim('\r'));

                    this.researchPointsNeeded.Add(new ResearchPoint(cm, tmptype, tmpcount, null));
                }

                int currentStr=tmpn*2+1;

                tmpn = Int32.Parse(tmplist[currentStr].Trim('\n').Trim('\r'))+currentStr;

                for (currentStr=currentStr+1; currentStr<=tmpn; currentStr++)
                {
                    this.addedBuildings.Add(new building(cm, 0f, 0f, Int32.Parse(tmplist[currentStr].Trim('\n').Trim('\r'))));
                }
            }
        }

        public void update(ContentManager cm)
        {
            this.oldState = Mouse.GetState();
        }

        public void draw(SpriteBatch spriteBatch, int x, int y)
        {
            if (this.researched)
            {
                spriteBatch.Draw(this.background, new Vector2(this.x + x, this.y + y), Color.White);
                spriteBatch.Draw(this.texture, new Vector2(this.x + x, this.y + y), Color.White);
            }
            else
            {
                Rectangle tmprect = new Rectangle(this.x + x, this.y + y, 87, 87);
                
                int i = 0;

                foreach (var currentPoint in this.researchPointsNeeded)
                {
                    if (currentPoint.amount > 0)
                    {
                        if (this.x + x + this.pointDrawCoords + i >= this.x + x + this.texture.Width)
                        {
                            currentPoint.drawForRecipe(spriteBatch, this.x + x + this.pointDrawCoords + i, this.y + y);
                        }

                        i += (int)currentPoint.getDrawRect().X;
                    }
                }

                if (tmprect.Contains(oldState.X, oldState.Y))
                {
                    if(this.pointDrawCoords<this.texture.Width*1.1f)
                    {
                        this.pointDrawCoords += 7;
                    }

                    if (oldState.LeftButton == ButtonState.Pressed)
                    {
                        spriteBatch.Draw(this.background, new Vector2(this.x + x, this.y + y), new Color(150, 150, 150));
                        spriteBatch.Draw(this.texture, new Vector2(this.x + x, this.y + y), new Color(150, 150, 150));
                    }
                    else
                    {
                        //a bit shity
                        //must be fixed
                        spriteBatch.Draw(this.background, new Vector2(this.x + x, this.y + y), new Color(175, 175, 175));
                        spriteBatch.Draw(this.texture, new Vector2(this.x + x, this.y + y), new Color(175, 175, 175));
                    }
                }
                else
                {
                    if (this.pointDrawCoords > 0)
                    {
                        this.pointDrawCoords -= 7;
                    }

                    spriteBatch.Draw(this.background, new Vector2(this.x + x, this.y + y), new Color(175, 175, 175));
                    spriteBatch.Draw(this.texture, new Vector2(this.x + x, this.y + y), new Color(175, 175, 175));
                }
            }
        }
        
        public bool canBeResearched(List<ResearchPoint> researchPoints)
        {
            List<Tuple<int, int>> tuples = new List<Tuple<int, int>>();

            foreach(var currentPoint in this.researchPointsNeeded)
            {
                tuples.Add(new Tuple<int, int>(currentPoint.type, currentPoint.amount));
            }

            //TODO
            foreach(var currentPoint in researchPoints)
            {
                bool b = true;

                for (int i = 0; i < tuples.Count && b; i++)
                {
                    if(currentPoint.type==tuples[i].Item1)
                    {
                        tuples[i] = new Tuple<int, int>(tuples[i].Item1, tuples[i].Item2-currentPoint.amount);
                        b = false;
                    }
                }
            }

            foreach(var currentTuple in tuples)
            {
                if (currentTuple.Item2 > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool canBeResearched(List<ResearchPoint> researchPoints, int x, int y)
        {
            if (oldState.LeftButton == ButtonState.Pressed)
            {
                var mouseState = Mouse.GetState();
                Rectangle tmprect = new Rectangle(this.x + x, this.y + y, 87, 87);

                if (mouseState.LeftButton != ButtonState.Released || !tmprect.Contains(mouseState.X, mouseState.Y))
                {
                    return false;
                }

                return this.canBeResearched(researchPoints);
            }

            return false; 
        }
    }
}