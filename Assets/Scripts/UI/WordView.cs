namespace WordPuzzle.UI
{
    using TMPro;
    using UnityEngine;

    [RequireComponent(typeof(RectTransform))]
    public class WordView : BaseObject, IPoolable
    {
        [SerializeField] private TextMeshProUGUI _txtWord;

        public event IPoolable.OnAddPool AddPool;

        public void SetText(string text)
        {
            _txtWord.text = text;
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}