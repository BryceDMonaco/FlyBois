namespace Sirenix.OdinInspector.Demos
{
    using Sirenix.Serialization;
    
    public class IncorrectUseOfOdinSerializeAttributeExamples2 : SerializedMonoBehaviour
    {
        [OdinSerialize]
        public MyCustomType UnityAndOdinSerializedField1;

        [OdinSerialize]
        public int UnityAndOdinSerializedField2;

        [System.Serializable]
        public class MyCustomType
        {
            public int Test;
        }
    }
}