using UnityEngine;

public class Target : MonoBehaviour, IPoolable
{
    public int PoolID { get; private set; }
    
    Rigidbody targetRb;

    [Header("GameSystem")]

    [SerializeField]
    float minForce = 15f;

    [SerializeField]
    float maxForce = 20f;

    [SerializeField]
    float minRotateForce = -15f;

    [SerializeField]
    float maxRotateForce = 15f;

    [SerializeField]
    float xRange = 4f;

    [SerializeField]
    float yRange = -6f;

    [Header("Point")]
    [SerializeField]
    int point=5;


    [Header("Effect")]
    [SerializeField]
    ParticleSystem explosionEffect;

    void Awake()
    {
        targetRb = GetComponent<Rigidbody>();
        
        gameObject.layer = LayerMask.NameToLayer("Target");
    }

    public void SetPoolIndex(int poolIndex)
    {
        PoolID = poolIndex;
    }

    private void OnMouseDown()
    {
        if ((GameManager.Instance.IsNotGameOver))
        {
            GameManager.Instance.ReturnToPool(PoolID, gameObject);
            GameManager.Instance.UpdateScore(point);
            if (explosionEffect)
            {
                Instantiate(explosionEffect, transform.position, transform.rotation);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.CompareTag("Good"))
        {
            GameManager.Instance.GameOver();
        }
        GameManager.Instance.ReturnToPool(PoolID, gameObject);
    }

    public void Initialize()
    {
        if (targetRb == null)
        {
            targetRb = GetComponent<Rigidbody>();
        }

        // 물리 상태 완전 초기화
        targetRb.linearVelocity = Vector3.zero;
        targetRb.angularVelocity = Vector3.zero;
        targetRb.Sleep(); // 물리 시뮬레이션 일시 정지
        
        // 위치 설정 (Rigidbody 물리적 위치)
        Vector3 spawnPos = RandomSpawnPos();
        targetRb.position = spawnPos;
        transform.position = spawnPos;
        
        // Rigidbody 활성화 후 힘 적용
        targetRb.WakeUp();
        targetRb.AddForce(RandomXRange(), ForceMode.Impulse);
        targetRb.AddTorque(RandomTorque(), RandomTorque(), 0);
    }

    public void OnReturnToPool()
    {
        // 물리 상태 초기화만 수행
        targetRb.linearVelocity = Vector3.zero;
        targetRb.angularVelocity = Vector3.zero;
    }

    private Vector3 RandomSpawnPos()
    {
        return new Vector3(Random.Range(-xRange, xRange), yRange);
    }

    private float RandomTorque()
    {
        return Random.Range(minRotateForce, maxRotateForce);
    }

    private Vector3 RandomXRange()
    {
        return Vector3.up * Random.Range(minForce, maxForce);
    }
}
