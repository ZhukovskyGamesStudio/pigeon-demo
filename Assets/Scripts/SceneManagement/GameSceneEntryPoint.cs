using UnityEngine.SceneManagement;

public class GameSceneEntryPoint : SceneEntryPoint {
    protected override void Start() { }

    public void ExitToMeta() {
        SceneManager.LoadScene("MetaScene");
    }
}