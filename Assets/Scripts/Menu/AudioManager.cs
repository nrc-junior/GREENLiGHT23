using UnityEngine;
using UnityEngine.Audio;

public class AudioManager: MonoBehaviour
{
    public AudioMixer audioMixer;


    void Start()
    {
        if(PlayerPrefs.HasKey("MasterVol"))
        {
            audioMixer.SetFloat("MasterVol", PlayerPrefs.GetFloat("MasterVol"));
        }

        if (PlayerPrefs.HasKey("MusicaVol"))
        {
            audioMixer.SetFloat("MusicaVol", PlayerPrefs.GetFloat("MusicaVol"));
        }

        if (PlayerPrefs.HasKey("EfeitosVol"))
        {
            audioMixer.SetFloat("EfeitosVol", PlayerPrefs.GetFloat("EfeitosVol"));
        }

    }

}
