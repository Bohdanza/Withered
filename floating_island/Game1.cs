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
        private Texture2D back_texture, loading_back, beginTexture, shadows, worldSelector;
        private button playButton, createButton;
        private bool worldLoaded = false, worldCreated = false;
        private List<string> worlds;
        private int worldListDrawY = 0, selectedWorld = 0;
        private TextSpace textSpace;
        private int timeSinceShow = 0;
        private SpriteFont font;
        private MouseState oldState;

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

            oldState = Mouse.GetState();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            back_texture = this.Content.Load<Texture2D>("main_menu_background");
            loading_back = this.Content.Load<Texture2D>("loadingback");
            beginTexture = this.Content.Load<Texture2D>("firstimage");
            shadows = this.Content.Load<Texture2D>("menuback2");
            worldSelector = this.Content.Load<Texture2D>("worldselect");

            font = Content.Load<SpriteFont>("menu_font");

            textSpace = new TextSpace(40, 330, 837, 65, 15, font);

            createButton = new button(0, 30, 500, 550, 209, this.Content.Load<Texture2D>("createbutton0"), this.Content.Load<Texture2D>("createbutton1"));
            
            playButton = new button(0, 30, 30, 550, 209, this.Content.Load<Texture2D>("playbutton0"), this.Content.Load<Texture2D>("playbutton1"));
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
                            this.main_world = new game_world(this.Content, @"info\worlds\" + worlds[selectedWorld]);
                        }
                        else if (worldCreated && main_world == null)
                        {
                            main_world = new game_world(Content, @"info\worlds\" + textSpace.currentString);
                        }
                        else
                        {
                            this.backgroundDrawX += Xmovement;

                            if (this.backgroundDrawX <= 1600 - this.back_texture.Width || this.backgroundDrawX >= 0)
                            {
                                this.Xmovement *= -1;
                            }

                            this.playButton.update();

                            if (this.playButton.pressed && selectedWorld >= 0 && selectedWorld < worlds.Count)
                            {
                                worldLoaded = true;
                            }

                            this.createButton.update();

                            if (createButton.pressed && textSpace.currentString.Length > 0)
                            {
                                worldCreated = true;
                                worldLoaded = false;
                            }

                            this.textSpace.update(Content);

                            if (mouseState.X >= 970)
                            {
                                if (mouseState.ScrollWheelValue > oldState.ScrollWheelValue && worldListDrawY < 0)
                                {
                                    worldListDrawY += 100;

                                    if (worldListDrawY > 0)
                                    {
                                        worldListDrawY = 0;
                                    }
                                }
                                else if (mouseState.ScrollWheelValue < oldState.ScrollWheelValue && worldListDrawY > worlds.Count * -80 + 900)
                                {
                                    worldListDrawY-=100;
                                }

                                if (mouseState.LeftButton == ButtonState.Released && oldState.LeftButton == ButtonState.Pressed)
                                {
                                    int tmpw = (mouseState.Y - worldListDrawY) / 80;

                                    if (tmpw >= 0 && tmpw < worlds.Count)
                                    {
                                        selectedWorld = tmpw;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                this.timeSinceShow++;
            }

            oldState = mouseState;

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
                    if (!this.worldLoaded && !this.worldCreated)
                    {
                        _spriteBatch.Draw(this.back_texture, new Vector2(this.backgroundDrawX, 0), Color.White);

                        _spriteBatch.Draw(shadows, new Vector2(0, 0), Color.White);

                        playButton.draw(_spriteBatch);

                        createButton.draw(_spriteBatch);

                        textSpace.draw(_spriteBatch);

                        for(int i=0; i<worlds.Count; i++)
                        {
                            if (selectedWorld == i)
                            {
                                _spriteBatch.Draw(worldSelector, new Vector2(975, i * 80 + worldListDrawY), Color.White);
                            }

                            _spriteBatch.DrawString(font, worlds[i], new Vector2(990, i * 80 + worldListDrawY), Color.Black);
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
