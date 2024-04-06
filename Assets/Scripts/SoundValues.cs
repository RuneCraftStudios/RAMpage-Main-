using UnityEngine;

public class SoundValues : MonoBehaviour
{
    private const int SAMPLE_SIZE = 1024;
    public float rmsValue;
    public float dbValue;
    public float pitchValue;
    public float rmsSensitivity;
    public float dbSensitivity;
    public float pitchSensitivity;
    public float hertzSensitivity = 1f;

    public Renderer[] renderersToUpdate; // Array of Renderer objects

    private AudioSource source;
    private float[] samples;
    private float[] spectrum;
    private float sampleRate;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        samples = new float[SAMPLE_SIZE];
        spectrum = new float[SAMPLE_SIZE];
        sampleRate = AudioSettings.outputSampleRate;
    }

    private void Update()
    {
        AnalyzeSound();
        UpdateShaderProperties();
    }

    private void AnalyzeSound()
    {
        source.GetOutputData(samples, 0);

        float sum = 0;
        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / SAMPLE_SIZE);
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);

        source.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        pitchValue = GetPitchFromSpectrum();
    }

    private float GetPitchFromSpectrum()
    {
        float maxFrequency = 0;
        float maxAmplitude = 0;

        for (int i = 0; i < SAMPLE_SIZE; i++)
        {
            if (spectrum[i] > maxAmplitude)
            {
                maxAmplitude = spectrum[i];
                maxFrequency = i * sampleRate / SAMPLE_SIZE;
            }
        }

        return maxFrequency;
    }

    private void UpdateShaderProperties()
    {
        if (renderersToUpdate != null)
        {
            foreach (Renderer renderer in renderersToUpdate)
            {
                if (renderer != null && renderer.material != null)
                {
                    // Update standard properties (DB, RMS, PITCH)
                    renderer.material.SetFloat("_DB", dbValue * dbSensitivity);
                    renderer.material.SetFloat("_RippleSpeed", rmsValue * rmsSensitivity);
                    renderer.material.SetFloat("_PITCH", pitchValue * pitchSensitivity);

                    // Calculate amplitude values for different frequency ranges
                    float bassAmplitude = CalculateAverageAmplitude(20, 250);
                    float midAmplitude = CalculateAverageAmplitude(250, 1000);
                    float highAmplitude = CalculateAverageAmplitude(1000, 4000);

                    // Set properties for frequency ranges in the material
                    renderer.material.SetFloat("_Bass", bassAmplitude * hertzSensitivity);
                    renderer.material.SetFloat("_Mid", midAmplitude * hertzSensitivity);
                    renderer.material.SetFloat("_High", highAmplitude * hertzSensitivity);
                }
            }
        }
    }

    private float CalculateAverageAmplitude(int startHz, int endHz)
    {
        // Frequency range calculation
        int startIndex = Mathf.FloorToInt(startHz * spectrum.Length / sampleRate);
        int endIndex = Mathf.FloorToInt(endHz * spectrum.Length / sampleRate);

        // Calculate average amplitude within the frequency range
        float sum = 0;
        for (int i = startIndex; i <= endIndex; i++)
        {
            sum += spectrum[i];
        }

        return sum / (endIndex - startIndex + 1);
    }
}
