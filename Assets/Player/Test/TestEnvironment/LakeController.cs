using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class LakeController : MonoBehaviour
    {
        public GameObject BreadPrefab;

        private Rigidbody2D _rigidBody;


        private Bounds _dimension;


        public BreadController GenerateNewBread()
        {
            var b = Instantiate(BreadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            return b.GetComponentInChildren<BreadController>();
        }

        public void DestroyBread(BreadController controller)
        {
            Destroy(controller.gameObject);
        }

        public Bounds getBounds()
        {
            return _dimension;
        }
        public bool Contains(Vector2 point)
        {
            Collider2D[] colliders = new Collider2D[100];
            _rigidBody.GetAttachedColliders(colliders);

            return colliders[0].OverlapPoint(point);
            //var bw = b.max.x - b.min.x;
            //var bh = b.max.y - b.min.y;

            //var d = Mathf.Sqrt(Mathf.Pow((2 * point.x) / bw, 2) + Mathf.Pow((2 * point.y) / bh, 2));

            //return (d <= 1);
        }


        private void Awake()
        {
            _dimension = transform.Find("Ground").GetComponent<SpriteRenderer>().bounds;
            _rigidBody = GetComponent<Rigidbody2D>();
        }








    }
}