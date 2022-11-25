using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEButtonCollisionDetectionComponent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent.gameObject.name == "QuickTimeEventManagerGameobject")
        {
            Debug.Log("TOUCH");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.transform.parent.gameObject.name == "QuickTimeEventManagerGameobject")
        {
            Debug.Log("UN-TOUCH");
        }
    }
}
