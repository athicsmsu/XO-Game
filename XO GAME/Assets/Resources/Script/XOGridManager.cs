using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class XOGridManager : MonoBehaviour
{
    public XOGameManager gameManager;
    public GameObject buttonPrefab;
    public Transform boardParent; // Grid Layout Group
    public int boardSize = 3;
    public Sprite xSprite;
    public Sprite oSprite;

    private Button[,] buttons;
    private Coroutine replayCoroutine; // เก็บ Coroutine ปัจจุบัน
    public GridLayoutGroup gridLayout;
    public Sprite winSprite; // ลาก Star Sprite เข้ามา

    void Start()
    {
        AdjustGridLayout(boardSize);
        CreateGrid();
    }

    public void CreateGrid()
    {
        buttons = new Button[boardSize, boardSize];

        // ทำลูปสร้างปุ่มตาม boardSize ปัจจุบัน
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                GameObject btnObj = Instantiate(buttonPrefab, boardParent);
                Button btn = btnObj.GetComponent<Button>();
                int cx = x;
                int cy = y;
                btn.onClick.AddListener(() => OnClickCell(cx, cy));
                buttons[x, y] = btn;
            }
        }
        ClearBoard();
    }
    

    public void AdjustGridLayout(int boardSize)
    {
        if (gridLayout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;

        gridLayout.constraintCount = boardSize;

        // ปรับ Cell Size ให้เหมาะกับ Panel
        RectTransform rt = gridLayout.GetComponent<RectTransform>();
        float cellSize = rt.rect.width / boardSize;
        gridLayout.cellSize = new Vector2(cellSize, cellSize);
    }


    // เมื่อคลิกที่เซลล์
    public void OnClickCell(int x, int y, bool isFromBot = false)
    {
        if (!isFromBot && gameManager.isBotGame && gameManager.CurrentPlayer != gameManager.player1 || gameManager.IsGameOver)
            return;

        Image img = buttons[x, y].GetComponent<Image>();
        if (img.sprite != null) return;

        string playerWhoMoved = gameManager.CurrentPlayer;

        // 1️⃣ เซ็ต sprite ของผู้เล่นก่อน
        img.sprite = (playerWhoMoved == gameManager.player1) ? xSprite : oSprite;

        // 2️⃣ เรียก Move ของ GameManager (ซึ่งจะตรวจ win)
        gameManager.PlayerMove(x, y);
    }



    // สำหรับ Replay
    public Sprite GetCellSprite(int x, int y)
    {
        return buttons[x, y].GetComponent<Image>().sprite;
    }

    // รีเซ็ตบอร์ดก่อน Replay
    public void ClearBoard()
    {
        for (int y = 0; y < boardSize; y++)
            for (int x = 0; x < boardSize; x++)
                buttons[x, y].GetComponent<Image>().sprite = null;
    }


    // เล่น Replay
    public IEnumerator ReplayMoves(List<Move> moves, float delay, Sprite xSpr, Sprite oSpr)
    {
        ClearBoard();

        foreach (var move in moves)
        {
            int x = move.x;
            int y = move.y;
            Image img = buttons[x, y].GetComponent<Image>();
            img.sprite = (move.player == gameManager.player1) ? xSpr : oSpr;

            yield return new WaitForSeconds(delay);
        }

        replayCoroutine = null; // จบ Coroutine
    }
    // ฟังก์ชันเรียก Replay ใหม่
    public void StartReplay(List<Move> moves, float delay, Sprite xSpr, Sprite oSpr)
    {
        // ถ้ามี Replay เก่ากำลังทำงาน ให้หยุดก่อน
        if (replayCoroutine != null)
            StopCoroutine(replayCoroutine);

        // รีเซ็ตบอร์ด
        ClearBoard();

        // เริ่ม Replay ใหม่
        replayCoroutine = StartCoroutine(ReplayMoves(moves, delay, xSpr, oSpr));
    }

    public void MarkWinningCells(Vector2Int start, Vector2Int end)
    {
        int dx = end.x - start.x;
        int dy = end.y - start.y;

        int steps = Math.Max(Math.Abs(dx), Math.Abs(dy)) + 1; // จำนวนช่องรวม start + end
        dx = (dx == 0) ? 0 : dx / Math.Abs(dx);  // normalize ให้เป็น -1, 0, 1
        dy = (dy == 0) ? 0 : dy / Math.Abs(dy);

        for (int i = 0; i < steps; i++)
        {
            int x = start.x + i * dx;
            int y = start.y + i * dy;
            buttons[x, y].GetComponent<Image>().sprite = winSprite;
        }
    }

}
