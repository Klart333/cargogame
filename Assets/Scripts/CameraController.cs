using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 FinishFollowOffset;

    [SerializeField]
    private float defaultFov = 70;

    private CinemachineVirtualCamera vcam;
    private LapHandler lapHandler;
    private CinemachineTransposer cinemachineTransposer;
    private CarMovement car;

    private void Start()
    {
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineTransposer = vcam.GetCinemachineComponent<CinemachineTransposer>();
        car = FindObjectOfType<CarMovement>();

        lapHandler = FindObjectOfType<LapHandler>();
        lapHandler.OnEndLap += LapHandler_OnEndLap;
    }

    private void LapHandler_OnEndLap()
    {
        StartCoroutine(WinSequence());
    }

    private void Update()
    {
        vcam.m_Lens.FieldOfView = defaultFov + car.Speed / 2;
    }

    public void SetFollow(Transform gm)
    {
        if (vcam == null)
        {
            vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        }

        vcam.Follow = gm;
        vcam.LookAt = gm;
    }

    public IEnumerator WinSequence()
    {
        float speed = 1;
        float t = 0;

        Vector3 ogOffset = cinemachineTransposer.m_FollowOffset;

        while (t < 1)
        {
            yield return null;

            cinemachineTransposer.m_FollowOffset = Vector3.Lerp(ogOffset, FinishFollowOffset, t);

            t += Time.deltaTime * speed;
        }

        cinemachineTransposer.m_FollowOffset = FinishFollowOffset;
    }
}
