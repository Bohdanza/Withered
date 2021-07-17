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
    public class bullet : map_object
    {
        public override int type { get; protected set; }
        public override bool alive { get; protected set; }
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        List<Texture2D> textures;
        private int damagePower, imgPhase;
        private float degDirection;
        public float speed { get; private set; }

        /// <summary>
        /// Initializing with file reading
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public bullet(ContentManager cm, int type, float x, float y, float direction)
        {
            this.alive = true;

            this.degDirection = direction;

            this.type = type;

            this.x = x;
            this.y = y;

            using (StreamReader sr = new StreamReader(@"info\global\bullets\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmpList = sr.ReadToEnd().Split('\n').ToList();

                this.damagePower = Int32.Parse(tmpList[0]);
                this.speed = float.Parse(tmpList[1]);
            }

            this.updateTexture(cm, true);
        }
        
        /// <summary>
        /// Initializing with sample
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direction"></param>
        public bullet(ContentManager cm, int type, float x, float y, float direction, bullet sampleBullet)
        {
            this.alive = true;

            this.degDirection = direction;

            this.type = type;

            this.x = x;
            this.y = y;

            this.damagePower = sampleBullet.damagePower;

            this.speed = sampleBullet.speed;

            this.updateTexture(cm, true);
        }
        private void updateTexture(ContentManager cm, bool somethingChanged)
        {
            if(somethingChanged)
            {
                this.textures = new List<Texture2D>();

                this.imgPhase = 0;

                while (File.Exists(@"Content\" + this.type.ToString() + "bullet" + this.imgPhase.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "bullet" + this.imgPhase.ToString()));
                    this.imgPhase++;
                }

                this.imgPhase = 0;
            }
            else
            {
                this.imgPhase++;

                if(this.imgPhase>=this.textures.Count)
                {
                    this.imgPhase = 0;
                }
            }    
        }
        public override void update(ContentManager cm, island my_island, int my_index)
        {
            this.x += (float)Math.Cos(this.degDirection / 180 * Math.PI) * this.speed;
            this.y += (float)Math.Sin(this.degDirection / 180 * Math.PI) * this.speed;

            /*if (my_island.get_dist(0.5f, 0.5f, this.x, this.y)>1f)
            {
                this.alive = false;
            }
            else
            {
                if(!my_island.is_point_free(new Vector2(this.x, this.y), my_index))
                {
                    my_island.getClosestObject(new Vector2(this.x, this.y), my_index).damage(this.damagePower);
                    this.alive = false;
                }
            }*/

            this.updateTexture(cm, false);
        }
        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(this.textures[this.imgPhase], new Vector2(x, y), Color.White);
        }
        public override List<string> save_list()
        {
            List<string> tmplist = new List<string>();

            tmplist.Add("#bullet");
            tmplist.Add(this.type.ToString());
            tmplist.Add(this.x.ToString());
            tmplist.Add(this.y.ToString());
            tmplist.Add(this.degDirection.ToString());

            return tmplist;
        }
    }
}