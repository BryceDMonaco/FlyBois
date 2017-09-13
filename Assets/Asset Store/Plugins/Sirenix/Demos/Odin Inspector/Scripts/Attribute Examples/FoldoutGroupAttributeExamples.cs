namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class FoldoutGroupAttributeExamples : MonoBehaviour
    {
        [FoldoutGroup("Group 1")]
        public int A;

        [FoldoutGroup("Group 1")]
        public int B;

        [FoldoutGroup("Group 1")]
        public int C;

        [FoldoutGroup("Group 2")]
        public int D;

        [FoldoutGroup("Group 2")]
        public int E;

        [FoldoutGroup("$GroupTitle")]
        public int One;

        [FoldoutGroup("$GroupTitle")]
        public int Two;

        public string GroupTitle = "Dynamic group title";
    }
}