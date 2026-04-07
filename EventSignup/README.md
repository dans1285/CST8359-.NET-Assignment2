# Event Signup Application

ASP.NET Core MVC application for managing events and attendee signups.

## Features

- View all available events
- Manage attendees for each event
- Sign up attendees with name and email
- View list of registered attendees per event

## Running Locally

```bash
dotnet restore
dotnet build
dotnet run
```

Navigate to `http://localhost:5000` in your browser.

## Project Structure

- **Models**: Event and Attendee domain models
- **Controllers**: EventController with hardcoded event data
- **Views**: Event Manager and Manage Attendees pages

## Technologies

- ASP.NET Core 9.0 MVC
- Bootstrap 5 for styling
- No database (in-memory data storage)
