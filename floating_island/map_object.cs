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
    public class map_object
    {
        public virtual float x { get; protected set; }
        public virtual float y { get; protected set; }
        public virtual int type { get; protected set; }
        public virtual Vector2 hitbox_left { get; protected set; }
        public virtual Vector2 hitbox_right { get; protected set; }
        public virtual int hp { get; protected set; }
        public virtual int maxhp { get; protected set; }
        public virtual bool alive { get; protected set; } = true;

        //shows if object must be drawn under other objects
        public virtual bool drawUnderOther { get; protected set; } = false;

        public virtual void update(ContentManager cm, island my_island, int my_index)
        {

        }

        public virtual void draw(SpriteBatch spriteBatch, int x, int y)
        {
            
        }

        public virtual bool contains_point(Vector2 point)
        {
            return false;
        }

        public virtual List<string> save_list()
        {
            return new List<string>();
        }

        //later i'll add defence against stupid here 
        public virtual bool changeCoords(Vector2 newCoords)
        {
            this.x = newCoords.X;
            this.y = newCoords.Y;

            return true;
        }

        public virtual void damage(int damage)
        {
            this.hp -= damage;
        }

        /// <summary>
        /// Get distance from given point without ignoring hitboxex
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual float getSmallestDist(float x, float y)
        {
            float tmpx, tmpy;

            if(x<this.x)
            {
                tmpx = Math.Abs(this.x + this.hitbox_left.X - x);
            }
            else
            {
                tmpx = Math.Abs(x - this.x - this.hitbox_right.X);
            }

            if (y < this.y)
            {
                tmpy = Math.Abs(this.y + this.hitbox_left.Y - y);
            }
            else
            {
                tmpy = Math.Abs(y - this.y - this.hitbox_right.Y);
            }

            return (float)Math.Sqrt(tmpx * tmpx + tmpy * tmpy);
        }
    }
}