// =============================================================================
// File:        Game.cs
// Project:     Team3Chess LMS
// Description: Model class representing a chess game played between two players
//              at a specific event. Maps to the Games table in the Team3Chess
//              database.
// Author:      Aliou Tippett and Owen Okolowitz
// Created:     2026-03-05
// =============================================================================

using System;


/// <summary>
/// Represents a chess game played between two players at a registered event.
/// Maps to the <c>Games</c> table in the Team3Chess database.
/// </summary>
/// <remarks>
/// This table uses a composite primary key consisting of
/// <see cref="Round"/>, <see cref="BlackPlayer"/>, <see cref="WhitePlayer"/>,
/// and <see cref="EId"/>.
/// </remarks>
public class Game
{
    /// <summary>
    /// The round in which the game was played. Part of the composite primary key.
    /// Required. Maximum length of 10 characters.
    /// </summary>
    public string Round { get; set; } = string.Empty;

    /// <summary>
    /// The result of the game.
    /// Required. Single character: typically '1' (White wins),
    /// '0' (Black wins), or 'd' (draw).
    /// </summary>
    public char Result { get; set; }

    /// <summary>
    /// The full move sequence of the game, stored in standard chess notation.
    /// Required. Maximum length of 2000 characters.
    /// </summary>
    public string Moves { get; set; } = string.Empty;

    // -------------------------------------------------------------------------
    // Foreign Keys
    // -------------------------------------------------------------------------

    /// <summary>
    /// Foreign key referencing <see cref="Player.PId"/>.
    /// Identifies the player controlling the black pieces.
    /// Part of the composite primary key.
    /// </summary>
    public uint BlackPlayer { get; set; }

    /// <summary>
    /// Foreign key referencing <see cref="Player.PId"/>.
    /// Identifies the player controlling the white pieces.
    /// Part of the composite primary key.
    /// </summary>
    public uint WhitePlayer { get; set; }

    /// <summary>
    /// Foreign key referencing <see cref="Event.EId"/>.
    /// Identifies the event at which this game was played.
    /// Part of the composite primary key.
    /// </summary>
    public uint EId { get; set; }

    // -------------------------------------------------------------------------
    // Navigation Properties
    // -------------------------------------------------------------------------

    /// <summary>
    /// Navigation property to the <see cref="Player"/> playing black.
    /// </summary>
    public Player? BlackPlayerNavigation { get; set; }

    /// <summary>
    /// Navigation property to the <see cref="Player"/> playing white.
    /// </summary>
    public Player? WhitePlayerNavigation { get; set; }

    /// <summary>
    /// Navigation property to the <see cref="Event"/> this game belongs to.
    /// </summary>
    public Event? EventNavigation { get; set; }
}
