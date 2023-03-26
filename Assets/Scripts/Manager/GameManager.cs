namespace WordPuzzle.Manager
{
    using UnityEngine;
    using WordPuzzle.Data;
    using WordPuzzle.Game;
    using WordPuzzle.UI;

    public class GameManager : BaseObject
    {
        [SerializeField] private TextAsset _dictionary;

        private static GameManager _instance;

        private ResourcesManager _resourcesManager;

        private BaseCanvas _canvas;

        private HomePanel _homePanel;
        private InGamePanel _inGamePanel;

        private LevelManager _levelManager;

        protected override void Awake()
        {
            if (_instance)
            {
                Debug.LogError("GameManager must be one!!");
                Destroy(gameObject);
                return;
            }

            base.Awake();
            _resourcesManager = ResourcesManager.Instance;
            InitCanvas();
            InitHomePanel();
        }

        private void InitCanvas()
        {
            _canvas = _resourcesManager.GetObject<BaseCanvas>();
        }

        private void InitHomePanel()
        {
            _homePanel = _resourcesManager.GetObject<HomePanel>(_canvas.transform);
            _homePanel.LevelSelected += OnLevelSelected;
        }

        private void OnLevelSelected(LevelContent selectedLevelContent)
        {
            if (!_levelManager)
                InitLevelManager();
            if (!_inGamePanel)
                _inGamePanel = _resourcesManager.GetObject<InGamePanel>(_canvas.transform);

            _inGamePanel.ClearPanelContent();
            _inGamePanel.Activate();
            _inGamePanel.SetLevelManager(_levelManager);

            _levelManager.SetLevel(selectedLevelContent);
        }

        private void InitLevelManager()
        {
            _levelManager = _resourcesManager.GetObject<LevelManager>();
            _levelManager.LevelCompleted += OnLevelCompleted;
        }

        private void OnLevelCompleted(LevelContent level, int levelScore)
        {
            int highScore = PlayerPrefs.GetInt("Level-" + level.levelNumber, 0);
            if (levelScore > highScore)
            {
                PlayerPrefs.SetInt("Level-" + level.levelNumber, levelScore);
                Effect effect = _resourcesManager.GetObject<Effect>("confetti");
                effect.SetPosition(Camera.main.transform.position);
                effect.PlayEffect(() =>
                {
                    BackToHomePanel();
                });
            }
            else
            {
                BackToHomePanel();
            }
        }

        private void BackToHomePanel()
        {
            _inGamePanel.DeActivate();
            _homePanel.Activate();
            _homePanel.ShowLevelsPopup();
        }
    }
}