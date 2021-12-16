using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class LevelTransition : MonoBehaviour
    {
        enum DestinationIdentifier {
            A, B,
            C, D,
            E, F,
            G, H,
            I, J,
            K, L
        }

        [Header("General")]
        [SerializeField] [Min(-1)] int sceneToLoad;
        [SerializeField] Transform spawnpoint;
        [SerializeField] DestinationIdentifier destination;

        [Header("Fading times")]
        [SerializeField] float fadeOutTime = 0.75f;
        [SerializeField] float fadeWaitTime = 0.5f;
        [SerializeField] float fadeInTime = 0.75f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) {
                StartCoroutine(Transition());
            }
        }

        private IEnumerator Transition()
        {
            if (sceneToLoad < 0) {
                Debug.LogError("Scene to Load is not set on " + name);
                yield break;
            }

            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);

            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            savingWrapper.Load();

            LevelTransition otherTransition = GetOtherTransition();
            UpdatePlayer(otherTransition);

            savingWrapper.Save();

            yield return new WaitForSeconds(fadeWaitTime);
            yield return fader.FadeIn(fadeInTime);

            Destroy(gameObject);
        }

        private LevelTransition GetOtherTransition()
        {
            LevelTransition[] levelTransitions = GameObject.FindObjectsOfType<LevelTransition>();

            foreach (LevelTransition levelTransition in levelTransitions) {
                if (levelTransition.destination != destination) 
                    continue;

                if (levelTransition == this)
                    continue;
                
                return levelTransition;
            }

            return null;
        }

        private void UpdatePlayer(LevelTransition otherTransition)
        {
            if (!otherTransition)
                return;

            Transform player = GameObject.FindGameObjectWithTag("Player").transform;
            player.GetComponent<NavMeshAgent>().enabled = false;

            Transform otherSpawnpoint = otherTransition.spawnpoint;

            player.position = otherSpawnpoint.position;
            player.rotation = otherSpawnpoint.rotation;

            player.GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
