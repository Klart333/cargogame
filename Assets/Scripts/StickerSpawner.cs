using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class StickerSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] stickers;

    private List<GameObject> spawnedStickers = new List<GameObject>();

    private new Collider collider;
    private MeshRenderer meshRenderer;

    private Vector2 xBounds;
    private Vector2 zBounds;
    private Vector2 yBounds;

    private float offset = 0.75f;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();

        xBounds = new Vector2(collider.bounds.min.x + offset, collider.bounds.max.x - offset);
        zBounds = new Vector2(collider.bounds.min.z + offset, collider.bounds.max.z - offset);
        yBounds = new Vector2(collider.bounds.min.y + offset, collider.bounds.max.y - offset);

        SpawnStickers();
    }

    public void SpawnStickers()
    {
        for (int i = 0; i < spawnedStickers.Count; i++)
        {
            Destroy(spawnedStickers[i].gameObject);
        }
        spawnedStickers.Clear();

        int[] stickers = Save.GetStickers();

        if (stickers.Length == 0)
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = false;
            }
            
            return;
        }
        else
        {
            if (meshRenderer != null)
            {
                meshRenderer.enabled = true;
            }
        }

        for (int i = 0; i < stickers.Length; i++)
        {
            SpawnSticker(stickers[i]);
        }
    }

    private void SpawnSticker(int index)
    {
        RaycastHit hitInfo;
        int evaq = 0;
        bool valid;
        do
        {
            float randX = UnityEngine.Random.Range(xBounds.x, xBounds.y);
            float randZ = UnityEngine.Random.Range(zBounds.x, zBounds.y);
            float randY = UnityEngine.Random.Range(yBounds.x, yBounds.y);
            Vector3 randomPos = new Vector3(randX, randY, randZ);
            randomPos += transform.up * 5;

            valid = Physics.Raycast(randomPos, -transform.up, out hitInfo, 10);
        } while (!valid && evaq++ < 10);

        if (valid)
        {
            var targetRot = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
            targetRot *= Quaternion.Euler(90, 0, 0);
            spawnedStickers.Add(Instantiate(stickers[index], hitInfo.point + hitInfo.normal * 0.1f, targetRot));
        }
    }
}
