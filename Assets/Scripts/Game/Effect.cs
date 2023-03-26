namespace WordPuzzle.Game
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class Effect : BaseObject, IPoolable
    {
        [SerializeField] private string _name;
        public override string VariantKey => _name;

        [SerializeField] private float _durationTime;

        public event IPoolable.OnAddPool AddPool;

        public void PlayEffect(Action onComplete = null)
        {
            StopAllCoroutines();
            StartCoroutine(PlayAction(onComplete));
        }

        public void SetPosition(Vector2 pos)
        {
            transform.position = pos;
        }

        private IEnumerator PlayAction(Action onComplete = null)
        {
            yield return new WaitForSeconds(_durationTime);
            onComplete?.Invoke();
            DeActivate();
        }

        public override void DeActivate()
        {
            base.DeActivate();
            AddPool?.Invoke(this);
        }
    }
}