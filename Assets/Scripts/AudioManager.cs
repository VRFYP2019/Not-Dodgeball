using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance = null;
    public AudioFile[] audioSources;
    public Dictionary<string,AudioFile> audioFiles;
    static AudioSource musicPlayer;

    void Awake () { 
        InitInstance();
        InitAudioFiles();
    }

    private void InitInstance() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Destroy (gameObject);
        }
        DontDestroyOnLoad (gameObject);        
    }

    private void InitAudioFiles() {
        audioFiles = new Dictionary<string, AudioFile> ();
        foreach (AudioFile s in audioSources) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;
            s.source.loop = s.isLooping;
            audioFiles.Add (s.audioName, s);
        }
    }

    void Update () {
        // TODO: Handle music pause / stop
    }
    
    // No 3D audio
    public static void PlaySoundOnce (string name) {
        if (!Instance.audioFiles.ContainsKey (name)) {
            return;
        }

        AudioFile s = Instance.audioFiles[name];
        s.source.PlayOneShot(s.audioClip);
    } 

    public static void PlayMusic (string name) {
        if (musicPlayer.clip == null || musicPlayer.clip.name != name) {
            musicPlayer.clip = Instance.audioFiles[name].audioClip;
            musicPlayer.Stop ();
            musicPlayer.loop = true;
            musicPlayer.Play ();
        } else {
            musicPlayer.loop = true;
            musicPlayer.Play ();
        }
    }

}
