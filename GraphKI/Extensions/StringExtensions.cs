using GraphKI.GameManagement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

        #region GetValuesForFace
        /// <summary>
        /// Determines a TriominoTile-Faces value for one tile name.
        /// </summary>
        /// <param name="name">the tiles name.</param>
        /// <param name="face">the face whose value is requested.</param>
        /// <returns>the faces value.</returns>
        public static string[] GetValuesForFace(this string name, TileFace face)
        {
            name.EnsureTriominoTileName();
            string[] parts = name.GetTriominoTileNumbersFromName();
            switch(face)
            {
                case TileFace.Right:
                    return new List<string>() { parts[0], parts[1] }.ToArray();
                case TileFace.Bottom:
                    return new List<string>() { parts[1], parts[2] }.ToArray();
                case TileFace.Left:
                    return new List<string>() { parts[2], parts[0] }.ToArray();
                default:
                    throw new ArgumentException("Face couldn't be recognized.");
            }
        }
        #endregion

        #region CheckIfFacesMatches
        /// <summary>
        /// Verifies if two faces of two tiles are macthing.
        /// </summary>
        /// <param name="tileName">name of first tile.</param>
        /// <param name="otherTileName">name of second tile.</param>
        /// <param name="tileFace">face of first tile.</param>
        /// <param name="otherTileFace">face of second tile.</param>
        /// <returns></returns>
        public static bool CheckIfFacesMatches(this string tileName, string otherTileName, TileFace tileFace, TileFace otherTileFace)
        {
            string[] tileFaceParts = tileName.GetValuesForFace(tileFace);
            string[] otherFaceParts = otherTileName.GetValuesForFace(otherTileFace);

            return tileFaceParts[0].Equals(otherFaceParts[1]) && tileFaceParts[1].Equals(otherFaceParts[0]);
        }
        #endregion

        public static string GetVertexValue(this String str)
        {
            return str.Replace("_", "");
        }
    }
}
