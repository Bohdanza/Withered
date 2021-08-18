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
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private game_world main_world = null;
        private bool saved = false;
        private int backgroundDrawX = 0, Xmovement = -1;
        private Texture2D back_texture, loading_back, beginTexture, shadows;
        private button playButton;
        private bool worldLoaded = false;
        private List<string> worlds;
        private int worldListDrawY = 0;
        private TextSpace textSpace;
        private int timeSinceShow = 0;
        private SpriteFont font;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.ApplyChanges();

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = TimeSpan.FromSeconds(1d / 60d);
            _graphics.ApplyChanges();

            _graphics.PreferredBackBufferWidth = 1600;
            _graphics.PreferredBackBufferHeight = 900;
            _graphics.ApplyChanges();

         //   _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            worlds = Directory.EnumerateDirectories(@"info\worlds\").ToList();

            for (int i = 0; i < worlds.Count; i++)
            {
                worlds[i] = worlds[i].Remove(0, 12);
            }
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.back_texture = this.Content.Load<Texture2D>("main_menu_background");
            this.loading_back = this.Content.Load<Texture2D>("loadingback");
            this.beginTexture = this.Content.Load<Texture2D>("firstimage");
            this.shadows = this.Content.Load<Texture2D>("menuback2");

            font = Content.Load<SpriteFont>("menu_font");

            this.textSpace = new TextSpace(40, 330, 515, 65, 20, font);

            this.playButton = new button(0, 30, 30, 550, 209, this.Content.Load<Texture2D>("playbutton0"), this.Content.Load<Texture2D>("playbutton1"));
        }

        protected override void Update(GameTime gameTime)
        {
            var mouseState = Mouse.GetState();

            if (this.timeSinceShow >= 500)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    if (this.main_world != null)
                    {
                        this.main_world.save(Content);
                        this.saved = true;
                    }

                    Exit();
                }

                if (IsActive == false)
                {
                    if (this.saved == false && this.main_world != null)
                    {
                        this.main_world.save(Content);
                        this.saved = true;
                    }
                }
                else
                {
                    if (this.main_world != null)
                    {
                        this.main_world.update(this.Content);

                        this.saved = false;
                    }
                    else
                    {
                        if (this.worldLoaded && this.main_world == null)
                        {
                            this.main_world = new game_world(this.Content, @"info\worlds\world1");
                        }
                        else
                        {
                            this.backgroundDrawX += Xmovement;

                            if (this.backgroundDrawX <= 1600 - this.back_texture.Width || this.backgroundDrawX >= 0)
                            {
                                this.Xmovement *= -1;
                            }

                            this.playButton.update();

                            if (this.playButton.pressed)
                            {
                                this.worldLoaded = true;
                            }

                            this.textSpace.update(Content);

                            /*if (mouseState.X >= 937)
                            {
                                
                            }*/
                        }
                    }
                }
            }
            else
            {
                this.timeSinceShow++;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            if (timeSinceShow >= 500)
            {
                if (this.main_world == null)
                {
                    if (!this.worldLoaded)
                    {
                        _spriteBatch.Draw(this.back_texture, new Vector2(this.backgroundDrawX, 0), Color.White);

                        _spriteBatch.Draw(shadows, new Vector2(0, 0), Color.White);

                        this.playButton.draw(_spriteBatch);

                        textSpace.draw(_spriteBatch);

                        for(int i=0; i<worlds.Count; i++)
                        {
                            _spriteBatch.DrawString(font, worlds[i], new Vector2(1000, i * 80 + worldListDrawY), Color.Black);
                        }
                    }
                    else
                    {
                        _spriteBatch.Draw(loading_back, new Vector2(0, 0), Color.White);
                    }
                }
                else
                {
                    this.main_world.draw(_spriteBatch);
                }
            }
            else if(timeSinceShow>=200)
            {
                int tmpbright = Math.Min(timeSinceShow-200, 255);

                _spriteBatch.Draw(beginTexture, new Vector2(0, 0), new Color(tmpbright, tmpbright, tmpbright));
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
