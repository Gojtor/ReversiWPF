using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Model
{
    public class ReversiEventArgs : EventArgs
    {
        private int player1Time;
        private int player2Time;
        private bool player1Won;
        private bool player2Won;
        private int player1PieceCount;
        private int player2PieceCount;
        private bool player1Pass;
        private bool player2Pass;

        public int Player1Time { get {  return player1Time; } }
        public int Player2Time { get { return player2Time; } }
        public bool Player1Won { get { return player1Won; } }
        public bool Player2Won { get { return player2Won; } }
        public int Player1PieceCount { get {  return player1PieceCount; } }
        public int Player2PieceCount { get { return player2PieceCount; } }
        public bool Player1Pass { get { return player1Pass; } }
        public bool Player2Pass { get { return player2Pass; } }

        public ReversiEventArgs(int player1Time, int player2Time, bool player1Won, bool player2Won,int player1PieceCount, int player2PieceCount,bool player1Pass,bool player2Pass)
        {
            this.player1Time = player1Time;
            this.player2Time = player2Time;
            this.player1Won = player1Won;
            this.player2Won = player2Won;
            this.player1PieceCount = player1PieceCount;
            this.player2PieceCount = player2PieceCount;
            this.player1Pass = player1Pass;
            this.player2Pass = player2Pass;
        }

        public ReversiEventArgs()
        {

        }

    }
}
