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
            EnsureName(name);

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
            if (name == null || name == string.Empty)
            {
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[0-5]-[0-5]-[0-5]$"))
            {
                return false;
            }

            int[] values = name.Split('-').Select(s => int.Parse(s)).ToArray();

            if (!(values[0] <= values[1] && values[1] <= values[2]))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region EnsureName
        /// <summary>
        /// Throws an ArgumentException if 'name' doesn't machtes the name format for TriominoTiles.
        /// </summary>
        /// <param name="name">The name.</param>
        public static void EnsureName(string name)
        {
            if (!CheckName(name))
            {
                throw new ArgumentException($"The name '{name}' is invalid. Tile names must be of following format: '0-0-0'");
            }
        }
        #endregion
    }
}
