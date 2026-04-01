using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("시작 UI 설정")]
    public GameObject PressKeyUI;
    public float BlinkSpeed = 0.5f;

    //중복 클릭 방지
    private bool isStarting = false;

    void Start()
    {
        if (PressKeyUI != null)
        {
            StartCoroutine(BlinkRoutine());
        }
    }


    void Update()
    {
        if (Input.anyKeyDown && !isStarting)
        {
            isStarting = true;
            StartGame();
        }
    }

    private IEnumerator BlinkRoutine()
    {
        while (!isStarting)
        {
            PressKeyUI.SetActive(!PressKeyUI.activeSelf);

            yield return new WaitForSeconds(BlinkSpeed);
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("BakeEventScene");
    }

    public void EndGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
