using System.Windows;
using PlatformerGame.Entities;

namespace PlatformerGame.Core
{
    public class Camera
    {
        public Vector Position { get; private set; }
        private Entity _target;

        // Camera boundaries (so it doesn't show outside the level)
        private double _levelWidth;
        private double _levelHeight;
        private double _viewportWidth;
        private double _viewportHeight;

        public Camera(double viewportWidth, double viewportHeight)
        {
            _viewportWidth = viewportWidth;
            _viewportHeight = viewportHeight;
            Position = new Vector(0, 0);
        }

        public void SetLevelBounds(double width, double height)
        {
            _levelWidth = width;
            _levelHeight = height;
        }

        public void Follow(Entity target)
        {
            _target = target;
        }

        public void Update(float dt)
        {
            if (_target == null) return;

            // Center the camera on the target
            double targetX = _target.Position.X + (_target.Size.X / 2) - (_viewportWidth / 2);
            double targetY = _target.Position.Y + (_target.Size.Y / 2) - (_viewportHeight / 2);

            // Clamp to level bounds
            if (targetX < 0) targetX = 0;
            if (targetY < 0) targetY = 0;
            if (targetX > _levelWidth - _viewportWidth) targetX = _levelWidth - _viewportWidth;
            if (targetY > _levelHeight - _viewportHeight) targetY = _levelHeight - _viewportHeight;

            // Lerp for smooth camera movement (optional)
            // Position += (new Vector(targetX, targetY) - Position) * 5f * dt;

            // Snappy camera
            Position = new Vector(targetX, targetY);
        }
    }
}