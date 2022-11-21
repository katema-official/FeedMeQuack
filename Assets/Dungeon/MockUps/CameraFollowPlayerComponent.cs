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
            if (gtest.transform.Find("Water").gameObject.GetComponent<CompositeCollider2D>().OverlapPoint(new Vector2(x, y)))
            {
                Debug.Log("OOO point is inside collider");
            }
        }

    }

    public int x;
    public int y;
    public GameObject gtest;


    
}
