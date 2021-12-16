using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        private bool hasPlayed = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !hasPlayed) {
                transform.parent.GetComponent<PlayableDirector>().Play();
                hasPlayed = true;
            }
        }

        public object CaptureState()
        {
            return hasPlayed;
        }

        public void RestoreState(object state)
        {
            bool newHasPlayed = (bool)state;
            hasPlayed = newHasPlayed;
        }
    }
}
