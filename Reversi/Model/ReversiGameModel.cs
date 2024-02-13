using Reversi.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reversi.Model
{
    public class ReversiGameModel
    {
        private IReversiDataAccess dataAccess;
        private ReversiTable table;
        private int player1Time=0;
        private int player2Time=0;
        private int mapSize=10;


        public ReversiTable Table { get { return table; } }
        public int Player1Time { get { return player1Time; } }
        public int Player2Time { get { return player2Time; } }
        public int MapSize { get { return mapSize; } set { mapSize = value; } }

        public bool GetCurrentPlayerStatus { get { return table.GetPlayer; } }
        public int Player1PieceCount { get { return table.Player1PieceCount; } }
        public int Player2PieceCount { get { return table.Player2PieceCount; } }
        public bool IsGameOver { get { return (table.IsFilled || table.GameOver); } }
        
        public event EventHandler<ReversiEventArgs>? Advance;
        public event EventHandler<ReversiButtonChangeEventArgs>? ButtonChange;
        public event EventHandler<ReversiEventArgs>? GameOver;

        public ReversiGameModel(IReversiDataAccess dataAccess) 
        {
            this.dataAccess=dataAccess;
            table = new ReversiTable();
        }


        public void NewGame()
        {
            table = new ReversiTable(mapSize);
            table.StartingPositions();
            player1Time= 0;
            player2Time = 0;
        }

        public void Update(int x, int y)
        {
            if(table.GetValue(x, y) == 2)
            {
                table.Change(x, y);
                WhenButtonChange(x,y);
            
            }
        }
        public void CheckValidPlaces()
        {
            table.ValidPlaces();
        }
        public void ChangePlayer()
        {
            table.ChangePlayer();
        }
        public void Tick()
        {
            if (IsGameOver)
            {
                if(table.Player1PieceCount>table.Player2PieceCount)
                {
                    WhenGameOver(true, false);
                }
                else
                {
                    WhenGameOver(false, true);
                }
            }

            if (table.GetPlayer)
            {
                player2Time++;
                table.Player2Time = player2Time;
            }
            else
            {
                player1Time++;
                table.Player1Time = player1Time;
            }
            if (table.CheckForPass())
            {
                if (table.GetPlayer)
                {
                    WhenAdvance(false, true);
                }
                else
                {
                    WhenAdvance(true, false);
                }
            }
            else
            {
                    WhenAdvance(false,false);
            }
        }
        public async Task SaveGameAsync(String path)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            await dataAccess.SaveAsync(path, table);
        }
        public async Task LoadGameAsync(String path)
        {
            if (dataAccess == null)
                throw new InvalidOperationException("No data access is provided.");

            table = await dataAccess.LoadAsync(path);
            player1Time = table.Player1Time;
            player2Time = table.Player2Time;
        }

        private void WhenButtonChange(int x, int y)
        {
            ButtonChange?.Invoke(this,new ReversiButtonChangeEventArgs(x,y));
        }
        private void WhenAdvance()
        {
            Advance?.Invoke(this, new ReversiEventArgs(player1Time, player2Time,false,false,table.Player1PieceCount,table.Player2PieceCount,false,false));
        }
        private void WhenAdvance(bool p1Pass,bool p2Pass)
        {
            Advance?.Invoke(this, new ReversiEventArgs(player1Time, player2Time, false, false, table.Player1PieceCount, table.Player2PieceCount, p1Pass, p2Pass));
        }
        private void WhenGameOver(bool p1Win,bool p2Win)
        {
            GameOver?.Invoke(this, new ReversiEventArgs(player1Time, player2Time, p1Win, p2Win, table.Player1PieceCount, table.Player2PieceCount,false,false));
        }

    }
}
