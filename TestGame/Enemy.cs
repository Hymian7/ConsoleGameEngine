using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestGame
{
    public class Enemy
    {
        public float PosX { get; set; }
        public float PosY { get; set; }

        public float XStep { get; set; }
        public float YStep { get; set; }

        private short ScreenHeight { get; set; }
        private short ScreenWidth { get; set; }

        public Enemy(short screenWidth, short screenHeight, short XStartPos, short YStartPos)
        {
            XStep = 0.04f;
            YStep = 0.004f;

            ScreenHeight = screenHeight;
            ScreenWidth = screenWidth;

            PosX = XStartPos;
            PosY = YStartPos    ;
        }

        public void FollowPlayer(Player player, float elapsedTime)
        {
            if (PosX > player.PosX) Move(Direction.Left, elapsedTime);
            if (PosX < player.PosX) Move(Direction.Right, elapsedTime);
            if (PosY > player.PosY) Move(Direction.Up, elapsedTime);
            if (PosY < player.PosY) Move(Direction.Down, elapsedTime);

        }

        public void Accelerate(float percentage)
        {
            XStep = XStep * (1 + percentage);
            YStep = YStep * (1 + percentage);
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
