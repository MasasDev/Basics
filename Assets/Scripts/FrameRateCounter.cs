using TMPro;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI frameRateCounterText;

    [SerializeField, Range(0.1f, 2f)]
    private float sampleDuration = 1f;

    [SerializeField] DisplayMode mode;

    private float duration, bestDuration = float.MaxValue, worstDuration = 0;
    private int frames;

    public enum DisplayMode { FPS, MS}

    private void Awake()
    {
        duration = Time.unscaledDeltaTime;
    }
    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;

        frames += 1;
        duration += frameDuration;

        if(frameDuration < bestDuration)
        {
            bestDuration = frameDuration;
        }
        if(frameDuration > worstDuration)
        {
            worstDuration = frameDuration;
        }

        if(duration >= sampleDuration)
        {
            if(mode == DisplayMode.FPS)
            {
                frameRateCounterText.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", 1f / bestDuration, frames / duration, 1f / worstDuration);
                frames = 0;
                duration = 0;
                bestDuration = float.MaxValue;
                worstDuration = 0;
            }
            else
            {
                frameRateCounterText.SetText("MS\n{0:1}\n{1:1}\n{2:1}", 1000f * bestDuration, 1000f * duration / frames, 1000f *  worstDuration);
                frames = 0;
                duration = 0;
                bestDuration = float.MaxValue;
                worstDuration = 0;
            }

            
            
        }

    }
}
