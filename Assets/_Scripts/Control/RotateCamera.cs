using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;
using System;
using Cinemachine;

namespace RPG.Control
{
	public class RotateCamera : MonoBehaviour, ISaveable
	{
		[Header("Speed")]
		[SerializeField] float horizontalSpeed = 100.0f;
		[SerializeField] float verticalSpeed = 10.0f;

		[Header("Edge values")]
		[SerializeField] float minCameraDistance = 2.0f;
		[SerializeField] float maxCameraDistance = 10.0f;
		[SerializeField] float minXRotation = 10.0f;
		[SerializeField] float maxXRotation = 60.0f;

		private CinemachineVirtualCamera cmVirtualCamera = null;
		private CinemachineFramingTransposer cmFramingTransposer = null;

		[Range(0.0f, 1.0f)] private float verticalT = 0.5f;

		private void Awake()
		{
			cmVirtualCamera = GetComponent<CinemachineVirtualCamera>();
			CinemachineComponentBase cmBody = cmVirtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);

			try {
				cmFramingTransposer = cmBody as CinemachineFramingTransposer;
			} catch (InvalidCastException) {
				cmFramingTransposer = null;
			}
		}

		private void Start()
		{
			verticalT = 0.5f;
			UpdateVerticalValues(verticalT);
		}

		private void Update()
		{
			RotateHorizontally();
			RotateVertically();
		}

		private void RotateHorizontally()
		{
			Vector3 rotationAngle = transform.rotation.eulerAngles;
			rotationAngle.y += Input.GetAxis("Horizontal") * horizontalSpeed * Time.deltaTime;

			transform.rotation = Quaternion.Euler(rotationAngle);
		}

		private void RotateVertically()
		{
			// Change verticalT
			verticalT -= Input.GetAxis("Vertical") * verticalSpeed * Time.deltaTime;

			// Clamp verticalT
			verticalT = Mathf.Clamp(verticalT, 0.0f, 1.0f);

			// Update vertical values
			UpdateVerticalValues(verticalT);
		}

		private void UpdateVerticalValues(float t)
		{
			// Update rotation
			Vector3 rotationAngle = transform.rotation.eulerAngles;
			rotationAngle.x = Mathf.Lerp(minXRotation, maxXRotation, t);
			transform.rotation = Quaternion.Euler(rotationAngle);

			// Update camera distance
			if (cmFramingTransposer) {
				cmFramingTransposer.m_CameraDistance = Mathf.Lerp(
					minCameraDistance,
					maxCameraDistance,
					t
				);
			}
		}

		public object CaptureState()
		{
			return new SerializableVector3(transform.rotation.eulerAngles);
		}

		public void RestoreState(object state)
		{
			Vector3 loadedEulerAngle = ((SerializableVector3)state).ToVector();
			transform.rotation = Quaternion.Euler(loadedEulerAngle);
		}
	}
}
