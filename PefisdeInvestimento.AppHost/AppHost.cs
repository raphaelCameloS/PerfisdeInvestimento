var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.PerfisdeInvestimento_API>("perfisdeinvestimento-api");

builder.Build().Run();
