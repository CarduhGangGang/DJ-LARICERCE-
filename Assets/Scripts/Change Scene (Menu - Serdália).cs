using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSerdalia : MonoBehaviour
{
	public void ChangeScene()
	{
		SceneManager.LoadScene("Serdalia"); // Certifique-se de que esse nome está correto no Build Settings
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