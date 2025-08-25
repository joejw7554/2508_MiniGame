using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class GameOverUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] Button restartButton;

    private void Awake()
    {
        if(gameOverText==null)
        {
            gameOverText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (restartButton == null)
        {
            restartButton = GetComponentInChildren<Button>();
            restartButton.onClick.AddListener(OnRestartClicked);
        }

    }

    public void ShowGameOverUI()
    {
        gameObject.SetActive(true);
    }

    public void HideGameOverUI()
    {
        gameObject.SetActive(false);
    }

    private void OnRestartClicked()
    {
        HideGameOverUI();
        GameManager.Instance.RestartGame();
    }

    private void OnDestroy()
    {
        if(restartButton)
        {
            restartButton.onClick.RemoveListener(OnRestartClicked);
        }
    }

}
