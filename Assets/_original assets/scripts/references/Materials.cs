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

        public static Material SnappingPeg;

        private static Materials Instance;
        private void Awake()
        {
            Instance = this;

            CircuitOn = new Material(DefaultMaterial) { color = Settings.CircuitOnColor };
            CircuitOff = new Material(DefaultMaterial) { color = Settings.CircuitOffColor };

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

            SnappingPeg = new Material(DefaultMaterial) { color = Settings.SnappingPegColor };
        }
    }
}