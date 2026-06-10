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
        GameObject spawned = Instantiate(cactusprefab, transform.position, Quaternion.identity);
        NormalizeSpawnedObject(spawned);
    }

    void spawnbird()
    {
        Vector3 spawnPosition = transform.position + new Vector3(0, 1f, 0);
        GameObject spawned = Instantiate(birdprefab, spawnPosition, Quaternion.identity);
        NormalizeSpawnedObject(spawned);
    }

    private void NormalizeSpawnedObject(GameObject spawned)
    {
        if (spawned == null)
            return;

        spawned.transform.rotation = Quaternion.identity;
        spawned.transform.localScale = new Vector3(
            Mathf.Abs(spawned.transform.localScale.x),
            Mathf.Abs(spawned.transform.localScale.y),
            Mathf.Abs(spawned.transform.localScale.z));
    }
}
