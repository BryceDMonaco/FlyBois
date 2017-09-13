namespace Sirenix.OdinInspector.Demos
{
    using UnityEngine;

    public class ButtonAndButtonGroupExamples : MonoBehaviour
    {
		[Title("Buttons and Button Groups")]
        public bool ToggleButtons;

        [Button]
        [GUIColor(1, 0, 0, 1)]
        private void SayHello()
        {
            Debug.Log("Hello");
        }

        [ShowIf("ToggleButtons")]
        [Button("Custom Button Name")]
        private void NamedButton()
        {
            Debug.Log("Custom Button Name");
        }

        [ButtonGroup("My Button Group 1")]
        private void A()
        {
            Debug.Log("Button A was pressed");
        }

        [ButtonGroup("My Button Group 1")]
        [ShowIf("ToggleButtons")]
        [GUIColor(0, 1, 0, 1)]
        private void B()
        {
            Debug.Log("Button B was pressed");
        }

        [ButtonGroup("My Button Group 1")]
        private void C()
        {
            Debug.Log("Button C was pressed");
        }

        [ButtonGroup("My Button Group 2")]
        private void D()
        {
            Debug.Log("Button D was pressed");
        }

        [ButtonGroup("My Button Group 2")]
        private void E()
        {
            Debug.Log("Button E was pressed");
        }

        [ButtonGroup("My Button Group 2")]
        private void F()
        {
            Debug.Log("Button F was pressed");
        }

        [ButtonGroup("My Button Group 2")]
        private void G()
        {
            Debug.Log("Button G was pressed");
        }

		[Title("Inline Buttons")]
		[PropertyOrder(10)]
		[InlineButton("Click")]
		public int InlineButton;

		[PropertyOrder(10)]
		[InlineButton("Click", "$DynamicLabel")]
		public string DynamicLabel = "Dynamic label";

		private void Click()
		{
			Debug.Log("Click");
		}
    }
}