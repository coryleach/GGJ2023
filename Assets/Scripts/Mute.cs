using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mute : MonoBehaviour
{
    public AudioListener audiolistener; // Can drag and drop gameobj with audiolistner on it
    public bool mute;
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        audiolistener = GetComponent<AudioListener>(); // This script needs to be on the same gameobject as audiolistner
    }


    public void OnMute()
    {
        mute = !mute;
        AudioListener.volume = System.Convert.ToSingle(mute);
    }

    public void UpdateVolume()
    {
        AudioListener.volume = volumeSlider.value;
    }
}
