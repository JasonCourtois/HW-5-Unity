//--------------------------------
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
//--------------------------------
public class QuestGiver : MonoBehaviour
{
	//--------------------------------
	//Human readable quest name
	public string QuestName = string.Empty;
	//Reference to UI Text Box
	public Text Captions = null;
	//List of strings to say
	public string[] CaptionText;


	private void Start()
	{


	}
	//--------------------------------
	void OnTriggerEnter2D(Collider2D other)
	{
		if (!other.CompareTag("Player")) return;

		Quest.QUESTSTATUS Status = QuestManager.GetQuestStatus(QuestName);

		Captions.text = CaptionText[(int)Status];
	}
	//--------------------------------
	void OnTriggerExit2D(Collider2D other)
	{
		Quest.QUESTSTATUS Status = QuestManager.GetQuestStatus(QuestName);
		if (Status == Quest.QUESTSTATUS.UNASSIGNED)
		{
			QuestManager.SetQuestStatus(QuestName, Quest.QUESTSTATUS.ASSIGNED);


			//find the objects with questItem component and activate them
			QuestItem[] questItems = FindObjectsOfTypeAll(typeof(QuestItem)) as QuestItem[];
			

			for (int i = 0; i < questItems.Length; i++)
			{
				if (questItems[i].QuestName.Equals(QuestName))
					questItems[i].gameObject.SetActive(true);
			}



		}

		if (Status == Quest.QUESTSTATUS.COMPLETE)
			SceneManager.LoadScene("scene_Level04");

	}
}
//--------------------------------