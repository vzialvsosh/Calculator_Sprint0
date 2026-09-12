using Core.Algorithm.Interfaces;
using Core.Algorithm.Implementations;

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddSingleton<ICalculator, Calculator>();

    var app = builder.Build();

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