using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace References
{
    public class Transforms : MonoBehaviour
    {

        [SerializeField] private Transform MegaMeshParentTransform;

        public static Transform MegaMeshParent { get { return Instance.MegaMeshParentTransform; } }


        private static Transforms Instance;
        private void Awake()
        {
            Instance = this;
        }
    }
}