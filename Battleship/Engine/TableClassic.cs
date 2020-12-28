using Battleship.Audio;
using Battleship.Screens;
using GameStateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Battleship.Engine
{
    public enum GameState { active, complete, won }

    /// <summary>
    /// Represents a game match.
    /// </summary>
    class TableClassic : Table
    {
        #region Fields
        private List<ConcreteShip> csList;
        private List<int> cpuGenCellNumbers;


        private MouseState prevMouseState;
        private KeyboardState prevKeybState;        
        public GameState gameState = GameState.active;        
        private bool cpuTurn;        
        private MessageBoxScreen mbScreen;
        #endregion

        #region Initialization
        public TableClassic(GameplayScreen game, SpriteBatch spriteBatch, DragonDrop<IDragAndDropItem> dd, TileGrid tg, Board plBoard, Board cBoard, SpriteFont font, ScreenManager scrMgr, AudioManager audMgr, 
            int stackOffsetH, int stackOffsetV,
            Texture2D bCircle, Texture2D bCross, Texture2D rCircle, Texture2D rCross)
            : base(game, spriteBatch, dd, font, scrMgr, audMgr, stackOffsetH, stackOffsetV, bCircle, bCross, rCircle, rCross)
        {
            this.game = game;

            tileGrid = tg;
            p1Board = plBoard;
            cpuBoard = cBoard;            
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Update match logic.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            int[] tCoords = null;
            if (!matchStarted)
            {
                if (curShipIdx < N_SHIPS)
                {
                    if (csList[curShipIdx].Direction == ShipDirection.Vertical)
                        tCoords = GetProperTileCoords(csList[curShipIdx].Position, csList[curShipIdx].Border.Bottom);
                    else
                        tCoords = GetProperTileCoords(csList[curShipIdx].Position, csList[curShipIdx].Border.Right);
                    tCoordX = tCoords[0];
                    tCoordY = tCoords[1];
                    lenOffset = csList[curShipIdx].GetLength() - 1;
                    if (
                        ((csList[curShipIdx].Direction == ShipDirection.Vertical && tCoordX >= 0 && tCoordX < Board.DIM && tCoordY >= 0 && tCoordY + lenOffset < Board.DIM)
                            || (csList[curShipIdx].Direction == ShipDirection.Horizontal && tCoordY >= 0 && tCoordX >= 0 && tCoordX + lenOffset < Board.DIM)
                           ))
                    {
                        Rectangle gRect = Rectangle.Empty;
                        //if (csList[curShipIdx].Direction == ShipDirection.Vertical)
                        gRect = tileGrid.GetTileRectangle(tCoordX, tCoordY);
                        //else
                        //gRect = tileGrid.GetTileRectangle(tCoordY, tCoordX);
                        tileGrid.GetTileRectangle(tCoordX, tCoordY);
                        Rectangle gUpRect = Rectangle.Empty;
                        if (csList[curShipIdx].Direction == ShipDirection.Vertical)
                        {
                            gUpRect = tileGrid.GetTileRectangle(tCoordX, tCoordY + lenOffset);
                        }
                        else
                        {
                            //assume horizontal
                            gUpRect = tileGrid.GetTileRectangle(tCoordX + lenOffset, tCoordY);
                        }

                        if (dragonDrop.dragItems[curShipIdx].Border.Intersects(gRect)
                            && dragonDrop.dragItems[curShipIdx].Border.Intersects(gUpRect))
                        {
                            tileIntersects = true;
                            if (!csList[curShipIdx].IsSelected)
                            {
                                if (CanPlaceShip())
                                {
                                    csList[curShipIdx].Position = new Vector2(gUpRect.X, gUpRect.Y);
                                    csList[curShipIdx].IsDraggable = false;
                                    p1Board.AddShip(csList[curShipIdx]);
                                    if (curShipIdx < N_SHIPS)
                                    {
                                        ++curShipIdx;
                                        NextShip();
                                    }
                                    if (curShipIdx == N_SHIPS)
                                    {
                                        ConcreteShip cpuShip;
                                        // CPU places the ships
                                        for (int i = 0; i < N_SHIPS; i++)
                                        {
                                            if (i < 3)
                                                cpuShip = new ConcreteShip(N_SHIPS - i, ShipDirection.Undefined, spriteBatch, game.ScreenManager.Game.Content);
                                            else if (i == 3)
                                                cpuShip = new ConcreteShip(i, ShipDirection.Undefined, spriteBatch, game.ScreenManager.Game.Content);
                                            else
                                                cpuShip = new ConcreteShip(2, ShipDirection.Undefined, spriteBatch, game.ScreenManager.Game.Content);
                                            bool shipPlaced = false;
                                            do
                                            {
                                                shipPlaced = cpuBoard.CanPlaceShipOntoBoard(cpuShip, true);
                                            }
                                            while (shipPlaced == false);
                                            if (shipPlaced)
                                                cpuBoard.AddShip(cpuShip);
                                        }

                                        matchStarted = true;
                                        //cpuHitCells.Clear();
                                        matchHasWinner = false;
                                        //resetAnimCells = true;
                                        //LogCpuShips();
                                    }
                                }
                                else
                                {
                                    csList[curShipIdx].Position = Vector2.Zero;
                                    csList[curShipIdx].StartCellLocation = 0;
                                }
                            }
                        }
                        else
                        {
                            csList[curShipIdx].IsDraggable = true;
                            tileIntersects = false;
                        }
                    }
                }
            }
            else
            {
                if (matchHasWinner)
                {
                    matchStarted = false;

                    if (!mbScreen.IsActive)
                        mbScreen.Activate(false);

                    ResetGame();
                    csList.Add(new ConcreteShip(N_SHIPS, ShipDirection.Vertical, spriteBatch, game.ScreenManager.Game.Content));

                    dragonDrop.Add(csList.ElementAt(curShipIdx));
                }
                else
                {
                    if (cpuTurn && !(mbScreen.ScreenState == ScreenState.Active))
                    {
                        if (cpuGenCellNumbers.Count < Board.DIM * Board.DIM)
                        {
                            bool hasEnded = false;
                            int cellNum = -1;
                            while (!hasEnded)
                            {
                                cellNum = cpuBoard.CpuMakesAMove(cpuHitCells);
                                if (!cpuGenCellNumbers.Contains(cellNum))
                                {
                                    cpuGenCellNumbers.Add(cellNum);
                                    hasEnded = true;
                                }
                            }
                            int xPos = cellNum % Board.DIM;
                            int yPos = cellNum / Board.DIM;

                            Cell c = p1Board.GetCell(xPos, yPos);
                            if (!c.Chosen)
                            {
                                c.Chosen = true;
                                //c.FadingTexture = null;
                                cpuHitCells.Add(c);

                                if (c.IsBusy)
                                {
                                    Ship foundShip = p1Board.UpdateShipState(c);
                                    c.Hit = true;
                                    if (foundShip != null)
                                    {
                                        if (foundShip.State == ShipState.Sink)
                                        {
                                            audioManager.Play3DSound("sink", false, null);
                                            if (mbScreen.Message != "")
                                                mbScreen.Message += "\nP1 has lost the " + foundShip.Name + "!";
                                            else
                                                mbScreen.Message = "P1 has lost the " + foundShip.Name + "!";
                                            screenManager.AddScreen(mbScreen, null);
                                            --p1AvailableShips;
                                            CheckEndOfMatch();
                                        }
                                        else if (foundShip.State == ShipState.Hit)
                                        {
                                            audioManager.Play3DSound("hit", false, null);
                                        }
                                    }

                                }
                                //mbScreen.Message = "CPU has moved!";
                                //screenManager.AddScreen(mbScreen, null);
                                cpuTurn = false;
                            }
                        }
                    }
                }
            }

            HandleMouse();
            //dragonDrop.Update(gameTime);

        }

        /// <summary>
        /// Resets game objects to their initial state.
        /// </summary>
        public void ResetGame()
        {
            dragonDrop.Clear();

            p1AvailableShips = N_SHIPS;
            cpuAvailableShips = N_SHIPS;

            foreach (Cell c in cpuHitCells)
            {
                c.Chosen = false;
                c.IsBusy = false;
                c.Hit = false;
                c.Ship = null;
            }
            cpuHitCells.Clear();
            p1HitCells.Clear();
            cpuGenCellNumbers.Clear();
            cpuBoard.RemoveAllShips();
            cpuBoard.ResetCellsState();
            p1Board.RemoveAllShips();
            p1Board.ResetCellsState();

            //cpuTurn = false;

            csList.Clear();
            curShipIdx = 0;
        }

        /// <summary>
        /// Initializes a new match.
        /// </summary>
        public void InitializeTable()
        {
            matchStarted = false;
            prevKeybState = Keyboard.GetState();
            prevMouseState = Mouse.GetState();

            p1AvailableShips = N_SHIPS;
            cpuAvailableShips = N_SHIPS;
            curShipIdx = 0;
            cpuTurn = false;
            
            cpuGenCellNumbers = new List<int>();
            p1HitCells = new List<Cell>();
            cpuHitCells = new List<Cell>();
            
            csList = new List<ConcreteShip>();
            csList.Add(new ConcreteShip(N_SHIPS, ShipDirection.Vertical, spriteBatch, game.ScreenManager.Game.Content));
            
            dragonDrop.Add(csList.ElementAt(curShipIdx));

            mbScreen = new MessageBoxScreen();
        }
        #endregion

        #region Private Methods
        private void HandleMouse()
        {
            MouseState state = Mouse.GetState();
            if (matchStarted && !matchHasWinner && !cpuTurn && !(mbScreen.ScreenState == ScreenState.Active))
            {
                if (state.LeftButton == ButtonState.Pressed
                    & !(prevMouseState.LeftButton == ButtonState.Pressed))
                {
                    int[] tCoords = null;
                    //System.Diagnostics.Debug.WriteLine(string.Format("Clicked at {0}, {1}", state.X, state.Y));
                    tCoords = GetProperTileFromMouseCoords(state.Position);
                    if (tCoords[0] >= 0 && tCoords[1] >= 0 && tCoords[0] < Board.DIM && tCoords[1] < Board.DIM)
                    {                        
                        Cell c = cpuBoard.GetCell(tCoords[0], tCoords[1]);
                        if (!c.Chosen)
                        {
                            c.Chosen = true;
                            p1HitCells.Add(c);
                            if (c.IsBusy)
                            {                                
                                Ship foundShip = cpuBoard.UpdateShipState(c);
                                c.Hit = true;

                                if (foundShip != null)
                                {
                                    if (foundShip.State == ShipState.Sink)
                                    {
                                        audioManager.Play3DSound("sink", false, null);
                                        mbScreen.Message = "CPU has lost the " + foundShip.Name + "!";
                                        screenManager.AddScreen(mbScreen, null);
                                        --cpuAvailableShips;
                                        CheckEndOfMatch();
                                    } else if (foundShip.State == ShipState.Hit)
                                    {
                                        audioManager.Play3DSound("hit", false, null);
                                    }
                                }
                            }
                           
                            cpuTurn = true;
                        }
                    }
                }
            }
            else
            {
                if (state.RightButton == ButtonState.Pressed
                    & !(prevMouseState.RightButton == ButtonState.Pressed))
                {
                    if (csList[curShipIdx].Direction == ShipDirection.Vertical)
                    {
                        csList[curShipIdx].Direction = ShipDirection.Horizontal;
                        csList[curShipIdx].ChangeTexture(csList[curShipIdx].GetLength(), false);
                        //csList[curShipIdx].Position = new Vector2(csList[curShipIdx].Position.X - csList[curShipIdx].Texture.Width / 2,
                        //    csList[curShipIdx].Position.Y - csList[curShipIdx].Texture.Height / 2);
                    }
                    else if (csList[curShipIdx].Direction == ShipDirection.Horizontal)
                    {
                        csList[curShipIdx].Direction = ShipDirection.Vertical;
                        csList[curShipIdx].ChangeTexture(csList[curShipIdx].GetLength(), true);
                        //csList[curShipIdx].Position = new Vector2(csList[curShipIdx].Position.X + csList[curShipIdx].Texture.Width / 2,
                        //    csList[curShipIdx].Position.Y + csList[curShipIdx].Texture.Height / 2);
                    }

                    dragShipOrientation = !dragShipOrientation;
                }
            }
            prevMouseState = state;
        }

        /// <summary>
        /// Checks if current match has ended.
        /// </summary>
        private void CheckEndOfMatch()
        {
            if (cpuAvailableShips == 0 || p1AvailableShips == 0)
            {
                cpuTurn = false;
                matchHasWinner = true;
                if (cpuAvailableShips == 0 && p1AvailableShips > 0)
                {
                    mbScreen.Message = "P1 wins";
                }
                if (p1AvailableShips == 0 && cpuAvailableShips > 0)
                {
                    mbScreen.Message = "CPU wins";
                }
                if (p1AvailableShips == 0 && cpuAvailableShips == 0)
                {
                    mbScreen.Message = "Draw Game";
                }
            }
        }
        
        /// <summary>
        /// Determines if the ship at current index can be placed.
        /// </summary>
        /// <returns>true if the ship can be placed, otherwise false</returns>
        private bool CanPlaceShip()
        {
            var theShip = csList[curShipIdx];
            if (dragShipOrientation)
                theShip.StartCellLocation = Board.DIM-1 - tCoordX + (Board.DIM-1 - tCoordY) * Board.DIM - (theShip.GetLength() - 1)*Board.DIM;
            else
                theShip.StartCellLocation = Board.DIM-1 - tCoordX + (Board.DIM-1 - tCoordY) * Board.DIM - theShip.GetLength() + 1;
            if (p1Board.CanPlaceShipOntoBoard(theShip, false))
                return true;
            else
            {
                theShip.StartCellLocation = 0;
                return false;
            }
        }

        /// <summary>
        /// Gets tile coordinates given actual mouse position.
        /// </summary>
        /// <param name="position">mouse position as a Point</param>
        /// <returns>tile coordinates</returns>
        private int[] GetProperTileFromMouseCoords(Point position)
        {
            int[] pos = new int[2];
            pos[0] = Board.DIM-1 - ((572 - position.X) / CollisionTile.DIM);
            pos[1] = Board.DIM-1 - (((int)TileGrid.screenCenter.Y + CollisionTile.DIM - position.Y) / CollisionTile.DIM);
            return pos;
        }

        /// <summary>
        /// Gets tile coordinates given ship position and offset.
        /// </summary>
        /// <param name="position">the ship position</param>
        /// <param name="bottom">the offset</param>
        /// <returns>tile coordinates</returns>
        private int[] GetProperTileCoords(Vector2 position, int bottom)
        {
            int[] pos = new int[2];
            if (dragShipOrientation == true)
            {
                pos[0] = (int)-((position.X - TileGrid.screenCenter.X) / CollisionTile.DIM);
                pos[1] = ((int)TileGrid.screenCenter.Y + CollisionTile.DIM - bottom) / CollisionTile.DIM;
            }
            
            else
            {
                pos[1] = (int)-((position.Y - TileGrid.screenCenter.Y))/ CollisionTile.DIM;
                pos[0] = (572 - bottom) / CollisionTile.DIM;
            }
            return pos;
        }

        /// <summary>
        /// Adds the player ship at current index to the proper lists.
        /// </summary>
        private void NextShip()
        {
            tileIntersects = false;
            dragShipOrientation = true;
            //dragonDrop.Clear();
            if (curShipIdx < N_SHIPS)
            {                
                switch (curShipIdx)
                {
                    case 1:
                        csList.Add(new ConcreteShip(4, ShipDirection.Vertical, spriteBatch, game.ScreenManager.Game.Content));
                        dragonDrop.Add(csList[curShipIdx]);
                        break;
                    case 2:
                    case 3:
                        csList.Add(new ConcreteShip(3, ShipDirection.Vertical, spriteBatch, game.ScreenManager.Game.Content));
                        dragonDrop.Add(csList[curShipIdx]);
                        break;
                    case 4:
                        csList.Add(new ConcreteShip(2, ShipDirection.Vertical, spriteBatch, game.ScreenManager.Game.Content));
                        dragonDrop.Add(csList[curShipIdx]);
                        break;
                    default:
                        break;
                }
                                
            }
        }

        //private void LogCpuShips()
        //{
        //    System.Diagnostics.Debug.WriteLine(string.Format(
        //            "CPU Ships : "));
        //    foreach (Ship s in cpuBoard.GetCurrentShips())
        //    {
        //        System.Diagnostics.Debug.WriteLine(string.Format(
        //            "  --- Ship: Name = {0}, Direction = {1}, StartCell = {2}, EndCell = {3}",
        //            s.Name, s.Direction.ToString(), s.StartCellLocation, s.EndCellLocation));
        //    }
        //}
        #endregion

    }
}
