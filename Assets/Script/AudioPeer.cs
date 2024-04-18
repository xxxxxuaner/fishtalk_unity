using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPeer : MonoBehaviour
{
    public AudioSource _audioSource;
    static float[] _samples = new float[512];
    static float[] _freqBand = new float[8];
    static float[] _bandBuffer = new float[8];
    float[] _bufferDecrease = new float[8];

    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];

    public float _audioProfile;





    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource> ();
        AudioProfile(_audioProfile);
    }

    // Update is called once per frame
    void Update()
    {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
        BandBuffer();
        CreateAudioBands(); 
    }

    void AudioProfile(float audioProfile){
        for (int i=0; i<8; i++){
            _freqBandHighest [i] = audioProfile;
        }
    }



    void CreateAudioBands(){
        for (int i=0; i<8; i++){
            if (_freqBand[i] > _freqBandHighest[i]){
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }



    void GetSpectrumAudioSource(){
        _audioSource.GetSpectrumData (_samples, 0, FFTWindow.Blackman);
    }

    void BandBuffer(){
        for (int g=0; g<8; ++g){
            if(_freqBand[g] > _bandBuffer[g]){
                _bandBuffer[g] = _freqBand [g];
                _bufferDecrease[g] = 0.005f;
            }

            if (_freqBand[g]< _bandBuffer[g]){
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void MakeFrequencyBands(){
        /*
        *22050 / 512 = 43hertz per sample
        *20-60hertz
        60-250hertz
        250-500hertz
        500-2000hertz
        2000-4000hertz
        4000-6000hertz
        6000-20000hertz

        0-2 = 86hertz
        1-4 = 172 hertz - 87-258
        2-8 = 344 hertz - 259-602
        3-16 = 688 hertz - 603 -1290
        4-32 = 1376 hertz - 1291-2666
        5-64
        6-128
        7-256
        510
        */

        int count = 0;

        for (int i=0; i<8; i++){

            float average = 0;
            int sampleCount = (int)Mathf.Pow(2,i)*2;

            if (i==7){
                sampleCount +=2;
            }
            for (int j = 0; j < sampleCount; j++) {
                average += _samples[count] * (count +1);
                count++;
            }

            average /= count;
            _freqBand [i] = average*10;
        }
    }
}
