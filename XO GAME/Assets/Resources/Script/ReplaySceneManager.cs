using UnityEngine;

public class ReplaySceneManager : MonoBehaviour
{
    public XOGameManager gameManager;

    void Start()
    {
        int gameId = PlayerPrefs.GetInt("ReplayGameId", -1);
        if (gameId != -1)
        {
            gameManager.LoadGameForReplay(gameId);
        }
    }
}
