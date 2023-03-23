namespace WordPuzzle.Game
{
    using DG.Tweening;
    using System;
    using TMPro;
    using UnityEngine;
    using WordPuzzle.Command;

    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Tile : BaseObject, ICommandMovement
    {
        [SerializeField] private TextMeshPro _txt;
        private SpriteRenderer _renderer;
        private Collider2D _collider;
        public int ID { get; set; }
        public int SortingOrder { get; set; }
        public Vector2 DefaultPosition { get; set; }
        public string Character { get; set; }
        public bool CanUse { get; set; }

        public delegate void OnPointerDown(Tile tile);
        public event OnPointerDown PointerDown;
        public delegate void OnPointerUp(Tile tile);
        public event OnPointerUp PointerUp;

        public event Action<Tile> Moved;
        public event Action<Tile> MoveBacked;

        protected override void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            base.Awake();
        }

        protected override void Start()
        {
            InitTile();
            base.Start();
        }

        private void InitTile()
        {
            _renderer.color = CanUse ? Color.white : Color.grey;

            _collider.enabled = CanUse;

            _renderer.sortingOrder = SortingOrder;
            _txt.sortingOrder = SortingOrder;

            _txt.text = Character;

            transform.position = DefaultPosition;
        }

        private void OnMouseDown()
        {
            if (!CanUse)
                return;
            PointerDown?.Invoke(this);
        }

        private void OnMouseUp()
        {
            if (!CanUse)
                return;
            PointerUp?.Invoke(this);
        }

        public void Move(Vector2 target)
        {
            transform.DOKill();
            transform.DOMove(target, 1f);
            Moved?.Invoke(this);
        }

        public void MoveBack()
        {
            transform.DOKill();
            transform.DOMove(DefaultPosition, 1f);
            MoveBacked?.Invoke(this);
        }
    }
}