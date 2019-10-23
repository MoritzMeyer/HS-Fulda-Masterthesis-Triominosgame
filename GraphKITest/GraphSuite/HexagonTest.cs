using GraphKI.GameManagement;
using GraphKI.GraphSuite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraphKITest.GraphSuite
{
    [TestClass]
    public class HexagonTest
    {
        #region Hexagon_DetermineHexagonPosition_has_to_work
        /// <summary>
        /// Verifies output of function DetermineHexagonPosition.
        /// </summary>
        [TestMethod]
        public void Hexagon_DetermineHexagonPosition_has_to_work()
        {
            HyperEdge edge = new HyperEdge("00", "00", "00", TileOrientation.Straight);
            Hexagon hexagon = new Hexagon(edge, TileFace.Right);
            Assert.AreEqual(0, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left);
            Assert.AreEqual(2, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom);
            Assert.AreEqual(4, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.DoubleTiltRight);
            hexagon = new Hexagon(edge, TileFace.Left);
            Assert.AreEqual(0, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom);
            Assert.AreEqual(2, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right);
            Assert.AreEqual(4, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.DoubleTiltLeft);
            hexagon = new Hexagon(edge, TileFace.Bottom);
            Assert.AreEqual(0, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right);
            Assert.AreEqual(2, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left);
            Assert.AreEqual(4, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.TiltLeft);
            hexagon = new Hexagon(edge, TileFace.Right);
            Assert.AreEqual(1, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left);
            Assert.AreEqual(3, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom);
            Assert.AreEqual(5, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.TiltRight);
            hexagon = new Hexagon(edge, TileFace.Left);
            Assert.AreEqual(1, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom);
            Assert.AreEqual(3, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right);
            Assert.AreEqual(5, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.Flipped);
            hexagon = new Hexagon(edge, TileFace.Bottom);
            Assert.AreEqual(1, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right);
            Assert.AreEqual(3, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left);
            Assert.AreEqual(5, hexagon.Pointer);
        }
        #endregion

        #region Hexagon_GetIncomingFace_has_to_work
        /// <summary>
        /// Verifies outoput for method GetIncomingFace.
        /// </summary>
        [TestMethod]
        public void Hexagon_GetIncomingFace_has_to_work()
        {
            HyperEdge edge = new HyperEdge("00", "00", "00", TileOrientation.Straight);
            Hexagon hexagon = new Hexagon(edge, TileFace.Right);

            Assert.AreEqual(TileFace.Left, hexagon.GetIncomingFace(0, TileOrientation.Straight));
            Assert.AreEqual(TileFace.Left, hexagon.GetIncomingFace(1, TileOrientation.TiltLeft));
            Assert.AreEqual(TileFace.Left, hexagon.GetIncomingFace(2, TileOrientation.DoubleTiltLeft));
            Assert.AreEqual(TileFace.Left, hexagon.GetIncomingFace(3, TileOrientation.Flipped));
            Assert.AreEqual(TileFace.Left, hexagon.GetIncomingFace(4, TileOrientation.DoubleTiltRight));
            Assert.AreEqual(TileFace.Left, hexagon.GetIncomingFace(5, TileOrientation.TiltRight));

            Assert.AreEqual(TileFace.Right, hexagon.GetIncomingFace(0, TileOrientation.DoubleTiltLeft));
            Assert.AreEqual(TileFace.Right, hexagon.GetIncomingFace(1, TileOrientation.Flipped));
            Assert.AreEqual(TileFace.Right, hexagon.GetIncomingFace(2, TileOrientation.DoubleTiltRight));
            Assert.AreEqual(TileFace.Right, hexagon.GetIncomingFace(3, TileOrientation.TiltRight));
            Assert.AreEqual(TileFace.Right, hexagon.GetIncomingFace(4, TileOrientation.Straight));
            Assert.AreEqual(TileFace.Right, hexagon.GetIncomingFace(5, TileOrientation.TiltLeft));

            Assert.AreEqual(TileFace.Bottom, hexagon.GetIncomingFace(0, TileOrientation.DoubleTiltRight));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetIncomingFace(1, TileOrientation.TiltRight));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetIncomingFace(2, TileOrientation.Straight));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetIncomingFace(3, TileOrientation.TiltLeft));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetIncomingFace(4, TileOrientation.DoubleTiltLeft));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetIncomingFace(5, TileOrientation.Flipped));
        }
        #endregion

        #region Hexagon_GetOutgoingFace_has_to_work
        /// <summary>
        /// Verifies output for mehtod GetOutgoingFace.
        /// </summary>
        [TestMethod]
        public void Hexagon_GetOutgoingFace_has_to_work()
        {
            HyperEdge edge = new HyperEdge("00", "00", "00", TileOrientation.Straight);
            Hexagon hexagon = new Hexagon(edge, TileFace.Right);

            Assert.AreEqual(TileFace.Right, hexagon.GetOutgoingFace(0, TileOrientation.Straight));
            Assert.AreEqual(TileFace.Right, hexagon.GetOutgoingFace(1, TileOrientation.TiltLeft));
            Assert.AreEqual(TileFace.Right, hexagon.GetOutgoingFace(2, TileOrientation.DoubleTiltLeft));
            Assert.AreEqual(TileFace.Right, hexagon.GetOutgoingFace(3, TileOrientation.Flipped));
            Assert.AreEqual(TileFace.Right, hexagon.GetOutgoingFace(4, TileOrientation.DoubleTiltRight));
            Assert.AreEqual(TileFace.Right, hexagon.GetOutgoingFace(5, TileOrientation.TiltRight));

            Assert.AreEqual(TileFace.Bottom, hexagon.GetOutgoingFace(0, TileOrientation.DoubleTiltLeft));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetOutgoingFace(1, TileOrientation.Flipped));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetOutgoingFace(2, TileOrientation.DoubleTiltRight));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetOutgoingFace(3, TileOrientation.TiltRight));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetOutgoingFace(4, TileOrientation.Straight));
            Assert.AreEqual(TileFace.Bottom, hexagon.GetOutgoingFace(5, TileOrientation.TiltLeft));

            Assert.AreEqual(TileFace.Left, hexagon.GetOutgoingFace(0, TileOrientation.DoubleTiltRight));
            Assert.AreEqual(TileFace.Left, hexagon.GetOutgoingFace(1, TileOrientation.TiltRight));
            Assert.AreEqual(TileFace.Left, hexagon.GetOutgoingFace(2, TileOrientation.Straight));
            Assert.AreEqual(TileFace.Left, hexagon.GetOutgoingFace(3, TileOrientation.TiltLeft));
            Assert.AreEqual(TileFace.Left, hexagon.GetOutgoingFace(4, TileOrientation.DoubleTiltLeft));
            Assert.AreEqual(TileFace.Left, hexagon.GetOutgoingFace(5, TileOrientation.Flipped));
        }
        #endregion
        
        [TestMethod]
        public void Hexagon_CheckIfLastConnectorConnectsInAndOutgoing_has_to_work()
        {

        }
    }
}
