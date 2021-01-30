using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public GameObject TitleMenu;
	public GameObject HowToPlayMenu;
	public GameObject OptionsMenu;
	public GameObject CreditsMenu;

	public Slider MusicSlider;
	public Slider EffectSlider;
	public AudioMixer MasterMixer;

	private const string effectVolumeKey = "EffectsVolume";
	private const string musicVolumeKey = "MusicVolume";

	// Start is called before the first frame update
	void Start()
	{
		InitAudio();
	}

	/// <summary>
	/// Initializes audio by reading in current settings from player prefs.
	/// </summary>
	public void InitAudio()
	{
		float effectVolume = -10f;
		if (PlayerPrefs.HasKey(effectVolumeKey))
		{
			effectVolume = PlayerPrefs.GetFloat(effectVolumeKey);
		}
		MasterMixer.SetFloat("EffectsVolume", effectVolume);

		float musicVolume = -10f;
		if (PlayerPrefs.HasKey(musicVolumeKey))
		{
			musicVolume = PlayerPrefs.GetFloat(musicVolumeKey);
		}
		MasterMixer.SetFloat("MusicVolume", musicVolume);

		EffectSlider.value = effectVolume;
		MusicSlider.value = musicVolume;
	}

	// Update is called once per frame
	void Update()
	{

	}

	/// <summary>
	/// Loads the main gameplay scene.
	/// </summary>
	public void PlayGame()
	{
		//SceneManager.LoadScene("todo");
	}

	/// <summary>
	/// Goes to the how to play menu.
	/// </summary>
	public void GoToHowToPlay()
	{
		TitleMenu.SetActive(false);
		HowToPlayMenu.SetActive(true);
	}

	/// <summary>
	/// Goes to the options menu.
	/// </summary>
	public void GoToOptions()
	{
		TitleMenu.SetActive(false);
		OptionsMenu.SetActive(true);
	}

	/// <summary>
	/// Goes to the credits menu.
	/// </summary>
	public void GoToCredits()
	{
		TitleMenu.SetActive(false);
		CreditsMenu.SetActive(true);
	}

	/// <summary>
	/// Closes the app.
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}

	/// <summary>
	/// Turns off all other menus and turns back on the main menu.
	/// </summary>
	public void ReturnToTitle()
	{
		TitleMenu.SetActive(true);
		HowToPlayMenu.SetActive(false);
		OptionsMenu.SetActive(false);
		CreditsMenu.SetActive(false);
	}

	/// <summary>
	/// Sets volume of sound effects based on the slider.
	/// </summary>
	public void SetEffectVolume(float _volume)
	{
		if (_volume < -39f)
		{
			_volume = -80f;
		}
		MasterMixer.SetFloat("EffectsVolume", _volume);
		PlayerPrefs.SetFloat(effectVolumeKey, _volume);
	}

	/// <summary>
	/// Sets volume of music based on the slider.
	/// </summary>
	public void SetMusicVolume(float _volume)
	{
		if (_volume < -39f)
		{
			_volume = -80f;
		}
		MasterMixer.SetFloat("MusicVolume", _volume);
		PlayerPrefs.SetFloat(musicVolumeKey, _volume);
	}
}
