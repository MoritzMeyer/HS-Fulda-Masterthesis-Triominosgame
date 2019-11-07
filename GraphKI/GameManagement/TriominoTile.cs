using GraphKI.Extensions;
using GraphKI.GraphSuite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GameManagement
{
    public class TriominoTile
    {
        public string Name { get; private set; }
        public TileOrientation Orientation { get; private set; }

        public Point TileGridPosition { get; set; }

        public TriominoTile(string name, TileOrientation orientation, Point tileGridPosition = default(Point))
        {
            name.EnsureTriominoTileName();

            this.Name = name;
            this.Orientation = orientation;
            this.TileGridPosition = tileGridPosition;
        }

        public string GetArrayName()
        {
            string[] nameParts = this.Name.Split('-');
            string arrayName = string.Empty;
            switch (this.Orientation)
            {
                case TileOrientation.Straight:
                case TileOrientation.Flipped:
                    arrayName = this.Name;
                    break;
                case TileOrientation.TiltLeft:
                case TileOrientation.DoubleTiltRight:
                    arrayName = nameParts[2] + "-" + nameParts[0] + "-" + nameParts[1];
                    break;
                case TileOrientation.DoubleTiltLeft:
                case TileOrientation.TiltRight:
                    arrayName = nameParts[1] + "-" + nameParts[2] + "-" + nameParts[0];
                    break;
            }

            return arrayName;
        }

        #region CheckName
        /// <summary>
        /// Checks a string for tile name Format: '1-2-3'
        /// </summary>
        /// <param name="name">The name which should be checked.</param>
        /// <returns>True, if the name matches the format, false if not</returns>
        public static bool CheckName(string name)
        {
            return name.IsTriominoTileName();
        }
        #endregion

        public HyperEdge CreateHyperEdgeFromTile()
        {
            string[] nameParts = this.Name.Split('-');
            return new HyperEdge(nameParts[0] + nameParts[1], nameParts[1] + nameParts[2], nameParts[2] + nameParts[0], this.Orientation);
        }
    }
}
