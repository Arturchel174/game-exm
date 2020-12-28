using System;

namespace Battleship
{
    /// <summary>
    /// Represents a cell in the board.
    /// </summary>
    public class Cell
    {
        #region Fields
        private bool isBusy;
        private string name;
        private int locationX;
        private int locationY;
        private Ship ship;
        #endregion

        #region Properties
        /// <summary>
        /// Determines if current cell is occupied by a ship.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                isBusy = value;
            }
        }

        /// <summary>
        /// The name given to the cell (e.g.: 'J10')
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        /// The cell zero-based x location.
        /// </summary>
        public int LocationX
        {
            get
            {
                return locationX;
            }
            set
            {
                locationX = value;
            }
        }

        /// <summary>
        /// The cell zero-based y location.
        /// </summary>
        public int LocationY
        {
            get
            {
                return locationY;
            }
            set
            {
                locationY = value;
            }
        }

        /// <summary>
        /// The ship associated with this cell (null if cell is free).
        /// </summary>
        public Ship Ship
        {
            get
            {
                return ship;
            }
            set
            {
                ship = value;
            }
        }
        
        /// <summary>
        /// True if the cell is chosen in a match.
        /// </summary>
        public bool Chosen { get; set; } = false;

        /// <summary>
        /// True if the cell is chosen and it holds a ship.
        /// </summary>
        public bool Hit { get; set; } = false;
        #endregion
    }
}
