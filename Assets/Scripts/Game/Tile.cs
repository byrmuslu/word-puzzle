namespace WordPuzzle.Game
{
    using System;
    using TMPro;
    using UnityEngine;
    using WordPuzzle.Command;

    [RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
    public class Tile : BaseObject, ICommand
    {
        [SerializeField] private TextMeshPro _txt;
        private SpriteRenderer _renderer;
        private Collider2D _collider;
        public int ID { get; private set; }
        public int SortingOrder { get; private set; }
        public Vector2 DefaultPosition { get; private set; }
        public string Character { get; private set; }
        public bool CanUse { get; private set; }

        public delegate void OnSelect(Tile tile);
        public event OnSelect Selected;

        public event Action<Tile> Executed;
        public event Action<Tile> Withdrawn;

        public Vector2 Target { get; set; }

        public bool IsMoving { get; private set; }

        protected override void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _collider = GetComponent<Collider2D>();
            base.Awake();
        }

        public void SetID(int id)
        {
            ID = id;
        }

        public void SetCharacter(string character)
        {
            Character = character;
            _txt.text = character;
        }

        public void SetSortingOrder(int sortingOrder) 
        {
            SortingOrder = sortingOrder;
            _renderer.sortingOrder = sortingOrder;
            _txt.sortingOrder = sortingOrder;
        }

        public void SetDefaultPosition(Vector2 position)
        {
            DefaultPosition = position;
            transform.position = position;
        }

        public void SetCanUse(bool canUse)
        {
            CanUse = canUse;
            _renderer.color = canUse ? Color.white : Color.grey;
            _collider.enabled = canUse;
        }

        private void OnMouseDown()
        {
            if (!CanUse)
                return;
            Select();
        }

        public void Select()
        {
            Selected?.Invoke(this);
        }

        public void Execute()
        {
            Executed?.Invoke(this);
        }

        public void Undo()
        {
            Withdrawn?.Invoke(this);
        }
    }
}