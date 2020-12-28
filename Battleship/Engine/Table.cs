using Battleship.Audio;
using Battleship.Graphics;
using Battleship.Screens;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Battleship.Engine
{
    /// <summary>
    /// Represents a base game match. 
    /// This class contains shared objects for the derived class and the drawing logic.
    /// </summary>
    public class Table
    {
        #region Fields
        // Z-Index constants
        //protected const int ON_TOP = 1000;

        protected const int N_SHIPS = 5;
        
        protected SpriteBatch spriteBatch;
        protected Texture2D redCross;
        protected Texture2D redCircle;
        protected Texture2D blueCross;
        protected Texture2D blueCircle;

        protected AudioManager audioManager;
        protected ScreenManager screenManager;
        protected DragonDrop<IDragAndDropItem> dragonDrop;
        protected TileGrid tileGrid;
        protected Board p1Board, cpuBoard;        
        protected List<Cell> p1HitCells, cpuHitCells;
        protected int p1AvailableShips;
        protected int cpuAvailableShips;
        protected bool dragShipOrientation = true;
        protected bool tileIntersects = false;
        protected bool matchStarted = false;
        protected bool matchHasWinner = false;
        protected int tCoordX = 0;
        protected int tCoordY = 0;
        protected int lenOffset = 0;
        protected int curShipIdx;
        protected GameplayScreen game;
        private const string p1ShipsMsg = "P1 Ships : ";
        private const string cpuShipsMsg = "CPU Ships : ";

        private SpriteFont enterMoveFont;
        #endregion

        #region Initialization
        public Table(GameplayScreen game, SpriteBatch spriteBatch, DragonDrop<IDragAndDropItem> dragonDrop, SpriteFont font, ScreenManager scrMgr, AudioManager audMgr, int stackOffsetH, int stackOffsetV,
            Texture2D bCircle, Texture2D bCross, Texture2D rCircle, Texture2D rCross)
        {
            this.game = game;
            this.spriteBatch = spriteBatch;
            this.dragonDrop = dragonDrop;
            screenManager = scrMgr;
            audioManager = audMgr;
            enterMoveFont = font;

            blueCircle = bCircle;
            blueCross = bCross;            
            redCircle = rCircle;
            redCross = rCross;            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// override this to set up your table
        /// </summary>
        public void SetTable() { }

        /// <summary>
        /// override this to clear the table
        /// </summary>
        public void Clear() { }

        /// <summary>
        /// override this to define update logic
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Draw(GameTime gameTime)
        {            
            // fixes the z-ordering stuff
            var items = dragonDrop.dragItems.OrderBy(z => z.ZIndex).ToList();
            foreach (var item in items)
            {
                var type = item.GetType();
                if (type == typeof(ConcreteShip)) item.Draw(gameTime);
            }
            if (!matchStarted)
            {
                if (tileGrid != null &&
                    ((dragShipOrientation && tCoordX >= 0 && tCoordX < Board.DIM && tCoordY >= 0 && tCoordY + lenOffset < Board.DIM) ||
                    ((!dragShipOrientation && tCoordY >= 0 && tCoordX >= 0 && tCoordX + lenOffset < Board.DIM)) &&
                        tileIntersects))
                {
                    Primitives2D.DrawRectangle(spriteBatch, tileGrid.GetTileRectangle(tCoordX, tCoordY), Color.Yellow);
                    if (dragShipOrientation)
                        Primitives2D.DrawRectangle(spriteBatch, tileGrid.GetTileRectangle(tCoordX, tCoordY + lenOffset), Color.Red);
                    else
                        Primitives2D.DrawRectangle(spriteBatch, tileGrid.GetTileRectangle(tCoordX + lenOffset, tCoordY), Color.Red);
                }
            }
            else
            {
                Color drawColor = Color.Blue;
                foreach (Cell c in p1HitCells)
                {
                    Rectangle cRect = tileGrid.GetTileRectangle(Board.DIM-1 - c.LocationX, Board.DIM-1 - c.LocationY);

                    if (c.Hit)
                        spriteBatch.Draw(redCross, cRect, Color.White);
                    else
                    {
                        spriteBatch.Draw(blueCross, cRect, Color.White);
                    }
                    //if (c.Hit)
                    //    drawColor = Color.Red;
                    //else
                    //    drawColor = Color.Blue;

                    //// draw cross
                    //Primitives2D.DrawLine(spriteBatch, new Vector2(cRect.Right,
                    //    cRect.Bottom), new Vector2(cRect.Left, cRect.Top), drawColor, 2f);
                    //Primitives2D.DrawLine(spriteBatch, new Vector2(cRect.Left,
                    //    cRect.Bottom), new Vector2(cRect.Right, cRect.Top), drawColor, 2f);
                }

                foreach (Cell c in cpuHitCells)
                {
                    Rectangle cRect = tileGrid.GetTileRectangle(Board.DIM-1 - c.LocationX, Board.DIM-1 - c.LocationY);

                    if (c.Hit)
                    {
                        spriteBatch.Draw(redCircle, cRect, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(blueCircle, cRect, Color.White);
                    }

                    //if (c.Hit)
                    //    drawColor = Color.Red;
                    //else
                    //    drawColor = Color.Blue;

                    //// draw circle
                    //Primitives2D.DrawCircle(spriteBatch, new Vector2(cRect.Right - cRect.Width / 2,
                    //    cRect.Bottom - cRect.Height/2), cRect.Width/2, 256, drawColor, 2f);

                }

                // show remaining ships
                Vector2 p1ShipsSize = enterMoveFont.MeasureString(p1ShipsMsg);
                spriteBatch.DrawString(enterMoveFont,
                    p1ShipsMsg + p1AvailableShips, 
                    new Vector2(game.ScreenManager.Game.GraphicsDevice.Viewport.Width - p1ShipsSize.X - 30, 20), 
                    Color.DarkRed);
                Vector2 cpuShipsSize = enterMoveFont.MeasureString(cpuShipsMsg);
                spriteBatch.DrawString(enterMoveFont,
                    cpuShipsMsg + cpuAvailableShips,
                    new Vector2(game.ScreenManager.Game.GraphicsDevice.Viewport.Width - cpuShipsSize.X - 30, 40),
                    Color.DarkRed);

            }
        }
        #endregion
    }
}
