using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;
using RPG.Control;

namespace RPG.Cinematics
{
    public class CinematicControlRemover : MonoBehaviour
    {
        private PlayableDirector playableDirector;
        private GameObject player;

        private void Awake()
        {
            playableDirector = GetComponent<PlayableDirector>();
            player = GameObject.FindGameObjectWithTag("Player");
        }

        private void Start()
        {
            playableDirector.played += DisableControl;
            playableDirector.stopped += EnableControl;
        }

        void DisableControl(PlayableDirector calledDirector)
        {
            if (calledDirector != playableDirector)
                return;

            player.GetComponent<ActionScheduler>().CancelCurrentAction();

            player.GetComponent<PlayerController>().enabled = false;
        }

        void EnableControl(PlayableDirector calledDirector)
        {
            if (calledDirector != playableDirector)
                return;

            player.GetComponent<PlayerController>().enabled = true;
        }
    }
}
