public interface IPoolable
{
    void Initialize();
    void OnReturnToPool();
    void SetPoolIndex(int poolIndex); // Ãß°¡
}