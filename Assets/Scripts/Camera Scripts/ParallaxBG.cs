using UnityEngine;

public class ParallaxBG : MonoBehaviour
{
    private float lenght;
    private float startPosition;
    public GameObject cam;
    public float parallaxEffect;
    private float dist;
    private float temp;

    void Start()
    {
        startPosition = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {

        temp = (cam.transform.position.x * (1 - parallaxEffect));
        dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startPosition + dist, transform.position.y, transform.position.z);

        if (temp > startPosition + lenght)
            startPosition += lenght;
        else if(temp < startPosition - lenght)
            startPosition -= lenght;
    }
}
