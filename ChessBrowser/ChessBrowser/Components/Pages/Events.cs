// =============================================================================
// File:        Event.cs
// Project:     Team3Chess LMS
// Description: Model class representing a chess event or tournament.
//              Maps to the Events table in the Team3Chess database.
// Author:      Aliou Tippett
// Created:     2026-03-05
// =============================================================================

using System;

/// <summary>
/// Represents a chess event or tournament within the LMS system.
/// Maps to the <c>Events</c> table in the Team3Chess database.
/// </summary>
public class Event
{
    /// <summary>
    /// Primary key. Uniquely identifies each event.
    /// Auto-incremented by the database on insert.
    /// </summary>
    public uint EId { get; set; }

    /// <summary>
    /// The name of the event or tournament.
    /// Required. Maximum length of 255 characters.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The physical or virtual location where the event is held.
    /// Required. Maximum length of 255 characters.
    /// </summary>
    public string Site { get; set; } = string.Empty;

    /// <summary>
    /// The date on which the event takes place.
    /// Stores date only — no time component.
    /// Required.
    /// </summary>
    public DateOnly Date { get; set; }
}
