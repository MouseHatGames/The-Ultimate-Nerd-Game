// because you STILL can't fucking assign static variables in the inspector!!!

using UnityEngine;

namespace References // my first time using a custom namespace!!!
{
    public class Prefabs : MonoBehaviour
    {
        private static Prefabs Instance;
        void Awake()
        {
            if (Settings.Get("UseThinWires", false)) { WirePrefab = ThinWirePrefab; }

            Instance = this;
        }


        public GameObject ThinWirePrefab;

        [Header("globally reachable")]

        [SerializeField] private GameObject ClusterPrefab;

        [SerializeField] private GameObject CombinedMeshGroupPrefab;

        [SerializeField] private GameObject CircuitBoardPrefab;
        [SerializeField] private GameObject WirePrefab;

        // for modders' convenience in making new components
        [SerializeField] private GameObject OutputPrefab;
        [SerializeField] private GameObject WhiteCubePrefab;

        [SerializeField] private GameObject InverterPrefab;
        [SerializeField] private GameObject PegPrefab;
        [SerializeField] private GameObject DelayerPrefab;
        [SerializeField] private GameObject ThroughPegPrefab;
        [SerializeField] private GameObject SwitchPrefab;
        [SerializeField] private GameObject ButtonPrefab;
        [SerializeField] private GameObject DisplayPrefab;
        [SerializeField] private GameObject LabelPrefab;
        [SerializeField] private GameObject PanelSwitchPrefab;
        [SerializeField] private GameObject PanelButtonPrefab;
        [SerializeField] private GameObject PanelDisplayPrefab;
        [SerializeField] private GameObject PanelLabelPrefab;
        [SerializeField] private GameObject BlotterPrefab;
        [SerializeField] private GameObject ThroughBlotterPrefab;
        [SerializeField] private GameObject ColorDisplayPrefab;
        [SerializeField] private GameObject PanelColorDisplayPrefab;
        [SerializeField] private GameObject NoisemakerPrefab;
        [SerializeField] private GameObject SnappingPegPrefab;
        [SerializeField] private GameObject MountPrefab;

        public static GameObject Cluster { get { return Instance.ClusterPrefab; } }
        public static GameObject CombinedMeshGroup { get { return Instance.CombinedMeshGroupPrefab; } }
        public static GameObject CircuitBoard { get { return Instance.CircuitBoardPrefab; } }
        public static GameObject Wire { get { return Instance.WirePrefab; } }
        public static GameObject Output { get { return Instance.OutputPrefab; } }
        public static GameObject WhiteCube { get { return Instance.WhiteCubePrefab; } }
        public static GameObject Inverter { get { return Instance.InverterPrefab; } }
        public static GameObject Peg { get { return Instance.PegPrefab; } }
        public static GameObject Delayer { get { return Instance.DelayerPrefab; } }
        public static GameObject ThroughPeg { get { return Instance.ThroughPegPrefab; } }
        public static GameObject Switch { get { return Instance.SwitchPrefab; } }
        public static GameObject Button { get { return Instance.ButtonPrefab; } }
        public static GameObject Display { get { return Instance.DisplayPrefab; } }
        public static GameObject Label { get { return Instance.LabelPrefab; } }
        public static GameObject PanelSwitch { get { return Instance.PanelSwitchPrefab; } }
        public static GameObject PanelButton { get { return Instance.PanelButtonPrefab; } }
        public static GameObject PanelDisplay { get { return Instance.PanelDisplayPrefab; } }
        public static GameObject PanelLabel { get { return Instance.PanelLabelPrefab; } }
        public static GameObject Blotter { get { return Instance.BlotterPrefab; } }
        public static GameObject ThroughBlotter { get { return Instance.ThroughBlotterPrefab; } }
        public static GameObject ColorDisplay { get { return Instance.ColorDisplayPrefab; } }
        public static GameObject PanelColorDisplay { get { return Instance.PanelColorDisplayPrefab; } }
        public static GameObject Noisemaker { get { return Instance.NoisemakerPrefab; } }
        public static GameObject SnappingPeg { get { return Instance.SnappingPegPrefab; } }
        public static GameObject Mount { get { return Instance.MountPrefab; } }


        public static GameObject ComponentTypeToPrefab(ComponentType objectType)
        {
            switch (objectType)
            {
                case ComponentType.CircuitBoard:
                    return CircuitBoard;
                case ComponentType.Wire:
                    return Wire;
                case ComponentType.Inverter:
                    return Inverter;
                case ComponentType.Peg:
                    return Peg;
                case ComponentType.Delayer:
                    return Delayer;
                case ComponentType.ThroughPeg:
                    return ThroughPeg;
                case ComponentType.Switch:
                    return Switch;
                case ComponentType.Button:
                    return Button;
                case ComponentType.Display:
                    return Display;
                case ComponentType.Label:
                    return Label;
                case ComponentType.PanelSwitch:
                    return PanelSwitch;
                case ComponentType.PanelButton:
                    return PanelButton;
                case ComponentType.PanelDisplay:
                    return PanelDisplay;
                case ComponentType.PanelLabel:
                    return PanelLabel;
                case ComponentType.Blotter:
                    return Blotter;
                case ComponentType.ThroughBlotter:
                    return ThroughBlotter;
                case ComponentType.ColorDisplay:
                    return ColorDisplay;
                case ComponentType.PanelColorDisplay:
                    return PanelColorDisplay;
                case ComponentType.Noisemaker:
                    return Noisemaker;
                case ComponentType.SnappingPeg:
                    return SnappingPeg;
                case ComponentType.Mount:
                    return Mount;
                case ComponentType.none:
                    return null;
            }

            return null;
        }
    }
}