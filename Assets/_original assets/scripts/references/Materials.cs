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

            SnappingPeg = new Material(DefaultMaterial) { color = Settings.SnappingPegColor };

            CircuitOn = new Material(DefaultMaterial) { color = Settings.CircuitOnColor, renderQueue = 2000 };
            CircuitOff = new Material(DefaultMaterial) { color = Settings.CircuitOffColor, renderQueue = 2000 };

            DisplayOff = new Material(DefaultMaterial) { color = Settings.DisplayOffColor, renderQueue = 2000 };
            DisplayRed = new Material(DefaultMaterial) { color = Settings.DisplayRedColor, renderQueue = 2000 };
            DisplayGreen = new Material(DefaultMaterial) { color = Settings.DisplayGreenColor, renderQueue = 2000 };
            DisplayBlue = new Material(DefaultMaterial) { color = Settings.DisplayBlueColor, renderQueue = 2000 };
            DisplayYellow = new Material(DefaultMaterial) { color = Settings.DisplayYellowColor, renderQueue = 2000 };
            DisplayOrange = new Material(DefaultMaterial) { color = Settings.DisplayOrangeColor, renderQueue = 2000 };
            DisplayPurple = new Material(DefaultMaterial) { color = Settings.DisplayPurpleColor, renderQueue = 2000 };
            DisplayWhite = new Material(DefaultMaterial) { color = Settings.DisplayWhiteColor, renderQueue = 2000 };
            DisplayCyan = new Material(DefaultMaterial) { color = Settings.DisplayCyanColor, renderQueue = 2000 };

            NoisemakerOn = new Material(DefaultMaterial) { color = Settings.NoisemakerOnColor, renderQueue = 2000 };
            NoisemakerOff = new Material(DefaultMaterial) { color = Settings.NoisemakerOffColor, renderQueue = 2000 };

            CircuitOnAlwaysOnTop= new Material(CircuitOn) { renderQueue = 2500 };
            CircuitOffAlwaysOnTop= new Material(CircuitOff) { renderQueue = 2500 };

            DisplayOffAlwaysOnTop= new Material(DisplayOff) { renderQueue = 2500 };
            DisplayRedAlwaysOnTop= new Material(DisplayRed) { renderQueue = 2500 };
            DisplayGreenAlwaysOnTop= new Material(DisplayGreen) { renderQueue = 2500 };
            DisplayBlueAlwaysOnTop= new Material(DisplayBlue) { renderQueue = 2500 };
            DisplayYellowAlwaysOnTop= new Material(DisplayYellow) { renderQueue = 2500 };
            DisplayOrangeAlwaysOnTop= new Material(DisplayOrange) { renderQueue = 2500 };
            DisplayPurpleAlwaysOnTop= new Material(DisplayPurple) { renderQueue = 2500 };
            DisplayWhiteAlwaysOnTop= new Material(DisplayWhite) { renderQueue = 2500 };
            DisplayCyanAlwaysOnTop= new Material(DisplayCyan) { renderQueue = 2500 };

            NoisemakerOnAlwaysOnTop= new Material(NoisemakerOn) { renderQueue = 2500 };
            NoisemakerOffAlwaysOnTop= new Material(NoisemakerOff) { renderQueue = 2500 };
        }

        private static Dictionary<Color, Material> BoardMaterials = new Dictionary<Color, Material>();
        private static Dictionary<Color, Material> SolidColorMaterials = new Dictionary<Color, Material>();

        public static Material BoardOfColor(Color color)
        {
            if (!BoardMaterials.ContainsKey(color))
            {
                BoardMaterials.Add(color, new Material(CircuitBoard) { color = color });
            }

            return BoardMaterials[color];
        }

        public static Material SolidColor(Color color)
        {
            if (!SolidColorMaterials.ContainsKey(color))
            {
                SolidColorMaterials.Add(color, new Material(Default) { color = color });
            }

            return SolidColorMaterials[color];
        }
    }
}