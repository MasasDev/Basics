using UnityEngine;
using UnityEngine.Rendering;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField, Range(10, 100)] private int resolution = 10;

    private Transform[] points;

    private void Awake()
    {
        Vector3 position = Vector3.zero;

        float range = 2f;

        Vector3 scale = Vector3.one * (range / resolution);

        float xOffset = 0.5f;

        points = new Transform[resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);

            position.x = (i + xOffset) * (range/resolution) - 1;

            point.localPosition = position;
            point.localScale = scale;
            point.SetParent(transform,false);
        }
    }
    private void Update()
    {
        float time = Time.time;

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i];

            Vector3 position = point.localPosition;

            position.y = Mathf.Sin(Mathf.PI * (position.x + time));

            point.localPosition = position;
        }
    }
}
