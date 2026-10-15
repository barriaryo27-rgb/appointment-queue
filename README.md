# Appointment Queue API

A small ASP.NET Core Web API (.NET 8) where an agency books appointments for customers,
issues queue tokens, and looks at the queue for any day.

Built for the Qwiik case study. Optional features are included: off days (public holidays)
and a maximum number of appointments per day with automatic overflow to the next day.

## How it works

1. The agency books a customer for a date: `POST /api/appointments`.
2. The service finds the first day, starting at the requested date, that is **not an off day**
   and **not full**.
3. The customer gets a **token number**: 1 for the first customer of that day, 2 for the next, and so on.
4. The agency opens the queue for a day: `GET /api/appointments?date=2026-10-20`.

Every appointment keeps both `requestedDate` and `appointmentDate`, so it is clear when a
customer was moved to a later day.

## Project structure

```
AppointmentQueue.sln
├── src/AppointmentQueue.Business   Business layer: models, interfaces, services, in-memory repositories
├── src/AppointmentQueue.Api        Web layer: controllers, Swagger, Autofac setup, error handling
└── tests/AppointmentQueue.Tests    Unit tests (xUnit + Moq)
```

The Business project knows nothing about the web. The Api project only receives requests,
calls a service, and returns the result.

## Run it

Requires the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```bash
dotnet run --project src/AppointmentQueue.Api
```

Open the URL printed in the console. The root page redirects to **Swagger UI**, where every
endpoint can be tried from the browser.

## Run the tests

```bash
dotnet test
```

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| POST | `/api/appointments` | Book an appointment and get a token. Body: `{ "customerName": "Andi", "date": "2026-10-20" }` |
| GET | `/api/appointments?date=2026-10-20` | Queue for that day, ordered by token (defaults to today) |
| GET | `/api/offdays` | List off days |
| POST | `/api/offdays` | Add an off day. Body: `{ "date": "2026-12-25", "description": "Christmas" }` |
| DELETE | `/api/offdays/2026-12-25` | Remove an off day |
| GET | `/api/settings/max-appointments` | Show the daily limit (default 10) |
| PUT | `/api/settings/max-appointments` | Change the daily limit. Body: `{ "maxAppointmentsPerDay": 5 }` |

Errors come back as JSON: `400` for invalid input (empty name, past date, limit below 1),
`409` for conflicts (adding an off day twice), `500` for anything unexpected. Every error and every booking is logged.

## Demonstration points

| Point | Where |
|---|---|
| Separation of business and web layer | `AppointmentQueue.Business` vs `AppointmentQueue.Api` |
| Third-party libraries via NuGet | Autofac, Swashbuckle (Swagger), xUnit, Moq |
| Ready for IoC/DI, with Autofac | Services depend only on interfaces; wiring is in `Program.cs` |
| LINQ | `AppointmentService` and the in-memory repositories |
| Swagger documentation | `/swagger`, generated from the controllers |
| Unit tests | `tests/AppointmentQueue.Tests` |
| Web API | `Controllers/` |
| Azure hosting | See below |
| GitHub | This repository, with a CI workflow that builds and tests every push |

## Assumptions and design decisions

The case study could not be clarified with the recruiter, so these are my interpretations:

- **Only an API, no UI.** The "grid" is the queue endpoint, viewable in Swagger.
- **In-memory storage.** Data is lost when the app restarts. Storage sits behind
  `IAppointmentRepository` and `IOffDayRepository`, so a database can replace it without changing the services.
- **Weekends are normal days** unless the agency adds them as off days.
- **Overflow:** when a day is full or an off day, the booking moves to the next available day.
  The token restarts from 1 on each day.
- **Past dates are rejected.** "Today" is taken from UTC and comes from `IClock`, so tests can fix the date.
- **Thread safety:** booking runs inside a lock, so two simultaneous requests cannot get the same token.
- Changing the daily limit does not move existing appointments; it only affects new bookings.

## Deploy to Azure App Service

With the [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli), from `src/AppointmentQueue.Api`:

```bash
az login
az webapp up --name <unique-app-name> --runtime "DOTNETCORE:8.0" --sku F1 --location southeastasia
```

Then open `https://<unique-app-name>.azurewebsites.net` to see Swagger.

Alternative without the CLI: in the Azure Portal create an **App Service** (Runtime stack `.NET 8`),
open **Deployment Center**, choose **GitHub**, and select this repository.

Note: the free tier may take a few seconds to wake up, and the in-memory data resets when the app restarts.
