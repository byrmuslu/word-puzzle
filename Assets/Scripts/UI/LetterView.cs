namespace WordPuzzle.UI
{
    using System.Collections;
    using TMPro;
    using UnityEngine;
    using WordPuzzle.Game;

    [RequireComponent(typeof(RectTransform))]
    public class LetterView : BaseObject
    {
        [SerializeField] private TextMeshProUGUI _txtLetter;
        public bool IsEmpty => Tile == null;
        public Tile Tile { get; set; }

        private RectTransform _rectTransform;

        public RectTransform RectTransform 
        {
            get
            {
                if(!_rectTransform)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        public void SetText(string text)
        {
            _txtLetter.text = text;
        }
    }
}