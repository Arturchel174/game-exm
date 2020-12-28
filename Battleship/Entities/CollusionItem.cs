using System;
using System.Collections.Generic;

namespace Battleship
{
    public class CollusionItem
    {
        public IDragAndDropItem item;
        public bool UnderMouse { get; set; } = false;


        public CollusionItem(IDragAndDropItem item)
        {

            this.item = item;

        }
    }
}
