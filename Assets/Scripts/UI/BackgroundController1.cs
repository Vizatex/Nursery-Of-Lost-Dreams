using UnityEngine;

public class BackgroundController1 : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxFactor = 0.5f;

    private Vector3 lastCamPos;
    private Vector2 tileSize;
    private Transform[] tiles;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCamPos = cameraTransform.position;

        tiles = new Transform[transform.childCount];
        for (int i = 0; i < tiles.Length; i++)
            tiles[i] = transform.GetChild(i);

        SpriteRenderer sr = tiles[0].GetComponent<SpriteRenderer>();
        tileSize = sr.bounds.size;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCamPos;
        transform.position += new Vector3(delta.x * parallaxFactor, delta.y * parallaxFactor, 0);
        lastCamPos = cameraTransform.position;

        foreach (Transform tile in tiles)
        {
            Vector3 camPos = cameraTransform.position;
            Vector3 tilePos = tile.position;

            float xDist = camPos.x - tilePos.x;
            float yDist = camPos.y - tilePos.y;

            if (xDist >= tileSize.x)
                tile.position += new Vector3(tileSize.x * 3, 0, 0);
            else if (xDist <= -tileSize.x)
                tile.position -= new Vector3(tileSize.x * 3, 0, 0);

            if (yDist >= tileSize.y)
                tile.position += new Vector3(0, tileSize.y * 3, 0);
            else if (yDist <= -tileSize.y)
                tile.position -= new Vector3(0, tileSize.y * 3, 0);
        }
    }
}
