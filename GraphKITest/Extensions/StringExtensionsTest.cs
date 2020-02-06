using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using GraphKI.Extensions;
using GraphKI.GameManagement;
using System.Security.Cryptography;
using System.IO;

namespace GraphKITest.Extensions
{
    /// <summary>
    /// TestClass for Class 'StringExtensions'
    /// </summary>
    [TestClass]
    public class StringExtensionsTest
    {
        #region IsTriominoTileName_has_to_work
        /// <summary>
        /// Verifies output for method IsTriominoTilename
        /// </summary>
        [TestMethod]
        public void IsTriominoTileName_has_to_work()
        {
            string name = "1-2-3";
            string name1 = "12-2-3";
            string name2 = "6-2-3";
            string name3 = "2-3";
            string name4 = "3-2-1";
            string name5 = "a-b-c";

            Assert.IsTrue(name.IsTriominoTileName());
            Assert.IsFalse(name1.IsTriominoTileName());
            Assert.IsFalse(name2.IsTriominoTileName());
            Assert.IsFalse(name3.IsTriominoTileName());
            Assert.IsFalse(name4.IsTriominoTileName());
            Assert.IsFalse(name5.IsTriominoTileName());
        }
        #endregion

        #region IsTripleTriomino_has_to_work
        /// <summary>
        /// Verifies output for mehtod IsTripleTriomino
        /// </summary>
        [TestMethod]
        public void IsTripleTriomino_has_to_work()
        {
            string name1 = "0-0-0";
            string name2 = "1-1-1";
            string name3 = "2-2-2";
            string name4 = "3-3-3";
            string name5 = "4-4-4";
            string name6 = "5-5-5";

            string name7 = "0-0-1";
            string name8 = "0-1-1";

            string name9 = "6-6-6";

            Assert.IsTrue(name1.IsTripleTriomino());
            Assert.IsTrue(name2.IsTripleTriomino());
            Assert.IsTrue(name3.IsTripleTriomino());
            Assert.IsTrue(name4.IsTripleTriomino());
            Assert.IsTrue(name5.IsTripleTriomino());
            Assert.IsTrue(name6.IsTripleTriomino());

            Assert.IsFalse(name7.IsTripleTriomino());
            Assert.IsFalse(name8.IsTripleTriomino());

            Assert.ThrowsException<ArgumentException>(() => { name9.IsTripleTriomino(); }, "The name '6-6-6' is invalid for a TriominoTile. Tile names must be of following format: '0-0-0'");
        }
        #endregion

        #region GetTriominoTileValue_has_to_work
        /// <summary>
        /// Verifies output for method GetTriominoTileValue
        /// </summary>
        [TestMethod]
        public void GetTriominoTileValue_has_to_work()
        {
            string name1 = "1-2-3";
            string name2 = "3-2-1";

            Assert.AreEqual(6, name1.GetTriominoTileValue());
            Assert.ThrowsException<ArgumentException>(() => { name2.IsTripleTriomino(); }, "The name '3-2-1' is invalid for a TriominoTile. Tile names must be of following format: '0-0-0'");
        }
        #endregion

        #region GetValuesForFace_has_to_work
        /// <summary>
        /// Verifies output for GetValueForFace.
        /// </summary>
        [TestMethod]
        public void GetValuesForFace_has_to_work()
        {
            string tileName = "1-2-3";
            Assert.AreEqual("1", tileName.GetValuesForFace(TileFace.Right)[0]);
            Assert.AreEqual("2", tileName.GetValuesForFace(TileFace.Right)[1]);

            Assert.AreEqual("2", tileName.GetValuesForFace(TileFace.Bottom)[0]);
            Assert.AreEqual("3", tileName.GetValuesForFace(TileFace.Bottom)[1]);

            Assert.AreEqual("3", tileName.GetValuesForFace(TileFace.Left)[0]);
            Assert.AreEqual("1", tileName.GetValuesForFace(TileFace.Left)[1]);
        }
        #endregion

        #region CheckIfFacesMatches_has_to_work
        /// <summary>
        /// Verifies output for CheckIfFacesMatches
        /// </summary>
        [TestMethod]
        public void CheckIfFacesMatches_has_to_work()
        {
            string tileName = "1-2-3";
            string otherTileName = "2-3-3";

            Assert.IsTrue(tileName.CheckIfFacesMatches(otherTileName, TileFace.Bottom, TileFace.Left));
            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Bottom, TileFace.Right));
            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Bottom, TileFace.Bottom));

            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Left, TileFace.Left));
            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Left, TileFace.Right));
            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Left, TileFace.Bottom));

            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Right, TileFace.Left));
            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Right, TileFace.Right));
            Assert.IsFalse(tileName.CheckIfFacesMatches(otherTileName, TileFace.Right, TileFace.Bottom));
        }
        #endregion

        #region ReverseString_has_to_work
        /// <summary>
        /// Checks output of Reverse Extension method.
        /// </summary>
        [TestMethod]
        public void ReverseString_has_to_work()
        {
            string test = "test";
            Assert.AreEqual("tset", test.ReverseString());
        }
        #endregion
    }
}

