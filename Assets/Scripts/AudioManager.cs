using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    public void PlayMusic(string name){
        Sound s = Array.Find(musicSounds, x=> x.name == name);

        if(s == null){
            Debug.Log("Sound Not Found");
        }

        else{
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    public void PauseMusic(){
        musicSource.Pause();
    }

    public void ResumeMusic(){
        musicSource.Play();
    }

    public void PlaySFX(string name){
        Sound s = Array.Find(sfxSounds, x=> x.name == name);

        if(s == null){
            Debug.Log("Sound Not Found");
        }

        else{
            sfxSource.PlayOneShot(s.clip);
        }
    }
    
    public void MusicVolume(float volume){
        musicSource.volume = volume;
    }
    
    public void SFXVolume(float volume){
        sfxSource.volume = volume;
    }
}
