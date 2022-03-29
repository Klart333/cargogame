using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarNamePainter : MonoBehaviour
{
    [SerializeField]
    private GameObject paintObject;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Transform nameParent;

    [SerializeField]
    private LayerMask deleteMask;

    private Camera cam;

    private Vector2 lastMouseCoordinates = Vector2.zero;

    [SerializeField]
    private float paintFrequency = 10; // In pixels

    public bool ShouldDraw { get; set; } = false;

    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (ShouldDraw && Input.GetMouseButton(0))
        {
            if (lastMouseCoordinates == Vector2.zero)
            {
                lastMouseCoordinates = Input.mousePosition;
            }

            Vector2 currentPos = Input.mousePosition;
            Vector2 diff = lastMouseCoordinates - currentPos;
            if (diff.magnitude < paintFrequency)
            {
                return;
            }

            int amount = Mathf.RoundToInt((float)diff.magnitude / (float)paintFrequency);

            for (int i = 0; i < amount; i++)
            {
                TryDrawAtScreenPoint(currentPos + diff * (i / amount));
            }

            lastMouseCoordinates = Input.mousePosition;
        }
        else if (ShouldDraw && Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, deleteMask))
            {
                Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    private void TryDrawAtScreenPoint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, layerMask))
        {
            var rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
            rotation *= Quaternion.Euler(90, 0, 0);
            var gm = Instantiate(paintObject, hitInfo.point, rotation);
            gm.transform.SetParent(nameParent);
        }
        
        
    }
}
