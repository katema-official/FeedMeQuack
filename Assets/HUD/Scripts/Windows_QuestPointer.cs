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
        GameObject pointerGameObject = Instantiate(transform.Find("pointerTemplate").gameObject,new Vector3(10000,10000), Quaternion.identity);
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


            double limitAngle = UtilsClass.GetAngleFromVector(new Vector3(1920,1080));
            double angleRad = angle * Math.PI / 180;
            double tan = Math.Tan(angleRad);
            double cotan = 1/tan;
            double cos = Math.Cos(angleRad);
            double sin = Math.Sin(angleRad);
            var rect = canvasRectTransform.rect;
            bool lateral = false;
            float x,y, width= 1920, height= 1080;
            
            if (angle >= limitAngle && angle< 180-limitAngle){ //top
                lateral = false;
                y= height* 0.96f;
                x = (float) (width* cotan)+ width/2;
            }
            else if (angle >= 180-limitAngle && angle< 180+limitAngle){ //left
                lateral = true;
                x = width* 0.04f;
                y = (float) (0.5f * (1 - tan) * height);
            }
            else if (angle >= 180+limitAngle && angle< 360-limitAngle){ //bottom
                lateral = false;
                y= height* 0.04f;
                x = (float) (width*0.5-width* 0.5 * cotan)* 0.95f;
            }
            else { //right
                lateral = true;
                x = width* 0.96f;
                y= (float) (0.5f * (1 + tan) * height);
            }
            x -= width / 2;
            y -= height / 2;
            if (lateral)
                y *= 1.25f;
            else
                x *= 0.375f;
            RectTransform rectTransform = pointerGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(x,y);
          /*
            float pad = 0.04f;
            //todo: se sopra/sotto, la x è cos(ang)/ cos(45), mentre la y per le laterali dovrebbe essere sin(ang)/sin(45)
            if (angle >= limitAngle && angle< 180-limitAngle){ //top
                lateral = false;
                y= 1-pad;
                x = (float) (-cos/ Math.Cos(limitAngle));
            }
            else if (angle >= 180-limitAngle && angle< 180+limitAngle){ //left
                lateral = true;
                x =pad;
                y = (float) (sin/Math.Sin(limitAngle));
            }
            else if (angle >= 180+limitAngle && angle< 360-limitAngle){ //bottom
                lateral = false;
                y= pad;
                x = (float) (cos/ Math.Cos(limitAngle));
            }
            else { //right
                lateral = true;
                x = 1-pad;
                y = (float) (sin/Math.Sin(limitAngle));
            }

            x *= width;
            y *= height;

            x -= width/2;
            y -= height/2;
            
            RectTransform rectTransform = pointerGameObject.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(x,y);*/
        }

        public void DestroySelf() {
            Destroy(pointerGameObject);
        }

    }
}
