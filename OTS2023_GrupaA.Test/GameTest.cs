using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OTS2026_GrupaA.Exceptions;
using OTS2026_GrupaA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OTS2026_GrupaA.Test
{
    [TestFixture]
    internal class GameTest
    {

        // Igrac moze da se pozicionira na sve pozicije unutar 30x30x30 mape i izvan mjesta koje predstaljaju barijeru. Barijere imamo na 2 mjesta
        // a to je na kordinatama od 5, 5, 0 do  5, 20, 10 i na kordinatama od 20, 5, 0 do 5, 20, 10.
        // Tako da bi neke od granicnih vrijednosti bile:
        // Za granice mape: X osa (-1, 0, 1, 28, 29, 30); Y osa (-1, 0, 1, 28, 29, 30); i Z osa  (-1, 0, 1, 28, 29, 30); # Ne idemo sa 29, 30, 31 jer je mapa krece od 0, a ne od 1.
        // Neke od granica za barijere:
        // 1 Barijera: X osa ( 4, 5, 6), Y osa (4, 5, 6, 19, 20, 21), Z osa (-1, 0, 1, 9, 10, 11)
        // 2 Barijera: X osa ( 21, 20, 19, 6, 5, 4), Y osa (4, 5, 6, 19, 20, 21), Z osa (-1, 0, 1, 9, 10, 11)

        [Test]
        public void PositionOutsideBoundariesOnGameCreationException() {
            Exception ex = Assert.Throws<PositionOutsideOfMapException>((TestDelegate)(() => new Game(new Position(40, 40, 40), new Position(40, 40, 40))));
            Assert.That(ex.Message, Is.EqualTo("Positions must be valid!"));
        }

        [Test]
        public void PositionOnBarrierOnGameCreationException()
        {
            Exception ex = Assert.Throws<PositionOutsideOfMapException>((TestDelegate)(() => new Game(new Position(6, 6, 1), new Position(1, 1, 1))));
            Assert.That(ex.Message, Is.EqualTo("Positions must be valid!"));
        }

        private Game game;

        [SetUp]
        public void SetUp() { 
            game = new Game(new Position(1, 2, 0), new Position(1, 24, 0));
        }


        // Testirano kretanje igraca u smjeru ka dole. Igrac se moze pomjerati u 6 smjerova sve dok njegova naredna pozicija ne predstavlja neku
        // od nevalidnih pozicija tj. pozicije izvan mape ili na border.

        [TestCase(1, 2, 0, 3)]
        public void SuccessfullPlayerMoveDown(int x, int y, int z, int expectedY) {
            game.Player.Position = new Position(x, y, z);
            game.MovePlayer(Move.Down);

            Assert.That(expectedY, Is.EqualTo(game.Player.Position.Y));
        }

        [TestCase(29, 29, 29, 29)]
        public void PlayerMoveOutsideMapBounderiseShouldNotMove(int x, int y, int z, int expectedY)
        {
            game.Player.Position = new Position(x, y, z);
            game.MovePlayer(Move.Down);

            Assert.That(expectedY, Is.EqualTo(game.Player.Position.Y));
        }



        //[TestCase(5, 4, 11, 4)]
        //public void PlayerMoveOnBorderShouldNotMove(int x, int y, int z, int expectedY)
        //{
        //    game.Player.Position = new Position(x, y, z);
        //    game.MovePlayer(Move.Down);
        //    Assert.That(expectedY, Is.EqualTo(game.Player.Position.Y));
        //}

        // Igrac ima mogucnost da pokupi Gold koji je vidljiv, kada dodje do polja za otrkivanje hidden stvari onda mu se omogucuje i
        // kupljenje skrivenog Golda. Testirano je i kupljenje golda, i otkrivanje skrivenih stvari i kupljene skrivenog golda.

        [Test]
        public void PlayerMoveDownAndCollectGold() {

            game.Player.Position = new Position(1, 2, 3);
            game.Player.AmountOfGold = 0;
            game.Map.Tiles[1, 3, 3].Content = TileContent.Gold;
            game.Map.Tiles[1, 3, 3].Type = TileType.Standard;
            game.MovePlayer(Move.Down);
            game.CollectItems();
            Assert.That(game.Player.AmountOfGold, Is.EqualTo(1));

        }

        [Test]
        public void PlayerRevealHiddenContent()
        {
            game.Player.Position = new Position(1, 2, 3);
            game.Player.CanRevealHidden = false;
            game.Map.Tiles[1, 3, 3].Content = TileContent.RevealHiddenItem;
            game.Map.Tiles[1, 3, 3].Type = TileType.Standard;
            game.MovePlayer(Move.Down);
            game.CollectItems();

            Assert.That(game.Player.CanRevealHidden, Is.True);
        }

        [Test]
        public void PlayerCollectHiddenGold()
        {
            game.Player.Position = new Position(1, 2, 3);
            game.Player.CanRevealHidden = true;
            game.Player.AmountOfHiddenGold = 0;
            game.Map.Tiles[1, 3, 3].Content = TileContent.Gold;
            game.Map.Tiles[1, 3, 3].Type = TileType.Hidden;
            game.MovePlayer(Move.Down);
            game.CollectItems();

            Assert.That(1, Is.EqualTo(game.Player.AmountOfHiddenGold));
        }

    }
}
