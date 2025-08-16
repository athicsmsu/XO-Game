using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class HistoryManager : MonoBehaviour
{
    public DBManager dbManager;
    public Transform contentParent; // Panel สำหรับวางปุ่ม
    public GameObject buttonPrefab;
    public Button resetButton; // ปุ่ม Reset History
    public GameObject resetHistoryPanel; // Panel สำหรับยืนยันการรีเซ็ตประวัติ
    private List<GameObject> historyButtons = new List<GameObject>();

    void Start()
    {
        ShowHistory();

        // เพิ่ม Listener ให้ปุ่ม Reset
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetHistory);
        }
        // ซ่อน Panel ยืนยันการรีเซ็ตประวัติเมื่อเริ่มเกม
        if (resetHistoryPanel != null)
        {
            resetHistoryPanel.SetActive(false);
        }
    }

    void ShowHistory()
    {
        // เคลียร์ปุ่มเก่า
        foreach (var btn in historyButtons)
        {
            Destroy(btn);
        }
        historyButtons.Clear();

        List<Game> games = dbManager.GetAllGames();

        foreach (var game in games)
        {
            GameObject btnObj = Instantiate(buttonPrefab, contentParent);
            historyButtons.Add(btnObj);

            Button btn = btnObj.GetComponent<Button>();
            Text txt = btnObj.GetComponentInChildren<Text>();
            txt.text = $"Game {game.game_id} | {game.player1} vs {game.player2}";

            int gId = game.game_id;
            btn.onClick.AddListener(() => ReplaySelectedGame(gId));
        }
    }

    void ReplaySelectedGame(int gameId)
    {
        PlayerPrefs.SetInt("ReplayGameId", gameId);
        SceneManager.LoadScene("ReplayScene");
    }

    public void ShowResetHistoryPanel(bool isActive){
        // แสดงหรือซ่อน Panel ยืนยันการรีเซ็ตประวัติ
        resetHistoryPanel.SetActive(isActive);
    }

    void ResetHistory()
    {
        // ลบข้อมูลเกมทั้งหมดใน DB
        dbManager.ClearAllGames();

        // อัปเดต UI ใหม่
        ShowHistory();
        ShowResetHistoryPanel(false); // ซ่อน Panel ยืนยันการรีเซ็ตประวัติ
    }
}
