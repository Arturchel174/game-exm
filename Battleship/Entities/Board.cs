using System;
using System.Collections.Generic;
using System.Text;

namespace Battleship
{
    /// <summary>
    /// Represents a board in the game.
    /// </summary>
    public class Board
    {
        #region Fields
        public static int DIM = 10;
        Cell[,] cell = new Cell[DIM, DIM];
        List<Ship> theShips;
        #endregion

        #region Initialization
        /// <summary>
        /// Initializes the board.
        /// </summary>
        public Board()
        {
            Initialize();
            theShips = new List<Ship>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Adds a new ship to the board.
        /// </summary>
        /// <param name="s">the ship to add</param>
        public void AddShip(Ship s)
        {
            if (s != null)
            {
                theShips.Add(s);
            }
        }

        /// <summary>
        /// Removes all the ships from the board.
        /// </summary>
        public void RemoveAllShips()
        {
            if (theShips != null)
            {
                theShips.RemoveRange(0, theShips.Count);
            }
        }

        /// <summary>
        /// Gets the list of ships put on the board.
        /// </summary>
        /// <returns>the list of ships</returns>
        public List<Ship> GetCurrentShips()
        {
            return theShips;
        }

        /// <summary>
        /// Updates the ship state for the chosen cell.
        /// </summary>
        /// <param name="c">the cell object</param>
        /// <returns>the ship itself or null</returns>
        public Ship UpdateShipState(Cell c)
        {
            if (!c.IsBusy)
                return null;
            else
            {
                Ship s = c.Ship;
                ++s.NumberOfHit;
                if (s.NumberOfHit < s.GetLength())
                    s.State = ShipState.Hit;
                else
                    s.State = ShipState.Sink;
                return s;
            }
        }

        /// <summary>
        /// Returns the next zero-based computed cell position (CPU move)
        /// </summary>
        /// <param name="cpuHitCells">the list of chosen cpu cells</param>
        /// <returns>the next cell position</returns>
        public int CpuMakesAMove(List<Cell> cpuHitCells)
        {
            Random r = new Random();
            if (cpuHitCells.Count == 0)
                return r.Next(DIM * DIM);
            else
            {
                int chos = r.Next(DIM * DIM);
                Cell lastChosen = cpuHitCells[cpuHitCells.Count - 1];
                int lastHitIdx = cpuHitCells.FindLastIndex(c => c.Ship != null && c.Ship.State == ShipState.Hit);

                if (lastHitIdx != -1 && cpuHitCells[lastHitIdx].Ship.State != ShipState.Sink)
                {
                    int lPos = cpuHitCells[lastHitIdx].LocationX
                            + DIM * cpuHitCells[lastHitIdx].LocationY;
                    int nextC = 0;
                    //check cells around
                    int nDir = 0;
                    if (lastHitIdx > 0 && cpuHitCells[lastHitIdx - 1].Ship != null && cpuHitCells[lastHitIdx - 1].Ship.State == ShipState.Hit)
                    {
                        int prevHit = cpuHitCells[lastHitIdx - 1].LocationX + DIM * cpuHitCells[lastHitIdx - 1].LocationY;
                        if (cpuHitCells[lastHitIdx - 1].Ship.Direction == ShipDirection.Vertical)
                        {
                            if (cpuHitCells[lastHitIdx - 1].Ship.NumberOfHit < cpuHitCells[lastHitIdx - 1].Ship.GetLength())
                            {
                                int stLoc = cpuHitCells[lastHitIdx - 1].Ship.StartCellLocation;
                                for (int k = stLoc + DIM * (cpuHitCells[lastHitIdx - 1].Ship.GetLength() - 1); k >= stLoc; k -= DIM)
                                {
                                    if (!cpuHitCells.Exists(c => c.LocationX == (k % DIM) && c.LocationY == k / DIM))
                                    {
                                        nextC = k;
                                        return nextC;
                                    }
                                }
                            }
                            else
                                return chos;
                        }
                        else if (cpuHitCells[lastHitIdx - 1].Ship.Direction == ShipDirection.Horizontal)
                        {
                            if (cpuHitCells[lastHitIdx - 1].Ship.NumberOfHit < cpuHitCells[lastHitIdx - 1].Ship.GetLength())
                            {
                                int stLoc = cpuHitCells[lastHitIdx - 1].Ship.StartCellLocation;
                                for (int k = stLoc + (cpuHitCells[lastHitIdx - 1].Ship.GetLength() - 1); k >= stLoc; k--)
                                {
                                    if (!cpuHitCells.Exists(c => c.LocationX == (k % DIM) && c.LocationY == k / DIM))
                                    {
                                        nextC = k;
                                        return nextC;
                                    }
                                }
                            }
                            else
                                return chos;
                        }
                    }
                    else if (lastHitIdx > 0 && cpuHitCells[lastHitIdx - 1].Ship == null)
                    {
                        if (cpuHitCells[lastHitIdx].Ship.Direction == ShipDirection.Horizontal)
                        {
                            if (cpuHitCells[lastHitIdx].LocationX < DIM-1)
                            {
                                if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX + 1
                                    && c.LocationY == cpuHitCells[lastHitIdx].LocationY))
                                {
                                    nextC = lPos + 1;
                                    return nextC;
                                }
                                else if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX - 1
                                    && c.LocationY == cpuHitCells[lastHitIdx].LocationY) && cpuHitCells[lastHitIdx].LocationX > 0)
                                {
                                    nextC = lPos - 1;
                                    return nextC;
                                }
                            }
                            else
                            {
                                if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX - 1
                                    && c.LocationY == cpuHitCells[lastHitIdx].LocationY))
                                {
                                    nextC = lPos - 1;
                                    return nextC;
                                }
                            }
                        }
                        else if (cpuHitCells[lastHitIdx].Ship.Direction == ShipDirection.Vertical)
                        {
                            if (cpuHitCells[lastHitIdx].LocationY < DIM-1)
                            {
                                if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX
                                    && c.LocationY == cpuHitCells[lastHitIdx].LocationY + 1))
                                {
                                    nextC = lPos + DIM;
                                    return nextC;
                                }
                                else
                                {
                                    if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX
                                    && c.LocationY == cpuHitCells[lastHitIdx].LocationY - 1) && cpuHitCells[lastHitIdx].LocationY > 0)
                                    {
                                        nextC = lPos - DIM;
                                        return nextC;
                                    }
                                }
                            }
                            else
                            {
                                if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX
                                    && c.LocationY == cpuHitCells[lastHitIdx].LocationY - 1))
                                {
                                    nextC = lPos - DIM;
                                    return nextC;
                                }
                            }
                        }
                    }
                    else
                        nDir = r.Next(0, 2);
                    if (nDir == 0)  //horizontal
                    {
                        if (cpuHitCells[lastHitIdx].LocationX - 1 >= 0)
                        {
                            if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX - 1
                            && c.LocationY == cpuHitCells[lastHitIdx].LocationY))
                            {
                                nextC = lPos - 1;
                                return nextC;
                            }
                            else if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX + 1
                            && c.LocationY == cpuHitCells[lastHitIdx].LocationY) && cpuHitCells[lastHitIdx].LocationX < DIM-1)
                            {
                                nextC = lPos + 1;
                                return nextC;
                            }
                        }
                        else
                        {
                            if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX + 1
                            && c.LocationY == cpuHitCells[lastHitIdx].LocationY))
                            {
                                nextC = lPos + 1;
                                return nextC;
                            }
                        }
                    }
                    else
                    {   //vertical
                        if (cpuHitCells[lastHitIdx].LocationY - 1 >= 0)
                        {
                            if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX
                            && c.LocationY == cpuHitCells[lastHitIdx].LocationY - 1))
                            {
                                nextC = lPos - DIM;
                                return nextC;
                            }
                            else if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX
                            && c.LocationY == cpuHitCells[lastHitIdx].LocationY + 1) && cpuHitCells[lastHitIdx].LocationY < DIM-1)
                            {
                                nextC = lPos + DIM;
                                return nextC;
                            }
                        }
                        else
                        {
                            if (!cpuHitCells.Exists(c => c.LocationX == cpuHitCells[lastHitIdx].LocationX
                            && c.LocationY == cpuHitCells[lastHitIdx].LocationY + 1))
                            {
                                nextC = lPos + DIM;
                                return nextC;
                            }
                        }
                    }
                    return chos;
                }
                else
                    return chos;
            }
        }

        //public int CpuMakesRndMove()
        //{
        //    Random r = new Random();
        //    return r.Next(100);
        //}

        /// <summary>
        /// Determines if the given ship can be put on the board.
        /// </summary>
        /// <param name="ship">the ship to place</param>
        /// <param name="cpuGen">true if it's the cpu turn</param>
        /// <returns>true if the ship can be placed, otherwise false</returns>
        public bool CanPlaceShipOntoBoard(Ship ship, bool cpuGen)
        {
            bool res = false;
            if (ship != null)
            {
                Random r = new Random();
                if (ship.Type == ShipType.Unknown)
                    return false;

                if (cpuGen)
                {
                    int rndDir = r.Next(2);
                    if (rndDir == 0)
                        ship.Direction = ShipDirection.Horizontal;
                    if (rndDir == 1)
                        ship.Direction = ShipDirection.Vertical;
                }
                int celPos;
                if (!cpuGen)
                    celPos = ship.StartCellLocation;
                else
                    celPos = r.Next(DIM*DIM);
                int xPos = celPos % DIM;
                int yPos = celPos / DIM;
                int shipLen = ship.GetLength();
                Cell cell = this.GetCell(xPos, yPos);

                if (ship.Direction == ShipDirection.Horizontal)
                {
                    if (!cell.IsBusy && cellsAroundHorizFree(this, xPos, yPos, shipLen))
                    {
                        //ship can be put horizontally
                        ship.StartCellLocation = celPos;
                        for (int i = 0; i < shipLen; i++)
                            if (xPos + shipLen - 1 < DIM)
                            {
                                this.GetCell(xPos + i, yPos).IsBusy = true;
                                this.GetCell(xPos + i, yPos).Ship = ship;
                            }
                            else
                            {
                                this.GetCell(xPos - i, yPos).IsBusy = true;
                                this.GetCell(xPos - i, yPos).Ship = ship;
                            }
                        if (xPos + shipLen - 1 < DIM)
                            ship.EndCellLocation = celPos + (shipLen - 1);
                        else
                            ship.EndCellLocation = celPos - (shipLen - 1);

                        if (ship.EndCellLocation < ship.StartCellLocation)
                        {
                            SwapCellBoundaries(ship);
                        }
                        res = true;
                    }
                }
                else if (ship.Direction == ShipDirection.Vertical)
                {
                    if (!cell.IsBusy && cellsAroundVertFree(this, xPos, yPos, shipLen))
                    {
                        //ship can be put vertically
                        ship.StartCellLocation = celPos;
                        for (int i = 0; i < shipLen; i++)
                            if (yPos + shipLen - 1 < DIM)
                            {
                                this.GetCell(xPos, yPos + i).IsBusy = true;
                                this.GetCell(xPos, yPos + i).Ship = ship;
                            }
                            else
                            {
                                this.GetCell(xPos, yPos - i).IsBusy = true;
                                this.GetCell(xPos, yPos - i).Ship = ship;
                            }
                        if (yPos + shipLen - 1 < DIM)
                            ship.EndCellLocation = celPos + (shipLen - 1) * DIM;
                        else
                            ship.EndCellLocation = celPos - (shipLen - 1) * DIM;

                        if (ship.EndCellLocation < ship.StartCellLocation)
                        {
                            SwapCellBoundaries(ship);
                        }
                        res = true;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// Gets the cell object at given position.
        /// </summary>
        /// <param name="x">the x position</param>
        /// <param name="y">the y position</param>
        /// <returns>the cell</returns>
        public Cell GetCell(int x, int y)
        {
            if (x < 0 || y < 0 || x > DIM - 1 || y > DIM - 1)
                return null;
            return cell[x, y];
        }

        /// <summary>
        /// Sets the state of each cell to default values.
        /// </summary>
        public void ResetCellsState()
        {
            for (int i = 0; i < DIM; i++)
            {
                for (int j = 0; j < DIM; j++)
                {
                    cell[i, j].Chosen = false;
                    cell[i, j].Hit = false;
                    cell[i, j].IsBusy = false;
                }
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Swap starting and ending locations for a ship.
        /// </summary>
        /// <param name="ship"></param>
        private void SwapCellBoundaries(Ship ship)
        {
            int tmp = ship.StartCellLocation;
            ship.StartCellLocation = ship.EndCellLocation;
            ship.EndCellLocation = tmp;
        }

        // checks if horizontal cells around the current one are busy or not
        private bool cellsAroundHorizFree(Board b, int xPos, int yPos, int dim)
        {
            bool busyFound = false;
            bool res = false;
            if (xPos + dim <= DIM)
            {
                for (int i = 1; i <= dim - 1; i++)
                {
                    if (b.GetCell(xPos + i, yPos).IsBusy)
                    {
                        busyFound = true;
                        break;
                    }
                }
                if (!busyFound)
                    res = true;
            }
            else
            {
                for (int i = 1; i <= dim - 1; i++)
                {
                    if (b.GetCell(xPos - i, yPos).IsBusy)
                    {
                        busyFound = true;
                        break;
                    }
                }
                if (!busyFound)
                    res = true;
            }
            return res;
        }

        // checks if vertical cells around the current one are busy or not
        private bool cellsAroundVertFree(Board b, int xPos, int yPos, int dim)
        {
            bool busyFound = false;
            bool res = false;
            if (yPos + dim <= DIM)
            {
                for (int i = 1; i <= dim - 1; i++)
                {
                    if (b.GetCell(xPos, yPos + i).IsBusy)
                    {
                        busyFound = true;
                        break;
                    }
                }
                if (!busyFound)
                    res = true;
            }
            else
            {
                for (int i = 1; i <= dim - 1; i++)
                {
                    if (b.GetCell(xPos, yPos - i).IsBusy)
                    {
                        busyFound = true;
                        break;
                    }
                }
                if (!busyFound)
                    res = true;
            }
            return res;
        }

        // initializes the cells
        private void Initialize()
        {
            for (int i = 0; i < DIM; i++)
            {
                for (int j = 0; j < DIM; j++)
                {
                    Cell c = new Cell();
                    c.IsBusy = false;
                    c.LocationX = i;
                    c.LocationY = j;
                    c.Name = NameCell(i, j);
                    cell[i, j] = c;
                }
            }
        }

        /// <summary>
        /// Returns the name for a cell.
        /// </summary>
        /// <param name="i">the horizontal index</param>
        /// <param name="j">the vertical index</param>
        /// <returns></returns>
        private string NameCell(int i, int j)
        {
            StringBuilder sb = new StringBuilder();

            switch (i)
            {
                case 0:
                    sb.Append('A');
                    break;
                case 1:
                    sb.Append('B');
                    break;
                case 2:
                    sb.Append('C');
                    break;
                case 3:
                    sb.Append('D');
                    break;
                case 4:
                    sb.Append('E');
                    break;
                case 5:
                    sb.Append('F');
                    break;
                case 6:
                    sb.Append('G');
                    break;
                case 7:
                    sb.Append('H');
                    break;
                case 8:
                    sb.Append('I');
                    break;
                case 9:
                    sb.Append('J');
                    break;
                default:
                    break;
            }

            sb.Append(j + 1);

            return sb.ToString();
        }
        #endregion
    }
}
