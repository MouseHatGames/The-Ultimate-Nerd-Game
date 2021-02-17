// based on http://mfyg.dk/cleaning-up-missing-scripts-in-unity/

# if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CleanUpShit : Editor
{
    [MenuItem("Tools/Clean Up Shit")]
    static void CleanShitUp()
    {
        //Get the current scene and all top-level GameObjects in the scene hierarchy
        Scene currentScene = SceneManager.GetActiveScene();
        GameObject[] rootObjects = currentScene.GetRootGameObjects();

        foreach (GameObject g in rootObjects)
        {
            //Get all components on the GameObject (and its children!), then loop through them 
            Component[] components = g.GetComponentsInChildren<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component currentComponent = components[i];

                if (currentComponent is CircuitBoard) // replace the type here with whatever you want to SMASH
                {
                    Debug.Log("SMASHED " + currentComponent + "!");
                    FindObjectOfType<RunMainMenu>().BoardColor.Add((Color)currentComponent.GetComponent<CircuitBoard>().BoardColor);
                    
                }
            }
        }
    }
}

#endif