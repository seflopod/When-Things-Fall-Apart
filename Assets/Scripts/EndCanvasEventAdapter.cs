using UnityEngine;
using System.Collections;

public class EndCanvasEventAdapter : MonoBehaviour
{
	public void LoadTitleMenu()
	{
		GameManager.Instance.Phase = GamePhase.ShowTitle;
		Application.LoadLevel("title");
	}
}
