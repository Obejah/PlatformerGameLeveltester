using System.Windows.Input;

namespace PlatformerGame.Core
{
    public class InputManager
    {
        public bool Left { get; private set; }
        public bool Right { get; private set; }
        public bool Run { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool Interact { get; private set; }

        private bool _jumpWasPressed = false;

        public void OnKeyDown(Key key)
        {
            if (key == Key.A || key == Key.Left) Left = true;
            if (key == Key.D || key == Key.Right) Right = true;
            if (key == Key.LeftShift) Run = true;
            if (key == Key.Space || key == Key.W || key == Key.Up) JumpHeld = true;
            if (key == Key.E) Interact = true;
        }

        public void OnKeyUp(Key key)
        {
            if (key == Key.A || key == Key.Left) Left = false;
            if (key == Key.D || key == Key.Right) Right = false;
            if (key == Key.LeftShift) Run = false;
            if (key == Key.Space || key == Key.W || key == Key.Up) JumpHeld = false;
            if (key == Key.E) Interact = false;
        }

        public void Poll()
        {
            if (JumpHeld && !_jumpWasPressed)
            {
                JumpPressed = true;
                _jumpWasPressed = true;
            }
            else
            {
                JumpPressed = false;
            }

            if (!JumpHeld)
            {
                _jumpWasPressed = false;
            }
        }
    }
}