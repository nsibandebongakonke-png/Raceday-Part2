# RaceDay API – Part 2: RESTful API Development


RaceDay is a platform for managing running, walking and cycling events. This repository contains the **RESTful API** that powers the entire system. It handles all data operations, enforces role-based access, and is fully covered by automated unit tests. The API is built to the endpoint plan produced in Part 1 and will be consumed by the MVC front end in Part 3.

---

## Table of Contents

1. [Tech Stack](#tech-stack)
2. [Features](#features)
3. [User Roles](#user-roles)
4. [Project Structure](#project-structure)
5. [Prerequisites](#prerequisites)
6. [Setup Instructions](#setup-instructions)
7. [Running the API](#running-the-api)
8. [Swagger Documentation](#swagger-documentation)
9. [API Endpoints](#api-endpoints)
10. [Authentication and Session Management](#authentication-and-session-management)
11. [Running the Unit Tests](#running-the-unit-tests)
12. [CI/CD with GitHub Actions](#cicd-with-github-actions)
13. [Demo Video](#demo-video)
14. [Author](#author)

---

## Tech Stack

| Area | Technology |
|------|------------|
| Language | C# |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core (Code-First) |
| Database | Microsoft SQL Server |
| Authentication | Session-based, role-based access |
| Password Security | Hashed passwords (no plain-text storage) |
| API Documentation | Swagger / Swashbuckle |
| Testing | xUnit (unit tests) |
| CI/CD | GitHub Actions |
| IDE | Visual Studio |

---

## Features

- User registration and login with role selection (Organiser or Participant)
- Session management to maintain authenticated state and role across requests
- Password hashing – original passwords are never stored
- Role-based authorisation on all non-public endpoints
- User profile viewing and updating
- Event management (create, update, delete, view)
- Age and distance categories per event
- Event enrolment with category selection
- Results capture (finish time and finishing position)
- Swagger UI for live endpoint viewing and testing
- Unit tests covering success and failure scenarios
- Automated build and test on every push via GitHub Actions

---

## User Roles

### Organiser
Organisers create and run events.

- Create, update and delete their own events
- Define age or distance categories for each event (e.g. Under 20, Senior, 10km, 21km)
- View all enrolments for their events
- Capture finish times and finishing positions for participants after an event
- View and update their own profile

### Participant
Participants take part in events.

- View all available events and categories
- Enrol in an event by selecting a category (the link between Participant, Event and Category is recorded)
- View their own results
- View and update their own profile

> Organiser functionality is not accessible to Participants, and Participant functionality is not accessible to Organisers. Unauthenticated users can only access public endpoints (register and login).

---

## Project Structure

> Update the folder and file names below so they match your solution.

```
RaceDay/
├── .github/
│   └── workflows/
│       └── dotnet.yml            # GitHub Actions CI workflow
├── RaceDay.Api/
│   ├── Controllers/              # API controllers
│   ├── Data/                     # DbContext
│   ├── Models/                   # Entity models
│   ├── DTOs/                     # Data transfer objects
│   ├── Migrations/               # EF Core migrations
│   ├── Program.cs
│   └── appsettings.json
├── RaceDay.Tests/                # Unit tests
├── docs/
│   └── images/
│       └── ci-green-build.png    # CI/CD screenshot
├── RaceDay.sln
└── README.md
```

---

## Prerequisites

Make sure the following are installed:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (with the **ASP.NET and web development** workload)
- [.NET SDK](https://dotnet.microsoft.com/download) (the version used by the project)
- [SQL Server](https://www.microsoft.com/sql-server) (Express, Developer or LocalDB)
- [Git](https://git-scm.com/)

---

## Setup Instructions

### 1. Clone the repository

```bash
git clone https://github.com/nsibandebongakonke-png/Raceday-Part2
cd Raceday-Part2
```

If Part 2 lives on its own branch:

```bash
git checkout <Raceday-Part2>
```

### 2. Configure the database connection

Open `RaceDay.Api/appsettings.json` and update the connection string to point to your SQL Server instance:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RaceDayDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Restore packages

```bash
dotnet restore
```

### 4. Create the database (EF Core Code-First migrations)

Using the **Package Manager Console** in Visual Studio:

```powershell
Update-Database
```

Or using the .NET CLI:

```bash
dotnet ef database update --project RaceDay.Api
```

This creates the database and all tables based on the entity models, matching the Part 1 ERD and SQL script.

---

## Running the API

**Visual Studio:** set `RaceDay.Api` as the startup project and press **F5** (or Ctrl + F5).

**Command line:**

```bash
dotnet run --project RaceDay.Api
```

The API will start on the URLs shown in the console (for example `https://localhost:7xxx`).

---

## Swagger Documentation

Swagger is integrated so every endpoint can be viewed and tested directly in the browser, with no additional tooling.

Once the API is running, open:

```
https://localhost:<port>/swagger
```

Each endpoint shows its purpose, the data it expects and the response it returns.

---

## API Endpoints

> Update this table so it matches your Part 1 plan and your controllers exactly.

### Authentication

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/auth/register` | Register a new user and select a role | Public |
| POST | `/api/auth/login` | Log in and start a session | Public |
| POST | `/api/auth/logout` | End the current session | Authenticated |

### User Profile

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/profile` | View own profile | Organiser, Participant |
| PUT | `/api/profile` | Update own profile | Organiser, Participant |

### Events

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/events` | View all events | Organiser, Participant |
| GET | `/api/events/{id}` | View a single event | Organiser, Participant |
| POST | `/api/events` | Create an event | Organiser |
| PUT | `/api/events/{id}` | Update an event | Organiser |
| DELETE | `/api/events/{id}` | Delete an event | Organiser |

Each event captures: **name, description, date, location, distance and event type (run, walk or cycle)**.

### Categories

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| GET | `/api/events/{eventId}/categories` | View categories for an event | Organiser, Participant |
| POST | `/api/events/{eventId}/categories` | Create a category | Organiser |
| PUT | `/api/categories/{id}` | Update a category | Organiser |
| DELETE | `/api/categories/{id}` | Delete a category | Organiser |

### Event Enrolments

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/enrolments` | Enrol in an event with a selected category | Participant |
| GET | `/api/events/{eventId}/enrolments` | View all enrolments for an event | Organiser |

### Results

| Method | Endpoint | Description | Access |
|--------|----------|-------------|--------|
| POST | `/api/results` | Capture finish time and position | Organiser |
| PUT | `/api/results/{id}` | Update a result | Organiser |
| GET | `/api/results/my` | View own results | Participant |

---

## Authentication and Session Management

- Users register with their personal details and choose a role (**Organiser** or **Participant**).
- Passwords are **hashed** before being stored; the original password is never saved.
- On login, the API creates a **session** that stores the user's authenticated state and role.
- Every non-public endpoint checks the session and rejects unauthenticated requests (**401 Unauthorized**).
- Role checks reject users accessing functionality that belongs to the other role (**403 Forbidden**).

---

## Running the Unit Tests

From the solution root:

```bash
dotnet test
```

Or in Visual Studio: **Test → Run All Tests**.

### What is covered

Tests cover both success and failure scenarios, for authenticated and unauthenticated requests:

- **User registration and login** – valid and invalid credentials, duplicate users
- **Event management** – Organisers can create, update and delete events
- **Role enforcement** – the wrong role is rejected when accessing role-based functionality
- **Event enrolments** – Participants can enrol and the enrolment is correctly recorded

All tests pass locally and through the GitHub Actions workflow.

---

## CI/CD with GitHub Actions

A GitHub Actions workflow (`.github/workflows/dotnet.yml`) builds the project and runs all unit tests on **every push**.




---

## Demo Video

Unlisted YouTube video demonstrating the running API, the code structure, endpoints, authentication and unit tests:

**[Watch the demo](https://youtu.be/YOUR_VIDEO_LINK)**

---

## Author

**Your Name**
Student Number: `ST10493442`
Module: `PROG6212`
The Independent Institute of Education, 2026