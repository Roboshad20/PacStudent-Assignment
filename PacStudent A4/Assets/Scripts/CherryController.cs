using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CherryController : MonoBehaviour
{
    public GameObject cherryPrefab;
    public float moveSpeed = 2f;
    public float spawnDelay = 5f;
    public Vector2 moveDirection;
    public float offscreenDistance = 20f;
    public float mapWidth = 15f;
    public float mapHeight = 19f;

    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(spawnDelay);
        SpawnCherry();
    }

    void SpawnCherry()
    {
        int side = Random.Range(0, 4);
        Vector3 levelCenter = Vector3.zero;

        switch (side)
        {
            case 0:
                startPos = new Vector3(-mapWidth / 2 - offscreenDistance, Random.Range(-mapHeight / 2, mapHeight / 2), 0);
                endPos = new Vector3(mapWidth / 2 + offscreenDistance, startPos.y, 0);
                break;
            case 1:
                startPos = new Vector3(mapWidth / 2 + offscreenDistance, Random.Range(-mapHeight / 2, mapHeight / 2), 0);
                endPos = new Vector3(-mapWidth / 2 - offscreenDistance, startPos.y, 0);
                break;
            case 2:
                startPos = new Vector3(Random.Range(-mapWidth / 2, mapWidth / 2), mapHeight / 2 + offscreenDistance, 0);
                endPos = new Vector3(startPos.x, -mapHeight / 2 - offscreenDistance, 0);
                break;
            case 3:
                startPos = new Vector3(Random.Range(-mapWidth / 2, mapWidth / 2), -mapHeight / 2 - offscreenDistance, 0);
                endPos = new Vector3(startPos.x, mapHeight / 2 + offscreenDistance, 0);
                break;
        }

        GameObject cherry = Instantiate(cherryPrefab, startPos, Quaternion.identity);
        CherryMover mover = cherry.AddComponent<CherryMover>();
        mover.startPos = startPos;
        mover.endPos = endPos;
        mover.moveSpeed = moveSpeed;
        mover.onDestroyed = () => {
            if (this != null)
                StartCoroutine(SpawnDelay());
        };
    }
}

public class CherryMover : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float moveSpeed;
    public System.Action onDestroyed;

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime * moveSpeed;
        transform.position = Vector3.Lerp(startPos, endPos, timer / 10f);

        if (Vector3.Distance(transform.position, endPos) < 0.1f)
        {
            onDestroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
