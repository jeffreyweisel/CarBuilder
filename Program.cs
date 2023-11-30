using CarBuilder.Models;
using CarBuilder.Models.DTOs;

// List of paint colors
List<PaintColor> paintColors = new()
{
    new() { Id = 1, Price = 124.99M, Color = "Silver" },
    new() { Id = 2, Price = 169.99M, Color = "Midnight Blue" },
    new() { Id = 3, Price = 189.99M, Color = "Firebrick Red" },
    new() { Id = 4, Price = 199.99M, Color = "Spring Green"}
};

//List of interior options
List<Interior> interiors = new()
{
    new() { Id = 1, Price = 199.99M, Material = "Beige Fabric" },
    new() { Id = 2, Price = 119.99M, Material = "Charcoal Fabric" },
    new() { Id = 3, Price = 189.99M, Material = "White Leather" },
    new() { Id = 4, Price = 109.99M, Material = "Black Leather"}
};

// List of technology packages
List<Technology> technologies = new()
{
    new () { Id = 1, Price = 199.99M, Package = "Basic Package (basic sound system)" },
    new () { Id = 2, Price = 399.99M, Package = "Navigation Package (includes integrated navigation controls)" },
    new ()  { Id = 3, Price = 289.99M, Package = "Visibility Package (includes side and rear cameras)" },
    new ()  { Id = 4, Price = 999.99M, Package = "Ultra Package (includes navigation and visibility packages)"}
};

// List of wheel options
List<Wheels> wheels = new()
{
    new () { Id = 1, Price = 699.99M, Style = "17-inch Pair Radial" },
    new () { Id = 2, Price = 499.99M, Style = "17-inch Pair Radial Black" },
    new () { Id = 3, Price = 899.99M, Style = "18-inch Pair Spoke Silver" },
    new () { Id = 4, Price = 999.99M, Style = "18-inch Pair Spoke Black"}
};

// List of orders
List<Order> orders = new()
{
    new() { Id = 1, TimeStamp = new DateTime(2023, 11, 7), WheelId = 1, TechnologyId = 2, PaintId = 3, InteriorId = 4, Completed = false }
};

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(options =>
                {
                    options.AllowAnyOrigin();
                    options.AllowAnyMethod();
                    options.AllowAnyHeader();
                });
}

app.UseHttpsRedirection();

// Get Paint Colors
app.MapGet("/paintcolors", () =>
{
    return paintColors.Select(pc => new PaintColorDTO
    {
        Id = pc.Id,
        Price = pc.Price,
        Color = pc.Color
    });
});

// Get Interior Options
app.MapGet("/interiors", () =>
{
    return interiors.Select(i => new InteriorDTO
    {
        Id = i.Id,
        Price = i.Price,
        Material = i.Material
    });
});

// Get Technology Packages
app.MapGet("technologies", () =>
{
    return technologies.Select(t => new TechnologyDTO
    {
        Id = t.Id,
        Price = t.Price,
        Package = t.Package
    });
});

// Get Wheel Options
app.MapGet("/wheels", () =>
{
    return wheels.Select(w => new WheelsDTO
    {
        Id = w.Id,
        Price = w.Price,
        Style = w.Style
    });
});

// Get orders that are not marked as completed
app.MapGet("/orders", () =>
{
    return orders.Where(o => o.Completed != true)
    .Select(o =>
    {
        var wheel = wheels.FirstOrDefault(w => w.Id == o.WheelId);
        var technology = technologies.FirstOrDefault(t => t.Id == o.TechnologyId);
        var paintColor = paintColors.FirstOrDefault(pc => pc.Id == o.PaintId);
        var interior = interiors.FirstOrDefault(i => i.Id == o.InteriorId);

        return new OrderDTO
        {
            Id = o.Id,
            TimeStamp = o.TimeStamp,
            WheelId = o.WheelId,
            TechnologyId = o.TechnologyId,
            PaintId = o.PaintId,
            InteriorId = o.InteriorId,
            TotalCost = (wheel?.Price ?? 0) + (interior?.Price ?? 0) + (technology?.Price ?? 0) + (paintColor?.Price ?? 0),
            Completed = o.Completed,

            Wheels = wheel == null ? null : new WheelsDTO
            {
                Id = wheel.Id,
                Price = wheel.Price,
                Style = wheel.Style
            },
            Technology = technology == null ? null : new TechnologyDTO
            {
                Id = technology.Id,
                Price = technology.Price,
                Package = technology.Package
            },
            Paint = paintColor == null ? null : new PaintColorDTO
            {
                Id = paintColor.Id,
                Price = paintColor.Price,
                Color = paintColor.Color
            },
            Interior = interior == null ? null : new InteriorDTO
            {
                Id = interior.Id,
                Price = interior.Price,
                Material = interior.Material
            }
        };
    });
});



// Mark order as complete by changing Completed value to true
app.MapPost("/orders/{id}/fulfill", (int id) =>
{

    Order orderToComplete = orders.FirstOrDefault(o => o.Id == id);

    if (orderToComplete == null)
    {
        return Results.NotFound();
    }

    orderToComplete.Completed = true;

    return Results.NoContent();

});

// Get order by Id
app.MapGet("/orders/{id}", (int id) =>
{
    Order order = orders.FirstOrDefault(o => o.Id == id);

    if (order == null)
    {
        return Results.NotFound();
    }

    var wheelChoice = wheels.FirstOrDefault(w => w.Id == order.WheelId);
    var interiorChoice = interiors.FirstOrDefault(i => i.Id == order.InteriorId);
    var techChoice = technologies.FirstOrDefault(t => t.Id == order.TechnologyId);
    var paintChoice = paintColors.FirstOrDefault(pc => pc.Id == order.PaintId);

    return Results.Ok(new OrderDTO
    {
        Id = order.Id,
        WheelId = order.WheelId,
        Wheels = wheelChoice == null ? null : new WheelsDTO
        {
            Id = wheelChoice.Id,
            Price = wheelChoice.Price,
            Style = wheelChoice.Style
        },
        InteriorId = order.InteriorId,
        Interior = interiorChoice == null ? null : new InteriorDTO
        {
            Id = interiorChoice.Id,
            Price = interiorChoice.Price,
            Material = interiorChoice.Material
        },
        TechnologyId = order.TechnologyId,
        Technology = techChoice == null ? null : new TechnologyDTO
        {
            Id = techChoice.Id,
            Price = techChoice.Price,
            Package = techChoice.Package
        },
        PaintId = order.PaintId,
        Paint = paintChoice == null ? null : new PaintColorDTO
        {
            Id = paintChoice.Id,
            Price = paintChoice.Price,
            Color = paintChoice.Color
        },
        TimeStamp = order.TimeStamp,
        Completed = order.Completed
    });
});

//Post a new order to the orders list
app.MapPost("/orders", (Order order) =>
{
    // Assigns Id to new order
    order.Id = orders.Max(o => o.Id) + 1;

    orders.Add(order);

    return Results.Created($"/orders/{order.Id}", new OrderDTO
    {
        Id = order.Id,
        TimeStamp = DateTime.Now,
        WheelId = order.WheelId,
        TechnologyId = order.TechnologyId,
        PaintId = order.PaintId,
        InteriorId = order.InteriorId,
        Completed = false
    });
});

app.Run();
