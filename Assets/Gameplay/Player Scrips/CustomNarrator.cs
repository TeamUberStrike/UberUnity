using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNarrator : MonoBehaviour
{
    AudioSource audioSource;
    float volume = 2f;
  
    public AudioClip[] takeDamageSounds;
    public AudioClip[] gotKillSounds;

    public AudioClip[] spawnSounds;
    public AudioClip[] deathSounds;

    public AudioClip[] meleeActivateSounds;
    public AudioClip[] weaponActivateSounds;
    public AudioClip[] sniperWeaponActivateSounds;
    public AudioClip[] machineWeaponActivateSounds;

    public AudioClip[] hitEnemySounds;

    public AudioClip[] randomEmotes;

    public AudioClip rapidFireLoop;
    public AudioClip shotgunFireSound;

    public AudioClip noAmmoSound;

    private bool dead = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Play(spawnSounds[Random.Range(0, spawnSounds.Length)]);
        InvokeRepeating("RandomEmote", 14f, 14f);
    }

    void Play(AudioClip a)
    {
        if(!dead)audioSource.PlayOneShot(a,volume);
        volume = 2f;
    }

    void TakeDamage()
    {
        Play(takeDamageSounds[Random.Range(0,takeDamageSounds.Length)]);
    }

    void GotKill()
    {
        audioSource.Stop();
        Play(gotKillSounds[Random.Range(0, gotKillSounds.Length)]);
    }

    void Die()
    {
        audioSource.Stop();
        Play(deathSounds[Random.Range(0, deathSounds.Length)]);
        dead = true;
    }

    void WeaponActivate(string type)
    {
        if (audioSource == null) return;
        if (audioSource.isPlaying) return;

        AudioClip[] a;
        if (type == "Sniper") a = sniperWeaponActivateSounds;
        else if (type == "Machinegun") a = machineWeaponActivateSounds;
        else if (type == "Melee") a = meleeActivateSounds;
        else a = weaponActivateSounds;

        Play(a[Random.Range(0, a.Length)]);
    }

    void OutOfAmmo()
    {
        if (audioSource == null) return;
        audioSource.Stop();
        Play(noAmmoSound);
    }

    void RandomEmote()
    {
        if (audioSource == null) return;
        if (audioSource.isPlaying) return;
        Play(randomEmotes[Random.Range(0, randomEmotes.Length)]);
    }

    void HitEnemy()
    {
        if (audioSource == null) return;
        if (audioSource.isPlaying) return;
        Play(hitEnemySounds[Random.Range(0, hitEnemySounds.Length)]);
    }

    void PrimaryFire(string type)
    {
        if (type == "Shotgun") Play(shotgunFireSound);        
    }

    void RapidFire()
    {        
        if (!audioSource.isPlaying)
        {
            volume = 3f;
            Play(rapidFireLoop);
        }     
    }
}
