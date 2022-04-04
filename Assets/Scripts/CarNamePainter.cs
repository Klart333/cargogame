using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    [SerializeField]
    private TextMeshProUGUI paintAmountText;

    private Camera cam;

    private Vector2 lastMouseCoordinates = Vector2.zero;

    [SerializeField]
    private float paintFrequency = 10; // In pixels

    private int maxPaintAmount = 1000;

    private int PaintAmount { get { return nameParent.childCount; } }

    public bool ShouldDraw { get; set; } = false;

    private void Start()
    {
        cam = Camera.main;
    }

    private void FixedUpdate()
    {
        if (ShouldDraw && Input.GetMouseButton(0) && PaintAmount < maxPaintAmount)
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
                Vector2 screenPos = currentPos + diff * ((float)i / (float)amount);
                TryDrawAtScreenPoint(screenPos);
            }
        }
        else if (ShouldDraw && Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, deleteMask))
            {
                Destroy(hitInfo.collider.gameObject);
            }
        }

        lastMouseCoordinates = Input.mousePosition;
        paintAmountText.text = string.Format("{0}/{1}", PaintAmount, maxPaintAmount);
    }

    private void TryDrawAtScreenPoint(Vector2 screenPos)
    {
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 50, layerMask))
        {
            var rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
            rotation *= Quaternion.Euler(90, 0, 0);
            var gm = Instantiate(paintObject, nameParent);
            gm.transform.position = hitInfo.point;
            gm.transform.rotation = rotation;
        }
    }

    public void ClearPaint()
    {
        for (int i = 0; i < nameParent.childCount; i++)
        {
            Destroy(nameParent.GetChild(i).gameObject);
        }
    }
}
