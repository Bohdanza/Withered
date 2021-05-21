﻿using Microsoft.VisualBasic;
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
        public virtual int what_to_do_with { get; protected set; }
        public virtual float x { get; protected set; }
        public virtual float y { get; protected set; }
        public virtual int type { get; protected set; }
        public virtual Vector2 hitbox_left { get; protected set; }
        public virtual Vector2 hitbox_right { get; protected set; }

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
    }
}