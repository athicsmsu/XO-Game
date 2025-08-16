using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class XOGameManager : MonoBehaviour
{
    public DBManager dbManager;
    public XOGridManager gridManager;

    public int boardSize = 3;
    public string player1 = "Player 1";
    public string player2 = "Player 2";
    public bool isBotGame = false; // true ถ้าเล่นกับ AI
    private int gameId;
    private int turnCount = 0;
    public string CurrentPlayer { get; private set; }
    public float replayDelay = 1f;
    private bool isGameOver = false;
    public bool IsGameOver => isGameOver;
    public Text StatusGamePlay;

    void Start()
    {
        isGameOver = false;
        isBotGame = PlayerPrefs.GetInt("IsBotGame", 0) == 1;

        // รับ Board Size จาก Home
        boardSize = Mathf.Clamp(PlayerPrefs.GetInt("BoardSize", 3), 3, 10);

        int replayGameId = PlayerPrefs.GetInt("ReplayGameId", -1);
        if (replayGameId != -1)
        {
            LoadGameForReplay(replayGameId);
        }
        else
        {
            StartNewGame();
        }
    }

    public void StartNewGame()
    {
        isGameOver = false;
        CurrentPlayer = player1;
        turnCount = 0;

        // รีเซ็ตข้อความ
        if (StatusGamePlay != null)
            StatusGamePlay.text = "";

        string player2Name = isBotGame ? "AI" : player2;
        gameId = dbManager.CreateGame(boardSize, 3, player1, player2Name);

        Debug.Log($"Game started: {player1} vs {player2Name} | Board {boardSize}x{boardSize}");

        // ปรับขนาด Grid และสร้างปุ่มใหม่
        gridManager.boardSize = boardSize;
        gridManager.AdjustGridLayout(boardSize);

        // ลบปุ่มเก่าและสร้างใหม่
        foreach (Transform child in gridManager.boardParent)
            GameObject.Destroy(child.gameObject);

        gridManager.CreateGrid();
    }

    public void PlayerMove(int x, int y)
    {
        turnCount++;
        dbManager.InsertMove(gameId, turnCount, CurrentPlayer, x, y);

        if (CheckWin(CurrentPlayer, x, y)) return;

        if (turnCount == boardSize * boardSize)
        {
            EndGame("Draw");
            return;
        }

        CurrentPlayer = (CurrentPlayer == player1) ? player2 : player1;

        if (isBotGame && CurrentPlayer == player2)
            StartCoroutine(BotMove());
    }


    // ตัวอย่าง Bot เดินง่ายๆ เลือกช่องว่างแบบสุ่ม
    private IEnumerator BotMove()
    {
        yield return new WaitForSeconds(0.5f); // ดีเลย์เหมือน Bot คิด

        List<(int x, int y)> emptyCells = new List<(int x, int y)>();
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                if (gridManager.GetCellSprite(x, y) == null)
                    emptyCells.Add((x, y));
            }
        }

        if (emptyCells.Count > 0)
        {
            var choice = emptyCells[UnityEngine.Random.Range(0, emptyCells.Count)];
            gridManager.OnClickCell(choice.x, choice.y, true);
        }
    }

    public void EndGame(string winner)
    {
        isGameOver = true;
        dbManager.SetWinner(gameId, winner);

        if (StatusGamePlay != null)
        {
            if (winner == "Draw")
            {
                StatusGamePlay.text = "Draw!";
            }
            else
            {
                string displayWinner = winner;
                if (isBotGame && winner == player2)
                    displayWinner = "AI";  // แสดง AI แทน Player 2
                StatusGamePlay.text = displayWinner + " Win!";
            }
        }

        Debug.Log("Winner is " + winner);
    }



    public void ReplayGame()
    {
        List<Move> moves = dbManager.GetMovesForReplay(gameId);
        gridManager.StartReplay(moves, replayDelay, gridManager.xSprite, gridManager.oSprite);
    }

    public void LoadGameForReplay(int gameId)
    {
        this.gameId = gameId;

        // 1️⃣ ดึงข้อมูลเกมจาก DB
        Game gameData = dbManager.FindGame(gameId); // ต้องมีฟังก์ชัน FindGame ใน DBManager
        if (gameData != null)
        {
            boardSize = gameData.boardSize; // ใช้ boardSize ของเกมจริง
            gridManager.boardSize = boardSize;

            // 2️⃣ ปรับ Grid Layout และสร้าง Grid ใหม่
            gridManager.AdjustGridLayout(boardSize);

            // ลบปุ่มเก่าและสร้างใหม่
            foreach (Transform child in gridManager.boardParent)
                GameObject.Destroy(child.gameObject);
            gridManager.CreateGrid();
        }

        // 3️⃣ ดึง Moves แล้วเริ่ม Replay
        List<Move> moves = dbManager.GetMovesForReplay(gameId);
        gridManager.StartReplay(moves, replayDelay, gridManager.xSprite, gridManager.oSprite);

        // reset ค่าเพื่อไม่ให้สร้างปัญหา
        PlayerPrefs.SetInt("ReplayGameId", -1);
    }


    public bool CheckWin(string player, int lastX, int lastY)
    {
        int winLength;

        // กำหนดจำนวนช่องชนะตาม BoardSize
        if (boardSize <= 3)
            winLength = 3;
        else if (boardSize == 4)
            winLength = 3;
        else if (boardSize == 5)
            winLength = 4;
        else // boardSize >= 6
            winLength = 5;

        var dirs = new Vector2Int[] {
        new Vector2Int(1, 0),   // แนวนอน →
        new Vector2Int(0, 1),   // แนวตั้ง ↑
        new Vector2Int(1, 1),   // เฉียง ↗
        new Vector2Int(1,-1),   // เฉียง ↘
    };

        foreach (var d in dirs)
        {
            int lenPos = CountInDirection(player, lastX, lastY, d.x, d.y);
            int lenNeg = CountInDirection(player, lastX, lastY, -d.x, -d.y);
            int count = 1 + lenPos + lenNeg;

            if (count >= winLength)
            {
                Vector2Int start = new Vector2Int(lastX - d.x * lenNeg, lastY - d.y * lenNeg);
                Vector2Int end = new Vector2Int(lastX + d.x * lenPos, lastY + d.y * lenPos);

                gridManager.MarkWinningCells(start, end);
                EndGame(player);
                return true;
            }
        }
        return false;
    }


    private int CountInDirection(string player, int startX, int startY, int dx, int dy)
    {
        int count = 0;
        int x = startX + dx;
        int y = startY + dy;

        while (x >= 0 && y >= 0 && x < boardSize && y < boardSize)
        {
            var sprite = gridManager.GetCellSprite(x, y);
            if (sprite == null) break;

            bool isMatch = (player == player1 && sprite == gridManager.xSprite) ||
                           (player == player2 && sprite == gridManager.oSprite);

            if (!isMatch) break;

            count++;
            x += dx;
            y += dy;
        }
        return count;
    }
}
