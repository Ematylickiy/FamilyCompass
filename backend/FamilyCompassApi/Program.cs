using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITransactionService, TransactionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/api/v1/transactions",
        (ITransactionService service) =>
            Results.Ok(service.GetAll()))
    .WithName("GetTransactions");

app.MapPost("/api/v1/transactions",
        (ITransactionService service, CreateTransactionRequest request) =>
        {
            var created = service.Create(request);
            return Results.Created($"/api/v1/transactions/{created.Id}", created);
        })
    .WithName("CreateTransaction");

app.MapDelete("/api/v1/transactions/{id:guid}",
        (ITransactionService service, Guid id) =>
            service.Delete(id) ? Results.NoContent() : Results.NotFound())
    .WithName("DeleteTransaction");

app.Run();

// app.UseHttpsRedirection();

public class Transaction
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }
};

public record CreateTransactionRequest(
    decimal Amount,
    string Type,
    string Category,
    DateTime Date,
    string? Note
);

public interface ITransactionService
{
    List<Transaction> GetAll();
    Transaction Create(CreateTransactionRequest request);
    bool Delete(Guid id);
}

public class TransactionService : ITransactionService
{
    private readonly AppDbContext _dbContext;

    public TransactionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public List<Transaction> GetAll() =>
        _dbContext.Transactions
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.CreatedAt)
            .ToList();

    public Transaction Create(CreateTransactionRequest request)
    {
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = request.Amount,
            Type = request.Type,
            Category = request.Category,
            Date = request.Date,
            Note = request.Note,
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.Transactions.Add(transaction);
        _dbContext.SaveChanges();
        return transaction;
    }

    public bool Delete(Guid id)
    {
        var transaction = _dbContext.Transactions.FirstOrDefault(t => t.Id == id);
        if (transaction is null) return false;
        _dbContext.Transactions.Remove(transaction);
        _dbContext.SaveChanges();
        return true;
    }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var transaction = modelBuilder.Entity<Transaction>();
        transaction.ToTable("transactions");
        transaction.HasKey(t => t.Id);
        transaction.Property(t => t.Amount).HasColumnType("decimal(18,2)");
        transaction.Property(t => t.Type).IsRequired();
        transaction.Property(t => t.Category).IsRequired();
        transaction.Property(t => t.Date).IsRequired();
        transaction.Property(t => t.CreatedAt).IsRequired();
    }
}