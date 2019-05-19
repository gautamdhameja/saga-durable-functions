using System;
using System.Collections.Generic;
using System.Text;

namespace DurableSaga.Models
{
    public class Booking
    {
        public string Id { get; set; }

        public bool Flight { get; set; }

        public bool Hotel { get; set; }

        public bool Taxi { get; set; }
    }
}
