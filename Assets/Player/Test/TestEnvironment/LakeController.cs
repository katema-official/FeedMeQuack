using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Player
{
    public class LakeController : MonoBehaviour
    {
        public GameObject BreadPrefab;


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

        private void Awake()
        {
            _dimension = transform.Find("Ground").GetComponent<SpriteRenderer>().bounds;
        }








    }
}