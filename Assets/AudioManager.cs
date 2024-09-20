using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource Radio, Mouth,Mought2;
    [SerializeField] AudioClip CatchPhrase, Intro;
    [SerializeField] AudioClip[] ShootingTime, grenade;
    [SerializeField] AudioClip[] GotHit,specialAttacks;

    [SerializeField] bool Test = false;
    // Start is called before the first frame update
    void Start()
    {
        Radio.playOnAwake = Radio.loop = false;
        Mouth.playOnAwake = Mouth.loop = false;
    }


    internal void PlayRadio(AudioClip clip,float volume)
    {
        //Debug.LogError("PLayRadioClientRpc onplay"+ Mouth.isPlaying+"  "+Radio.isPlaying );
        //Debug.LogError("PLayRadioClientRpc onplay"+ clip );
        //Debug.LogError("PLayRadioClientRpc onplay"+ volume );

        Radio.Stop();
        Radio.clip = clip;
        Radio.volume = volume;
        Radio.Play();
    }

    private void FixedUpdate()
    {
        if(Test)
        {
            Test = false;
            PlaygettingHit();
        }
    }

    internal void SetDialogue(AudioClip clip,float volume, bool Overlap=false)
    {
        if (Mouth.isPlaying && !Overlap) return;

        Mouth.Stop();
        Mouth.clip = clip;
        Mouth.volume = volume;
        Mouth.Play();
    }

    internal void PlayCatchPhrase()
    {
        SetDialogue(CatchPhrase, .8f);
    }
    
    internal void PlayIntro()
    {
        SetDialogue(Intro, .8f);
    }
    
    internal void PlayShooting()
    {
        SetDialogue(ShootingTime[Random.Range(0, ShootingTime.Length)], .8f);
    }

    
    
    internal void PlaygettingHit()
    {
        if (Mouth.isPlaying) return;
        Mought2.Stop();
        Mought2.clip = GotHit[Random.Range(0, GotHit.Length)];
        Mought2.volume = .8f;
        Mought2.Play();
        //SetDialogue(GotHit[Random.Range(0,GotHit.Length)], .8f,true);
    }
    
    internal void PlayGrenade()
    {
        SetDialogue(grenade[Random.Range(0, grenade.Length)], .8f);
    }

    internal void PlaySpecialAttacks(int i)
    {
        Debug.LogError("Here");
        SetDialogue(specialAttacks[i], 1f,true);
    }
    
}
