/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class Windows_QuestPointer : MonoBehaviour {

    [SerializeField] private Camera uiCamera;
    [SerializeField] private Sprite arrowSprite;

    private List<QuestPointer> questPointerList;

    private void Awake() {
        questPointerList = new List<QuestPointer>();
    }

    private void Update() {
        foreach (QuestPointer questPointer in questPointerList) {
            questPointer.Update();
        }
    }

    private void Start(){
        uiCamera = FindObjectOfType<Camera>();
    }

    public QuestPointer CreatePointer(Vector3 targetPosition) {
        GameObject pointerGameObject = Instantiate(transform.Find("pointerTemplate").gameObject);
        pointerGameObject.SetActive(true);
        pointerGameObject.transform.SetParent(transform, false);
        QuestPointer questPointer = new QuestPointer(targetPosition, pointerGameObject, uiCamera, arrowSprite);
        questPointerList.Add(questPointer);
        return questPointer;
    }

    public void DestroyPointer(QuestPointer questPointer) {
        questPointerList.Remove(questPointer);
        questPointer.DestroySelf();
    }

    public class QuestPointer {

        private Vector3 targetPosition;
        private GameObject pointerGameObject;
        private Sprite arrowSprite;
        private Camera uiCamera;
        private RectTransform pointerRectTransform;
        private Image pointerImage;

        public QuestPointer(Vector3 targetPosition, GameObject pointerGameObject, Camera uiCamera, Sprite arrowSprite) {
            this.targetPosition = targetPosition;
            this.pointerGameObject = pointerGameObject;
            this.uiCamera = uiCamera;
            this.arrowSprite = arrowSprite;
            
            pointerRectTransform = pointerGameObject.GetComponent<RectTransform>();
            pointerImage = pointerGameObject.GetComponent<Image>();
        }

        public void Update() {
            float borderSize = 100f;
            Vector3 targetPositionScreenPoint = uiCamera.WorldToScreenPoint(targetPosition);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSize || targetPositionScreenPoint.x >= Screen.width - borderSize || targetPositionScreenPoint.y <= borderSize || targetPositionScreenPoint.y >= Screen.height - borderSize;

            if (isOffScreen) {
                RotatePointerTowardsTargetPosition();
                
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderSize, Screen.width - borderSize);
                cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderSize, Screen.height - borderSize);

                Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
            } else {
                Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

                pointerRectTransform.localEulerAngles = Vector3.zero;
            }
            MoveToCanvasV2();
        }

        private void RotatePointerTowardsTargetPosition() {
            Vector3 toPosition = targetPosition;
            Vector3 fromPosition = uiCamera.transform.position;
            fromPosition.z = 0f;
            Vector3 dir = (toPosition - fromPosition).normalized;
            float angle = UtilsClass.GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        private void MoveToCanvasV2(){
            Vector3 toPosition = targetPosition;
            Vector3 fromPosition = uiCamera.transform.position;
            Vector2 dir = (toPosition - fromPosition).normalized;
            float angle = UtilsClass.GetAngleFromVectorFloat(dir);
            // Get the center position of the canvas
            RectTransform canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
            Vector2 center = canvasRectTransform.rect.center;

            // Calculate the direction vector
            dir.Normalize();

            // Calculate the distance from the center to the border
            //float distance = Mathf.Max(canvasRectTransform.rect.width, canvasRectTransform.rect.height) * 0.90f;

            // Calculate the final position on the border
            //Vector2 finalPosition = center + dir * distance*0.9f;

            float x,y;

            double angleRad = angle * Math.PI / 180;
            double tan = Math.Tan(angleRad);
            double cotan = 1/tan;
            var rect = canvasRectTransform.rect;
            
            if (angle is >= 45 and < 135){ //top
                y= rect.height* 0.95f- rect.height*0.5f;
                x = (float) (rect.width* 0.5 * cotan);
            }
            else if (angle is >= 135 and < 225){ //left
                x = rect.width* 0.05f- rect.width * 0.5f;
                y = (float) (0.5f * (1 - tan) * rect.height)- 0.5f* rect.height;
            }
            else if (angle is >= 225 and < 315){ //bottom
                y= rect.height* 0.05f;
                x = (float) (rect.width*0.5-rect.width* 0.5 * cotan)* 0.95f;
            }
            else { //right
                x = rect.width* 0.95f * 0.5f;
                y= (float) (0.5f * (1 + tan) * rect.height)- 0.5f* rect.height;
            }
            
            RectTransform rectTransform = pointerGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(x,y);
        }

        public void DestroySelf() {
            Destroy(pointerGameObject);
        }

        private void MoveToCanvas(){
            // Get the camera's viewport coordinates of the game object
            Vector3 viewportPoint = uiCamera.WorldToViewportPoint(targetPosition);

            Debug.Log(viewportPoint);

            // Convert the viewport coordinates to canvas coordinates
            Vector2 canvasPoint;
            RectTransform canvasRectTransform = FindObjectOfType<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, viewportPoint, uiCamera, out canvasPoint);

            // Set the position of the game object on the canvas
            RectTransform rectTransform = pointerGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = canvasPoint;
        }

    }
}
