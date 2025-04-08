using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSerdalia : MonoBehaviour
{
	public void ChangeScene()
	{
		SceneManager.LoadScene("Serdalia");
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