using System;

// =============================================================================
// File:        PgnParser.cs
// Project:     Team3Chess LMS
// Description: Static utility class for parsing PGN-formatted chess game files
//              into a list of ChessGame objects for database insertion.
// Author:      [Your Name]
// Created:     2026-03-05
// =============================================================================

using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Provides static methods for parsing PGN (Portable Game Notation) files
/// into strongly-typed <see cref="ChessGame"/> objects.
/// </summary>
public static class PgnParser
{
    // Matches a PGN tag of the form: [TagName "Tag Value"]
    private static readonly Regex TagRegex = new Regex(@"^\[(\w+)\s+""(.*)""\]$", RegexOptions.Compiled);

    /// <summary>
    /// Parses an array of lines from a PGN file and returns a list of
    /// <see cref="ChessGame"/> objects representing each game found.
    /// </summary>
    /// <param name="lines">All lines read from a PGN file.</param>
    /// <returns>A list of parsed <see cref="ChessGame"/> instances.</returns>
    public static List<ChessGame> Parse(string[] lines)
    {
        var games = new List<ChessGame>();
        int i = 0;

        while (i < lines.Length)
        {
            // Skip any  blank lines between games
            while (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
                i++;

            if (i >= lines.Length) break;

            // --- Parse tag section ---
            var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
            {
                string line = lines[i].Trim();
                Match match = TagRegex.Match(line);
                if (match.Success)
                    tags[match.Groups[1].Value] = match.Groups[2].Value;
                i++;
            }

            // Skip blank line between tags and moves
            while (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
                i++;

            // --- Parse moves section ---
            var movesBuilder = new StringBuilder();

            while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]))
            {
                movesBuilder.AppendLine(lines[i].Trim());
                i++;
            }

            // Only create a game if we got meaningful tag data
            if (tags.Count > 0)
                games.Add(BuildChessGame(tags, movesBuilder.ToString().Trim()));
        }

        return games;
    }

    /// <summary>
    /// Constructs a <see cref="ChessGame"/> from a dictionary of PGN tags and move text.
    /// </summary>
    /// <param name="tags">Key-value pairs of PGN tag names to their values.</param>
    /// <param name="moves">The raw move sequence text extracted from the PGN.</param>
    /// <returns>A populated <see cref="ChessGame"/> instance.</returns>
    private static ChessGame BuildChessGame(Dictionary<string, string> tags, string moves)
    {
        return new ChessGame
        {
            EventName = GetTag(tags, "Event"),
            Site = GetTag(tags, "Site"),
            Round = GetTag(tags, "Round"),
            WhiteName = GetTag(tags, "White"),
            BlackName = GetTag(tags, "Black"),
            WhiteElo = ParseElo(GetTag(tags, "WhiteElo")),
            BlackElo = ParseElo(GetTag(tags, "BlackElo")),
            Result = ParseResult(GetTag(tags, "Result")),
            EventDate = ParseDate(GetTag(tags, "EventDate")),
            Moves = moves
        };
    }

    /// <summary>
    /// Safely retrieves a tag value by name, returning an empty string if not found.
    /// </summary>
    private static string GetTag(Dictionary<string, string> tags, string key)
        => tags.TryGetValue(key, out string? val) ? val : string.Empty;

    /// <summary>
    /// Converts a PGN result string to a single character result code.
    /// '1-0' → 'W', '0-1' → 'B', '1/2-1/2' → 'D'.
    /// Defaults to 'D' for unrecognized values.
    /// </summary>
    private static char ParseResult(string result) => result.Trim() switch
    {
        "1-0" => 'W',
        "0-1" => 'B',
        "1/2-1/2" => 'D',
        _ => 'D'
    };

    /// <summary>
    /// Parses a PGN date string (e.g. "2018.01.23") into a MySQL-compatible
    /// date string (e.g. "2018-01-23"). Returns "0000-00-00" for malformed dates.
    /// Converts PGN date to MySQL format or "0000-00-00" for dirty/missing dates
    /// </summary>
    // 
    private static string ParseDate(string date)
    {
        if (string.IsNullOrWhiteSpace(date))
            return "0000-00-00";

        string[] parts = date.Split('.');
        if (parts.Length == 3)
        {
            // Use 00 for unknown month/day
            string year = string.IsNullOrWhiteSpace(parts[0]) ? "0000" : parts[0];
            string month = string.IsNullOrWhiteSpace(parts[1]) || parts[1] == "??" ? "00" : parts[1];
            string day = string.IsNullOrWhiteSpace(parts[2]) || parts[2] == "??" ? "00" : parts[2];

            return $"{year}-{month}-{day}";
        }

        return "0000-00-00";
    }

    /// <summary>
    /// Attempts to parse an Elo rating string to an unsigned integer.
    /// Returns null if the value is missing, '?', or non-numeric.
    /// </summary>
    private static uint? ParseElo(string elo)
    {
        if (string.IsNullOrWhiteSpace(elo) || elo == "?") return null;
        return uint.TryParse(elo, out uint val) ? val : null;
    }
}
