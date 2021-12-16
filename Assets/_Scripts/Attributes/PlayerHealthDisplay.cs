using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RPG.Attributes
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
		private Health health;
		private TMP_Text healthText;

		private void Awake()
		{
			healthText = GetComponent<TMP_Text>();
			health = GameObject.FindWithTag("Player").GetComponent<Health>();
		}

		private void Update()
		{
			healthText.text = String.Format("Player Health: {0:0.0}%", health.GetCurrentHealthPercentage);
		}
	}
}
