using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneCar : MonoBehaviour
{
    private CarMovement carMovement;

    private PosRot[] posRots;

    private bool driving = false;
    private int frame = 0;

    public void Drive(PosRot[] _posRots)
    {
        posRots = _posRots;
        driving = true;
    }

    private void FixedUpdate()
    {
        if (driving && frame < posRots.Length)
        {
            transform.position = new Vector3(posRots[frame].PosX, posRots[frame].PosY, posRots[frame].PosZ);
            transform.rotation = Quaternion.Euler(posRots[frame].RotX, posRots[frame].RotY, posRots[frame].RotZ);

            frame += 1;
        }
    }
}
