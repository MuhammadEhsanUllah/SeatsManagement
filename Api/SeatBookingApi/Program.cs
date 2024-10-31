using Microsoft.EntityFrameworkCore;
using SeatBookingApi.Domain;
using SeatBookingApi.Interfaces;
using SeatBookingApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SeatBookingConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:3000")
                          // specify allowed origins
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});
//Admin
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IVenueService, VenueService>();
//Client
builder.Services.AddScoped<IClientSeatService, ClientSeatService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
