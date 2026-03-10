var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<OrderService.Services.OrderRepository>();


builder.Services.AddHttpClient<OrderService.Services.IOrderService,
                               OrderService.Services.OrderService>(client =>
                               {
                                   var productServiceUrl = builder.Configuration["ProductServiceUrl"]
                                                           ?? "http://localhost:5001";
                                   client.BaseAddress = new Uri(productServiceUrl);
                               });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();