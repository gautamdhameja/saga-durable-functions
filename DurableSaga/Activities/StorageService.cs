using DurableSaga.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DurableSaga.Activities
{
    public static class StorageService
    {
        public static ConcurrentDictionary<string, Booking> Bookings = new ConcurrentDictionary<string, Booking>();

        [FunctionName("GetBooking")]
        public static Booking GetBooking([ActivityTrigger] string id, ILogger log)
        {
            // return a booking object from storage
            if(Bookings.ContainsKey(id))
            {
                return Bookings[id];
            }

            return new Booking { Id = id };
        }

        [FunctionName("SaveBooking")]
        public static void SaveBooking([ActivityTrigger] Booking booking, ILogger log)
        {
            Bookings.AddOrUpdate(booking.Id, booking, (k, o) => booking);
        }
    }
}
