using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Battleship
{
    /// <summary>
    /// Interface describing necessary implementation for working with the DragonDrop.
    /// </summary>
    public interface IDragAndDropItem
    {
        Vector2 Position { get; set; }
        bool IsSelected { get; set; }
        bool IsMouseOver { get; set; }        
        Rectangle Border { get; }
        bool IsDraggable { get; set; }
        int ZIndex { get; set; }
        Texture2D Texture { get; }

        void OnSelected();
        void OnDeselected();
        bool Contains(Vector2 pointToCheck);
        void OnCollusion(IDragAndDropItem item);
        void Update(GameTime gameTime);
        void Draw(GameTime gameTime);
    }
}
