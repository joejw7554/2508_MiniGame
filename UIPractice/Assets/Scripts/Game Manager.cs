using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [SerializeField]
    List<GameObject> prefabs;

    ObjectPool<GameObject>[] pools;

    [SerializeField]
    float spawnRate = 3f;

    bool bIsNotGameOver = true;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        InitializePools();
    }

    private void Start()
    {
        StartCoroutine("SpawnCoroutine");
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
            Debug.LogWarning("Prefabs list is empty or null!");
            return;
        }

        pools = new ObjectPool<GameObject>[prefabs.Count];

        for (int i = 0; i < prefabs.Count; i++)
        {
            int index = i;
            pools[index] = new ObjectPool<GameObject>(
                createFunc: () => Instantiate(prefabs[index]),
                actionOnGet: (obj) => {
                    obj.SetActive(true);
                    
                    var poolable = obj.GetComponent<IPoolable>();
                    if (poolable != null)
                    {
                        poolable.SetPoolIndex(index);
                        poolable.Initialize();
                    }
                },
                actionOnRelease: (obj) => {
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

            // Pre-warming: 미리 10개 오브젝트 생성
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

        // actionOnRelease에서 OnReturnToPool()이 호출되므로 바로 반환
        pools[poolIndex].Release(obj);
    }

    public void RequestReturnToPool(GameObject obj)
    {
        if (obj.TryGetComponent<Target>(out var target))
        {
            ReturnToPool(target.PoolID, obj);
        }
    }
}

