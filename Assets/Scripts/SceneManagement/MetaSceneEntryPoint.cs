using UnityEngine.SceneManagement;

public class MetaSceneEntryPoint : SceneEntryPoint {
    protected override void Start() { }

    public void Play() {
        SceneManager.LoadScene("GameScene");
    }
}