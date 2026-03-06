// =============================================================================
// File:        Player.cs
// Project:     Team3Chess LMS
// Description: Model class representing a chess player registered in the system.
//              Maps to the Players table in the Team3Chess database.
// Author:      Aliou Tippett (u1415075)
// Created:     2026-03-05
// =============================================================================

using System;

/// <summary>
/// Represents a chess player registered within the LMS system.
/// Maps to the <c>Players</c> table in the Team3Chess database.
/// </summary>
public class Player
{
    /// <summary>
    /// Primary key. Uniquely identifies each player.
    /// Auto-incremented by the database on insert.
    /// </summary>
    public uint PId { get; set; }

    /// <summary>
    /// The full name of the player.
    /// Optional. Maximum length of 255 characters.
    /// Must be unique across all players when provided.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The player's Elo rating, representing their skill level.
    /// Optional. A higher value indicates a stronger player.
    /// </summary>
    public uint? Elo { get; set; }
}
