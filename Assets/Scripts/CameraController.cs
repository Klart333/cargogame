using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Vector3 FinishFollowOffset;

    private CinemachineVirtualCamera vcam;
    private LapHandler lapHandler;
    private CinemachineTransposer cinemachineTransposer;

    private void Start()
    {
        vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineTransposer = vcam.GetCinemachineComponent<CinemachineTransposer>();

        lapHandler = FindObjectOfType<LapHandler>();
        lapHandler.OnEndLap += LapHandler_OnEndLap;
    }

    private void LapHandler_OnEndLap()
    {
        StartCoroutine(WinSequence());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(WinSequence());
        }
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
