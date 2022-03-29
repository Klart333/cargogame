using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintParent : MonoBehaviour
{
    [SerializeField]
    private GameObject paintPrefab;

    private void Start()
    {
        SpawnPaint();   
    }

    public void SavePaint()
    {
        List<PosRot> posRots = new List<PosRot>();
        for (int i = 0; i < transform.childCount; i++)
        {
            posRots.Add(new PosRot(transform.GetChild(i).localPosition.x, transform.GetChild(i).localPosition.y, transform.GetChild(i).localPosition.z, transform.GetChild(i).localEulerAngles.x, transform.GetChild(i).localEulerAngles.y, transform.GetChild(i).localEulerAngles.z));
        }

        Save.SavePaint(posRots);
    }

    public void SpawnPaint()
    {
        List<PosRot> posRots = Save.GetPaint();

        if (posRots == null)
        {
            return;
        }

        for (int i = 0; i < posRots.Count; i++)
        {
            var gm = Instantiate(paintPrefab, transform);
            gm.transform.localPosition = new Vector3(posRots[i].PosX, posRots[i].PosY, posRots[i].PosZ);
            gm.transform.localRotation = Quaternion.Euler(posRots[i].RotX, posRots[i].RotY, posRots[i].RotZ);
        }
    }

}
