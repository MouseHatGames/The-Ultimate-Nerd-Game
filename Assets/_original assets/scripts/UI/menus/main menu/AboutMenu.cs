using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutMenu : MonoBehaviour {

	public void OpenEmail() { Application.OpenURL("http://eepurl.com/c_6LBH"); }
    public void OpenReddit() { Application.OpenURL("https://www.reddit.com/r/TheUltimateNerdGame/"); }
    public void OpenTwitter() { Application.OpenURL("https://twitter.com/mousehatgames"); }

}
