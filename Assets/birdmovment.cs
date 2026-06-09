using UnityEngine;

public class birdmovment : MonoBehaviour
{
    public float speed = 5;
    public float deathzone = -30;
    public LogicScript logic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + (Vector3.left * speed) * Time.deltaTime;
        
        if (transform.position.x < deathzone)
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Bird murder attempt success");
    }
}