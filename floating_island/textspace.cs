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
using System.Text;

namespace floating_island
{
    public class TextSpace
    {
        public SpriteFont font { get; private set; }
        public string currentString { get; private set; }
        public int x, y, width, height;
        public bool selected { get; private set; }
        private int tick = 0, timeSinceLastPress=0;
        private List<Keys> forbiddenKeys;
        private int cursorPos = 0;

        public TextSpace(int x, int y, int width, int height, SpriteFont font)
        {
            this.x = x;
            this.y = y;

            this.width = width;
            this.height = height;

            this.font = font;

            currentString = "";
            selected = false;

            forbiddenKeys = new List<Keys>();

            forbiddenKeys.Add(Keys.LeftShift);
            forbiddenKeys.Add(Keys.RightShift);
            forbiddenKeys.Add(Keys.LeftControl);
            forbiddenKeys.Add(Keys.RightControl);
            forbiddenKeys.Add(Keys.LeftAlt);
            forbiddenKeys.Add(Keys.RightAlt);
            forbiddenKeys.Add(Keys.Back);
            forbiddenKeys.Add(Keys.Space);
            forbiddenKeys.Add(Keys.Left);
            forbiddenKeys.Add(Keys.Right);
            forbiddenKeys.Add(Keys.Up);
            forbiddenKeys.Add(Keys.Down);
            forbiddenKeys.Add(Keys.LeftWindows);
            forbiddenKeys.Add(Keys.RightWindows);

            forbiddenKeys.Add(Keys.D0);
            forbiddenKeys.Add(Keys.D1);
            forbiddenKeys.Add(Keys.D2);
            forbiddenKeys.Add(Keys.D3);
            forbiddenKeys.Add(Keys.D4);
            forbiddenKeys.Add(Keys.D5);
            forbiddenKeys.Add(Keys.D6);
            forbiddenKeys.Add(Keys.D7);
            forbiddenKeys.Add(Keys.D8);
            forbiddenKeys.Add(Keys.D9);

            forbiddenKeys.Add(Keys.NumLock);
            forbiddenKeys.Add(Keys.NumPad0);
            forbiddenKeys.Add(Keys.NumPad1);
            forbiddenKeys.Add(Keys.NumPad2);
            forbiddenKeys.Add(Keys.NumPad3);
            forbiddenKeys.Add(Keys.NumPad4);
            forbiddenKeys.Add(Keys.NumPad5);
            forbiddenKeys.Add(Keys.NumPad6);
            forbiddenKeys.Add(Keys.NumPad7);
            forbiddenKeys.Add(Keys.NumPad8);
            forbiddenKeys.Add(Keys.NumPad9);

            forbiddenKeys.Add(Keys.CapsLock);

            forbiddenKeys.Add(Keys.Delete);
        }

        public void update(ContentManager contentManager)
        {
            tick++;
            timeSinceLastPress++;

            tick %= 250;
            
            var mouseState = Mouse.GetState();
            Rectangle rectangle = new Rectangle(x, y, width, height);

            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                if (rectangle.Contains(mouseState.X, mouseState.Y))
                {
                    selected = true;
                }
                else
                {
                    selected = false;
                }
            }

            if (selected && timeSinceLastPress >= 7)
            {
                var keyboardState = Keyboard.GetState();
                var keys = keyboardState.GetPressedKeys();

                if (keys.Length > 0)
                {
                    timeSinceLastPress = 0;

                    if (!forbiddenKeys.Contains(keys[0]))
                    {
                        char keyValue = keys[0].ToString()[0];

                        if (!keys.Contains(Keys.LeftShift) && !keys.Contains(Keys.RightShift))
                        {
                            keyValue += (char)32;
                        }

                        string tmpstr = "" + keyValue;

                        currentString = currentString.Insert(cursorPos, tmpstr);

                        cursorPos++;
                    }
                    else if (keys[0] == Keys.Back)
                    {
                        if (currentString.Length > 0 && cursorPos > 0)
                        {
                            currentString = currentString.Remove(cursorPos - 1, 1);

                            cursorPos--;
                        }
                    }
                    else if (keys[0] == Keys.Space)
                    {
                        currentString += " ";

                        cursorPos++;
                    }
                    else if (keys[0] == Keys.D0 || keys[0] == Keys.D1 || keys[0] == Keys.D2 || keys[0] == Keys.D3 || keys[0] == Keys.D4 || keys[0] == Keys.D5 || keys[0] == Keys.D6 || keys[0] == Keys.D7 || keys[0] == Keys.D8 || keys[0] == Keys.D9)
                    {
                        string tmpstr = keys[0].ToString();

                        tmpstr = tmpstr.Remove(0, 1);

                        currentString = currentString.Insert(cursorPos, tmpstr);

                        cursorPos++;
                    }
                    else if (keys[0] == Keys.Right && cursorPos < currentString.Length)
                    {
                        cursorPos++;
                    }
                    else if (keys[0] == Keys.Left && cursorPos > 0)
                    {
                        cursorPos--;
                    }
                    else if (keys[0] == Keys.Delete && currentString.Length > 0 && cursorPos < currentString.Length)
                    {
                        currentString = currentString.Remove(cursorPos, 1);
                    }
                }
            }
        }

        public void draw(SpriteBatch spriteBatch)
        { 
            if (selected && this.tick % 60 >= 30)
            {
                spriteBatch.DrawString(font, currentString.Insert(cursorPos, "|"), new Vector2(x, y), Color.Black);
            }
            else
            {
                spriteBatch.DrawString(font, currentString, new Vector2(x, y), Color.Black);
            }
        }
    }
}