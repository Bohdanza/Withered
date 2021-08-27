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
        private float degDirection;
        private int rotationProbability, rotationPower;
        public int attackSpeed { get; private set; }
        public int attackPower { get; private set; }
        private int timeSinceLastAttack = 0;

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

            using (StreamReader sr = new StreamReader(@"info/global/monsters/" + this.type.ToString() + @"/main_info"))
            {
                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                this.hp = Int32.Parse(tmplist[0]);
                this.maxhp = this.hp;

                this.speed = float.Parse(tmplist[1]);

                this.rotationProbability = Int32.Parse(tmplist[2]);

                this.rotationPower = Int32.Parse(tmplist[3]);

                this.attackSpeed = Int32.Parse(tmplist[4]);
                this.attackPower = Int32.Parse(tmplist[5]);
            }

            this.action = "no";
            this.direction = "s";

            this.degDirection = 0f;

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

            using (StreamReader sr = new StreamReader(@"info/global/monsters/" + this.type.ToString() + @"/main_info"))
            {
                List<string> tmplist = sr.ReadToEnd().Split('\n').ToList();

                this.maxhp = Int32.Parse(tmplist[0]);

                this.speed = float.Parse(tmplist[1]);

                this.rotationProbability = Int32.Parse(tmplist[2]);

                this.rotationPower = Int32.Parse(tmplist[3]);

                this.attackSpeed = Int32.Parse(tmplist[4]);
                this.attackPower = Int32.Parse(tmplist[5]);
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

            this.degDirection = 0f;

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

            this.rotationProbability = sampleMonster.rotationProbability;
            this.rotationPower = sampleMonster.rotationPower;

            this.attackSpeed = sampleMonster.attackSpeed;
            this.attackPower = sampleMonster.attackPower;

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

            this.degDirection = 0f;

            this.update_texture(contentManager, true);
        }

        public override void update(ContentManager cm, island my_island, int my_index)
        {
            bool actchanged = false;

            this.timeSinceLastAttack++;

            float px = this.x;
            float py = this.y;

            var rnd = new Random();

            if (this.action == "wa")
            {
                this.x += (float)Math.Cos(this.degDirection / 180 * Math.PI) * this.speed;
                this.y += (float)Math.Sin(this.degDirection / 180 * Math.PI) * this.speed;

                if (rnd.Next(0, 100) <= this.rotationProbability)
                {
                    this.degDirection += rnd.Next(-this.rotationPower, this.rotationPower + 1);
                }
            }

            this.degDirection %= 360;

            string pd = this.direction;

            if (this.degDirection <= 45 || this.degDirection > 315)
            {
                this.direction = "d";
            }
            else if (this.degDirection > 45 && this.degDirection <= 135)
            {
                this.direction = "s";
            }
            else if (this.degDirection > 135 && this.degDirection <= 225)
            {
                this.direction = "a";
            }
            else
            {
                this.direction = "w";
            }

            if (pd != this.direction)
            {
                actchanged = true;
            }

            map_object tmpObject = my_island.getClosestObject(new Vector2(this.x, this.y), my_index, "#building");

            if (tmpObject != null && ((building)tmpObject).itemsToComplete.Count <= 0)
            {
                if (tmpObject.getSmallestDist(this.x, this.y) <= this.speed * 3f)
                {
                    if (this.action == "at" && this.imgPhase == this.textures.Count - 1)
                    {
                        if (this.action != "no")
                        {
                            actchanged = true;
                        }

                        this.action = "no";
                    }

                    if (this.timeSinceLastAttack >= this.attackSpeed)
                    {
                        if (this.action != "at")
                        {
                            actchanged = true;
                        }

                        this.action = "at";

                        tmpObject.damage(this.attackPower);
                        this.timeSinceLastAttack = 0;
                    }
                }
                else
                {
                    if (this.action != "wa")
                    {
                        actchanged = true;
                    }

                    this.action = "wa";
                }
            }
            else
            {
                if (this.action != "wa")
                {
                    actchanged = true;
                }

                this.action = "wa";
            }

            if (!my_island.is_point_free(new Vector2(this.x, this.y), my_index))
            {
                this.x = px;
                this.y = py;

                this.degDirection = (float)rnd.NextDouble() * 360f;
            }

            this.degDirection %= 360;

            this.update_texture(cm, actchanged);
        }

        private void update_texture(ContentManager cm, bool somethingChanged = false)
        {
            if (somethingChanged == false)
            {
                this.imgPhase++;

                if (this.imgPhase >= this.textures.Count)
                {
                    this.imgPhase = 0;
                }
            }
            else
            {
                this.textures = new List<Texture2D>();

                this.imgPhase = 0;

                while (File.Exists(@"Content/" + this.type.ToString() + "monster" + this.action + this.direction + this.imgPhase.ToString() + ".xnb"))
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

            if(this.hp<=0)
            {
                this.alive = false;
            }
        }

        public override List<string> save_list()
        {
            List<string> tmplist = new List<string>();

            tmplist.Add("#monster");
            tmplist.Add(this.type.ToString());
            tmplist.Add(this.x.ToString());
            tmplist.Add(this.y.ToString());
            tmplist.Add(this.hp.ToString());

            return tmplist;
        }

        public override Texture2D GetTexture()
        {
            return textures[imgPhase];
        }
    }
}