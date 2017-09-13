namespace Sirenix.OdinInspector.Demos
{
    using System;
    using UnityEngine;

    public class BoxGroupExamples : MonoBehaviour
    {
        [BoxGroup("Centered Title", centerLabel: true)]
        public int A;

        [BoxGroup("Centered Title", centerLabel: true)]
        public int B;

        [BoxGroup("Centered Title", centerLabel: true)]
        public int C;

        [BoxGroup("Left Oriented Title")]
        public int D;

        [BoxGroup("Left Oriented Title")]
        public int E;

        [BoxGroup("$DynamicTitle1"), LabelText("Dynamic Title")]
        public string DynamicTitle1 = "Dynamic box title";

        [BoxGroup("$DynamicTitle1")]
        public int F;

        [BoxGroup("$DynamicTitle2")]
        public int G;

        [BoxGroup("$DynamicTitle2")]
        public int H;

        [InfoBox("You can also hide the label of a box group.")]
        [BoxGroup("NoLabel", false)]
        public int I;

        [BoxGroup("NoLabel")]
        public int J;

        [BoxGroup("NoLabel")]
        public int K;

#if UNITY_EDITOR
        public string DynamicTitle2
        {
            get
            {
                Utilities.Editor.GUIHelper.RequestRepaint(); // Repaints the inspector window.
                return "Time Since Startup: " + UnityEditor.EditorApplication.timeSinceStartup;
            }
        }
#endif

        [BoxGroup("Boxed Struct"), HideLabel]
        public SomeStruct BoxedStruct;

        public SomeStruct DefaultStruct;

        [Serializable]
        public struct SomeStruct
        {
            public int One;
            public int Two;
            public int Three;
        }
    }
}