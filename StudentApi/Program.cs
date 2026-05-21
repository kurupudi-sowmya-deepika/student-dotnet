var builder = WebApplication.CreateBuilder(args);  //creates app builder

// Add services
builder.Services.AddControllers(); // enables controller

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build(); // builds application

// Configure HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // generates swagger json

    app.UseSwaggerUI(); // shows swagger webpage
}

app.UseHttpsRedirection();

app.UseAuthorization(); 

app.MapControllers(); // connects controller routes

app.Run(); // start webserver