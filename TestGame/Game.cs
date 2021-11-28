using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameEngine
{
    public class Game : ConsoleGameEngine
    {

        public Player player { get; set; }
        public List<Enemy> enemies { get; set; }

        public bool IsGameStarted = false;
        public bool IsGameOver = false;

        public float totalTimeElapsed = 0f;
        public int level = 1;

        public override bool OnUserCreate()
        {
            player = new Player(ScreenWidth, ScreenHeight);
            enemies = new List<Enemy>();
            enemies.Add(new Enemy(ScreenWidth, ScreenHeight, 2, 2));
            enemies.Add(new Enemy(ScreenWidth, ScreenHeight, 100, 20));

            return true ;
        }

        public override bool OnUserUpdate(float fElapsedTime)
        {
            if (!IsGameStarted) // Draw welcome screen
            {
                DrawObjects();
                DrawText((uint)ScreenWidth / 2, 2, "Press Space to start...");

                if (KeyStates[(int)ConsoleKey.Spacebar].isPressed)
                {
                    IsGameStarted = true;
                }
                return true;
            }

            if (IsGameOver)
            {
                //ClearScreen();
                DrawText((uint)ScreenWidth/2, (uint) 2 ,"Game Over");
                if (KeyStates[(int)ConsoleKey.Spacebar].isPressed)
                {
                    return false;
                }
                return true;
            }


            if (KeyStates[(int)ConsoleKey.Escape].isPressed) return false;

            if (KeyStates[(int)ConsoleKey.LeftArrow].isPressed) player.Move(Direction.Left, fElapsedTime);
            if (KeyStates[(int)ConsoleKey.RightArrow].isPressed) player.Move(Direction.Right, fElapsedTime);
            if (KeyStates[(int)ConsoleKey.UpArrow].isPressed) player.Move(Direction.Up, fElapsedTime);
            if (KeyStates[(int)ConsoleKey.DownArrow].isPressed) player.Move(Direction.Down, fElapsedTime);


            foreach (var enemy in enemies)
            {
                enemy.FollowPlayer(player, fElapsedTime);

                // Check for Game Over
                if ((int)player.PosX == (int)enemy.PosX && (int)player.PosY == (int)enemy.PosY)
                {
                    IsGameOver = true;
                }
            }


            totalTimeElapsed += fElapsedTime;
            var newlevel = (int)(totalTimeElapsed / 10000f) +1;

            if (newlevel > level)
            {
                foreach (var enemy in enemies)
                {
                    enemy.Accelerate(0.12f);
                }
                level = newlevel;
            }


            DrawObjects();
            DrawScore();

            return true;
        }

        private void DrawObjects()
        {
            ClearScreen();

            // Draw Player
            Screen[(int)player.PosY * ScreenWidth + (int)player.PosX].Char.AsciiChar = 178;
            Screen[(int)player.PosY * ScreenWidth + (int)player.PosX].Attributes = 3;

            // Draw Enemies
            foreach (var enemy in enemies)
            {
                Screen[(int)enemy.PosY * ScreenWidth + (int)enemy.PosX].Char.AsciiChar = 178;
                Screen[(int)enemy.PosY * ScreenWidth + (int)enemy.PosX].Attributes = 10;
            }
        }

        private void ClearScreen()
        {
            // Draw Screen
            for (int i = 0; i < ScreenWidth * ScreenHeight; i++)
            {
                Screen[i].Char.AsciiChar = (byte)' ';
                Screen[i].Attributes = 2;
            }
        }

        private void DrawText(uint posX, uint posY, string text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                if(posX+i < ScreenWidth) Screen[posY * ScreenWidth + posX + i].Char.AsciiChar = (byte)text[i];
            }
        }

        private void DrawScore()
        {
            DrawText(0, 0, $"Level: {level}");
            DrawText(0, 1, $"Speed: {enemies[0].XStep}");
        }
    }
}
