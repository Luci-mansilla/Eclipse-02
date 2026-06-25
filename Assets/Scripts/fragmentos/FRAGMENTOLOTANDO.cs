using UnityEngine;

public class FRAGMENTOLOTANDO : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float floatHeight = 0.2f;

    public float tiltAmount = 5f;
    public float tiltSpeed = 1f;

    private Vector3 startPos;   
     void Start()
    {
          startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;

        transform.position = new Vector3(
            startPos.x,
            newY,
            startPos.z
        );

        float tiltZ = Mathf.Sin(Time.time * tiltSpeed) * tiltAmount;

        transform.rotation = Quaternion.Euler(0f, 0f, tiltZ);
    }
}
