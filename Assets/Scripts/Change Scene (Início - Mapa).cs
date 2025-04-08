using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMapa : MonoBehaviour
{
	public void ChangeScene()
	{
		SceneManager.LoadScene("Mapa");
	}

	void Start()
	{
		Button button = GetComponent<Button>();
		if (button != null)
		{
			button.onClick.AddListener(ChangeScene);
		}
		else
		{
			Debug.LogWarning("Nenhum botão encontrado no GameObject " + gameObject.name);
		}
	}
}

