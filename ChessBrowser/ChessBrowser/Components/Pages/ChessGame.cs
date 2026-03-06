using System;

// =============================================================================
// File:        ChessGame.cs
// Project:     Team3Chess LMS
// Description: Data model representing a single chess game parsed from a PGN
//              file, including player info, event metadata, and move sequence.
// Author:      [Your Name]
// Created:     2026-03-05
// =============================================================================

/// <summary>
/// Represents a single chess game parsed from a PGN file.
/// Stores all relevant metadata and move data needed for database insertion.
/// </summary>
public class ChessGame
{
    /// <summary>The name of the event at which the game was played.</summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>The site where the event took place.</summary>
    public string Site { get; set; } = string.Empty;

    /// <summary>The round in which the game was played (e.g. "1", "2.1", "1.4.10").</summary>
    public string Round { get; set; } = string.Empty;

    /// <summary>The name of the player controlling the white pieces.</summary>
    public string WhiteName { get; set; } = string.Empty;

    /// <summary>The name of the player controlling the black pieces.</summary>
    public string BlackName { get; set; } = string.Empty;

    /// <summary>
    /// The Elo rating of the white player at the time of the game.
    /// Null if not provided or unparseable.
    /// </summary>
    public uint? WhiteElo { get; set; }

    /// <summary>
    /// The Elo rating of the black player at the time of the game.
    /// Null if not provided or unparseable.
    /// </summary>
    public uint? BlackElo { get; set; }

    /// <summary>
    /// The result of the game as a single character.
    /// 'W' = white wins, 'B' = black wins, 'D' = draw.
    /// </summary>
    public char Result { get; set; }

    /// <summary>
    /// The date of the event. May be a default/zero date if the PGN value was malformed.
    /// </summary>
    public string EventDate { get; set; } = "0000-00-00";

    /// <summary>
    /// The full move sequence of the game in standard PGN notation.
    /// Stored verbatim from the PGN file.
    /// </summary>
    public string Moves { get; set; } = string.Empty;
}
