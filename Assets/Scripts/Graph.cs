using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class Graph : MonoBehaviour
{
    [SerializeField] private Transform pointPrefab;
    [SerializeField, Range(10,100)] private int resolution = 10;
    [SerializeField] FunctionLibrary.FunctionName functionName;
    [SerializeField] private float functionDuration = 1f, transitionDuration;
    [SerializeField] private TransitionMode transitionMode; 

    private Transform[] points;
    private float range = 2f;
    private float duration = 0f;
    private FunctionLibrary.FunctionName transitionFunctionName;
    private bool transitioning;

    private enum TransitionMode { Cyclic, Random};

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
        duration += Time.deltaTime;

        if(transitioning)
        {
            if(duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if(duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunctionName = functionName;
            PickNextFunctionName();
        }
        
        if(transitioning)
        {
            UpdateFunctionTransition();
        }
        else
        {
            UpdateFunction();
        }
    }

    private void PickNextFunctionName()
    {
        if (transitionMode == TransitionMode.Random)
        {
            functionName = FunctionLibrary.GetRandomFunctionName(functionName);
        }
        else if (transitionMode == TransitionMode.Cyclic)
        {
            functionName = FunctionLibrary.GetNextFunctionName(functionName);
        }
    }

    private void UpdateFunction()
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
    private void UpdateFunctionTransition()
    {
        FunctionLibrary.Function from = FunctionLibrary.GetFunction(transitionFunctionName),
            to = FunctionLibrary.GetFunction(functionName);

        float progress = duration / transitionDuration;

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

            point.localPosition = FunctionLibrary.Morph(u, v, time, from, to, progress);
        }
    }
}
