using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LearnMgr : MonoBehaviour
{
    GlobalsMgr g;
    public AudioSource audioSource;
    public AudioClip audioClip;
    float[] spectrum;
    float[] spectrum2;
    GameObject goDisplay;
    int numValues = 1024;
    public Texture2D texNew;
    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency1;

    [Range(1, 20000)]  //Creates a slider in the inspector
    public float frequency2;

    public float sampleRate = 44100;
    public float waveLengthInSeconds = 2.0f;
    int timeIndex = 0;

    private void Awake()
    {
        g = GetComponent<GlobalsMgr>();
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        spectrum = new float[numValues];
        spectrum2 = new float[numValues];
        //        UseMicrophone();
        //        UseX();
        //UseClip();
        UseSin();
    }

    // Update is called once per frame
    void Update()
    {
//        UpdateKeyPress();
        LoadDisplay();
        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        audioSource.GetSpectrumData(spectrum, 1, FFTWindow.Rectangular);
        CreateTexture();
    }

    void UpdateKeyPress()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!audioSource.isPlaying)
            {
                timeIndex = 0;  //resets timer before playing sound
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    void UseSin()
    {
        audioSource.clip = null;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0; //force 2D sound
        audioSource.loop = true;
        audioSource.Play(); //avoids audiosource from starting to play automatically
    }

    void UseClip()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
        audioSource.loop = true;
    }

    void UseMicrophone()
    {
        audioSource.clip = Microphone.Start("Built-in Microphone", true, 10, 44100);
        audioSource.loop = true;
        while (!(Microphone.GetPosition(null) > 0)) { }
        audioSource.Play();
    }

    void CreateTexture()
    {
//        goDisplay.GetComponent<Renderer>().material.mainTexture = texNew;
//        goDisplay.GetComponent<Renderer>().material.color = Color.green;
//        return;
        Texture2D texture = new Texture2D(numValues, numValues, TextureFormat.ARGB32, false);
        for(int x = 0; x < numValues; x++)
        {
            int v = (int)Mathf.Round(spectrum[x] * 100000);
            int v2 = (int)Mathf.Round(spectrum2[x] * 100000);
            //Debug.Log(spectrum[x] + "\n");
            int vSum = v + v2;
            for (int y = 0; y < vSum; y++)
            {
                if (v < v2)
                {
                    texture.SetPixel(x, y, Color.black);
                } else
                {
                    texture.SetPixel(x, y, Color.red);
                }
            }
        }
        texture.Apply();
        goDisplay.GetComponent<Renderer>().material.SetTexture("_MainTex", texture);

    }

    void LoadDisplay()
    {
        goDisplay = GameObject.Find("QuadDisplay");
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        for (int i = 0; i < data.Length; i += channels)
        {
            //data[i] = CreateSine(timeIndex, frequency1, sampleRate);

            if (channels == 2)
            {
                data[i + 1] = CreateSine(timeIndex, frequency2, sampleRate);
            }
            timeIndex++;

            //if timeIndex gets too big, reset it to 0
            if (timeIndex >= (sampleRate * waveLengthInSeconds))
            {
                timeIndex = 0;
            }
        }
    }

    //Creates a sinewave
    public float CreateSine(int timeIndex, float frequency, float sampleRate)
    {
        float s1 = Mathf.Sin(2 * Mathf.PI * timeIndex * frequency1 / sampleRate);
        float s2 = Mathf.Sin(2 * Mathf.PI * timeIndex * frequency2 / sampleRate);
        return (s1 + s2) / 2;
    }

}


