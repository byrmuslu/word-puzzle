namespace WordPuzzle.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;
    using WordPuzzle.Data;
    using WordPuzzle.Manager;

    public class HomePanel : BaseObject, IPoolable
    {
        [SerializeField] private Button _btnLevels;

        public event Action<LevelContent> LevelSelected;
        public event IPoolable.OnAddPool AddPool;

        protected override void Awake()
        {
            Registration();

            base.Awake();
        }

        protected override void OnDestroy()
        {
            UnRegistration();

            base.OnDestroy();
        }

        private void Registration()
        {
            _btnLevels.onClick.AddListener(OnBtnLevelsClicked);
        }

        private void UnRegistration()
        {
            _btnLevels.onClick.RemoveListener(OnBtnLevelsClicked);
        }

        private void OnBtnLevelsClicked()
        {
            ShowLevelsPopup();
        }

        public void ShowLevelsPopup()
        {
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            LevelsPopup levelsPopup = resourcesManager.GetObject<LevelsPopup>(BaseCanvas.Instance.transform);
            levelsPopup.LevelSelected += OnLevelSelected;
            levelsPopup.DeActivated += OnLevelsPopupDeActivated;
        }

        private void OnLevelsPopupDeActivated(BaseObject disabledObject)
        {
            disabledObject.DeActivated -= OnLevelsPopupDeActivated;
            (disabledObject as LevelsPopup).LevelSelected -= OnLevelSelected;
        }

        private void OnLevelSelected(LevelContent selectedLevelContent)
        {
            LevelSelected?.Invoke(selectedLevelContent);
            DeActivate();
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}