using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.GameManagement
{
    public class TriominoTileEventArgs : EventArgs
    {
        public string TileName { get; set; }
        public string OtherTileName { get; set; }
        public TileFace? TileFace { get; set; }
        public TileFace? OtherTileFAce { get; set; }
        public PlayerCode? Player { get; set; }
    }
}
