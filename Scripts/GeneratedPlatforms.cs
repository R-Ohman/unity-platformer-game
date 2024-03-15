using System.Collections;
using UnityEngine;

public class GeneratedPlatforms : MonoBehaviour
{
    [SerializeField] private GameObject platformPrefab;
    private const int PLATFORMS_NUM = 10;
    private GameObject[] platforms;
    private Vector3[] positions;
    private float speed = 2.0f; // Pr?dko?? ruchu platformy

    void Awake()
    {
        platforms = new GameObject[PLATFORMS_NUM];
        positions = new Vector3[PLATFORMS_NUM];

        GeneratePlatforms();
    }

    void GeneratePlatforms()
    {
        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            float xPosition = i * 2.0f + 23.0f;
            float yPosition = Mathf.Sin(i * 0.5f) * 2.0f;

            positions[i] = new Vector3(xPosition, yPosition, 0f);
            platforms[i] = Instantiate(platformPrefab, positions[i], Quaternion.identity);
        }
    }

    void Update()
    {
        MovePlatforms();
    }

    void MovePlatforms()
    {
        for (int i = 0; i < PLATFORMS_NUM; i++)
        {
            // Przyk?ad: Ruch sinusoidalny w g?r? i w d??
            float newYPosition = Mathf.Sin(Time.time * speed + i) * 2.0f + 1.0f;

            // Ustaw now? pozycj? platformy za pomoc? Vector3.MoveTowards
            platforms[i].transform.position = Vector3.MoveTowards(platforms[i].transform.position, new Vector3(platforms[i].transform.position.x, newYPosition, platforms[i].transform.position.z), speed * Time.deltaTime);
        }
    }
}