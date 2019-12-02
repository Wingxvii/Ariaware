using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponLogic : MonoBehaviour
{
    public ParticleSystem shotFlash;
    public bool playing = false;
    private void Start()
    {
        shotFlash = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    public void StartPlaying()
    {
        playing = true;
        shotFlash.Play();
    }

    public void StopPlaying()
    {
        playing = false;
        shotFlash.Stop();
    }

}
