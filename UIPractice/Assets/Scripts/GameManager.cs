using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [Header("오브젝트 풀링")]
    [SerializeField]
    List<GameObject> prefabs;

    [SerializeField]
    float spawnRate = 3f;

    [Header("UI 요소")]
    TextMeshProUGUI scoreText;

    GameOverUI gameOverUI;

    [Header("score")]
    int score = 0;

    [Header("게임 상태")]
    [SerializeField]
    bool bIsNotGameOver = true;

    public bool IsNotGameOver { get { return bIsNotGameOver; }   }
    ObjectPool<GameObject>[] pools;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        InitializeUI();

        InitializePools();
    }

    private void InitializeUI()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }


        var allTexts = canvas.GetComponentsInChildren<TextMeshProUGUI>(true);

        // 모든 텍스트 컴포넌트 정보 출력
        for (int i = 0; i < allTexts.Length; i++)
        {
            var text = allTexts[i];
        }

        foreach (var text in allTexts)
        {
            if (text.CompareTag("ScoreText"))
            {
                scoreText = text;
            }
        }

        gameOverUI = FindFirstObjectByType<GameOverUI>();

        if(gameOverUI == null)
        {
            GameObject gameoverPanel = GameObject.FindGameObjectWithTag("GameOverUI");

            if(gameoverPanel)
            {
                gameOverUI= gameoverPanel.GetComponent<GameOverUI>();

            }
        }

            gameOverUI.HideGameOverUI();

    }

    private void Start()
    {
        StartCoroutine("SpawnCoroutine");

        if (scoreText != null)
        {
            UpdateScore(0);
        }

    }

    IEnumerator SpawnCoroutine()
    {
        while (bIsNotGameOver)
        {
            int random = Random.Range(0, prefabs.Count); // 수정: -1 제거
            GetPooledObject(random);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void InitializePools()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            return;
        }

        pools = new ObjectPool<GameObject>[prefabs.Count];

        for (int i = 0; i < prefabs.Count; i++)
        {
            int index = i;
            pools[index] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefabs[index]),
                actionOnGet: (obj) =>
                {
                    obj.SetActive(true);

                    var poolable = obj.GetComponent<IPoolable>();
                    if (poolable != null)
                    {
                        poolable.SetPoolIndex(index);
                        poolable.Initialize();
                    }
                },
                actionOnRelease: (obj) =>
                {
                    obj.SetActive(false);

                    var poolable = obj.GetComponent<IPoolable>();
                    if (poolable != null)
                    {
                        poolable.OnReturnToPool();
                    }
                },
                actionOnDestroy: (obj) => Destroy(obj),
                defaultCapacity: 10,
                maxSize: 100
            );

            for (int j = 0; j < 10; j++)
            {
                var prewarmedObj = pools[index].Get();
                pools[index].Release(prewarmedObj);
            }
        }
    }

    public GameObject GetPooledObject(int poolIndex)
    {
        if (pools == null || poolIndex < 0 || poolIndex >= pools.Length)
            return null;

        return pools[poolIndex].Get();
    }

    public void ReturnToPool(int poolIndex, GameObject obj)
    {
        if (pools == null || poolIndex < 0 || poolIndex >= pools.Length)
            return;

        pools[poolIndex].Release(obj);
    }

    public void RequestReturnToPool(GameObject obj)
    {
        if (obj.TryGetComponent<Target>(out var target))
        {
            ReturnToPool(target.PoolID, obj);
        }


    }
    public void UpdateScore(int InValue)
    {
        score += InValue;
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        gameOverUI.ShowGameOverUI();
        bIsNotGameOver=false;

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}

