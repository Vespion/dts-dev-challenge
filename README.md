
# Application coding challenge

This is my submission for the technical challenge portion of the [17385 - Software Developer](https://jobs.justice.gov.uk/careers/JobDetail/17385) role.

It uses the .net Aspire stack to manage a frontend and backend applications along with a postgres database for persistance.

## Prerequisites

- .NET 10 SDK
- Docker
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

When the application is ready a link will be displayed in the console for the aspire dashboard.

This dashboard will display the state of each service, most notably the frontend (which will be running on a local port such as `http://localhost:5151/`)
## Running Tests

To run tests, run the following command from the project root

```bash
  dotnet test
```

