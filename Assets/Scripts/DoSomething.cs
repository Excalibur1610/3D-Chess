using UnityEngine;
using UnityEngine.SceneManagement;

public class DoSomething : MonoBehaviour {
    private float ApplicationX = Screen.width, ApplicationY = Screen.height;   //window dimensions of game
    private bool showCredits = false;

    public void DoClick() {
        if (transform.name.ToUpper().Equals("START"))
            SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        else if (transform.name.ToUpper().Equals("QUIT"))
            Application.Quit();
        else if (transform.name.ToUpper().Equals("CREDITS"))
            showCredits = true;
    }

    void OnGUI() {
        if (showCredits)
        {
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(ApplicationX / 2 - 40, ApplicationY - 160, 80, 30), "Game Design");
            GUI.Label(new Rect(ApplicationX / 2 - 120, ApplicationY - 130, 240, 30), "Jonah Borders    Tim Lunik");
            GUI.Label(new Rect(ApplicationX / 2 - 30, ApplicationY - 100, 60, 30), "Graphics");
            GUI.Label(new Rect(ApplicationX / 2 - 120, ApplicationY - 70, 240, 30), "Tim Lunik    Jonah Borders");
            if (GUI.Button(new Rect(ApplicationX / 2 - 30, ApplicationY - 40, 60, 40), "Close"))
            {
                showCredits = false;
            }
        }
    }
}
