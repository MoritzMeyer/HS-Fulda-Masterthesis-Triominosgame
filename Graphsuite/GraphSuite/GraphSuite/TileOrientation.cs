using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSuite
{
    public enum TileOrientation
    {
        Straight, TiltLeft, TiltRight, DoubleTiltLeft, DoubleTiltRight, Flipped
    }

    public static class TileOrientationExtension
    {
        public static ArrayTileOrientation ToArrayTileOrientation(this TileOrientation orientation)
        {
            ArrayTileOrientation arrayOrientation;
            switch (orientation)
            {
                case TileOrientation.TiltLeft:
                case TileOrientation.TiltRight:
                case TileOrientation.Flipped:
                    arrayOrientation = ArrayTileOrientation.TopDown;
                    break;
                case TileOrientation.Straight:
                case TileOrientation.DoubleTiltLeft:
                case TileOrientation.DoubleTiltRight:
                default:
                    arrayOrientation = ArrayTileOrientation.BottomUp;
                    break;
            }

            return arrayOrientation;
        }
    }
}
