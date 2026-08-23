namespace Game.scripts.Extensions;

using Godot;

public static class CanvasItemExtensions
{
    extension(CanvasItem canvasItem)
    {
        public void SetModulateAlpha(float alpha)
        {
            var color = canvasItem.Modulate;
            color.A = alpha;
            canvasItem.Modulate = color;
        }
    }
}
