using DurableSaga.Models;
using DurableSaga.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableSaga.Activities
{
    public static class Flight
    {
        [FunctionName("FlightBookingOrchestrator")]
        public static async Task<Booking> FlightBookingOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var id = context.GetInput<string>();
            var booking = await context.CallActivityAsync<Booking>("GetBooking", id);
            booking.Flight = await context.CallActivityAsync<bool>("BookFlight", null);
            await context.CallActivityAsync("SaveBooking", booking);
            return booking;
        }

        [FunctionName("BookFlight")]
        public static bool BookFlight([ActivityTrigger] string id, ILogger log)
        {
            var randomFlag = RandomFlagGenerator.Generate();

            log.LogInformation($"Flight {randomFlag.Message}.");
            return randomFlag.Flag;
        }

        [FunctionName("CancelFlight")]
        public static bool CancelFlight([ActivityTrigger] object input, ILogger log)
        {
            log.LogInformation($"Flight cancelled.");
            return false;
        }
    }
}
