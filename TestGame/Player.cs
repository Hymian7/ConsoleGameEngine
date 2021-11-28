using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleGameEngine
{
    public class Player
    {

        public float PosX { get; set; }
        public float PosY { get; set; }

        public float XStep { get; set; }
        public float YStep { get; set; }

        private short ScreenHeight { get; set; }
        private short ScreenWidth { get; set; }

        public Player(short screenWidth, short screenHeight)
        {
            XStep = 0.08f;
            YStep = 0.02f;

            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;

            PosX = (int)ScreenWidth / 2;
            PosY = (int)ScreenHeight / 2;
        }

        public void Move(Direction direction, float elapsedTime)
        {
            switch (direction)
            {
                case Direction.Left:
                    PosX -= XStep * elapsedTime;
                    break;
                case Direction.Right:
                    PosX += XStep * elapsedTime;
                    break;
                case Direction.Up:
                    PosY -= YStep * elapsedTime;
                    break;
                case Direction.Down:
                    PosY += YStep * elapsedTime;
                    break;
                default:
                    break;
            }

            // Collision Detection Borders
            if (PosX > ScreenWidth) PosX -= XStep * elapsedTime;
            if (PosX < 0) PosX += XStep * elapsedTime;
            if (PosY > ScreenHeight) PosY -= YStep * elapsedTime;
            if (PosY < 0) PosY += YStep * elapsedTime;

        }

    }
}
