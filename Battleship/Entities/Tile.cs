using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Battleship
{
    /// <summary>
    /// Represents a tile for one cell.
    /// </summary>
    public class Tile
    {        
        protected Texture2D texture;        
        private Rectangle rectangle;

        /// <summary>
        /// The tile rectangle.
        /// </summary>
        public Rectangle Rectangle
        {
            get
            {
                return rectangle;
            }
            protected set
            {
                rectangle = value;
            }
        }
        
        /// <summary>
        /// Draws a tile.
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, rectangle, null, Color.White);
        }
    }

    /// <summary>
    /// Represents a collision tile.
    /// </summary>
    public class CollisionTile : Tile
    {
        public static int DIM = 40; // tile dimension

        /// <summary>
        /// Initializes a tile. 
        /// </summary>
        /// <param name="cm"></param>
        /// <param name="i"></param>
        /// <param name="newRectangle"></param>
        public CollisionTile(ContentManager cm, int i, Rectangle newRectangle)
        {
            texture = cm.Load<Texture2D>("Tile" + i);
            this.Rectangle = newRectangle;
        }

        /// <summary>
        /// The texture associated with the tile.
        /// </summary>
        public Texture2D Texture
        {
            get
            {
                return texture;
            }
            /*set
            {
                texture = value;
            }*/
        }
    }
}
