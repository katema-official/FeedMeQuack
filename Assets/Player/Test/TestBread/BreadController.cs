using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Player
{
    public class BreadController : MonoBehaviour
    {
        private float _breadPoints = 0;
        private SpriteRenderer _renderer = null;

        public void SetPoints(float points)
        {
            _breadPoints = points;
            UpdateColor();
        }
        public float GetPoints()
        {
            return _breadPoints;
        }
        public void EatPoints(float points)
        {
            _breadPoints -= points;
            UpdateColor();

        }
        private void UpdateColor()
        {
            _renderer.color = new Color(0.2f + _breadPoints / 20.0f, 0.2f + _breadPoints / 20.0f, 0);
        }
        // Start is called before the first frame update
        void Awake()
        {
            _renderer = GetComponentInChildren<SpriteRenderer>();
            SetPoints(Random.Range(1, 15));
        }



    }
}

