using DurableSaga.Models;
using Microsoft.Azure.WebJobs;
using System;
using System.Threading.Tasks;

namespace DurableSaga
{
    public static class Orchestrator
    {
       [FunctionName("SagaOrchestrator")]
        public static async Task<Booking> RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext context)
        {
            // Initial state. (false, false, false)
            var booking = await context.CallActivityAsync<Booking>("GetBooking", context.InstanceId);
            Console.WriteLine("Flight: {0}, Hotel: {1}, Taxi: {2}", booking.Flight, booking.Hotel, booking.Taxi);

            var hotelBooking = await context.CallSubOrchestratorAsync<Booking>("HotelBookingOrchestrator", null, context.InstanceId);
            Console.WriteLine("Flight: {0}, Hotel: {1}, Taxi: {2}", hotelBooking.Flight, hotelBooking.Hotel, hotelBooking.Taxi);

            if (hotelBooking.Hotel)
            {
                var taxiBooking = await context.CallSubOrchestratorAsync<Booking>("TaxiBookingOrchestrator", null, context.InstanceId);
                Console.WriteLine("Flight: {0}, Hotel: {1}, Taxi: {2}", taxiBooking.Flight, taxiBooking.Hotel, taxiBooking.Taxi);

                Booking flightBooking = null;

                if (taxiBooking.Taxi)
                {
                    flightBooking = await context.CallSubOrchestratorAsync<Booking>("FlightBookingOrchestrator", null, context.InstanceId);
                    Console.WriteLine("Flight: {0}, Hotel: {1}, Taxi: {2}", flightBooking.Flight, flightBooking.Hotel, flightBooking.Taxi);
                }
                else // hotel booked, taxi not booked - cancel hotel
                {
                    Console.WriteLine("Cancelling Hotel.");
                    await context.CallSubOrchestratorAsync<Booking>("HotelCancellationOrchestrator", null, context.InstanceId);
                }

                // hotel booked, taxi booked, flight not booked - cancel hotel & taxi
                if (flightBooking != null && flightBooking.Taxi && !flightBooking.Flight)
                {
                    Console.WriteLine("Cancelling Hotel and Taxi.");
                    await context.CallSubOrchestratorAsync<Booking>("HotelCancellationOrchestrator", null, context.InstanceId);
                    await context.CallSubOrchestratorAsync<Booking>("TaxiCancellationOrchestrator", null, context.InstanceId);
                }
            }

            // This is just to check the final state.
            // Not really needed.
            var completedBooking = await context.CallActivityAsync<Booking>("GetBooking", context.InstanceId);
            Console.WriteLine("Flight: {0}, Hotel: {1}, Taxi: {2}", completedBooking.Flight, completedBooking.Hotel, completedBooking.Taxi);
            return completedBooking;
        }
    }
}