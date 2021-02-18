using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace References
{
    public class Materials : MonoBehaviour
    {
        [SerializeField] private Material CircuitBoardMaterial;
        [SerializeField] private Material DefaultMaterial;

        public static Material CircuitBoard { get { return Instance.CircuitBoardMaterial; } }
        public static Material Default { get { return Instance.DefaultMaterial; } }
        public static Material SnappingPeg;

        public static Material CircuitOn;
        public static Material CircuitOff;
        
        public static Material DisplayOff;
        public static Material DisplayRed;
        public static Material DisplayGreen;
        public static Material DisplayBlue;
        public static Material DisplayYellow;
        public static Material DisplayOrange;
        public static Material DisplayPurple;
        public static Material DisplayWhite;
        public static Material DisplayCyan;

        public static Material NoisemakerOn;
        public static Material NoisemakerOff;

        public static Material CircuitOnAlwaysOnTop;
        public static Material CircuitOffAlwaysOnTop;

        public static Material DisplayOffAlwaysOnTop;
        public static Material DisplayRedAlwaysOnTop;
        public static Material DisplayGreenAlwaysOnTop;
        public static Material DisplayBlueAlwaysOnTop;
        public static Material DisplayYellowAlwaysOnTop;
        public static Material DisplayOrangeAlwaysOnTop;
        public static Material DisplayPurpleAlwaysOnTop;
        public static Material DisplayWhiteAlwaysOnTop;
        public static Material DisplayCyanAlwaysOnTop;

        public static Material NoisemakerOnAlwaysOnTop;
        public static Material NoisemakerOffAlwaysOnTop;


        private static Materials Instance;
        private void Awake()
        {
            Instance = this;

            CircuitOn = new Material(DefaultMaterial) { color = Settings.CircuitOnColor };
            CircuitOff = new Material(DefaultMaterial) { color = Settings.CircuitOffColor };

            SnappingPeg = new Material(DefaultMaterial) { color = Settings.SnappingPegColor };

            DisplayOff = new Material(DefaultMaterial) { color = Settings.DisplayOffColor };
            DisplayRed = new Material(DefaultMaterial) { color = Settings.DisplayRedColor };
            DisplayGreen = new Material(DefaultMaterial) { color = Settings.DisplayGreenColor };
            DisplayBlue = new Material(DefaultMaterial) { color = Settings.DisplayBlueColor };
            DisplayYellow = new Material(DefaultMaterial) { color = Settings.DisplayYellowColor };
            DisplayOrange = new Material(DefaultMaterial) { color = Settings.DisplayOrangeColor };
            DisplayPurple = new Material(DefaultMaterial) { color = Settings.DisplayPurpleColor };
            DisplayWhite = new Material(DefaultMaterial) { color = Settings.DisplayWhiteColor };
            DisplayCyan = new Material(DefaultMaterial) { color = Settings.DisplayCyanColor };

            NoisemakerOn = new Material(DefaultMaterial) { color = Settings.NoisemakerOnColor };
            NoisemakerOff = new Material(DefaultMaterial) { color = Settings.NoisemakerOffColor };

            CircuitOnAlwaysOnTop= new Material(CircuitOn) { renderQueue = 5000 };
            CircuitOffAlwaysOnTop= new Material(CircuitOff) { renderQueue = 5000 };

            DisplayOffAlwaysOnTop= new Material(DisplayOff) { renderQueue = 5000 };
            DisplayRedAlwaysOnTop= new Material(DisplayRed) { renderQueue = 5000 };
            DisplayGreenAlwaysOnTop= new Material(DisplayGreen) { renderQueue = 5000 };
            DisplayBlueAlwaysOnTop= new Material(DisplayBlue) { renderQueue = 5000 };
            DisplayYellowAlwaysOnTop= new Material(DisplayYellow) { renderQueue = 5000 };
            DisplayOrangeAlwaysOnTop= new Material(DisplayOrange) { renderQueue = 5000 };
            DisplayPurpleAlwaysOnTop= new Material(DisplayPurple) { renderQueue = 5000 };
            DisplayWhiteAlwaysOnTop= new Material(DisplayWhite) { renderQueue = 5000 };
            DisplayCyanAlwaysOnTop= new Material(DisplayCyan) { renderQueue = 5000 };

            NoisemakerOnAlwaysOnTop= new Material(NoisemakerOn) { renderQueue = 5000 };
            NoisemakerOffAlwaysOnTop= new Material(NoisemakerOff) { renderQueue = 5000 };
        }
    }
}