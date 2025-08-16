using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    public GameObject SoundOn;
    public GameObject SoundOff;
    public GameObject ExitGamePanel;
    [Header("Board Size UI")]
    public Text boardSizeText; // Text แสดงค่าขนาดบอร์ด
    public int selectedBoardSize = 3; // ค่าเริ่มต้น
    public GameObject BoardSizePanel;

    void Start()
    {
        if (ExitGamePanel != null) ExitGamePanel.SetActive(false);

        if (AudioListener.volume > 0)
        {
            SoundOn.SetActive(false);
            SoundOff.SetActive(true);
        }
        else
        {
            SoundOn.SetActive(true);
            SoundOff.SetActive(false);
        }
        BoardSizePanel.SetActive(false);
    }
    // ปุ่ม +
    public void SetBoardSizeUp()
    {
        selectedBoardSize = Mathf.Min(selectedBoardSize + 1, 10); // สูงสุด 10
        UpdateBoardSizeUI();
    }

    // ปุ่ม -
    public void SetBoardSizeDown()
    {
        selectedBoardSize = Mathf.Max(selectedBoardSize - 1, 3); // ต่ำสุด 3
        UpdateBoardSizeUI();
    }

    private void UpdateBoardSizeUI()
    {
        if (boardSizeText != null)
            boardSizeText.text = selectedBoardSize.ToString();
    }
    public void ShowBoardSizePanel()
    {
        if(BoardSizePanel.activeSelf)
        {
            BoardSizePanel.SetActive(false);
        }
        else
        {
            BoardSizePanel.SetActive(true);
        }
    }
    

    public void PlayWithAI()
    {
        PlayerPrefs.SetInt("IsBotGame", 1); // 1 = เล่นกับ Bot
        PlayerPrefs.SetInt("BoardSize", selectedBoardSize);
        SceneManager.LoadScene("GamePlay");
    }

    public void PlayWithFriend()
    {
        PlayerPrefs.SetInt("IsBotGame", 0);
        PlayerPrefs.SetInt("BoardSize", selectedBoardSize);
        SceneManager.LoadScene("GamePlay");
    }

    public void ShowExitGamePanel(bool isActive)
    {
        ExitGamePanel.SetActive(isActive);
    }
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game exited");
    }
    public void SoundToggle(bool isOn)
    {
        if (isOn)
        {
            SoundOn.SetActive(false);
            SoundOff.SetActive(true);
            AudioListener.volume = 1f; // เปิดเสียง
        }
        else
        {   SoundOn.SetActive(true);
            SoundOff.SetActive(false);
            AudioListener.volume = 0f; // ปิดเสียง
        }
    }
}
