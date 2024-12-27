using JetBrains.Annotations;
using TreeEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static FunctionLibrary;

public class GPUGraph : MonoBehaviour
{
    private const int maxResolution = 1000;

    [SerializeField] private ComputeShader computeShader;

    [SerializeField, Range(10, maxResolution)]
    private int resolution = 10;

    [SerializeField] private FunctionLibrary.FunctionName functionName;
    [SerializeField] private float functionDuration = 1f, transitionDuration;
    [SerializeField] private TransitionMode transitionMode;
    [SerializeField] private Material material;
    [SerializeField] private Mesh mesh;

    private float range = 2f;
    private float duration = 0f;
    private FunctionLibrary.FunctionName transitionFunctionName;
    private bool transitioning;
    private enum TransitionMode { Cyclic, Random };

    private ComputeBuffer positionsBuffer;

    private static readonly int
        positionsID = Shader.PropertyToID("_Positions"),
        resolutionID = Shader.PropertyToID("_Resolution"),
        stepID = Shader.PropertyToID("_Step"),
        timeID = Shader.PropertyToID("_Time"),
        transitionProgressID = Shader.PropertyToID("_TransitionProgress");

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * 4);
    }
    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
    private void Update()
    {
        duration += Time.deltaTime;

        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                duration -= transitionDuration;
                transitioning = false;
            }
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunctionName = functionName;
            PickNextFunctionName();
        }

        UpdateFunctionOnGPU();
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
    private void UpdateFunctionOnGPU()
    {
        float step = range / resolution;

        float transitionProgress = 0;

        var kernelIndex =
            (int)functionName + (int)(transitioning ? transitionFunctionName : functionName) * 5;

        transitionProgress = duration / transitionDuration;

        computeShader.SetInt(resolutionID, resolution);
        computeShader.SetFloat(timeID, Time.time);
        computeShader.SetFloat(stepID, step);
        computeShader.SetBuffer(kernelIndex, positionsID, positionsBuffer);
        computeShader.SetFloat(transitionProgressID, transitionProgress);

        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);

        material.SetBuffer(positionsID, positionsBuffer);
        material.SetFloat(stepID, step);

        Bounds bounds = new Bounds(Vector3.zero, Vector3.one * (range + (range / resolution)));

        Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, resolution * resolution);
    }
}
