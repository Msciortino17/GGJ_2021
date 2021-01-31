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
	[SerializeField] private Image blackBackdrop;
	private float targetBlackAlpha;
	private float blackFadeSpeed;
	private float fullHealthWidth;
	private Queue<string> dialogueQueue = new Queue<string>();
	private Queue<float> dialogueTimeQueue = new Queue<float>();
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

		AddDialogue("", 3f);
		AddDialogue("Captain's Log", 5f);
		AddDialogue("I don't remember much... Who I am, or how we ended here.", 7f);
		AddDialogue("All I know is that I am the captain of this submarine, and we are trying to find the lost city of Atlantis.", 8f);
		AddDialogue("Perhaps finding some Atlantian artifacts will shed light on our situation...", 8f);

		FadeBlack(0f, 0.5f);
	}

	// Update is called once per frame
	void Update()
	{
		UpdateHealthBar();
		UpdateDialogue();
		UpdateFadeBlack();
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
				dialogueTimer = dialogueTimeQueue.Dequeue();
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
	public void AddDialogue(string _message, float _time)
	{
		dialogueQueue.Enqueue(_message);
		dialogueTimeQueue.Enqueue(_time);
	}

	/// <summary>
	/// Checks if there are no dialogues loaded into the queue.
	/// </summary>
	public bool DialogueEmpty()
	{
		return dialogueQueue.Count == 0;
	}

	/// <summary>
	/// Will change the target alpha and speed for the black backdrop.
	/// </summary>
	public void FadeBlack(float _alpha, float _speed = 1f)
	{
		targetBlackAlpha = _alpha;
		blackFadeSpeed = _speed;
	}

	/// <summary>
	/// Handles changing the black backdrop's color
	/// </summary>
	private void UpdateFadeBlack()
	{
		Color color = blackBackdrop.color;
		
		if (color.a > targetBlackAlpha)
		{
			color.a -= blackFadeSpeed * Time.deltaTime;
		}
		else if (color.a < targetBlackAlpha)
		{
			color.a += blackFadeSpeed * Time.deltaTime;
		}

		blackBackdrop.color = color;
	}
}
