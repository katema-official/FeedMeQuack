using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QTEStealNamespace {

    public class QTEButtonCollisionDetectionComponent : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.transform.parent.gameObject.name == "QuickTimeEventManagerGameobject")
            {
                Debug.Log("TOUCH");
                transform.parent.gameObject.GetComponent<QTEButtonManagerComponent>().ChangeState(EnumsQTESteal.QTEButtonState.InPressing);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.transform.parent.gameObject.name == "QuickTimeEventManagerGameobject")
            {
                Debug.Log("UN-TOUCH");
                transform.parent.gameObject.GetComponent<QTEButtonManagerComponent>().ChangeState(EnumsQTESteal.QTEButtonState.Failure);
            }
        }
    }
}