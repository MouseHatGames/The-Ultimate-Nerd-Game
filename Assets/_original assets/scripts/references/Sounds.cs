using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace References
{
    public class Sounds : MonoBehaviour
    {
        [SerializeField] private AudioClip[] PlaceOnBoardSound;
        [SerializeField] private AudioClip PlaceOnTerrainSound;
        [SerializeField] private AudioClip FailDoSomethingSound;
        [SerializeField] private AudioClip RotateSomethingSound;
        [SerializeField] private AudioClip DeleteSomethingSound;
        [SerializeField] private AudioClip ConnectionInitialSound;
        [SerializeField] private AudioClip ConnectionFinalSound;
        [SerializeField] private AudioClip ButtonDownSound;
        [SerializeField] private AudioClip ButtonUpSound;
        [SerializeField] private AudioClip SwitchOnSound;
        [SerializeField] private AudioClip SwitchOffSound;
        [SerializeField] private AudioClip UIButtonSound;
        [SerializeField] private AudioClip ScreenshotSound;

        public static AudioClip[] PlaceOnBoard { get { return Instance.PlaceOnBoardSound; } }
        public static AudioClip PlaceOnTerrain { get { return Instance.PlaceOnTerrainSound; } }
        public static AudioClip FailDoSomething { get { return Instance.FailDoSomethingSound; } }
        public static AudioClip RotateSomething { get { return Instance.RotateSomethingSound; } }
        public static AudioClip DeleteSomething { get { return Instance.DeleteSomethingSound; } }
        public static AudioClip ConnectionInitial { get { return Instance.ConnectionInitialSound; } }
        public static AudioClip ConnectionFinal { get { return Instance.ConnectionFinalSound; } }
        public static AudioClip ButtonDown { get { return Instance.ButtonDownSound; } }
        public static AudioClip ButtonUp { get { return Instance.ButtonUpSound; } }
        public static AudioClip SwitchOn { get { return Instance.SwitchOnSound; } }
        public static AudioClip SwitchOff { get { return Instance.SwitchOffSound; } }
        public static AudioClip UIButton { get { return Instance.UIButtonSound; } }
        public static AudioClip Screenshot { get { return Instance.ScreenshotSound; } }

        private static Sounds Instance;
        private void Awake()
        {
            Instance = this;
        }
    }
}