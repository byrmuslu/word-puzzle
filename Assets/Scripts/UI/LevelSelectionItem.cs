namespace WordPuzzle.UI
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using WordPuzzle.Data;

    public class LevelSelectionItem : BaseObject, IPoolable
    {
        [SerializeField] private TextMeshProUGUI _txtContent;
        [SerializeField] private TextMeshProUGUI _txtHighscore;
        [SerializeField] private Button _btnPlay;

        public event Action<LevelContent> Selected;
        public event IPoolable.OnAddPool AddPool;

        private LevelContent _levelContent;

        protected override void Awake()
        {
            Registration();

            base.Awake();
        }

        public void SetLevelContent(LevelContent contentLevel)
        {
            _levelContent = contentLevel;
            _txtContent.text = $"Level {contentLevel.levelNumber} - {contentLevel.GetData().title}";
            _txtHighscore.text = $"Highscore : {PlayerPrefs.GetInt("Level-" + contentLevel.levelNumber, 0)}";
        }

        public void CanPlay(bool canPlay)
        {
            _btnPlay.interactable = canPlay;
        }

        protected override void OnDestroy()
        {
            UnRegistration();

            base.OnDestroy();
        }

        private void Registration()
        {
            _btnPlay.onClick.AddListener(OnBtnPlayClicked);
        }

        private void UnRegistration()
        {
            _btnPlay.onClick.RemoveListener(OnBtnPlayClicked);
        }

        private void OnBtnPlayClicked()
        {
            Selected?.Invoke(_levelContent);
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}