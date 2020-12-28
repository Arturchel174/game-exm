using System;

namespace Battleship
{
    public enum ShipDirection
    {
        Horizontal = 0,
        Vertical = 1,
        Undefined = 2
    }

    public enum ShipType
    {
        Unknown = 1,
        Destroyer = 2,
        Submarine = 3,
        BattleShip = 4,
        AirCraft = 5
    }

    public enum ShipState
    {
        Unhit = 0,
        Hit = 1,
        Sink = 2
    }

    /// <summary>
    /// An abstract ship object.
    /// </summary>
    public abstract class Ship
    {
        #region Fields
        protected int length;
        protected int numberOfHit;

        private string name;
        private int startCellLocation;
        private int endCellLocation;

        private ShipType type;
        private ShipDirection direction;
        private ShipState state;
        #endregion

        #region Properties
        /// <summary>
        /// // The ship name.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// The zero-based starting location in the grid.
        /// </summary>
        public int StartCellLocation
        {
            get
            {
                return startCellLocation;
            }
            set
            {
                this.startCellLocation = value;
            }
        }

        /// <summary>
        /// The zero-based ending location in the grid.
        /// </summary>
        public int EndCellLocation
        {
            get
            {
                return endCellLocation;
            }
            set
            {
                this.endCellLocation = value;
            }
        }

        /// <summary>
        /// The ship direction.
        /// </summary>
        public ShipDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                this.direction = value;
            }
        }

        /// <summary>
        /// The ship type.
        /// </summary>
        public ShipType Type
        {
            get
            {
                return type;
            }
            set
            {
                this.type = value;
            }

        }

        /// <summary>
        /// The ship state.
        /// </summary>
        public ShipState State
        {
            get
            {
                return state;
            }
            set
            {
                this.state = value;
            }
        }

        /// <summary>
        /// Returns the length of the ship.
        /// </summary>
        /// <returns>the ship length</returns>
        public abstract int GetLength();

        /// <summary>
        /// The number of cell locations that were hit.
        /// </summary>
        public int NumberOfHit
        {
            get
            {
                return numberOfHit;
            }
            set
            {
                numberOfHit = value;
            }
        }
        #endregion
    }
}
