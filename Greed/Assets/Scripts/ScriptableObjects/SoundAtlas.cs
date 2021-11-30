using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "SoundAtlas", menuName = "Greed/SoundAtlas", order = 0)]
public class SoundAtlas : ScriptableObject {
    
    [SerializeField] private List<SoundDataPair> atlas;

    public AudioClip GetSound(string name) {
        foreach (var sound in atlas) {
            if (String.Equals(sound.name, name))
                return sound.clip;
        }
        return null;
    }

}

[Serializable] public struct SoundDataPair {
    public AudioClip clip;
    public string name;
}

