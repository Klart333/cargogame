﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlexibleLayoutGroup : MonoBehaviour
{
    [Header("0 Means Undefined")]
    public int rows = 1;
    public int coloumns = 1;

    [Header("Elastic Clamping")]
    [SerializeField]
    private bool useElasticClamping = true;

    [SerializeField]
    private bool clampingX = false;

    [Header("Restrictions")]
    [SerializeField]
    private bool restrictToSquare = false;

    private RectTransform rect;

    void Start()
    {
        float width = gameObject.GetComponent<RectTransform>().rect.width;
        float height = gameObject.GetComponent<RectTransform>().rect.height;
        GridLayoutGroup layoutGroup = GetComponent<GridLayoutGroup>();

        layoutGroup.cellSize = new Vector2(coloumns == 0 ? layoutGroup.cellSize.x - layoutGroup.spacing.x : (width - (layoutGroup.padding.left + layoutGroup.padding.right)) / coloumns - layoutGroup.spacing.x, rows == 0 ? layoutGroup.cellSize.y - layoutGroup.spacing.y : (height - (layoutGroup.padding.top + layoutGroup.padding.bottom)) / rows - layoutGroup.spacing.y);
        
        if (useElasticClamping)
        {
            rect = transform as RectTransform;

            if (!clampingX)
            {
                rect.offsetMin = new Vector2(rect.offsetMin.x, (transform.childCount * -(layoutGroup.cellSize.y + layoutGroup.spacing.y)) + rect.rect.height);
            }
            else
            {
                rect.offsetMax = new Vector2((transform.childCount * (layoutGroup.cellSize.x + layoutGroup.spacing.x)) - rect.rect.width + layoutGroup.padding.right, rect.offsetMin.y);
            }
        }

        if (restrictToSquare)
        {
            if (rows == 0)
            {
                layoutGroup.cellSize = new Vector2(layoutGroup.cellSize.x, layoutGroup.cellSize.x);
            }
            else if (coloumns == 0)
            {
                layoutGroup.cellSize = new Vector2(layoutGroup.cellSize.y, layoutGroup.cellSize.y);
            }
        }
    }

}
