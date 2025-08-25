using UnityEngine;
using UnityEngine.UI;


public enum Level
{
    Easy,
    Medium,
    Hard
}

[System.Serializable]
public class DifficultySettings
{
    public Level level;
    public float spawnRate;
}

public class DifficultyManager : MonoBehaviour
{
    [Header("난이도 설정")]
    [SerializeField]
    private DifficultySettings[] difficultySettings = new DifficultySettings[]
    {
        new DifficultySettings { level = Level.Easy, spawnRate = 1f},
        new DifficultySettings { level = Level.Medium, spawnRate = 2f },
        new DifficultySettings { level = Level.Hard, spawnRate = 3f }
    };

    [Header("현재 난이도")]
    [SerializeField] private Level currentDifficulty;

    // 자신의 GameObject 참조
    private GameObject myGameObject;

    void Awake()
    {
        myGameObject = gameObject; // 자신의 GameObject 참조 저장
        
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if(canvas)
        {
            Transform mainMenu = canvas.transform.Find("MainMenu");
            
            if (mainMenu)
            {
                Transform easy = mainMenu.transform.Find("EasyButton");
                Transform normal = mainMenu.transform.Find("NormalButton");
                Transform hard = mainMenu.transform.Find("HardButton");



                var easyButton = easy?.GetComponent<Button>();
                var normalButton = normal?.GetComponent<Button>();
                var hardButton = hard?.GetComponent<Button>();

                easyButton?.onClick.AddListener(() => SetDifficulty(Level.Easy));
                normalButton?.onClick.AddListener(() => SetDifficulty(Level.Medium));
                hardButton?.onClick.AddListener(() => SetDifficulty(Level.Hard));
            }
        }
    }

    public void SetDifficulty(Level difficulty)
    {
        currentDifficulty = difficulty;

        var settings = GetDifficultySettings(currentDifficulty);
        if (settings != null)
        {
            GameManager.Instance.SetSpawnRate(settings.spawnRate);
            GameManager.Instance.StartGame();
            
            HideDifficultyMenu();
        }
    }

    public void HideDifficultyMenu()
    {
        if (myGameObject != null)
        {
            myGameObject.SetActive(false);
            Debug.Log("Difficulty Menu 숨김");
        }
    }

    public void ShowDifficultyMenu()
    {
        if (myGameObject != null)
        {
            myGameObject.SetActive(true);
            Debug.Log("Difficulty Menu 표시");
        }
    }

    private DifficultySettings GetDifficultySettings(Level difficulty)
    {
        foreach (var setting in difficultySettings)
        {
            if (setting.level == difficulty)
                return setting;
        }
        return null;
    }
}
