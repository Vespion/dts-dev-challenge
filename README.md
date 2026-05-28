
# Application coding challenge

This is my submission for the technical challenge portion of the [17385 - Software Developer](https://jobs.justice.gov.uk/careers/JobDetail/17385) role.

It uses the .NET Aspire stack to manage a frontend and backend application, along with a PostgreSQL database for persistence.

## Prerequisites

- .NET 10 SDK (10.0.107 or later)
- Docker (Docker also needs to be running, as the stack will not start it automatically)
## Run Locally

Clone the project

```bash
  git clone https://github.com/Vespion/dts-dev-challenge
```

Go to the app host directory

```bash
  cd dts-dev-challenge/dts-dev-challenge.AppHost
```

Start the stack

```bash
  dotnet run
```

When the application is ready, a link will be displayed in the console for the Aspire dashboard.

Like this:
```
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17252
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17252/login?t=3eb68aaf6507403b42f33f650906750a

```

This dashboard will display the state of each service, most notably the frontend (which will be running on a local port such as `https://localhost:7211/`). Clicking this link will open the frontend project in a new tab.

![The aspire dashboard with running services](https://raw.githubusercontent.com/Vespion/dts-dev-challenge/refs/heads/main/aspire%20dashboard.png)

## Running Tests

To run tests, run the following command from the project root

```bash
  dotnet test
```

## API Documentation

The backend API exposes a standard OpenAPI specification at the following endpoint:

```http
  GET /openapi/v1.json
```

However, the API documentation is also available in a more user-friendly format at the following endpoint:

```http
  GET /scalar
```

This documentation is generated from the OpenAPI specification and provides an interactive interface for exploring the API endpoints, request parameters, and response formats.

There is also an exported documentation file in the repository at `API.md` which contains the same information but does not require the server to be running.

Accessing the API documentation at the `/scalar` endpoint is recommended, as it provides interactivity through a more user-friendly interface.
