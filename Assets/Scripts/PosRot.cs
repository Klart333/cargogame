[System.Serializable]
public struct PosRot
{
    public float PosX;
    public float PosY;
    public float PosZ;

    public float RotX;
    public float RotY;
    public float RotZ;

    public PosRot(float posX, float posY, float posZ, float rotX, float rotY, float rotZ)
    {
        PosX = posX;
        PosY = posY;
        PosZ = posZ;
        RotX = rotX;
        RotY = rotY;
        RotZ = rotZ;
    }
}
