using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
	[SerializeField] private GameObject DifficultySelector;
	[SerializeField] private Outline[] DifficultyButtonOutlines;

	private void Awake()
	{
		SetActiveOutline();
	}

	public void RestartGame()
	{
		PlayerControl.Reset();
		QuestManager.Reset();
		SceneManager.LoadScene(1);
	}

	// 0 = easy, 1 = medium, 2 = hard
	public void SetDifficulty(int value)
	{
		QuestManager.CurrentDifficulty = (Difficulty)value;
		SetActiveOutline();
	}

	private void SetActiveOutline()
	{
		int currentActiveOutline = (int)QuestManager.CurrentDifficulty;
		for (int i = 0; i < DifficultyButtonOutlines.Length; i++)
		{
			DifficultyButtonOutlines[i].enabled = i == currentActiveOutline;
		}
	}
}
