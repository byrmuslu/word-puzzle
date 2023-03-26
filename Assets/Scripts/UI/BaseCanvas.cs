namespace WordPuzzle.UI
{
    using UnityEngine;

    [DefaultExecutionOrder(-45)]
    public class BaseCanvas : BaseObject
    {
        public static BaseCanvas Instance { get; private set; }

        protected override void Awake()
        {
            if (Instance)
            {
                Debug.LogError("BaseCanvas must be one!!");
                Destroy(gameObject);
                return;
            }

            base.Awake();
            Instance = this;
        }
    }
}