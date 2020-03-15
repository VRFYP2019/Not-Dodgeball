using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance = null;
    public AudioFile[] audioSources;
    public Dictionary<string,AudioFile> audioFiles;
    static AudioSource musicPlayer;
    int currBgm;

    void Awake () { 
        InitInstance();
        InitAudioFiles();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.buildIndex == 0) {
            PlayLobbyMusic();
        } else if (scene.buildIndex == 1) {
            PlayRandomMusic();
            GameManager.Instance.RestartEvent.AddListener(PlayRandomMusic);
        }
    }

    private void InitInstance() {
        if (Instance == null) {
            Instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
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
        s.source.PlayOneShot(s.audioClip, s.volume);
    }

    private void PlayLobbyMusic() {
        PlayMusic("sportsbgm3");
        currBgm = 3;
    }

    private void PlayRandomMusic() {
        int bgmToPlay = Random.Range(0, 3);
        while (bgmToPlay == currBgm) {
            bgmToPlay = Random.Range(0, 3);
        }
        currBgm = bgmToPlay;
        switch (bgmToPlay) {
            case 0:
                PlayMusic("sportsbgm0");
                break;
            case 1:
                PlayMusic("sportsbgm1");
                break;
            case 2:
                PlayMusic("sportsbgm2");
                break;
        }
    }

    public static void PlayMusic (string name) {
        if (!Instance.audioFiles.ContainsKey(name)) {
            Debug.Log("Music file " + name + " not found");
            return;
        }

        AudioFile s = Instance.audioFiles[name];
        if (musicPlayer == null) {
            musicPlayer = s.source;
            musicPlayer.Play();
        } else if (musicPlayer != s.source) {
            musicPlayer.Stop();
            musicPlayer = s.source;
            musicPlayer.Play();
        }
    }

}
