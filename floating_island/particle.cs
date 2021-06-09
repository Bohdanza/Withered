//TODO: THIS CLASS MUST BE UNCOMMENTED AND FINISHED

/*using Microsoft.VisualBasic;
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
    public class particle : map_object
    {
        public Texture2D texture { get; private set; }
        public override int type { get; protected set; }
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        public Vector2 drawCoords { get; private set; } = new Vector2(0, 0);

        public particle(ContentManager cm, int type, float x, float y)
        {
            this.type = type;
            
            this.x = x;
            this.y = y;

            this.texture = cm.Load<Texture2D>(this.type.ToString() + "particle");
        }

        public override void update(ContentManager cm, island my_island, int my_index)
        {
            this.drawCoords = new Vector2(this.drawCoords.X + new Random().Next(-1, 2), this.drawCoords.Y + 1);
        }
    }
}*/