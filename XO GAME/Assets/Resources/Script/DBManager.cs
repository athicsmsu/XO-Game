using UnityEngine;
using System.IO;
using SQLite4Unity3d;
using System;
using System.Collections.Generic;
using System.Linq;

public class DBManager : MonoBehaviour
{
    private string dbPath;
    private SQLiteConnection db;

    void Awake()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        dbPath = Path.Combine(Application.persistentDataPath, "xo_game.db");
#else
        dbPath = Path.Combine(Application.persistentDataPath, "xo_game.db");
#endif
        db = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

        // สร้างตารางอัตโนมัติ
        db.CreateTable<Game>();
        db.CreateTable<Move>();
    }

    // สร้างเกมใหม่
    public int CreateGame(int boardSize, int winLength, string player1, string player2)
    {
        var game = new Game
        {
            board_size = boardSize,
            win_length = winLength,
            player1 = player1,
            player2 = player2,
            winner = "",
            played_at = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        db.Insert(game);
        return game.game_id;
    }

    // เพิ่มการเดินแต่ละตา
    public void InsertMove(int gameId, int turn, string player, int x, int y)
    {
        var move = new Move
        {
            game_id = gameId,
            turn = turn,
            player = player,
            x = x,
            y = y
        };
        db.Insert(move);
    }

    // อัปเดตผู้ชนะ
    public void SetWinner(int gameId, string winner)
    {
        var game = db.Find<Game>(gameId);
        if (game != null)
        {
            game.winner = winner;
            db.Update(game);
        }
    }

    // ดึง Moves สำหรับ Replay เรียงตาม turn
    public List<Move> GetMovesForReplay(int gameId)
    {
        return db.Table<Move>()
                 .Where(m => m.game_id == gameId)
                 .OrderBy(m => m.turn)
                 .ToList(); // ต้องมี using System.Linq;
    }
    public List<Game> GetAllGames()
    {
        return db.Table<Game>()
                 .OrderByDescending(g => g.played_at)
                 .ToList();
    }
    public void ClearAllGames()
    {
        db.DeleteAll<Move>();
        db.DeleteAll<Game>();

        // รีเซ็ต auto-increment ของ Game
        db.Execute("DELETE FROM sqlite_sequence WHERE name='Game';");
    }

    public Game FindGame(int gameId)
    {
        return db.Find<Game>(gameId); // SQLite4Unity3d
    }
}

// Model Classes
public class Game
{
    [PrimaryKey, AutoIncrement]
    public int game_id { get; set; }
    public int board_size { get; set; }
    public int win_length { get; set; }
    public string player1 { get; set; }
    public string player2 { get; set; }
    public string winner { get; set; }
    public string played_at { get; set; }

    // helper
    public int boardSize => board_size;
    public int winLength => win_length;
}

public class Move
{
    [PrimaryKey, AutoIncrement]
    public int move_id { get; set; }
    public int game_id { get; set; }
    public int turn { get; set; }
    public string player { get; set; }
    public int x { get; set; }
    public int y { get; set; }
}
