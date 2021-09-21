using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class Card : MonoBehaviour
{
    Camera MainCamera;
    Vector3 offset;

    public GameObject Prefab;

    private void Awake()
    {
        MainCamera = Camera.allCameras[0];
    }


    private void OnMouseDown()
    {
        offset = transform.position - MainCamera.ScreenToWorldPoint(Input.mousePosition);
        offset.z = 0;
    }

    private void OnMouseDrag()
    {
        Vector3 newPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0;
        transform.position = newPos + offset;
    }

    private void OnMouseUp()
    {
        CreateObject();
        Destroy(gameObject);
    }

    private void CreateObject()
    {
        // TilemapsManager.TilemapStructure.SetTile(TilemapsManager.TilemapStructure.WorldToCell(transform.position), Prefab.GetComponent<Structure>().structureTile);   // TODO разная механика создания юнита и структуры
    }
}
