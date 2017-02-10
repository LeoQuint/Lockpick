
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Data : MonoBehaviour {

    public Difficulty difficulty;
    public static Data instance;

    

    void Awake() {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
#if UNITY_STANDALONE
        Cursor.visible = true;
#endif
        DontDestroyOnLoad(gameObject);
    }

    public void SetDifficulty(int d)
    {
        difficulty = (Difficulty)d;
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
