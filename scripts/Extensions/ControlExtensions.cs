namespace Game.scripts.Extensions;

using System.Linq;
using Godot;

public static class ControlExtensions
{
    extension(Control control)
    {
        public float PositionX
        {
            get => control.Position.X;
            set
            {
                var position = control.Position;
                position.X = value;
                control.Position = position;
            }
        }

        public float PositionY
        {
            get => control.Position.Y;
            set
            {
                var position = control.Position;
                position.Y = value;
                control.Position = position;
            }
        }

        /// <summary>
        /// Godot Bug?
        ///
        /// Setting
        ///  - "MouseFilter" to "Control.MouseFilterEnum.Ignore"
        ///  - "MouseBehaviorRecursive" to "Control.MouseBehaviorRecursiveEnum.Enabled"
        /// on the first child should be the correct way, but that does not seem to work
        /// </summary>
        public void SetMouseFilterToIgnoreAllChildren()
        {
            var children = control.GetChildren().OfType<Control>();
            foreach (var child in children)
            {
                child.MouseFilter = Control.MouseFilterEnum.Ignore;
                child.SetMouseBehaviorRecursive(Control.MouseBehaviorRecursiveEnum.Disabled);
            }
        }
    }
}
