using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadMapa : MonoBehaviour
{
	public void ChangeScene()
	{
		SceneManager.LoadScene("Mapa"); // Certifique-se de que a cena "Mapa" está no Build Settings
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

