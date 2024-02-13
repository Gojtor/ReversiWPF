using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Reversi.Persistance
{
    public class ReversiTable
    {
        private int[,] fieldPieces;

        private bool player = false; // Player 1 false
        private bool zeroValid = false;
        public int Player1Time { get; set; }
        public int Player2Time { get; set; }
        public int Player1PieceCount { get; set; } = 2;
        public int Player2PieceCount { get; set; } = 2;
        public int ValidCounter {
            get
            {
                int count = 0;
                for (int i = 0; i < fieldPieces.GetLength(0); i++)
                {
                    for (int j = 0; j < fieldPieces.GetLength(0); j++)
                    {
                        if(fieldPieces[i,j] == 2)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
        }
        public bool GameOver { get; private set; } = false;

        public bool IsFilled {
            get
            {
                foreach (var piece in fieldPieces)
                {
                    if (piece == 0 || piece == 2)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public int FieldSize { get { return fieldPieces.GetLength(0); } }

        public ReversiTable() : this(10) { }
        public ReversiTable(int fieldSize)
        {
            fieldPieces = new int[fieldSize, fieldSize];
        }
        public ReversiTable(SaveReversiTable saveTable)
        {
            this.player = saveTable.Player;
            this.Player1PieceCount = saveTable.Player1PieceCount;
            this.Player2PieceCount = saveTable.Player2PieceCount;
            this.Player1Time = saveTable.Player1Time;
            this.Player2Time = saveTable.Player2Time;
            //this.ValidCounter = saveTable.ValidCounter;
            int size = (int)Math.Sqrt(saveTable.FieldPieces!.GetLength(0));
            this.fieldPieces = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    this.fieldPieces[i, j] = saveTable.FieldPieces[i*size+j];
                }
            }
        }
        

        public int GetValue(int x, int y)
        {
            return fieldPieces[x, y];
        }
        public void SetValue(int x, int y, int value)
        {
            fieldPieces[x, y] = value;
        }

        public bool GetPlayer { get { return player; } }
        public bool SetPlayer { set { player = value; } }

        public void Change(int x, int y)
        {
            if (!player) { fieldPieces[x, y] = 1; }
            else { fieldPieces[x, y] = -1; }
            ValidReset();
            LookAround(x, y, true);
            PlayerPieceAdd();
            ChangePlayer();
        }
        public bool CheckForPass()
        {
            if (ValidCounter == 0)
            {
                return true;
            }
            return false;
        }
        public void ChangePlayer()
        {
            player = !player;
            ValidPlaces();
            CheckGameOverCondition();
        }
        public void StartingPositions()
        {
            int size = fieldPieces.GetLength(0);
            fieldPieces[ size/ 2 - 1, size / 2 - 1]= 1;
            fieldPieces[size / 2 - 1, size / 2]= -1;
            fieldPieces[size / 2, size / 2 - 1]= -1;
            fieldPieces[size / 2, size / 2]= 1;
        }
        private void CheckGameOverCondition()
        {
            if (ValidCounter == 0 && !zeroValid)
            {
                zeroValid = true;
            }
            else if (ValidCounter == 0 && zeroValid)
            {
                GameOver = true;
            }
            else
            {
                zeroValid = false;
            }
        }
        private void PlayerPieceAdd()
        {
            if (player)
            {
                Player2PieceCount++;
            }
            else
            {
                Player1PieceCount++;
            }
        }
        private void LookAround(int x, int y, bool type)
        {
            int fieldLength = fieldPieces.GetLength(0);
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if((x + i >= 0 && x + i < fieldLength) && (y + j >= 0 && y + j < fieldLength))
                    {
                        if (fieldPieces[x + i, y + j] == fieldPieces[x, y] * -1 && type)
                        {                         
                            FlipPiecesRec(x+i, y+j, i, j, false);
                        }
                        else if (!type && fieldPieces[x + i, y + j] == fieldPieces[x, y] * -1)
                        {
                            
                            ValidPlacesRec(x, y, i, j);
                        }
                    }                 
                }
            }

        }
        private bool FlipPiecesRec(int x, int y, int xDir, int yDir, bool valid)
        {
            int fieldLength = fieldPieces.GetLength(0);
            if ((x + xDir >= 0 && x + xDir < fieldLength) && (y + yDir >= 0 && y + yDir < fieldLength))
            {
                if (fieldPieces[x + xDir, y + yDir] == fieldPieces[x, y])
                {
                    if (FlipPiecesRec(x + xDir, y + yDir, xDir, yDir, valid))
                    {
                        fieldPieces[x, y] = fieldPieces[x, y] * -1;
                        if (player)
                        {
                            Player1PieceCount--;
                            Player2PieceCount++;
                        }
                        else
                        {
                            Player1PieceCount++;
                            Player2PieceCount--;
                        }
                    }
                }
                if (fieldPieces[x + xDir, y + yDir] == fieldPieces[x, y] * -1)
                {
                    if (player)
                    {
                        Player1PieceCount--;
                        Player2PieceCount++;
                    }
                    else
                    {
                        Player1PieceCount++;
                        Player2PieceCount--;
                    }
                    fieldPieces[x, y] = fieldPieces[x, y] * -1;
                    return true;
                }
                else
                {
                    return false;
                }            
            }
            return false;
        }
        public void ValidPlaces()
        {
            int fieldLength = fieldPieces.GetLength(0);
            for (int i = 0; i < fieldLength; i++)
            {
                for (int j = 0; j < fieldLength; j++)
                {
                    if (fieldPieces[i, j] != 0 && fieldPieces[i, j] != 2) 
                    {
                        LookAround(i, j, false);
                    }
                }
            }
        }
        public void ValidReset()
        {
            int fieldLength = fieldPieces.GetLength(0);
            for (int i = 0; i < fieldLength; i++)
            {
                for (int j = 0; j < fieldLength; j++)
                {
                    if (fieldPieces[i, j] == 2)
                    {
                        fieldPieces[i, j] = 0;
                    }
                }
            }
        }
        private void ValidPlacesRec(int x, int y, int xDir, int yDir)
        {
            int fieldLength = fieldPieces.GetLength(0);
            if ((x + xDir >= 0 && x + xDir < fieldLength) && (y + yDir >= 0 && y + yDir < fieldLength))
            {
                if (player)
                {
                    if(fieldPieces[x + xDir, y + yDir] == 1)
                    {
                        ValidPlacesRec(x + xDir, y + yDir, xDir, yDir);
                    }
                }
                else
                {
                    if (fieldPieces[x + xDir, y + yDir] == -1)
                    {
                        ValidPlacesRec(x + xDir, y + yDir, xDir, yDir);
                    }
                }
                if (fieldPieces[x + xDir, y + yDir] == 0)
                {
                    fieldPieces[x + xDir, y + yDir] = 2;
                }
            }
        }
    }
}
