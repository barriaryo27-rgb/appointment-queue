# Appointment Queue API

A simple appointment booking API for the Qwiik case study, built with ASP.NET Core (.NET 8).

An agency can book appointments for customers, each customer gets a queue token, and the agency can see the queue for any day. I also did the optional parts: off days and a maximum number of appointments per day.

## How booking works

- The customer asks for a date.
- If that date is an off day or already full, the booking moves to the next available day.
- The token is the customer's queue number for that day, starting from 1.
- Both `requestedDate` and `appointmentDate` are saved, so it is easy to see when someone was moved.

## Project structure

- `src/AppointmentQueue.Business` – models, interfaces, services and in-memory storage (all the booking rules)
- `src/AppointmentQueue.Api` – controllers, Swagger, Autofac setup, error handling
- `tests/AppointmentQueue.Tests` – unit tests (xUnit and Moq)

I kept the business logic out of the controllers. Services only depend on interfaces, and Autofac wires them up in `Program.cs`.

## Run it

You need the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet test
dotnet run --project src/AppointmentQueue.Api
```

Then open the URL shown in the console. It goes straight to Swagger, where you can try every endpoint.

## Endpoints

| Method | Route | What it does |
|---|---|---|
| POST | `/api/appointments` | Book an appointment, e.g. `{ "customerName": "Andi", "date": "2026-10-20" }` |
| GET | `/api/appointments?date=2026-10-20` | Queue for a day, ordered by token (default: today) |
| GET / POST | `/api/offdays` | List or add off days |
| DELETE | `/api/offdays/2026-12-25` | Remove an off day |
| GET / PUT | `/api/settings/max-appointments` | Show or change the daily limit (default 10) |

Errors return JSON with a clear message: 400 for invalid input, 409 for conflicts, 500 for anything unexpected. Bookings and errors are logged.

## Assumptions

The brief was not very specific and I could not ask questions, so I decided the following:

- API only, no UI. The "grid" is the queue endpoint, which can be viewed in Swagger.
- Data is stored in memory and is lost on restart. Storage is behind repository interfaces, so a database can replace it later.
- Weekends are normal days unless added as off days.
- Past dates are rejected.
- Booking uses a lock so two requests at the same time cannot get the same token.
- Changing the daily limit only affects new bookings.

## What I would improve with more time

- A real database (SQL Server with EF Core)
- Cancel and reschedule appointments
- Login for the agency
- Weekends as off days by default

## Deploy to Azure

```bash
cd src/AppointmentQueue.Api
az login
az webapp up --name <unique-app-name> --runtime "DOTNETCORE:8.0" --sku F1 --location southeastasia
```

Live demo: _add the Azure link here once deployed_