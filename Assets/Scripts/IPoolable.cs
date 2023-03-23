namespace WordPuzzle
{
    public interface IPoolable
    {
        public string VariantKey { get; }
        public void SetParent(UnityEngine.Transform parent);
        public void Activate();
        public void DeActivate();
        public delegate void OnAddPool(IPoolable obj);
        public event OnAddPool AddPool;
    }
}