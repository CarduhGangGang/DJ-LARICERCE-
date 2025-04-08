using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadValmira : MonoBehaviour
{
	public void ChangeScene()
	{
		SceneManager.LoadScene("Valmira");
	}

	void Start()
	{
		Button button = GetComponent<Button>();
		if (button != null)
		{
			button.onClick.AddListener(ChangeScene);
		}
	}
}
