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
            HyperEdge connector = new HyperEdge("00", "00");
            Hexagon hexagon = new Hexagon(edge, TileFace.Right, connector);
            Assert.AreEqual(0, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left, connector);
            Assert.AreEqual(2, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom, connector);
            Assert.AreEqual(4, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.DoubleTiltRight);
            hexagon = new Hexagon(edge, TileFace.Left, connector);
            Assert.AreEqual(0, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom, connector);
            Assert.AreEqual(2, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right, connector);
            Assert.AreEqual(4, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.DoubleTiltLeft);
            hexagon = new Hexagon(edge, TileFace.Bottom, connector);
            Assert.AreEqual(0, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right, connector);
            Assert.AreEqual(2, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left, connector);
            Assert.AreEqual(4, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.TiltLeft);
            hexagon = new Hexagon(edge, TileFace.Right, connector);
            Assert.AreEqual(1, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left, connector);
            Assert.AreEqual(3, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom, connector);
            Assert.AreEqual(5, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.TiltRight);
            hexagon = new Hexagon(edge, TileFace.Left, connector);
            Assert.AreEqual(1, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Bottom, connector);
            Assert.AreEqual(3, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right, connector);
            Assert.AreEqual(5, hexagon.Pointer);

            edge = new HyperEdge("00", "00", "00", TileOrientation.Flipped);
            hexagon = new Hexagon(edge, TileFace.Bottom, connector);
            Assert.AreEqual(1, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Right, connector);
            Assert.AreEqual(3, hexagon.Pointer);
            hexagon = new Hexagon(edge, TileFace.Left, connector);
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
            Hexagon hexagon = new Hexagon(edge, TileFace.Right, new HyperEdge("00", "00"));

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
            Hexagon hexagon = new Hexagon(edge, TileFace.Right, new HyperEdge("00", "00"));

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

        #region Hexagon_CheckIfLastConnectorConnectsInAndOutgoing_has_to_work
        /// <summary>
        /// Verifies output for method CheckIfLastConnectorConnectsInAndOutgoing.
        /// </summary>
        [TestMethod]
        public void Hexagon_CheckIfLastConnectorConnectsInAndOutgoing_has_to_work()
        {
            HyperEdge edge = new HyperEdge("00", "00", "00", TileOrientation.Straight);
            Hexagon hexagon = new Hexagon(edge, TileFace.Right, new HyperEdge("00", "00"));

            HyperEdge connector1 = new HyperEdge("00", "00");
            HyperEdge connector2 = new HyperEdge("01", "10");

            Assert.IsTrue(hexagon.CheckIfLastConnectorConnectsInAndOutgoing(new Vertex("00"), new Vertex("00"), connector1));
            Assert.IsTrue(hexagon.CheckIfLastConnectorConnectsInAndOutgoing(new Vertex("01"), new Vertex("10"), connector2));
            Assert.IsTrue(hexagon.CheckIfLastConnectorConnectsInAndOutgoing(new Vertex("10"), new Vertex("01"), connector2));

            Assert.IsFalse(hexagon.CheckIfLastConnectorConnectsInAndOutgoing(new Vertex("10"), new Vertex("10"), connector2));
            Assert.IsFalse(hexagon.CheckIfLastConnectorConnectsInAndOutgoing(new Vertex("01"), new Vertex("01"), connector2));
        }
        #endregion

        #region Hexagon_TryAddToHexagon_has_to_work
        /// <summary>
        /// Verifies output for Method TryAddToHexagon
        /// </summary>
        [TestMethod]
        public void Hexagon_TryAddToHexagon_has_to_work()
        {
            // Positive test
            HyperEdge edge0 = new HyperEdge("00", "00", "00", TileOrientation.Straight);
            HyperEdge edge1 = new HyperEdge("00", "02", "20", TileOrientation.Flipped);
            HyperEdge edge2 = new HyperEdge("02", "22", "20", TileOrientation.DoubleTiltLeft);
            HyperEdge edge3 = new HyperEdge("01", "12", "20", TileOrientation.Flipped);
            HyperEdge edge4 = new HyperEdge("01", "11", "10", TileOrientation.DoubleTiltRight);
            HyperEdge edge5 = new HyperEdge("00", "01", "10", TileOrientation.TiltRight);

            HyperEdge connector0u5 = new HyperEdge("00", "00");
            HyperEdge connector1u2 = new HyperEdge("02", "20");
            HyperEdge connector3u4 = new HyperEdge("01", "10");            

            Hexagon hexagon = new Hexagon(edge0, TileFace.Right, connector0u5);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge2, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge3, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge4, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge5, connector0u5));
            Assert.IsTrue(hexagon.IsComplete);

            hexagon = new Hexagon(edge1, TileFace.Bottom, connector1u2);
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge2, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge3, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge4, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge5, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge0, connector0u5));
            Assert.IsTrue(hexagon.IsComplete);

            hexagon = new Hexagon(edge2, TileFace.Right, connector1u2);
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge3, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge4, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge5, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge0, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsTrue(hexagon.IsComplete);

            hexagon = new Hexagon(edge3, TileFace.Right, connector3u4);
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge4, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge5, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge0, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge2, connector1u2));
            Assert.IsTrue(hexagon.IsComplete);

            hexagon = new Hexagon(edge4, TileFace.Right, connector3u4);
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge5, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge0, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge2, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge3, connector3u4));
            Assert.IsTrue(hexagon.IsComplete);

            hexagon = new Hexagon(edge5, TileFace.Right, connector0u5);
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge0, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge2, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge3, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge4, connector3u4));
            Assert.IsTrue(hexagon.IsComplete);

            // Negativ tests
            hexagon = new Hexagon(edge0, TileFace.Right, connector0u5);
            edge1.Orientation = TileOrientation.Straight;
            Assert.IsFalse(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);

            edge1.Orientation = TileOrientation.TiltLeft;
            Assert.IsFalse(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);

            edge1.Orientation = TileOrientation.TiltRight;
            Assert.IsFalse(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);

            edge1.Orientation = TileOrientation.DoubleTiltLeft;
            Assert.IsFalse(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);

            edge1.Orientation = TileOrientation.DoubleTiltRight;
            Assert.IsFalse(hexagon.TryAddToHexagon(edge1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);

            edge1.Orientation = TileOrientation.Flipped;
            Assert.IsFalse(hexagon.TryAddToHexagon(edge1, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);

            HyperEdge false1 = new HyperEdge("00", "03", "30", TileOrientation.Flipped);
            hexagon = new Hexagon(edge2, TileFace.Right, connector1u2);
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge3, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge4, connector3u4));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge5, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsTrue(hexagon.TryAddToHexagon(edge0, connector0u5));
            Assert.IsFalse(hexagon.IsComplete);
            Assert.IsFalse(hexagon.TryAddToHexagon(false1, connector1u2));
            Assert.IsFalse(hexagon.IsComplete);

        }
        #endregion
    }
}
