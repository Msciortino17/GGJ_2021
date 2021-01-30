using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	[SerializeField] private Inventory playerInventory;
	[SerializeField] private Submarine submarine;
	[SerializeField] private RectTransform healthBar;
	[SerializeField] private Text dialogueText;
	private float fullHealthWidth;
	private Queue<string> dialogueQueue = new Queue<string>();
	public float DialogueTime;
	private float dialogueTimer;

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
		UpdateDialogue();
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

	/// <summary>
	/// This will fade in text and fade it out as things are added.
	/// </summary>
	private void UpdateDialogue()
	{
		if (dialogueTimer < 0f)
		{
			if (dialogueQueue.Count > 0)
			{
				dialogueText.text = dialogueQueue.Dequeue();
				dialogueTimer = DialogueTime;
			}
		}
		else
		{
			Color color = dialogueText.color;
			if (dialogueTimer > 1f)
			{
				color.a += Time.deltaTime;
				color.a = Mathf.Min(color.a, 1f);
			}
			else
			{
				color.a -= Time.deltaTime;
				color.a = Mathf.Max(color.a, 0f);
			}
			dialogueText.color = color;

			dialogueTimer -= Time.deltaTime;
		}
	}

	/// <summary>
	/// Takes in message for the dialogue
	/// </summary>
	public void AddDialogue(string _message)
	{
		dialogueQueue.Enqueue(_message);
	}
}
