using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTEStealNamespace
{

    public class DummyCreateQTEComponent : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        [SerializeField] private GameObject QTEManagerGameobject;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
            {
                GameObject go = Instantiate(QTEManagerGameobject);
                go.GetComponent<QTEManagerComponment>().Initialize(0, 0, 40, 15, 1, 8);

            }
        }
    }
}