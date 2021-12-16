using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace RPG.Core
{
    public class BGMusicRandomizer : MonoBehaviour
    {
        [SerializeField] AudioClip[] backgroundSongs = new AudioClip[3];

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            if (!audioSource)
                return;

            StartCoroutine(StartPlaying());
        }

        private IEnumerator StartPlaying()
        {
            ShuffleSongs();

            for (int i = 0; i < backgroundSongs.Length; ++i)
            {
                AudioClip currentSong = backgroundSongs[i];
                audioSource.clip = currentSong;

                audioSource.Play();

                yield return new WaitForSeconds(currentSong.length);
            }

            yield return null;

            StartCoroutine(StartPlaying());
        }
        private void ShuffleSongs()
        {
            for (int i = backgroundSongs.Length - 1; i >= 1; --i)
            {
                int randomIndex = Random.Range(0, i);

                AudioClip tmp = backgroundSongs[i];
                backgroundSongs[i] = backgroundSongs[randomIndex];
                backgroundSongs[randomIndex] = tmp;
            }
        }
    }
}
