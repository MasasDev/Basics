using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField, Range(10,100)] private int resolution = 10;
    [SerializeField] FunctionLibrary.FunctionName functionName;

    private Transform[] points;
    private float range = 2f;

    private void Awake()
    {
        float step = range / resolution;

        Vector3 scale = Vector3.one * step;

        points = new Transform[resolution * resolution];

        for (int i = 0; i < points.Length; i++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);

            point.localScale = scale;
            point.SetParent(transform,false);
        }
    }
    private void Update()
    {
        FunctionLibrary.Function function = FunctionLibrary.GetFunction(functionName);

        float time = Time.time;

        float step = range / resolution;

        float u = 0;
        float v = 0.5f * step - 1;

        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1;
            }
            u = (x + 0.5f) * step - 1;

            Transform point = points[i];

            point.localPosition = function(u, v, time);
        }
    }
}
