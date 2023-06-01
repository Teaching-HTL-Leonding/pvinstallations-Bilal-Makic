using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PvContext>(options =>
 options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
var app = builder.Build();

app.MapPost("/installations", async (PvInstallation installation, PvContext context) => {
    var dbPvInstallation = new PvInstallation{
        Longitude = installation.Longitude,
        Latitude = installation.Latitude,
        Address = installation.Address,
        OwnerName = installation.OwnerName,
        IsActive = true,
        Comments = installation.Comments
    };

    await context.PvInstallations.AddAsync(dbPvInstallation);
    await context.SaveChangesAsync();
    return Results.Ok(dbPvInstallation.Id);
});

app.MapPost("/installations/{id}/deactivate", async (int id, PvContext context) => {
    var installation = await context.PvInstallations.FindAsync(id);
    if(installation == null) return Results.NotFound();
    installation.IsActive = false;
    await context.SaveChangesAsync();
    return Results.Ok();
});

app.MapPost("/installations/{id}/reports", async (int id, ProductionReport report, PvContext context) => {
    var installation = await context.PvInstallations.FindAsync(id);
    if(installation == null) return Results.NotFound();
    var dbReport = new ProductionReport{
        Timestamp = DateTime.UtcNow,
        ProducedWattage = report.ProducedWattage,
        HouseholdWattage = report.HouseholdWattage,
        BatteryWattage = report.BatteryWattage,
        GridWattage = report.GridWattage,
        PvInstallation = installation
    };
    await context.ProductionReports.AddAsync(dbReport);
    await context.SaveChangesAsync();
    return Results.Ok();
});

app.MapGet("/installations/{id}/reports", async (int id, DateTime start, int duration, PvContext context) => {
    var installation = await context.PvInstallations.FindAsync(id);
    if (installation == null) { return Results.NotFound(); }

    var allProducedWattage = await context.ProductionReports
    .Where(pr => pr.Id == id)
    .Where(pr => pr.Timestamp >= start && pr.Timestamp < start.AddMinutes(duration))
    .SumAsync(pr => pr.ProducedWattage);

    return Results.Ok(allProducedWattage);
});


app.MapGet("/", () => "Hello World!");

app.Run();


class PvInstallation{
    public int Id { get; set; }
    public float Longitude { get; set; }
    public float Latitude { get; set; }
    public string Address { get; set; } = "";
    public string OwnerName { get; set; } = "";
    public bool IsActive { get; set; }
    public string Comments { get; set; } = "";
}

/*
A ProductionReport entity with the following properties:

Id: A unique identifier for the report record, assigned by the system (mandatory)
Timestamp: A date and time value in UTC representing when the data was collected (mandatory)
ProducedWattage: A float representing the wattage produced by the installation in the minute after the Timestamp (mandatory)
HouseholdWattage: A float representing the wattage used by the household in the minute after the Timestamp (mandatory)
BatteryWattage: A float representing the wattage stored in the batteries in the minute after the Timestamp (mandatory)
GridWattage: A float representing the wattage fed into the grid in the minute after the Timestamp (mandatory)
Relation to the installation that produced the report (mandatory)
*/

class ProductionReport {
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    public float ProducedWattage { get; set; }
    public float HouseholdWattage { get; set; }
    public float BatteryWattage { get; set; }
    public float GridWattage { get; set; }
    public PvInstallation? PvInstallation { get; set; }
}

/*
A DbContext class, PvDbContext, which includes DbSet properties for PvInstallation and ProductionReport entities.
*/

class PvContext: DbContext{

    public PvContext(DbContextOptions<PvContext> options) : base(options) { }
    public DbSet<PvInstallation> PvInstallations => Set<PvInstallation>();
    public DbSet<ProductionReport> ProductionReports => Set<ProductionReport>();
    
}