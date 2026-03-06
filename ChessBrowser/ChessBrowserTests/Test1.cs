// =============================================================================
// File:        PgnParserTests.cs
// Project:     ChessBrowserTests
// Description: Unit tests for the PgnParser class. Verifies correct parsing
//              of PGN files into ChessGame objects, including all fields,
//              result conversions, and edge cases found in test data.
// Author:      [Your Name]
// Created:     2026-03-05
// =============================================================================

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System;
using ChessBrowser;

namespace ChessBrowserTests
{
    [TestClass]
    public class PgnParserTests
    {
        // Shared path and parsed games for all tests
        private static List<ChessGame> _games;

        /// <summary>
        /// Runs once before all tests. Loads and parses the PGN file once
        /// so each test doesn't have to re-read the file.
        /// </summary>
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test1.pgn");
            string[] lines = File.ReadAllLines(path);
            _games = PgnParser.Parse(lines);
        }

        // =====================================================================
        // Game Count
        // =====================================================================

        /// <summary>
        /// Verifies the correct number of games are parsed from the file.
        /// The test PGN contains 6 games (last one is incomplete but should still parse).
        /// </summary>
        [TestMethod]
        public void TestParsesCorrectGameCount()
        {
            Assert.AreEqual(6, _games.Count);
        }

        // =====================================================================
        // Game 0 - "4. IIFL Wealth Mumbai Op" (Draw)
        // =====================================================================

        [TestMethod]
        public void TestGame0_EventName()
        {
            Assert.AreEqual("4. IIFL Wealth Mumbai Op", _games[0].EventName);
        }

        [TestMethod]
        public void TestGame0_Site()
        {
            Assert.AreEqual("Mumbai IND", _games[0].Site);
        }

        [TestMethod]
        public void TestGame0_Round()
        {
            Assert.AreEqual("2.9", _games[0].Round);
        }

        [TestMethod]
        public void TestGame0_WhiteName()
        {
            Assert.AreEqual("Sundararajan, Kidambi", _games[0].WhiteName);
        }

        [TestMethod]
        public void TestGame0_BlackName()
        {
            Assert.AreEqual("Ziatdinov, Raset", _games[0].BlackName);
        }

        [TestMethod]
        public void TestGame0_WhiteElo()
        {
            Assert.AreEqual((uint?)2458, _games[0].WhiteElo);
        }

        [TestMethod]
        public void TestGame0_BlackElo()
        {
            Assert.AreEqual((uint?)2252, _games[0].BlackElo);
        }

        [TestMethod]
        public void TestGame0_Result_Draw()
        {
            // 1/2-1/2 should convert to 'D'
            Assert.AreEqual('D', _games[0].Result);
        }

        [TestMethod]
        public void TestGame0_EventDate()
        {
            Assert.AreEqual("2018-12-30", _games[0].EventDate);
        }

        [TestMethod]
        public void TestGame0_MovesNotEmpty()
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(_games[0].Moves));
        }

        [TestMethod]
        public void TestGame0_MovesStartCorrectly()
        {
            Assert.IsTrue(_games[0].Moves.StartsWith("1.c4"));
        }

        // =====================================================================
        // Game 1 - "Australian Open 2019" (Black wins)
        // =====================================================================

        [TestMethod]
        public void TestGame1_EventName()
        {
            Assert.AreEqual("Australian Open 2019", _games[1].EventName);
        }

        [TestMethod]
        public void TestGame1_Site()
        {
            Assert.AreEqual("Melbourne AUS", _games[1].Site);
        }

        [TestMethod]
        public void TestGame1_Result_BlackWins()
        {
            // 0-1 should convert to 'B'
            Assert.AreEqual('B', _games[1].Result);
        }

        [TestMethod]
        public void TestGame1_Round()
        {
            // Tests multi-decimal round parsing
            Assert.AreEqual("9.8", _games[1].Round);
        }

        [TestMethod]
        public void TestGame1_EventDate()
        {
            Assert.AreEqual("2018-12-27", _games[1].EventDate);
        }

        // =====================================================================
        // Game 2 - "4. IIFL Wealth Mumbai Op" (White wins)
        // =====================================================================

        [TestMethod]
        public void TestGame2_Result_WhiteWins()
        {
            // 1-0 should convert to 'W'
            Assert.AreEqual('W', _games[2].Result);
        }

        [TestMethod]
        public void TestGame2_WhiteName()
        {
            Assert.AreEqual("Visakh, NR", _games[2].WhiteName);
        }

        [TestMethod]
        public void TestGame2_BlackName()
        {
            Assert.AreEqual("Sarwat, Walaa", _games[2].BlackName);
        }

        [TestMethod]
        public void TestGame2_WhiteElo()
        {
            Assert.AreEqual((uint?)2491, _games[2].WhiteElo);
        }

        // =====================================================================
        // Game 3 - "Hastings Masters 2018-19"
        // =====================================================================

        [TestMethod]
        public void TestGame3_EventName()
        {
            // Tests event name with special characters (hyphens)
            Assert.AreEqual("Hastings Masters 2018-19", _games[3].EventName);
        }

        [TestMethod]
        public void TestGame3_Site()
        {
            Assert.AreEqual("Hastings ENG", _games[3].Site);
        }

        [TestMethod]
        public void TestGame3_Round()
        {
            Assert.AreEqual("4.7", _games[3].Round);
        }

        // =====================================================================
        // Game 4 - "48. Rilton Cup 2018-19" (Draw)
        // =====================================================================

        [TestMethod]
        public void TestGame4_EventName()
        {
            Assert.AreEqual("48. Rilton Cup 2018-19", _games[4].EventName);
        }

        [TestMethod]
        public void TestGame4_Result_Draw()
        {
            Assert.AreEqual('D', _games[4].Result);
        }

        [TestMethod]
        public void TestGame4_WhiteElo()
        {
            Assert.AreEqual((uint?)2546, _games[4].WhiteElo);
        }

        [TestMethod]
        public void TestGame4_BlackElo()
        {
            Assert.AreEqual((uint?)2368, _games[4].BlackElo);
        }

        // =====================================================================
        // Result Conversion Coverage
        // =====================================================================

        [TestMethod]
        public void TestAllThreeResultTypesPresent()
        {
            // Verify the parser correctly handles all three result types
            // across the full file
            bool hasWhiteWin = false;
            bool hasBlackWin = false;
            bool hasDraw = false;

            foreach (var game in _games)
            {
                if (game.Result == 'W') hasWhiteWin = true;
                if (game.Result == 'B') hasBlackWin = true;
                if (game.Result == 'D') hasDraw = true;
            }

            Assert.IsTrue(hasWhiteWin, "No white wins found");
            Assert.IsTrue(hasBlackWin, "No black wins found");
            Assert.IsTrue(hasDraw, "No draws found");
        }

        // =====================================================================
        // Data Integrity
        // =====================================================================

        [TestMethod]
        public void TestAllGamesHaveNonEmptyMoves()
        {
            foreach (var game in _games)
                Assert.IsFalse(string.IsNullOrWhiteSpace(game.Moves),
                    $"Game with event '{game.EventName}' has empty moves");
        }

        [TestMethod]
        public void TestAllGamesHaveNonEmptyPlayerNames()
        {
            foreach (var game in _games)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(game.WhiteName),
                    $"Game in '{game.EventName}' has empty white player name");
                Assert.IsFalse(string.IsNullOrWhiteSpace(game.BlackName),
                    $"Game in '{game.EventName}' has empty black player name");
            }
        }

        [TestMethod]
        public void TestAllGamesHaveValidResult()
        {
            foreach (var game in _games)
                Assert.IsTrue(game.Result == 'W' || game.Result == 'B' || game.Result == 'D',
                    $"Game in '{game.EventName}' has invalid result '{game.Result}'");
        }

        [TestMethod]
        public void TestAllGamesHaveNonEmptyEventName()
        {
            foreach (var game in _games)
                Assert.IsFalse(string.IsNullOrWhiteSpace(game.EventName),
                    "A game has an empty event name");
        }
    }
}

