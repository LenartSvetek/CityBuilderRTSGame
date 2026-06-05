using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]
public enum BackgroundMusicType
{
    Ambient = 0,
    Combat = 1
}

public class BackgroundMusic : MonoBehaviour
{
    OrchestratorScript _orch;

    AudioSource _oldSource;
    AudioSource _source;

    [SerializeField]
    List<AudioClip> ambientMusic = new ();

    [SerializeField]
    List<AudioClip> combatMusic = new();

    [SerializeField]
    BackgroundMusicType type = BackgroundMusicType.Ambient;

    [SerializeField]
    [Tooltip("In ms")]
    float bufferTime = 300f;
    bool hasTriggeredEnding = false;

    [SerializeField]
    [Tooltip("In s")]
    float combatMusicDuration = 15f;
    float combatMusicTimer = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _orch = GetComponent<OrchestratorScript>();
        _orch.OnBuildingAttacked += OnAttacked;

        _oldSource = gameObject.AddComponent<AudioSource>();
        _source = gameObject.AddComponent<AudioSource>();

        _oldSource.spatialBlend = 0f;
        _source.spatialBlend = 0f;
        
        if(ambientMusic.Count > 0)
        {
            _source.clip = ambientMusic[0];
            _source.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        combatMusicTimer -= Time.unscaledDeltaTime;
        if (type == BackgroundMusicType.Combat && combatMusicTimer <= 0)
        {
            type = BackgroundMusicType.Ambient;
            OnMusicAboutToEnd();
        }

        if(_source.isPlaying && _source != null)
        {
            float timeRemaining = _source.clip.length - _source.time;

            if (timeRemaining <= bufferTime / 1000 && !hasTriggeredEnding)
            {
                OnMusicAboutToEnd();
                hasTriggeredEnding = true;
            }

            // Reset the flag if a new song starts playing from the beginning
            if (_source.time < 1.0f && hasTriggeredEnding)
            {
                hasTriggeredEnding = false;
            }
        }
    }

    //public void OnValidate()
    //{
    //    OnMusicAboutToEnd();
    //}

    void OnMusicAboutToEnd()
    {
        var tmp = _source;
        _source = _oldSource;
        _oldSource = tmp;

        if (_oldSource.clip != null)
            StartCoroutine(FadeOut(bufferTime));

        if (_source.clip != null)
            _source.Stop();
        if (type == BackgroundMusicType.Ambient && ambientMusic.Count > 0) _source.clip = ambientMusic[Mathf.FloorToInt(UnityEngine.Random.value * ambientMusic.Count)];
        else if (type == BackgroundMusicType.Combat && combatMusic.Count > 0) _source.clip = combatMusic[Mathf.FloorToInt(UnityEngine.Random.value * combatMusic.Count)];

        if (_source.clip != null)
            _source.Play();
    }

    void OnAttacked(HealthComp comp) {
        combatMusicTimer = combatMusicDuration;
        if(type == BackgroundMusicType.Combat)
        {
            return;    
        }

        type = BackgroundMusicType.Combat;
        OnMusicAboutToEnd();
    }

    IEnumerator FadeOut(float duration)
    {
        float startVolume = _oldSource.volume;

        while (_oldSource.volume > 0)
        {

            _oldSource.volume -= startVolume * Time.unscaledDeltaTime * 1000 / duration;


            yield return null;
        }


        _oldSource.volume = 0;
        _oldSource.Stop();


        _oldSource.volume = startVolume;
    }
}
