using DurableSaga.Models;
using DurableSaga.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableSaga.Activities
{
    public static class Taxi
    {
        [FunctionName("TaxiBookingOrchestrator")]
        public static async Task<Booking> TaxiBookingOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var id = context.GetInput<string>();
            var booking = await context.CallActivityAsync<Booking>("GetBooking", id);
            booking.Taxi = await context.CallActivityAsync<bool>("BookTaxi", null);
            await context.CallActivityAsync("SaveBooking", booking);
            return booking;
        }

        [FunctionName("TaxiCancellationOrchestrator")]
        public static async Task<Booking> TaxiCancellationOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var id = context.GetInput<string>();
            var booking = await context.CallActivityAsync<Booking>("GetBooking", id);
            booking.Taxi = await context.CallActivityAsync<bool>("CancelTaxi", null);
            await context.CallActivityAsync("SaveBooking", booking);
            return booking;
        }

        [FunctionName("BookTaxi")]
        public static bool BookTaxi([ActivityTrigger] object input, ILogger log)
        {
            var randomFlag = RandomFlagGenerator.Generate();

            log.LogInformation($"Taxi {randomFlag.Message}.");
            return randomFlag.Flag;
        }

        [FunctionName("CancelTaxi")]
        public static bool CancelTaxi([ActivityTrigger] object input, ILogger log)
        {
            log.LogInformation($"Taxi cancelled.");
            return false;
        }
    }
}
