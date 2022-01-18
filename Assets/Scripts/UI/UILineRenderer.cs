using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    public List<Vector2> Points;

    public Vector2Int GridSize;
    public UIGridRenderer grid;

    public float Thickness = 10f;

    public float pointFrequency = 0.01f;

    private float width;
    private float height;
    private float unitWidth;
    private float unitHeight;

    private VertexHelper vertexHelper = new VertexHelper();

    private void Update()
    {
        if (grid != null)
        {
            if (GridSize != grid.gridSize)
            {
                GridSize = grid.gridSize;
            }
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        width = rectTransform.rect.width;
        height = rectTransform.rect.height;

        unitWidth = width / (float)GridSize.x;
        unitHeight = height / (float)GridSize.y;

        if (Points.Count < 2)
        {
            return;
        }

        float angle = 0;

        for (int i = 0; i < Points.Count; i++)
        {
            Vector2 point = Points[i];

            if (i < Points.Count - 1)
            {
                angle = GetAngle(Points[i], Points[i + 1]) + 45f;
            }

            DrawVecticesForPoint(point, vh, angle);
        }

        for (int i = 0; i < Points.Count - 1; i++)
        {
            int index = i * 2;
            vh.AddTriangle(index + 0, index + 1, index + 3);
            vh.AddTriangle(index + 3, index + 2, index + 0);
        }

    }

    public void AddPoint(float percentY)
    {
        Vector2 pos = Vector2.zero;
        float y = ((float)GridSize.y * (float)percentY) - ((float)GridSize.y / 2.0f);
        if (Points.Count > 0)
        {
            pos = new Vector2(Points[Points.Count - 1].x + pointFrequency, y);
        }
        else
        {
            pos = new Vector2(-(float)GridSize.x / 2.0f, y);
        }

        Points.Add(pos);
        SetAllDirty();
    }

    public void MovePoint(int index, Vector2 position)
    {
        Points[index] = position;
    }

    public void RemovePoint(int index)
    {
        Points.RemoveAt(index);
    }

    public void RemovePoint(Vector2 position)
    {
        Points.Remove(position);
    }

    public float GetAngle(Vector2 me, Vector2 target)
    {
        return (float)(Mathf.Atan2(target.y -me.y, target.x - me.x) * (180.0f / Mathf.PI));
    }

    private void DrawVecticesForPoint(Vector2 point, VertexHelper vh, float angle)
    {
        UIVertex vertex = UIVertex.simpleVert;
        vertex.color = color;

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-Thickness / 2.0f, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);

        vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(Thickness / 2.0f, 0);
        vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
        vh.AddVert(vertex);
    }

    
}
