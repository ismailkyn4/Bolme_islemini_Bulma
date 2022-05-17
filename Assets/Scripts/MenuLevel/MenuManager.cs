using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject start_btn, exit_btn;
    void Start()
    {
        fadeOut();
    }
    void fadeOut() //start ve exit butonlarının görünürlüğünü(alpha değeri) normale çekiyoruz.
    {
        start_btn.GetComponent<CanvasGroup>().DOFade(1, 0.8f); //unity sahnemizde sıfır olarak ayarlamıştık. Burda 0.8sn de 1e çekiyoruz.Yavaş bir geçişle görünürlüğü düzeliyor.
        exit_btn.GetComponent<CanvasGroup>().DOFade(1, 0.8f).SetDelay(0.5f); //çıkış butonu 0.5 kadar sonra görünür olsun dedik.
    }
    public void exitGame()
    {
        Application.Quit();
    }
    public void  startGameLevel()
    {
        SceneManager.LoadScene(1);
    }
}
