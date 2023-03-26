namespace WordPuzzle.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using WordPuzzle.Data;
    using WordPuzzle.Manager;

    public class LevelsPopup : BaseObject, IPoolable
    {
        [SerializeField] private RectTransform _contentParent;

        public event IPoolable.OnAddPool AddPool;
        public event Action<LevelContent> LevelSelected;

        private readonly List<LevelSelectionItem> _selectionItems = new List<LevelSelectionItem>();

        protected override void OnEnable()
        {
            base.OnEnable();
            InitLevels();
        }

        private void InitLevels()
        {
            ClearLevels();
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            List<LevelContent> levelContents = resourcesManager.GetScriptableObjects<LevelContent>();
            levelContents.Sort((l1, l2) =>
            {
                if(l1.levelNumber > l2.levelNumber) return 1;
                else if(l1.levelNumber< l2.levelNumber) return -1;
                else return 0;
            });

            LevelSelectionItem levelSelectionItemPrefab = resourcesManager.GetObjectPrefab<LevelSelectionItem>();
            
            Vector2 sizeDelta = _contentParent.sizeDelta;
            sizeDelta.y = levelContents.Count * levelSelectionItemPrefab.GetComponent<RectTransform>().sizeDelta.y;
            _contentParent.sizeDelta = sizeDelta;
            
            for (int i = 0; i < levelContents.Count; i++)
            {
                LevelSelectionItem levelSelectionItem = resourcesManager.GetObject<LevelSelectionItem>(_contentParent);
                levelSelectionItem.SetLevelContent(levelContents[i]);
                bool isComplete = i == 0 || PlayerPrefs.HasKey("Level-" + levelContents[i - 1].levelNumber);
                levelSelectionItem.CanPlay(isComplete);
                levelSelectionItem.Selected += OnLevelSelected;
                _selectionItems.Add(levelSelectionItem);
            }
        }

        private void ClearLevels()
        {
            _selectionItems.ForEach(i => i.DeActivate());
            _selectionItems.Clear();
        }

        private void OnLevelSelected(LevelContent levelContent)
        {
            LevelSelected?.Invoke(levelContent);
            DeActivate();
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}