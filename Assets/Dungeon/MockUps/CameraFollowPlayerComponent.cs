using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayerComponent : MonoBehaviour
{
    [SerializeField] public Transform Player;

    [SerializeField] public GameObject Terrain;
    private float _terrainWidth;
    private float _terrainHeight;

    private float _terrainCenterX;
    private float _terrainCenterY;

    private float _cameraWidth;
    private float _cameraHeight;


    private void Start()
    {
        Terrain = GameObject.Find("Terrain");
        _terrainWidth = Terrain.transform.localScale.x;
        _terrainHeight = Terrain.transform.localScale.y;
        _terrainCenterX = Terrain.transform.position.x;
        _terrainCenterY = Terrain.transform.position.y;
        Vector2 topRightCorner = new Vector2(1, 1);
        Vector2 edgeVector = Camera.main.ViewportToWorldPoint(topRightCorner);
        float height = edgeVector.y * 2;
        float width = edgeVector.x * 2;
        _cameraWidth = width;
        _cameraHeight = height;
        Debug.LogFormat("width and height = {0} & {1}", width, height);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Clamp(Player.transform.position.x, _terrainCenterX - _terrainWidth/2 + _cameraWidth/2, _terrainCenterX + _terrainWidth / 2 - _cameraWidth / 2),
                                        Mathf.Clamp(Player.transform.position.y, _terrainCenterY - _terrainHeight / 2 + _cameraHeight / 2, _terrainCenterY + _terrainHeight / 2 - _cameraHeight / 2),
                                        -50);


        /*transform.position = Player.transform.position + new Vector3(0, 0, -50);
        
        Debug.LogFormat("width and height = {0} & {1}", width, height);
        var pos = transform.position;
        pos.x = Mathf.Clamp(transform.position.x, terrainCenterX - terrainWidth / 2 + width / 2, terrainCenterX + terrainWidth / 2 - width / 2);
        pos.y = Mathf.Clamp(transform.position.y, terrainCenterY - terrainHeight / 2 + height / 2, terrainCenterY + terrainHeight / 2 - height / 2);
        transform.position = pos;*/


        if (Input.GetKeyDown(KeyCode.P))
        {
            float range = 10000f;

            Ray rayNorth = new Ray(new Vector3(x, y, 0), new Vector3(0, 1, 0));
            Ray raySouth = new Ray(new Vector3(x, y, 0), new Vector3(0, -1, 0));
            Ray rayWest = new Ray(new Vector3(x, y, 0), new Vector3(1, 0, 0));
            Ray rayEast = new Ray(new Vector3(x, y, 0), new Vector3(-1, 0, 0));
            


            Debug.DrawRay(new Vector3(x, y, 0), new Vector3(0, 1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(new Vector3(x, y, 0), new Vector3(0, -1, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(new Vector3(x, y, 0), new Vector3(1, 0, 0) * range, Color.red, 10f, false);
            Debug.DrawRay(new Vector3(x, y, 0), new Vector3(-1, 0, 0) * range, Color.red, 10f, false);

            RaycastHit2D hit1 = Physics2D.Raycast(new Vector2(x, y), new Vector2(0, 1), range, LayerMask.GetMask("WaterLayer"));
            RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(x, y), new Vector2(0, -1), range, LayerMask.GetMask("WaterLayer"));
            RaycastHit2D hit3 = Physics2D.Raycast(new Vector2(x, y), new Vector2(1, 0), range, LayerMask.GetMask("WaterLayer"));
            RaycastHit2D hit4 = Physics2D.Raycast(new Vector2(x, y), new Vector2(-1, 0), range, LayerMask.GetMask("WaterLayer"));

            if (hit1 && hit2 && hit3 && hit4)
            {
                Debug.Log("INSIDE");
            }
            else
            {
                Debug.Log("OUTSIDE");
            }
            

            Debug.Log("AAA" + LayerMask.NameToLayer("Water") + " AAA " + LayerMask.GetMask("WaterLayer"));
        }

    }

    public int x;
    public int y;


    
}
