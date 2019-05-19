# Saga-Durable-Functions

Saga pattern implementation using Azure Durable Functions.

## Scenario

In this example, Saga pattern is implemented as a workflow which includes booking of a flight, hotel and taxi (yeah, the most common workflow example).

To manage the state locally, `ConcurrentDictionary` is used to easily work across tasks. Each item in the `ConcurrentDictionary` is a booking which encapsulates the state of flight, hotel and taxi booking as bool values. See `Models/Booking.cs` and `Activities/StorageService.cs` for details.

For bookings and cancellations, `SubOrchestrator`s are used which include calling activities to book and then save the booking status. As an example, see `Activities/Hotel.cs` for details.

Finally, a top-most main `Orchestrator` is used as Saga manager to call `SubOrchestrator`s and activities.

The aim is to always have the system in a consistent state; either all three of flight, hotel and taxi are booked or none of them.

## Usage

* Clone this repo.
* Build and run solution.
* Using postman or any other API tool, call the following endpoint to start the workflow:

```
GET http://localhost:7071/api/SagaClient
```

* Monitor the logs in the console to check the status of bookings.

At the end of the workflow execution, the status of bookings will always be either all false or all true. Hence putting the system in an *eventually* consistent state.