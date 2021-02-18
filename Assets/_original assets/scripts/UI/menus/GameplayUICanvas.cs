// just for identification purposes

using UnityEngine;

public class GameplayUICanvas : MonoBehaviour {

    public static GameplayUICanvas Instance;

    private void Awake()
    {
        Instance = this;
    }
}