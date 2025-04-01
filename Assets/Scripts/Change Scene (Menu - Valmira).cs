using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneOnClick : MonoBehaviour
{
	public void ChangeScene()
	{
		SceneManager.LoadScene("Valmira");
	}

	void OnMouseDown()
	{
		// Verifica se o objeto clicado é o "Icon morada"
		if (gameObject.name == "Icon 2")
		{
			ChangeScene();
		}
	}
}