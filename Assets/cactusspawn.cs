using UnityEngine;

public class cactusspawn : MonoBehaviour
{
    public GameObject cactusprefab;
    public float spawnrate = 2;
    private float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawncactus();
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnrate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            spawncactus();
            timer = 0;
        }
    }
    void spawncactus()
    {
        Instantiate(cactusprefab, transform.position, transform.rotation);
    }
}
