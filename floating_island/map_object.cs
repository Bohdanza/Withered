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
        public virtual List<int> what_to_do_with { get; protected set; }
        public virtual float x { get; protected set; }
        public virtual float y { get; protected set; }
        public virtual int type { get; protected set; }
        public virtual Vector2 hitbox_left { get; protected set; }
        public virtual Vector2 hitbox_right { get; protected set; }
        public virtual bool selected { get; protected set; }

        public virtual void update(ContentManager cm, island my_island, int my_index, bool somethingSelected)
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
    }
}