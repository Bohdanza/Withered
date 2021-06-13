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
    public class monster : map_object
    {
        public override float x { get; protected set; }
        public override float y { get; protected set; }
        public override int type { get; protected set; }
        public override int hp { get; protected set; }
        public override int maxhp { get; protected set; } 
        public float speed { get; private set; }
        private List<Texture2D> textures;
        private int imgPhase;
        public string action { get; private set; }
        public string direction { get; private set; }

        /// <summary>
        /// Initializing with file reading. Full HP
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public monster(ContentManager contentManager, int type, float x, float y)
        {
            this.type = type;
            this.x = x;
            this.y = y;

            using(StreamReader sr = new StreamReader(@"info\global\monsters\"+this.type.ToString()+@"\main_info"))
            {
                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                this.hp = Int32.Parse(tmplist[0]);
                this.maxhp = this.hp;
                
                this.speed = float.Parse(tmplist[1]);
            }

            this.action = "no";
            this.direction = "s";

            this.update_texture(contentManager, true);
        }
        
        /// <summary>
        /// Initializing with file reading. HP can be also initialized 
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public monster(ContentManager contentManager, int type, float x, float y, int hp)
        {
            this.type = type;
            this.x = x;
            this.y = y;

            using (StreamReader sr = new StreamReader(@"info\global\monsters\" + this.type.ToString() + @"\main_info"))
            {
                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                this.maxhp = Int32.Parse(tmplist[0]);

                this.speed = float.Parse(tmplist[1]);
            }

            this.hp = hp;

            if (this.hp < 0)
            {
                this.hp = 0;
            }
            else if (this.hp > this.maxhp)
            {
                this.hp = this.maxhp;
            }

            this.action = "no";
            this.direction = "s";

            this.update_texture(contentManager, true);
        }

        /// <summary>
        /// Initializing with sample
        /// </summary>
        /// <param name="contentManager"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="hp"></param>
        /// <param name="sampleMonster"></param>
        public monster(ContentManager contentManager, float x, float y, int hp, monster sampleMonster)
        {
            this.x = x;
            this.y = y;

            this.maxhp = sampleMonster.maxhp;
            this.type = sampleMonster.type;
            this.speed = sampleMonster.speed;

            this.hp = hp;

            if (this.hp < 0)
            {
                this.hp = 0;
            }
            else if (this.hp > this.maxhp)
            {
                this.hp = this.maxhp;
            }

            this.action = "no";
            this.direction = "s";

            this.update_texture(contentManager, true);
        }

        public override void update(ContentManager cm, island my_island, int my_index)
        {
           
            this.update_texture(cm, false);       
        }

        private void update_texture(ContentManager cm, bool somethingChanged = false)
        {
            if(somethingChanged == false)
            {
                this.imgPhase++;

                if(this.imgPhase>=this.textures.Count)
                {
                    this.imgPhase = 0;
                }
            }
            else
            {
                this.textures = new List<Texture2D>();

                this.imgPhase = 0;

                while (File.Exists(@"Content\" + this.type.ToString() + "monster" + this.action+this.direction + this.imgPhase.ToString() + ".xnb"))
                {
                    this.textures.Add(cm.Load<Texture2D>(this.type.ToString() + "monster" + this.action + this.direction + this.imgPhase.ToString()));

                    this.imgPhase++;
                }

                this.imgPhase = 0;
            }
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y)
        {
            spriteBatch.Draw(this.textures[this.imgPhase], new Vector2(x - this.textures[this.imgPhase].Width / 2, y - this.textures[this.imgPhase].Height), Color.White);
        }

        public override void damage(int damage)
        {
            this.hp -= damage;
        }

        public override List<string> save_list()
        {
            List<string> tmplist = new List<string>();

            tmplist.Add("#monster");
            tmplist.Add(this.x.ToString());
            tmplist.Add(this.y.ToString());
            tmplist.Add(this.hp.ToString());

            return tmplist;
        }
    }
}