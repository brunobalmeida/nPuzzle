using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace nPuzzle
{
    class Tile : Button
    {

        private Puzzle owner;

        public Tile(Puzzle p)
        {
            this.owner = p;
        }

    }
}
