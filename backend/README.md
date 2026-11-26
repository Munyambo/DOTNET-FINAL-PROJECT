# Backend (ASP.NET Core Web API)

The API project is in `RMSWebApi`

-> How to run it

1. Open `RMSWebApi/RMSWebApi.slnx` in Visual Studio
2. Choose the `http` profile next to the green ▶ and press ▶
3. Open Swagger to test the endpoints: http://localhost:5103/swagger

-> Rules

- One controller per topic in `Controllers/` (e.g. `BranchController`)
- Routes: `api/[controller]` with `GetAll`, `Get/{id}`, `Create`, `Update`, `Delete/{id}`
