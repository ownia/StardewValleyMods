using System;

namespace BusTicketPrice
{
    public class ModConfig
    {
        public int DesertBusTicketPrice { get; set; }

        public ModConfig()
        {
            this.DesertBusTicketPrice = 2;
        }

        public bool IsValid()
        {
            return DesertBusTicketPrice >= 0 && DesertBusTicketPrice <= 999999;
        }
    }
}

