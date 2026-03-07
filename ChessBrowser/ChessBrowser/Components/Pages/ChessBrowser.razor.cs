using Microsoft.AspNetCore.Components.Forms;
using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;


namespace ChessBrowser.Components.Pages
{
  public partial class ChessBrowser
  {
    /// <summary>
    /// Bound to the Unsername form input
    /// </summary>
    private string Username = "";

    /// <summary>
    /// Bound to the Password form input
    /// </summary>
    private string Password = "";

    /// <summary>
    /// Bound to the Database form input
    /// </summary>
    private string Database = "";

    /// <summary>
    /// Represents the progress percentage of the current
    /// upload operation. Update this value to update 
    /// the progress bar.
    /// </summary>
    private int    Progress = 0;



    /// <summary>
    /// This method runs when a PGN file is selected for upload.
    /// Given a list of lines from the selected file, parses the 
    /// PGN data, and uploads each chess game to the user's database.
    /// </summary>
    /// <param name="PGNFileLines">The lines from the selected file</param>
    private async Task InsertGameData(string[] PGNFileLines)
    {
      // This will build a connection string to your user's database on atr,
      // assuimg you've filled in the credentials in the GUI
      string connection = GetConnectionString();

      // Parse all games from the PGN file
      List<ChessGame> games = PgnParser.Parse(PGNFileLines);

        // Track unique players and events to avoid duplicate inserts.
        var players = new Dictionary<string, uint>();   // name  → pID
        var events = new Dictionary<string, uint>();   // key   → eID

        using (MySqlConnection conn = new MySqlConnection(connection))
          {
                try
                {
                    // Open a connection
                    conn.Open();

                    for (int i = 0; i < games.Count; i++)
                    {
                        ChessGame game = games[i];

                        uint whiteID;
                        uint blackID;
                        uint eventID;

                        //WHITE PLAYER
                        if (!players.TryGetValue(game.WhiteName, out whiteID))
                        {
                            var cmd = new MySqlCommand(
                                "SELECT pID, Elo FROM Players WHERE Name=@name", conn);
                            cmd.Parameters.AddWithValue("@name", game.WhiteName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    whiteID = reader.GetUInt32("pID");
                                    uint? currentElo = reader.IsDBNull("Elo") ? null : reader.GetUInt32("Elo");
                                    reader.Close();

                                    if (game.WhiteElo.HasValue && (!currentElo.HasValue || game.WhiteElo > currentElo))
                                    {
                                        var update = new MySqlCommand(
                                            "UPDATE Players SET Elo=@elo WHERE pID=@pid", conn);
                                        update.Parameters.AddWithValue("@elo", game.WhiteElo);
                                        update.Parameters.AddWithValue("@pid", whiteID);
                                        update.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    reader.Close();
                                    var insert = new MySqlCommand(
                                        "INSERT INTO Players(Name,Elo) VALUES(@name,@elo); SELECT LAST_INSERT_ID();",
                                        conn);

                                    insert.Parameters.AddWithValue("@name", game.WhiteName);
                                    insert.Parameters.AddWithValue("@elo", game.WhiteElo);

                                    whiteID = Convert.ToUInt32(insert.ExecuteScalar());
                                }
                            }

                            players[game.WhiteName] = whiteID;

                        }
                        //BLACK PLAYER
                        if (!players.TryGetValue(game.BlackName, out blackID))
                        {
                            var cmd = new MySqlCommand(
                                "SELECT pID, Elo FROM Players WHERE Name=@name", conn);
                            cmd.Parameters.AddWithValue("@name", game.BlackName);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    blackID = reader.GetUInt32("pID");
                                    uint? currentElo = reader.IsDBNull("Elo") ? null : reader.GetUInt32("Elo");
                                    reader.Close();

                                    if (game.BlackElo.HasValue && (!currentElo.HasValue || game.BlackElo > currentElo))
                                    {
                                        var update = new MySqlCommand(
                                            "UPDATE Players SET Elo=@elo WHERE pID=@pid", conn);
                                        update.Parameters.AddWithValue("@elo", game.BlackElo);
                                        update.Parameters.AddWithValue("@pid", blackID);
                                        update.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    reader.Close();
                                    var insert = new MySqlCommand(
                                        "INSERT INTO Players(Name,Elo) VALUES(@name,@elo); SELECT LAST_INSERT_ID();",
                                        conn);

                                    insert.Parameters.AddWithValue("@name", game.BlackName);
                                    insert.Parameters.AddWithValue("@elo", game.BlackElo);

                                    blackID = Convert.ToUInt32(insert.ExecuteScalar());
                                }
                            }

                            players[game.BlackName] = blackID;
                        }

                        //EVENT
                        string eventKey = $"{game.EventName}|{game.Site}|{game.EventDate}";

                        if (!events.TryGetValue(eventKey, out eventID))
                        {
                            var cmd = new MySqlCommand(
                                "SELECT eID FROM Events WHERE Name=@name AND Site=@site AND Date=@date",
                                conn);

                            cmd.Parameters.AddWithValue("@name", game.EventName);
                            cmd.Parameters.AddWithValue("@site", game.Site);
                            cmd.Parameters.AddWithValue("@date", game.EventDate);

                            var result = cmd.ExecuteScalar();

                            if (result != null)
                            {
                                eventID = Convert.ToUInt32(result);
                            }
                            else
                            {
                                var insert = new MySqlCommand(
                                    "INSERT INTO Events(Name,Site,Date) VALUES(@name,@site,@date); SELECT LAST_INSERT_ID();",
                                    conn);

                                insert.Parameters.AddWithValue("@name", game.EventName);
                                insert.Parameters.AddWithValue("@site", game.Site);
                                insert.Parameters.AddWithValue("@date", game.EventDate);

                                eventID = Convert.ToUInt32(insert.ExecuteScalar());
                            }

                            events[eventKey] = eventID;
                        }

                        //INSERT GAME
                        var insertGame = new MySqlCommand(
                            @"INSERT INTO Games(Round, Result, Moves, BlackPlayer, WhitePlayer, eID)
                            VALUES (@round,@result,@moves,@black,@white,@eid)", conn);

                        insertGame.Parameters.AddWithValue("@round", game.Round);
                        insertGame.Parameters.AddWithValue("@result", game.Result);
                        insertGame.Parameters.AddWithValue("@moves", game.Moves);
                        insertGame.Parameters.AddWithValue("@black", blackID);
                        insertGame.Parameters.AddWithValue("@white", whiteID);
                        insertGame.Parameters.AddWithValue("@eid", eventID);

                        insertGame.ExecuteNonQuery();

                        //UPDATE PROGRESS
                        Progress = (int)(((double)(i + 1) / games.Count) * 100);
                        await InvokeAsync(StateHasChanged);
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
          }

        }


    /// <summary>
    /// Queries the database for games that match all the given filters.
    /// The filters are taken from the various controls in the GUI.
    /// </summary>
    /// <param name="white">The white player, or "" if none</param>
    /// <param name="black">The black player, or "" if none</param>
    /// <param name="opening">The first move, e.g. "1.e4", or "" if none</param>
    /// <param name="winner">The winner as "W", "B", "D", or "" if none</param>
    /// <param name="useDate">true if the filter includes a date range, false otherwise</param>
    /// <param name="start">The start of the date range</param>
    /// <param name="end">The end of the date range</param>
    /// <param name="showMoves">true if the returned data should include the PGN moves</param>
    /// <returns>A string separated by newlines containing the filtered games</returns>
    private string PerformQuery(string white, string black, string opening,
      string winner, bool useDate, DateTime start, DateTime end, bool showMoves)
    {
      // This will build a connection string to your user's database on atr,
      // assuimg you've typed a user and password in the GUI
      string connection = GetConnectionString();

      // Build up this string containing the results from your query
      string parsedResult = "";

      // Use this to count the number of rows returned by your query
      // (see below return statement)
      int numRows = 0;

      using (MySqlConnection conn = new MySqlConnection(connection))
      {
        try
        {
            conn.Open();

            string sql = @"
                SELECT E.Name, E.Site, E.Date, 
                       PW.Name AS WhiteName, PW.Elo AS WhiteElo,
                       PB.Name AS BlackName, PB.Elo AS BlackElo,
                       G.Round, G.Result, G.Moves
                FROM Games G
                JOIN Events E  ON G.eID         = E.eID
                JOIN Players PW ON G.WhitePlayer = PW.pID
                JOIN Players PB ON G.BlackPlayer = PB.pID
                WHERE (@white   = '' OR PW.Name   = @white)
                AND   (@black   = '' OR PB.Name   = @black)
                AND   (@winner  = '' OR G.Result  = @winner)
                AND   (@opening = '' OR G.Moves   LIKE @openingLike)"
                + (useDate ? " AND (E.Date BETWEEN @start AND @end)" : "");

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@white",       white);
                cmd.Parameters.AddWithValue("@black",       black);
                cmd.Parameters.AddWithValue("@winner",      winner);
                cmd.Parameters.AddWithValue("@opening",     opening);
                cmd.Parameters.AddWithValue("@openingLike", opening + "%");
                if (useDate)
                {
                    cmd.Parameters.AddWithValue("@start", start.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@end",   end.ToString("yyyy-MM-dd"));
                }

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        numRows++;

                        string eventName = reader.GetString("Name");
                        string site      = reader.GetString("Site");
                        string whiteName = reader.GetString("WhiteName");
                        string blackName = reader.GetString("BlackName");
                        string round     = reader.GetString("Round");
                        string result    = reader.GetString("Result");
                        string whiteElo  = reader.IsDBNull(reader.GetOrdinal("WhiteElo"))
                                           ? "?" : reader.GetUInt32("WhiteElo").ToString();
                        string blackElo  = reader.IsDBNull(reader.GetOrdinal("BlackElo"))
                                           ? "?" : reader.GetUInt32("BlackElo").ToString();

                        string dateStr;
                        try   { dateStr = reader.GetDateTime("Date").ToString("yyyy-MM-dd"); }
                        catch { dateStr = "0000-00-00"; }

                        parsedResult +=
                            $"Event: {eventName}, Site: {site}, Date: {dateStr}\n" +
                            $"White: {whiteName} ({whiteElo}), Black: {blackName} ({blackElo})\n" +
                            $"Round: {round}, Result: {result}\n";

                        if (showMoves)
                            parsedResult += reader.GetString("Moves") + "\n";

                        parsedResult += "\n";
                    }
                }
            }
        }
        catch (Exception e)
        {
          System.Diagnostics.Debug.WriteLine(e.Message);
        }
      }

      return numRows + " results\n" + parsedResult;
    }


    private string GetConnectionString()
    {
      return "server=atr.eng.utah.edu;database=" + Database + ";uid=" + Username + ";password=" + Password;
    }


    /// <summary>
    /// This method will run when the file chooser is used.
    /// It loads the files contents as an array of strings,
    /// then invokes the InsertGameData method.
    /// </summary>
    /// <param name="args">The event arguments, which contains the selected file name</param>
    private async void HandleFileChooser(EventArgs args)
    {
      try
      {
        string fileContent = string.Empty;

        InputFileChangeEventArgs eventArgs = args as InputFileChangeEventArgs ?? throw new Exception("unable to get file name");
        if (eventArgs.FileCount == 1)
        {
          var file = eventArgs.File;
          if (file is null)
          {
            return;
          }

          // load the chosen file and split it into an array of strings, one per line
          using var stream = file.OpenReadStream(1000000); // max 1MB
          using var reader = new StreamReader(stream);                   
          fileContent = await reader.ReadToEndAsync();
          string[] fileLines = fileContent.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

          // insert the games, and don't wait for it to finish
          // _ = throws away the task result, since we aren't waiting for it
          _ = InsertGameData(fileLines);
        }
      }
      catch (Exception e)
      {
        Debug.WriteLine("an error occurred while loading the file..." + e);
      }
    }


     /// <summary>
     /// Parses a PGN file's lines into a list of ChessGame objects.
      /// This method exists solely for unit testing the parsing logic,
        /// independent of any database operations.
        /// </summary>
        /// <param name="PGNFileLines">The lines from a PGN file.</param>
        /// <returns>A list of parsed ChessGame objects.</returns>
        public List<ChessGame> ParsePGNFile(string[] PGNFileLines)
        {
            return PgnParser.Parse(PGNFileLines);
        }
    }

}
