using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<AudioSource> bgMusicList;

    private int previousTrackId = 0;


    void Update()
    {
        if (!IsTrackPlaying())
            PlayTrack(GetNewRandomTrackIndex());
    }

    int GetNewRandomTrackIndex()
    {
        int randomTrackIndex;
        do
        {
            randomTrackIndex = Random.Range(0, bgMusicList.Count);
        } while (randomTrackIndex == previousTrackId);

        return randomTrackIndex;
    }

    bool IsTrackPlaying()
    {
        foreach(AudioSource audioSource in bgMusicList)
            if(audioSource.isPlaying)
                return true;
        return false;
    }

    void PlayTrack(int index)
    {
        bgMusicList[index].Play();
        previousTrackId = index;
    }
}
