using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTaker : MonoBehaviour {

    private void Start()
    {
        // save the values to settings.txt if they do not exist
        // TODO: have this as a method, this code is duplicated waaaaaay too much...
        if (!ES3.KeyExists("ScreenshotSupersize", "settings.txt")) { ES3.Save<int>("ScreenshotSupersize", 1, "settings.txt"); }

        SuperSize = ES3.Load<int>("ScreenshotSupersize", "settings.txt", 1);
    }

    int SuperSize;

    void Update () {
        if (Input.GetButtonDown("Screenshot"))
        {
            string FileName = NewGame.ValidateSaveName(System.DateTime.Now.ToLocalTime().ToString("u")) + ".png";
            string FolderPath = Application.persistentDataPath + "/screenshots/";

            // Create the folder beforehand if not exists
            if (!System.IO.Directory.Exists(FolderPath))
                System.IO.Directory.CreateDirectory(FolderPath);

            ScreenCapture.CaptureScreenshot(FolderPath + FileName, SuperSize);
        }
	}
}
