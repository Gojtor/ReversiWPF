using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reversi.Persistance
{
    public class SaveReversiTable
    {

        public int[]? FieldPieces { get; set; }
        public bool Player { get; set; }
        public int Player1Time { get; set; }
        public int Player2Time { get; set; }
        public int Player1PieceCount { get; set; }
        public int Player2PieceCount { get; set; }
        public int ValidCounter { get; set; }
        public SaveReversiTable(ReversiTable table)
        {
            this.Player = table.GetPlayer;
            this.Player1Time = table.Player1Time;
            this.Player2Time = table.Player2Time;
            this.Player1PieceCount = table.Player1PieceCount;
            this.Player2PieceCount = table.Player2PieceCount;
            this.ValidCounter = table.ValidCounter;

            this.FieldPieces = new int[table.FieldSize * table.FieldSize];
            for (int i = 0; i < table.FieldSize; i++)
            {
                for (int j = 0; j < table.FieldSize; j++)
                {
                    if (table.GetValue(i, j) != 2)
                    {
                        this.FieldPieces[i * table.FieldSize + j] = table.GetValue(i,j);
                    }
                    else
                    {
                        this.FieldPieces[i * table.FieldSize + j] = 0;
                    }
                }
            }
        }
        [JsonConstructor]
        public SaveReversiTable() { }
    }
}
