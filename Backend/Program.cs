using Core.Algorithm.Interfaces;
using Core.Algorithm.Implementations;
using Microsoft.EntityFrameworkCore;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<ICalculator, Calculator>();
    builder.Services.AddSingleton<IParser, Parser>();
    builder.Services.AddScoped<IHistoryRepository, HistoryRepository>();

    builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=calculator.db"));

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();
    app.Run();
}
catch (Exception e)
{
    Console.WriteLine($"Fatal error: {e.Message}");
    Console.WriteLine(e.StackTrace);
}