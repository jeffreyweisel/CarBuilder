using CarBuilder.Models;
using CarBuilder.Models.DTOs;

// List of paint colors
List<PaintColor> paintColors = new List<PaintColor>
{
    new PaintColor { Id = 1, Price = 124.99M, Color = "Silver" },
    new PaintColor { Id = 2, Price = 169.99M, Color = "Midnight Blue" },
    new PaintColor { Id = 3, Price = 189.99M, Color = "Firebrick Red" },
    new PaintColor { Id = 4, Price = 199.99M, Color = "Spring Green"}
};

//List of interior options
List<Interior> interiors = new List<Interior>
{
    new Interior { Id = 1, Price = 199.99M, Material = "Beige Fabric" },
    new Interior { Id = 2, Price = 119.99M, Material = "Charcoal Fabric" },
    new Interior { Id = 3, Price = 189.99M, Material = "White Leather" },
    new Interior { Id = 4, Price = 109.99M, Material = "Black Leather"}
};

// List of technology packages
List<Technology> technologies = new List<Technology>
{
    new Technology { Id = 1, Price = 199.99M, Package = "Basic Package (basic sound system)" },
    new Technology { Id = 2, Price = 399.99M, Package = "Navigation Package (includes integrated navigation controls)" },
    new Technology { Id = 3, Price = 289.99M, Package = "Visibility Package (includes side and rear cameras)" },
    new Technology { Id = 4, Price = 999.99M, Package = "Ultra Package (includes navigation and visibility packages)"}
};

// List of wheel options
List<Wheels> wheels = new List<Wheels>
{
    new Wheels { Id = 1, Price = 699.99M, Style = "17-inch Pair Radial" },
    new Wheels { Id = 2, Price = 499.99M, Style = "17-inch Pair Radial Black" },
    new Wheels { Id = 3, Price = 899.99M, Style = "18-inch Pair Spoke Silver" },
    new Wheels { Id = 4, Price = 999.99M, Style = "18-inch Pair Spoke Black"}
};

// List of orders
List<Order> orders = new List<Order>
{
    new Order { Id = 1, TimeStamp = DateTime.Now, WheelId = 1, TechnologyId = 2, PaintId = 3, InteriorId = 4 }
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

// Get All Orders
app.MapGet("/orders", () =>
{
    return orders.Select(o => new OrderDTO
    {
        Id = o.Id,
        TimeStamp = o.TimeStamp,
        WheelId = o.WheelId,
        TechnologyId = o.TechnologyId,
        PaintId = o.PaintId,
        InteriorId = o.InteriorId,
        
        Wheels = wheels.FirstOrDefault(w => w.Id == o.WheelId) == null ? null : new WheelsDTO
        {
            Id = o.WheelId,
            Price = wheels.First(w => w.Id == o.WheelId).Price,
            Style = wheels.First(w => w.Id == o.WheelId).Style
        },
        Technology = technologies.FirstOrDefault(t => t.Id == o.TechnologyId) == null ? null : new TechnologyDTO
        {
            Id = o.TechnologyId,
            Price = technologies.First(t => t.Id == o.TechnologyId).Price,
            Package = technologies.First(t => t.Id == o.TechnologyId).Package
        },
        Paint = paintColors.FirstOrDefault(pc => pc.Id == o.PaintId) == null ? null : new PaintColorDTO
        {
            Id = o.PaintId,
            Price = paintColors.First(pc => pc.Id == o.PaintId).Price,
            Color = paintColors.First(pc => pc.Id == o.PaintId).Color
        },
        Interior = interiors.FirstOrDefault(i => i.Id == o.InteriorId) == null ? null : new InteriorDTO
        {
            Id = o.InteriorId,
            Price = interiors.First(i => i.Id == o.InteriorId).Price,
            Material = interiors.First(i => i.Id == o.InteriorId).Material
        }
    });
});

// Get order by order Id
app.MapGet("/orders/{id}", (int id) =>
{
    Order order = orders.FirstOrDefault(o => o.Id == id);
    
    if (order == null)
    {
        return Results.NotFound();
    }
    
    Wheels wheelChoice = wheels.FirstOrDefault(w => w.Id == order.WheelId);
    Interior interiorChoice = interiors.FirstOrDefault(i => i.Id == order.InteriorId);
    Technology techChoice = technologies.FirstOrDefault(t => t.Id == order.TechnologyId);
    PaintColor paintChoice = paintColors.FirstOrDefault(pc => pc.Id == order.PaintId);
    
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
        TimeStamp = order.TimeStamp
    });
});

//Post a new order to the orders list
app.MapPost("/orders", (Order order) =>
{
    order.Id = orders.Max(o => o.Id) + 1;
    
    orders.Add(order);

    return Results.Created($"/orders/{order.Id}", new OrderDTO
    {
        Id = order.Id,
        TimeStamp = DateTime.Now,
        WheelId = order.WheelId,
        TechnologyId = order.TechnologyId,
        PaintId = order.PaintId,
        InteriorId = order.InteriorId
    });
});

app.Run();
