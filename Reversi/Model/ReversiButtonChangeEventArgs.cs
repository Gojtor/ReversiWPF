using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Model
{
    public class ReversiButtonChangeEventArgs : EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public ReversiButtonChangeEventArgs(int x,int y)
        {
            X = x;
            Y = y;
        }
    }
}
