namespace Sirenix.OdinInspector.Demos
{
    using System.Collections.Generic;
    using UnityEngine;

    public class HideInEditorAndPlayModeExamples : MonoBehaviour
    {
        [Title("Hidden in play mode")]
        [HideInPlayMode]
        public AnimationCurve MyCurve;

        [HideInPlayMode]
        public Material MyMaterial;

        [Title("Hidden in editor mode")]
        [HideInEditorMode]
        public int MyInt;

        [HideInEditorMode]
        public GameObject Prefab;
    }
}