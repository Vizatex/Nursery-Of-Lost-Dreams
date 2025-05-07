using UnityEngine;

public class ParallaxLooper : MonoBehaviour
{
    public GameObject cam;
    public float parallaxEffect = 0.5f;

    private Vector2 startPos;
    private float lengthX;

    void Start()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        lengthX = sr.bounds.size.x;
        startPos = transform.position;
    }

    void LateUpdate()
    {
        float camX = cam.transform.position.x;
        float distX = camX * parallaxEffect;
        float tempX = camX * (1 - parallaxEffect);

        transform.position = new Vector3(startPos.x + distX, transform.position.y, transform.position.z);

        if (tempX > startPos.x + lengthX)
            startPos.x += lengthX;
        else if (tempX < startPos.x - lengthX)
            startPos.x -= lengthX;
    }
}
