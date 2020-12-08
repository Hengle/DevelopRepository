using UnityEngine;
using UnityEngine.SceneManagement;

namespace Conf
{

public class ConfSceneSelect : MonoBehaviour
{
	public bool GUIHide = false;
	public bool GUIHide2 = false;
	public bool GUIHide3 = false;
	
    public void LoadSceneDemo01()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Conf01");
    }
    public void LoadSceneDemo02()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Conf02");
    }
    public void LoadSceneDemo03()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf03");
    }
    public void LoadSceneDemo04()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf04");
    }
    public void LoadSceneDemo05()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf05");
    }
    public void LoadSceneDemo06()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf06");
    }
    public void LoadSceneDemo07()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf07");
    }
    public void LoadSceneDemo08()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf08");
    }
    public void LoadSceneDemo09()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf09");
    }
    public void LoadSceneDemo10()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf10");
    }
	public void LoadSceneDemo11()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf11");
    }
	public void LoadSceneDemo12()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf12");
    }
	public void LoadSceneDemo13()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf13");
    }
	public void LoadSceneDemo14()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf14");
    }
	public void LoadSceneDemo15()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf15");
    }
	public void LoadSceneDemo16()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf16");
    }
	public void LoadSceneDemo17()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf17");
    }
	public void LoadSceneDemo18()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf18");
    }
	public void LoadSceneDemo19()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf19");
    }
	public void LoadSceneDemo20()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf20");
    }
	public void LoadSceneDemo21()
    {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Conf21");
    }
	
	void Update ()
	 {
 
     if(Input.GetKeyDown(KeyCode.J))
	 {
         GUIHide = !GUIHide;
     
         if (GUIHide)
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasSceneSelect").GetComponent<Canvas> ().enabled = true;
         }
     }
	      if(Input.GetKeyDown(KeyCode.K))
	 {
         GUIHide2 = !GUIHide2;
     
         if (GUIHide2)
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("Canvas").GetComponent<Canvas> ().enabled = true;
         }
     }
		if(Input.GetKeyDown(KeyCode.L))
	 {
         GUIHide3 = !GUIHide3;
     
         if (GUIHide3)
		 {
             GameObject.Find("CanvasTips").GetComponent<Canvas> ().enabled = false;
         }
		 else
		 {
             GameObject.Find("CanvasTips").GetComponent<Canvas> ().enabled = true;
         }
     }
	 }
}
}