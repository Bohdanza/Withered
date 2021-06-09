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
        public int type { get; private set; }
        public int parentType { get; private set; }
        private Texture2D background;
        private Texture2D texture;
        public bool researched;
        public List<int> researchPointsNeeded = new List<int>();
        public int x = 0, y = 0;
        private MouseState oldState = Mouse.GetState();

        public researchRecipe(ContentManager cm, int type, bool researched, int parentType)
        { 
            this.type = type;

            this.background = cm.Load<Texture2D>("buildingrecipebackground");
            this.texture = cm.Load<Texture2D>(this.type.ToString() + "rec");

            this.researched = researched;
            this.parentType = parentType;

            using(StreamReader sr = new StreamReader(@"info\global\recipes\" + this.type.ToString() + @"\main_info"))
            {
                for(int i=0; i<3; i++)
                {
                    try
                    {
                        this.researchPointsNeeded.Add(Int32.Parse(sr.ReadLine().Trim('\n').Trim('\r')));
                    }
                    catch(Exception e)
                    {
                        this.researchPointsNeeded.Add(1);
                    }
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

                if (oldState.LeftButton == ButtonState.Pressed && tmprect.Contains(oldState.X, oldState.Y))
                {
                    spriteBatch.Draw(this.background, new Vector2(this.x + x, this.y + y), new Color(150, 150, 150));
                    spriteBatch.Draw(this.texture, new Vector2(this.x + x, this.y + y), new Color(150, 150, 150));
                }
                else
                {
                    spriteBatch.Draw(this.background, new Vector2(this.x + x, this.y + y), new Color(175, 175, 175));
                    spriteBatch.Draw(this.texture, new Vector2(this.x + x, this.y + y), new Color(175, 175, 175));
                }
            }
        }

        public bool canBeResearched(List<ResearchPoint> researchPoints)
        {
            var mouseState = Mouse.GetState();
            Rectangle tmprect = new Rectangle(this.x, this.y, 87, 87);

            if (oldState.LeftButton != ButtonState.Pressed || mouseState.LeftButton != ButtonState.Released || !tmprect.Contains(mouseState.X, mouseState.Y))
            {
                return false;
            }

            List<int> tmplist = this.researchPointsNeeded;

            foreach(var currentPoint in researchPoints)
            {
                if(currentPoint.type < tmplist.Count)
                {
                    tmplist[currentPoint.type] -= currentPoint.amount;
                }
            }

            foreach(var currentPoint in tmplist)
            {
                if (currentPoint > 0)
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

                List<int> tmplist = this.researchPointsNeeded;

                foreach (var currentPoint in researchPoints)
                {
                    if (currentPoint.type < tmplist.Count)
                    {
                        tmplist[currentPoint.type] -= currentPoint.amount;
                    }
                }

                foreach (var currentPoint in tmplist)
                {
                    if (currentPoint > 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false; 
        }
    }
}