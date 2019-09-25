using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.Extensions
{
    public static class StringExtensions
    {
        #region IsTriominoTileName
        /// <summary>
        /// Verifies if a string matches the format for a TriominoTile-Name: '1-2-3'
        /// </summary>
        /// <param name="name">The name to be checked.</param>
        /// <returns>True if the string matches the TriominoTile-Name format, false if not.</returns>
        public static bool IsTriominoTileName(this string name)
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

        #region EnsureTriominoTileName
        /// <summary>
        /// Throws an appropriate Exception, if name is not in the right format
        /// for a TriominoTile-Name
        /// </summary>
        /// <param name="name">The tiles name.</param>
        public static void EnsureTriominoTileName(this string name)
        {
            if (!name.IsTriominoTileName())
            {
                throw new ArgumentException($"The name '{name}' is invalid for a TriominoTile. Tile names must be of following format: '0-0-0'");
            }
        }
        #endregion

        #region IsTripleTriomino
        /// <summary>
        /// Verifies, if a string is a tripleTriominoTile-name
        /// </summary>
        /// <param name="name">The string to be verified</param>
        /// <returns>True, if it is a Triple-Triomino-Tile-Name, false if not</returns>
        public static bool IsTripleTriomino(this string name)
        {
            name.EnsureTriominoTileName();

            // if all values are the same, there is only one distinct value
            return name.Split('-').Distinct().Count() == 1;
        }
        #endregion

        #region GetTriominoTileValue
        /// <summary>
        /// Determines the value of a triominoTile, based on its name.
        /// </summary>
        /// <param name="name">The name of the triominoTile</param>
        /// <returns>The value of the triominoTile</returns>
        public static int GetTriominoTileValue(this string name)
        {
            EnsureTriominoTileName(name);
            string[] nameParts = name.Split('-');
            int[] values = nameParts.Select(n => int.Parse(n)).ToArray();

            return values.Aggregate((a, b) => a + b);
        }
        #endregion

        #region GetTriominoTileNumbersFromName
        /// <summary>
        /// Determines all three single Numberchars from a TriominoTile-Name
        /// </summary>
        /// <param name="name">Name of the TriominoTile</param>
        /// <returns>Array with all thre numbers as string.</returns>
        public static string[] GetTriominoTileNumbersFromName(this string name)
        {
            EnsureTriominoTileName(name);

            return name.Split('-');
        }
        #endregion
    }
}
