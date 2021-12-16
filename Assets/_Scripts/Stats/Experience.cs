using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Stats
{
	public class Experience : MonoBehaviour, ISaveable
	{
		public float GetExperiencePoints {
			get {
				return experiencePoints;
			}
		}

		[SerializeField] float experiencePoints = 0;

		public event Action onExperienceGained;

		public void GainExperience(float experience)
		{
			experiencePoints += experience;
			onExperienceGained();
		}

		public object CaptureState()
		{
			return experiencePoints;
		}

		public void RestoreState(object state)
		{
			experiencePoints = (float)state;
		}
	}
}
