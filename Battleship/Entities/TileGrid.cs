using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Battleship
{
    /// <summary>
    /// Represents a grid of tiles.
    /// </summary>
    public class TileGrid
    {
        #region Fields
        private int[][] grid;

        private int width;
        private int height;
        private int cellWidth;
        private int cellHeight;
        private Rectangle visibleTiles;
        private Vector2 displaySize;
        private CollisionTile sheet;
        private SpriteFont gridFont;

        public static Vector2 screenCenter = Vector2.Zero;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a grid of tiles.
        /// </summary>
        /// <param name="tileWidth">the tile width</param>
        /// <param name="tileHeight">the tile height</param>
        /// <param name="numXTiles">the number of tiles to draw in x direction</param>
        /// <param name="numYTiles">the number of tiles to draw in y direction</param>
        /// <param name="tileSheet">the tile object</param>
        /// <param name="gFont">the font used for the grid letters</param>
        public TileGrid(int tileWidth, int tileHeight, 
            int numXTiles, int numYTiles,
            CollisionTile tileSheet,
            SpriteFont gFont)            
        {            
            sheet = tileSheet;
            gridFont = gFont;
            width = numXTiles;
            height = numYTiles;
            cellWidth = tileWidth;
            cellHeight = tileHeight;

            displaySize.X = Constants.SCR_WIDTH;
            displaySize.Y = Constants.SCR_HEIGHT;

            visibleTiles = new Rectangle(0, 0, width, height);

            grid = new int[width][];
            for (int i = 0; i < width; i++)
            {
                grid[i] = new int[height];
                for (int j = 0; j < height; j++)
                {
                    grid[i][j] = 0;
                }
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Sets a tile to given value.
        /// </summary>
        /// <param name="xIndex">the x position</param>
        /// <param name="yIndex">the y position</param>
        /// <param name="tile">the tile value</param>
        public void SetTile(int xIndex, int yIndex, int tile)
        {
            grid[xIndex][yIndex] = tile;
        }

        /// <summary>
        /// Returns the tile rectangle object.
        /// </summary>
        /// <param name="x">the x position</param>
        /// <param name="y">the y position</param>
        /// <returns></returns>
        public Rectangle GetTileRectangle(int x, int y)
        {
            Vector2 position = Vector2.Zero;
            position.X = (float)-x * CollisionTile.DIM + 2 * displaySize.X / 3;
            position.Y = (float)-y * CollisionTile.DIM + 4 * displaySize.Y / 5;
            return new Rectangle((int)position.X, (int)position.Y, CollisionTile.DIM, CollisionTile.DIM);
        }

        /// <summary>
        /// Draws the grid of tiles.
        /// </summary>
        /// <param name="batch"></param>
        public void Draw(SpriteBatch batch)
        {
            float scaledTileWidth = (float)cellWidth * 1.0f;
            float scaledTileHeight = (float)cellHeight * 1.0f;
            screenCenter = new Vector2(
               (2*displaySize.X / 3),
               (4*displaySize.Y / 5));

            //begin a batch of sprites to be drawn all at once
            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            Rectangle sourceRect = new Rectangle();
            Vector2 scale = Vector2.One;
            char letter = 'J';
            int number = Board.DIM;

            for (int x = visibleTiles.Left; x < visibleTiles.Right; x++)
            {
                for (int y = visibleTiles.Top; y < visibleTiles.Bottom; y++)
                {
                    if (grid[x][y] != 0)
                    {
                        //Get the tile's position from the grid
                        //in this section we're using reference methods
                        //for the high frequency math functions
                        Vector2 position = Vector2.Zero;
                        position.X = (float)x * scaledTileWidth;
                        position.Y = (float)y * scaledTileHeight;

                        // get the source rectnagle that defines the tile
                        sourceRect = sheet.Rectangle;
                        //.GetRectangle(ref grid[x][y], out sourceRect);

                        //Draw the tile
                        batch.Draw(sheet.Texture, screenCenter, sourceRect, Color.White,
                           0, position, scale, SpriteEffects.None, 0.0f);

                        if (y == visibleTiles.Bottom - 1)
                        {
                            Vector2 lblPos = new Vector2(screenCenter.X - position.X + cellWidth/2, screenCenter.Y - position.Y - cellHeight/2);
                            // draw the column letter
                            batch.DrawString(gridFont, Convert.ToString(letter), lblPos, Color.Black);
                            --letter;
                        }

                        if (x == visibleTiles.Right - 1)
                        {
                            Vector2 lblPos = new Vector2(screenCenter.X - visibleTiles.Right * cellWidth + cellWidth / 2, screenCenter.Y - position.Y + cellHeight/2);
                            // draw the column letter
                            batch.DrawString(gridFont, number.ToString(), lblPos, Color.Black);
                            --number;
                        }

                    }
                }
            }

            batch.End();
        }

        #endregion
    }
}
