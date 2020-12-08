using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelectButton : MonoBehaviour
{
    public Button button;

    int stageNomber;

    public void SetStatus(int stageNomber)
    {
        this.stageNomber = stageNomber;
    }

    public void LoadStage()
    {
        DataManager.Instance.ChangeNowStage(stageNomber);
        DataManager.Instance.Save();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}