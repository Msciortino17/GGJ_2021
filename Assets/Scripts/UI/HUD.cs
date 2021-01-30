using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	[SerializeField] private Inventory playerInventory;
	[SerializeField] private Submarine submarine;
	[SerializeField] private RectTransform healthBar;
	private float fullHealthWidth;

	public GameObject[] ArtifactIcons;

	// Start is called before the first frame update
	void Start()
	{
		Init();
	}

	/// <summary>
	/// Bind the callback for refreshing the UI, and turn off all icons to start.
	/// </summary>
	private void Init()
	{
		playerInventory.CollectedArtifactCallback += RefreshArtifactIcons;
		for (int i = 0; i < ArtifactIcons.Length; i++)
		{
			ArtifactIcons[i].SetActive(false);
		}

		fullHealthWidth = healthBar.sizeDelta.x;
	}

	// Update is called once per frame
	void Update()
	{
		UpdateHealthBar();
	}

	/// <summary>
	/// We want to read which artifacts have been collected, and turn on the appropriate icon.
	/// </summary>
	private void RefreshArtifactIcons()
	{
		for (int i = 0; i < playerInventory.NumArtifacts; i++)
		{
			ArtifactIcons[i].SetActive(playerInventory.HasArtifact(i));
		}
	}

	/// <summary>
	/// Updates the size of the health bar depending on the submarine's current health.
	/// </summary>
	private void UpdateHealthBar()
	{
		Vector2 healthSize = healthBar.sizeDelta;
		healthSize.x = fullHealthWidth * submarine.GetHealthRatio();
		healthBar.sizeDelta = healthSize;
	}
}
