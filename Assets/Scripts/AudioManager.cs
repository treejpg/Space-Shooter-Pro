using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _laserShot;
    [SerializeField]
    private AudioSource _explosionSound;
    [SerializeField]
    private AudioSource _powerupSound;

    public void LaserShotAudio()
    {
        _laserShot.Play();
    }

    public void ExplosionSoundAudio()
    {
        _explosionSound.Play();
    }

    public void PowerupSoundAudio()
    {
        _powerupSound.Play();
    }
}
