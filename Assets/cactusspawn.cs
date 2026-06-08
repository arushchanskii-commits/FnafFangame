using UnityEngine;

public class cactusspawn : MonoBehaviour
{
    public GameObject cactusprefab;
    public GameObject birdprefab;
    public float spawnrate = 2;
    private float timer = 0;
    private int randomnumber;
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
            randomnumber = Random.Range(1, 4);
            
            if (randomnumber == 3)
            {
                spawnbird();
            }
            else
            {
                spawncactus();
            }
         
            timer = 0;
        }
    }
    void spawncactus()
    {
        Instantiate(cactusprefab, transform.position, transform.rotation);
    }
    void spawnbird()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
        Instantiate(birdprefab, spawnPosition, transform.rotation);
    }
}
