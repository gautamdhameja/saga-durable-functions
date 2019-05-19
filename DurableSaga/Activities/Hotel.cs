using DurableSaga.Models;
using DurableSaga.Utils;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DurableSaga.Activities
{
    public static class Hotel
    {
        [FunctionName("HotelBookingOrchestrator")]
        public static async Task<Booking> HotelBookingOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var id = context.GetInput<string>();
            var booking = await context.CallActivityAsync<Booking>("GetBooking", id);
            booking.Hotel = await context.CallActivityAsync<bool>("BookHotel", null);
            await context.CallActivityAsync("SaveBooking", booking);
            return booking;
        }

        [FunctionName("HotelCancellationOrchestrator")]
        public static async Task<Booking> HotelCancellationOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            var id = context.GetInput<string>();
            var booking = await context.CallActivityAsync<Booking>("GetBooking", id);
            booking.Hotel = await context.CallActivityAsync<bool>("CancelHotel", null);
            await context.CallActivityAsync("SaveBooking", booking);
            return booking;
        }

        [FunctionName("BookHotel")]
        public static bool BookHotel([ActivityTrigger] object input, ILogger log)
        {
            var randomFlag = RandomFlagGenerator.Generate();

            log.LogInformation($"Hotel {randomFlag.Message}.");
            return randomFlag.Flag;
        }

        [FunctionName("CancelHotel")]
        public static bool CancelHotel([ActivityTrigger] object input, ILogger log)
        {
            log.LogInformation($"Hotel cancelled.");
            return false;
        }
    }
}
