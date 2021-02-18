using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using References;

public static class ScreenshotTaker
{
    static int SuperSize = Settings.Get("ScreenshotSupersize", 2);

    public static void RunScreenshotTaking ()
    {
        if (Input.GetButtonDown("Screenshot"))
        {
            if (Input.GetButton("Mod"))
            {
                System.Diagnostics.Process.Start(@Application.persistentDataPath + "/screenshots");
                return;
            }

            string FileName = FileUtilities.CurrentTimestamp + ".png";
            string FolderPath = Application.persistentDataPath + "/screenshots/";

            // Create the folder if it does not exist
            if (!System.IO.Directory.Exists(FolderPath))
                System.IO.Directory.CreateDirectory(FolderPath);

            ScreenCapture.CaptureScreenshot(FolderPath + FileName, SuperSize);

            if (FirstPersonController.Instance != null)
            {
                SoundPlayer.PlaySoundAt(Sounds.Screenshot, FirstPersonController.Instance.transform);
            }
            else
            {
                SoundPlayer.PlaySoundGlobal(Sounds.Screenshot);
            }
        }
	}
}