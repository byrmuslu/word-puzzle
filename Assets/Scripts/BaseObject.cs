namespace WordPuzzle
{
    using UnityEngine;

    public class BaseObject : MonoBehaviour
    {
        public virtual string VariantKey { get; } = "";

        public delegate void OnObjectInitialized(BaseObject initializedObject);
        public event OnObjectInitialized ObjectInitialized;
        public delegate void OnObjectEnabled(BaseObject enabledObject);
        public event OnObjectEnabled ObjectEnabled;
        public delegate void OnActivated(BaseObject enabledObject);
        public event OnActivated Activated;
        public delegate void OnObjectStarted(BaseObject startedObject);
        public event OnObjectStarted ObjectStarted;
        public delegate void OnObjectDisabled(BaseObject disabledObject);
        public event OnObjectDisabled ObjectDisabled;
        public delegate void OnDeActivated(BaseObject disabledObject);
        public event OnDeActivated DeActivated;
        public delegate void OnObjectDestroyed(BaseObject destroyedObject);
        public event OnObjectDestroyed ObjectDestroyed;

        protected virtual void Awake()
        {
            ObjectInitialized?.Invoke(this);
        }

        protected virtual void OnEnable()
        {
            ObjectEnabled?.Invoke(this);
        }

        protected virtual void Start()
        {
            ObjectStarted?.Invoke(this);
        }

        protected virtual void OnDisable()
        {
            ObjectDisabled?.Invoke(this);
        }

        protected virtual void OnDestroy()
        {
            ObjectDestroyed?.Invoke(this);
        }

        public virtual void Activate()
        {
            gameObject.SetActive(true);
            Activated?.Invoke(this);
        }

        public virtual void DeActivate()
        {
            gameObject.SetActive(false);
            DeActivated?.Invoke(this);
        }

        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
    }
}