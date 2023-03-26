namespace WordPuzzle.UI
{
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;
    using WordPuzzle.Manager;
    using System;
    using System.Collections.Generic;
    using WordPuzzle.Game;
    using System.Linq;
    using DG.Tweening;
    using UnityEngine.EventSystems;
    using Unity.VisualScripting;
    using System.Collections;

    public class InGamePanel : BaseObject
    {
        [SerializeField] private TextMeshProUGUI _txtTotalScore;
        [SerializeField] private TextMeshProUGUI _txtTitle;
        [SerializeField] private RectTransform _wordsContentParent;
        [SerializeField] private RectTransform _letterViewsContentParent;
        [SerializeField] private TextMeshProUGUI _txtWordScore;
        [Space(5)]
        [SerializeField] private Button _btnUndo;
        [SerializeField] private Button _btnDone;
        [SerializeField] private Button _btnAutoSolve;

        private const float LETTER_VIEW_OFFSET = 10f;

        private LevelManager _levelManager;

        private readonly List<LetterView> _letterViews = new List<LetterView>();
        private readonly List<WordView> _wordViews = new List<WordView>();

        private Coroutine _allUndoRoutine;
        private bool _autoSolver;
        protected override void Awake()
        {
            Registration();

            base.Awake();

            _btnDone.interactable = false;
            _btnUndo.interactable = false;
        }

        public void ClearPanelContent()
        {
            SetLetterViews(0);
            _wordViews.ForEach(w => w.DeActivate());
            _wordViews.Clear();
        }

        private void SetLetterViews(int count)
        {
            ClearLetterViews();
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            float size = resourcesManager.GetObjectPrefab<LetterView>().RectTransform.sizeDelta.x;

            Vector2 sizeDelta = _letterViewsContentParent.sizeDelta;
            sizeDelta.x = size * count + (LETTER_VIEW_OFFSET * count);
            _letterViewsContentParent.sizeDelta = sizeDelta;

            for (int i = 0; i < count; i++)
            {
                LetterView newLetterView = resourcesManager.GetObject<LetterView>(_letterViewsContentParent);
                newLetterView.SetText("");
                newLetterView.Tile = null;
                _letterViews.Add(newLetterView);
            }
        }

        private void ClearLetterViews()
        {
            _txtWordScore.gameObject.SetActive(false);
            _letterViews.ForEach(t => t.Destroy());
            _letterViews.Clear();
        }


        private void Registration()
        {
            _btnUndo.onClick.AddListener(OnBtnUndoClicked);
            _btnDone.onClick.AddListener(OnBtnDoneClicked);
            _btnAutoSolve.onClick.AddListener(OnBtnAutoSolveClicked);
            EventTrigger trigger = _btnUndo.AddComponent<EventTrigger>();
            EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
            entryPointerDown.eventID = EventTriggerType.PointerDown;
            entryPointerDown.callback.AddListener(OnBtnUndoPointerDown);
            trigger.triggers.Add(entryPointerDown);
            trigger = _btnUndo.GetComponent<EventTrigger>();
            EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
            entryPointerUp.eventID = EventTriggerType.PointerUp;
            entryPointerUp.callback.AddListener(OnBtnUndoPointerUp);
            trigger.triggers.Add(entryPointerUp);
        }

        private void UnRegistration()
        {
            _btnUndo.onClick.RemoveListener(OnBtnUndoClicked);
            _btnDone.onClick.RemoveListener(OnBtnDoneClicked);
            _btnAutoSolve.onClick.AddListener(OnBtnAutoSolveClicked);
            if (_levelManager)
            {
                _levelManager.TitleChanged -= OnLevelTitleChanged;
                _levelManager.WordCompletionStatusChanged -= OnWordCompletionStatusChanged;
                _levelManager.PhaseChanged-= OnPhaseChanged;
                _levelManager.PhaseDone -= OnPhaseDone;
                _levelManager.TilePointerDown-= OnTilePointerDown;
                _levelManager.ActionAvailabilityChanged -= OnActionAvailabilityChanged;
            }
        }

        private void OnBtnAutoSolveClicked()
        {
            _autoSolver = true;
            _levelManager.AutoSolve(() =>
            {
                _btnAutoSolve.interactable = true;
                _autoSolver = false;
            });
            _btnAutoSolve.interactable = false;
        }

        private void OnBtnUndoPointerDown(BaseEventData eventData)
        {
            if (_autoSolver)
                return;
            _allUndoRoutine = StartCoroutine(AllActionsUndoWithDuration(1f));
        }

        private void OnBtnUndoPointerUp(BaseEventData eventData)
        {
            if(_allUndoRoutine!= null)
                StopCoroutine(_allUndoRoutine);
        }

        private IEnumerator AllActionsUndoWithDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            _levelManager.AllUndo();
        }

        protected override void OnDestroy()
        {
            UnRegistration();

            base.OnDestroy();
        }

        public void SetLevelManager(LevelManager levelManager)
        {
            _levelManager = levelManager;

            levelManager.TitleChanged -= OnLevelTitleChanged;
            levelManager.WordCompletionStatusChanged -= OnWordCompletionStatusChanged;
            levelManager.PhaseChanged -= OnPhaseChanged;
            levelManager.PhaseDone -= OnPhaseDone;
            levelManager.TilePointerDown -= OnTilePointerDown;
            levelManager.ActionAvailabilityChanged -= OnActionAvailabilityChanged;

            levelManager.TitleChanged += OnLevelTitleChanged;
            levelManager.WordCompletionStatusChanged += OnWordCompletionStatusChanged;
            levelManager.PhaseChanged += OnPhaseChanged;
            levelManager.PhaseDone += OnPhaseDone;
            levelManager.TilePointerDown += OnTilePointerDown;
            levelManager.ActionAvailabilityChanged += OnActionAvailabilityChanged;
        }

        private void OnLevelTitleChanged(string title)
        {
            _txtTitle.text = title;
        }

        private void OnActionAvailabilityChanged(bool status)
        {
            _btnUndo.interactable = status;
        }

        private void OnPhaseDone(string word)
        {
            _btnUndo.interactable = false;
            _btnDone.interactable = false;

            if (string.IsNullOrEmpty(word))
                return;
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            float heightItem = resourcesManager.GetObjectPrefab<WordView>().GetComponent<RectTransform>().sizeDelta.y;

            WordView wordView = resourcesManager.GetObject<WordView>(_wordsContentParent);
            wordView.SetText(word.ToUpper());

            _wordViews.Add(wordView);

            Vector2 sizeDelta = _wordsContentParent.GetComponent<RectTransform>().sizeDelta;
            sizeDelta.y = (heightItem * ((_wordViews.Count / 2) + 1));
            _wordsContentParent.GetComponent<RectTransform>().sizeDelta = sizeDelta;
        }

        private void OnTilePointerDown(Tile tile)
        {
            if (!_letterViews.Any(t => t.IsEmpty))
                return;
            LetterView view = _letterViews.First(t => t.IsEmpty);
            view.Tile = tile;
            
            LetterView dummyView = GetDummyLetterView();
            dummyView.SetText(tile.Character);
            dummyView.RectTransform.position= Camera.main.WorldToScreenPoint(tile.transform.position);
            dummyView.RectTransform.DOMove(view.RectTransform.position, 1f)
                                   .OnComplete(() =>
                                   {
                                       view.SetText(tile.Character);
                                       dummyView.Destroy();
                                   });
            tile.Execute();
            tile.DeActivate();
            tile.Withdrawn += OnTileWithdrawn;
        }

        private void OnTileWithdrawn(Tile tile)
        {
            tile.Withdrawn -= OnTileWithdrawn;
            if (!_letterViews.Any(v => v.Tile == tile))
                return;

            LetterView view = _letterViews.Find(v => v.Tile == tile);
            view.SetText("");
            view.Tile = null;
            LetterView dummyView = GetDummyLetterView();
            dummyView.SetText(tile.Character);
            dummyView.RectTransform.position = view.RectTransform.position;
            Vector3 targetPos = Camera.main.WorldToScreenPoint(tile.transform.position);
            dummyView.RectTransform.DOMove(targetPos, 1f)
                                   .OnUpdate(() => view.SetText(""))
                                   .OnComplete(() =>
                                   {
                                       tile.Activate();
                                       dummyView.Destroy();
                                   });
        }

        private void OnPhaseChanged(int maxWordLength,int totalScore)
        {
            _btnUndo.interactable = false;
            _btnDone.interactable = false;
            _txtTotalScore.text = "Score : " + totalScore.ToString();
            SetLetterViews(maxWordLength);
        }

        private void OnWordCompletionStatusChanged(bool hasWord, int wordScore)
        {
            _btnDone.interactable = hasWord;
            _txtWordScore.gameObject.SetActive(hasWord);
            _txtWordScore.text = $"Word Score : {wordScore}";
        }

        private void OnBtnDoneClicked()
        {
            if (_autoSolver)
                return;
            _levelManager.Done();
        }

        private void OnBtnUndoClicked()
        {
            if (_autoSolver)
                return;
            _levelManager.Undo();
        }

        private LetterView GetDummyLetterView()
        {
            ResourcesManager resourcesManager = ResourcesManager.Instance;

            LetterView dummy = resourcesManager.GetObject<LetterView>(transform);

            return dummy;
        }
    }
}