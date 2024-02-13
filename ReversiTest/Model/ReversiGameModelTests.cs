using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi;
using Reversi.Model;
using Reversi.Persistance;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Drawing;
using System.Reflection;

namespace ReversiGameModel.Tests
{
    [TestClass()]
    public class ReversiGameModelTests
    {
        private Reversi.Model.ReversiGameModel modelTest = null!;
        

        [TestMethod()]
        public void NewGameTest()
        {
            IReversiDataAccess dataAccess1 = new ReversiFileDataAccess();
            IReversiDataAccess dataAccess2 = new ReversiFileDataAccess();
            IReversiDataAccess dataAccess3 = new ReversiFileDataAccess();


            Reversi.Model.ReversiGameModel model1 = new Reversi.Model.ReversiGameModel(dataAccess1);
            Reversi.Model.ReversiGameModel model2 = new Reversi.Model.ReversiGameModel(dataAccess1);
            Reversi.Model.ReversiGameModel model3 = new Reversi.Model.ReversiGameModel(dataAccess1);

            model1.MapSize=10;
            model1.NewGame();
            model1.MapSize=15;
            model1.NewGame();
            model1.MapSize=30;
            model1.NewGame();
            model1.MapSize=50;
            model1.NewGame();

            Assert.AreEqual(50, model1.Table.FieldSize);
            Assert.AreEqual(2, model1.Table.Player1PieceCount);
            Assert.AreEqual(2, model1.Table.Player2PieceCount);

            model2.MapSize = 20;
            model2.NewGame();
            model3.MapSize = 30;
            model3.NewGame();

            for (int i = 1; i < 4; i++)
            {
                IReversiDataAccess dataAccess = new ReversiFileDataAccess();

                Reversi.Model.ReversiGameModel model = new Reversi.Model.ReversiGameModel(dataAccess);
                model.MapSize = 10 * i;
                model.NewGame();

                Assert.AreEqual(10 * i, model.Table.FieldSize);
                Assert.AreEqual(0, model.Player1Time);
                Assert.AreEqual(0, model.Player2Time);
                Assert.AreEqual(2, model.Table.Player1PieceCount);
                Assert.AreEqual(2, model.Table.Player2PieceCount);
                Assert.AreEqual(1, model.Table.GetValue(10*i / 2 - 1, 10*i / 2 - 1));
                Assert.AreEqual(-1, model.Table.GetValue(10 * i / 2 - 1, 10 * i / 2));
                Assert.AreEqual(-1, model.Table.GetValue(10 * i / 2, 10 * i / 2 - 1));
                Assert.AreEqual(1, model.Table.GetValue(10 * i / 2, 10 * i / 2));


            }




        }

        [TestMethod()]
        public void UpdateTest()
        {
            IReversiDataAccess dataAccess = new ReversiFileDataAccess();
            Reversi.Model.ReversiGameModel model = new Reversi.Model.ReversiGameModel(dataAccess);
            model.MapSize = 10;
            model.NewGame();
            model.CheckValidPlaces();

            Assert.AreEqual(2, model.Player1PieceCount);
            Assert.AreEqual(2, model.Player2PieceCount);
            Assert.AreEqual(0, model.Player1Time);
            Assert.AreEqual(0, model.Player2Time);
            

            model.Update(0, 0);
            Assert.AreEqual(0, model.Table.GetValue(0, 0));
            Assert.AreEqual(2, model.Player1PieceCount);
            Assert.AreEqual(2, model.Player2PieceCount);
            Assert.AreEqual(0, model.Player1Time);
            Assert.AreEqual(0, model.Player2Time);

            model.Update(5, 3);
            Assert.AreEqual(1, model.Table.GetValue(5, 3));
            Assert.AreEqual(4, model.Player1PieceCount);
            Assert.AreEqual(1, model.Player2PieceCount);
            Assert.AreEqual(0, model.Player1Time);
            Assert.AreEqual(0, model.Player2Time);

        }

        [TestMethod()]
        public void CheckValidPlacesTest()
        {
            IReversiDataAccess dataAccess = new ReversiFileDataAccess();
            modelTest = new Reversi.Model.ReversiGameModel(dataAccess);
            modelTest.MapSize = 10;
            modelTest.NewGame();
            modelTest.CheckValidPlaces();
            Assert.IsTrue(modelTest.Table.ValidCounter == 4);
            modelTest.MapSize = 20;
            modelTest.NewGame();
            modelTest.CheckValidPlaces();
            Assert.IsTrue(modelTest.Table.ValidCounter == 4);
            modelTest.MapSize = 30;
            modelTest.NewGame();
            modelTest.CheckValidPlaces();
            Assert.IsTrue(modelTest.Table.ValidCounter == 4);
        }

        [TestMethod()]
        public void ChangePlayerTest()
        {
            IReversiDataAccess dataAccess = new ReversiFileDataAccess();
            modelTest = new Reversi.Model.ReversiGameModel(dataAccess);
            modelTest.MapSize = 10;
            modelTest.NewGame();

            bool testPlayer = modelTest.GetCurrentPlayerStatus;
            modelTest.ChangePlayer();
            Assert.IsTrue(modelTest.GetCurrentPlayerStatus!=testPlayer);
            modelTest.ChangePlayer();
            Assert.IsTrue(modelTest.GetCurrentPlayerStatus == testPlayer);
        }

        [TestMethod()]
        public void GetCurrentPlayerStatusTest()
        {
            IReversiDataAccess dataAccess = new ReversiFileDataAccess();
            modelTest = new Reversi.Model.ReversiGameModel(dataAccess);
            modelTest.MapSize = 10;
            modelTest.NewGame();

            Assert.IsFalse(modelTest.GetCurrentPlayerStatus);
            modelTest.ChangePlayer();
            Assert.IsTrue(modelTest.GetCurrentPlayerStatus);
        }


        [TestMethod()]
        public void TickTest()
        {
            IReversiDataAccess dataAccess = new ReversiFileDataAccess();
            modelTest = new Reversi.Model.ReversiGameModel(dataAccess);
            modelTest.Advance += new EventHandler<ReversiEventArgs>(TestAdvance);
            modelTest.GameOver += new EventHandler<ReversiEventArgs>(TestGameOver);
            modelTest.MapSize = 10;
            modelTest.NewGame();
            int p1Time = modelTest.Player1Time;
            int p2Time = modelTest.Player2Time;
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (!modelTest.IsGameOver)
                    {
                        if (modelTest.Table.ValidCounter == 0)
                        {
                            modelTest.Table.ChangePlayer();
                            continue;
                        }

                        List<Vector2> pos = GetPositions(modelTest);
                        int index = rnd.Next(0, pos.Count);

                        modelTest.Update((int)pos[index].X, (int)pos[index].Y);
                        
                        modelTest.Tick();

                        if (modelTest.GetCurrentPlayerStatus)
                        {
                            p2Time++;
                        }
                        else
                        {
                            p1Time++;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            Assert.AreEqual(p1Time,modelTest.Player1Time);
            Assert.AreEqual(p2Time,modelTest.Player2Time);
            Assert.IsTrue(modelTest.IsGameOver);
        }
        [TestMethod]
        public async Task LoadTest()
        {
            IReversiDataAccess dataAccess = new ReversiFileDataAccess();
            modelTest = new Reversi.Model.ReversiGameModel(dataAccess);
            modelTest.MapSize = 10;
            modelTest.NewGame();

            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string file = System.IO.Path.Combine(currentDirectory, @"..\..\..\Model\test.json");
            string filePath = Path.GetFullPath(file);


            await modelTest.LoadGameAsync(filePath);
            Assert.AreEqual(1, modelTest.Table.GetValue(10 / 2 - 1, 10 / 2 - 1));
            Assert.AreEqual(-1, modelTest.Table.GetValue(10 / 2 - 1, 10 / 2));
            Assert.AreEqual(-1, modelTest.Table.GetValue(10 / 2, 10 / 2 - 1));
            Assert.AreEqual(1, modelTest.Table.GetValue(10 / 2, 10 / 2));
            Assert.AreEqual(2, modelTest.Player1PieceCount);
            Assert.AreEqual(2, modelTest.Player2PieceCount);

        }



        [TestMethod]
        public void RandomPlay()
        {

            IReversiDataAccess dataAccess = new ReversiFileDataAccess();

            Reversi.Model.ReversiGameModel model = new Reversi.Model.ReversiGameModel(dataAccess);
            model.MapSize = 50;
            model.NewGame();
            
            Random rnd = new Random();

            while (!model.IsGameOver)
            {
                if (model.Table.ValidCounter == 0)
                {
                    model.Table.ChangePlayer();
                    continue;
                }

                List<Vector2> pos = GetPositions(model);
                int index = rnd.Next(0, pos.Count);

                model.Update((int)pos[index].X, (int)pos[index].Y);
            }

            Assert.AreNotEqual(4, model.Table.Player1PieceCount + model.Table.Player2PieceCount);
           
        }


        private List<Vector2> GetPositions(Reversi.Model.ReversiGameModel model)
        {
            List<Vector2> positions = new List<Vector2>();
            for (int i = 0; i < model.Table.FieldSize; i++)
            {
                for (int j = 0; j < model.Table.FieldSize; j++)
                {
                    if (model.Table.GetValue(i, j)==2)
                    {
                        positions.Add(new Vector2(i, j));
                    }
                }
            }
            return positions;
        }

        private void TestAdvance(object? sender, ReversiEventArgs e)
        {
            Assert.IsTrue(modelTest.Player1Time>=0);
            Assert.IsTrue(modelTest.Player2Time >= 0);
            Assert.AreEqual(modelTest.Table.IsFilled || modelTest.Table.GameOver, modelTest.IsGameOver);

            Assert.AreEqual(e.Player1PieceCount, modelTest.Player1PieceCount);
            Assert.AreEqual(e.Player2PieceCount,modelTest.Player2PieceCount);
            Assert.AreEqual(e.Player1Time,modelTest.Player1Time);
            Assert.AreEqual(e.Player2Time,modelTest.Player2Time);
            Assert.IsFalse(e.Player1Won);
            Assert.IsFalse(e.Player2Won);

        }
        private void TestGameOver(object? sender, ReversiEventArgs e)
        {
            Assert.IsTrue(e.Player1Won || e.Player2Won );
            Assert.IsTrue(modelTest.IsGameOver);
        }
    }
}