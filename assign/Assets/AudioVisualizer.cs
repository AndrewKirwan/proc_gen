using UnityEngine;
using System.Collections;

public class AudioVisualizer : MonoBehaviour
{

    int qSamples = 1024;
    float refValue = 0.1f;
    float threshold = 0.02f;
    float rmsValue, dbValue, pitchValue, fSample;
    public float dbMap, pitchMap, rmsMap;
    public AudioSource audioSrc;
    Vector3 sampleScale;
    Color colourStart, colourEnd;
    float oldScale = 1;
    float rate = 1;

    private float[] samples;
    private float[] spectrum;

    // Use this for initialization
    void Start()
    {
        colourStart = new Color(Random.value, Random.value, Random.value);
        colourEnd = Color.blue;
        audioSrc = GetComponent<AudioSource>();
        samples = new float[qSamples];
        spectrum = new float[qSamples];
        fSample = AudioSettings.outputSampleRate;
        //display = GetComponent<GUIText>();
    }

    void AnalyzeSound()
    {
        audioSrc.GetOutputData(samples, 0);
        int i;
        float sum = 0.0f;
        for (i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i];
        }
        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        if (dbValue < -160)
        {
            dbValue = -160;
        }
        audioSrc.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float maxV = 0.0f;
        int maxN = 0;
        for (i = 0; i < qSamples; i++)
        {
            if (spectrum[i] > maxV && spectrum[i] > threshold)
            {
                maxV = spectrum[i];
                maxN = i;
            }
        }
        float freqN = maxN;
        if (maxN > 0 && maxN < qSamples - 1)
        {
            var dL = spectrum[maxN - 1] / spectrum[maxN];
            var dR = spectrum[maxN + 1] / spectrum[maxN];
            freqN += 0.5f * (dR * dR - dL * dL);
        }
        pitchValue = freqN * (fSample / 2) / qSamples;
    }

    public static float Map(float value, float r1, float r2, float m1, float m2)
    {
        float dist = value - r1;
        float range1 = r2 - r1;
        float range2 = m2 - m1;
        return m1 + ((dist / range1) * range2);
    }

    //GUIText display;

    // Update is called once per frame
    void Update()
    {
        float i = 0;
        if (Input.GetKeyDown("p"))
        {
            audioSrc.Play();
        }
        AnalyzeSound();
        //if (display)
        //{
        //    display.text = "RMS: " + rmsValue.ToString("F2") +
        //    " (" + dbValue.ToString("F1") + " dB\n" +
        //    "Pitch: " + pitchValue.ToString("F0") + " Hz";
        //}
        //Debug.Log("RMS: " + rmsValue.ToString("F2") +
        //    " (" + dbValue.ToString("F1") + " dB\n" +
        //    "Pitch: " + pitchValue.ToString("F0") + " Hz");

        dbMap = Map(dbValue, -10, 10, -3f, 8);
        pitchMap = Map(pitchValue, 0, 1200, -3f, 8);
        rmsMap = Map(rmsValue, 0, 0.3f, -3f, 8);

        sampleScale = new Vector3(rmsMap, dbMap, pitchMap);

        i += Time.deltaTime * rate;
        GetComponent<Renderer>().material.color = Color.Lerp(colourStart, colourEnd, i);

        if(i >= 1) {
            i = 0;
            colourStart = GetComponent<Renderer>().material.color;
            if(colourStart == Color.green)
            {
                colourEnd = Color.blue;
            }
            else
            {
                colourEnd = Color.green;
            }
        }

        if (pitchValue != 0)
        {
            transform.localScale = Vector3.Lerp (transform.localScale, sampleScale, 2.0f * Time.deltaTime);
            oldScale = pitchMap;

        }
        //else
        //{
        //    transform.localScale = new Vector3(rmsMap, dbMap, oldScale);
        //}
       

    }
}