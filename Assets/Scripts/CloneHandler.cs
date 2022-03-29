using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneHandler : MonoBehaviour
{
    [SerializeField]
    private CloneCar[] cars;

    private List<PosRot> recordingInputs = new List<PosRot>();

    private LapHandler lapHandler;
    private CarMovement car;

    private bool recording = false;

    private void Start()
    {
        car = FindObjectOfType<CarMovement>();
        lapHandler = FindObjectOfType<LapHandler>();
        lapHandler.OnStartLap += LapHandler_OnStartLap;
        lapHandler.OnEndLap += LapHandler_OnEndLap;
    }

    private void LapHandler_OnStartLap()
    {
        recording = true;

        CloneData? cloneData = Save.GetInputsForTrack(GameManager.Instance.GetTrackIndex());
        print("Got Data");
        if (cloneData.HasValue)
        {
            print("It has value");
            Vector3 pos = FindObjectOfType<CarSpawner>().SpawnPosition.position;
            Quaternion rot = FindObjectOfType<CarSpawner>().SpawnPosition.rotation;
            CloneCar car = cars[cloneData.Value.CarIndex];
            CloneCar clone = Instantiate(car, Vector3.zero, Quaternion.identity);
            clone.Drive(cloneData.Value.CarPosRots);
        }
    }

    private void LapHandler_OnEndLap()
    {
        recording = false;
    }

    private void FixedUpdate()
    {
        if (recording)
        {
            recordingInputs.Add(new PosRot(car.transform.position.x, car.transform.position.y, car.transform.position.z, car.transform.eulerAngles.x, car.transform.eulerAngles.y, car.transform.eulerAngles.z));
        }
    }

    public void SaveInputs()
    {
        Save.SaveInputsOnTrack(new CloneData(GameManager.Instance.SavedCarIndex, recordingInputs.ToArray()), GameManager.Instance.GetTrackIndex());
    }

}

[System.Serializable]
public struct CloneData
{
    public int CarIndex;

    public PosRot[] CarPosRots;

    public CloneData(int carIndex, PosRot[] carInputs)
    {
        CarIndex = carIndex;
        CarPosRots = carInputs;
    }
}
