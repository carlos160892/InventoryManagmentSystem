var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios 

builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings")); 
builder.Services.AddSingleton<SmtpEmailService>();

builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization(); 
app.MapControllers(); 

app.UseHttpsRedirection();
app.Run();
