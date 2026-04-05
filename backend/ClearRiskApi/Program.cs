using ClearRiskApi.Data;
using ClearRiskApi.Models;
using ClearRiskApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<RiskScoringService>();
builder.Services.AddHttpClient<BlockchainValidationService>();
builder.Services.AddScoped<BlockchainLoggingService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=clearrisk.db"));

var app = builder.Build();

// Ensure database is created / migrated on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

static bool IsValidEthereumAddress(string address, out string errorMessage)
{
    errorMessage = string.Empty;

    if (string.IsNullOrWhiteSpace(address))
    {
        errorMessage = "Contract address is required.";
        return false;
    }

    if (!address.StartsWith("0x"))
    {
        errorMessage = "Contract address must start with 0x.";
        return false;
    }

    if (address.Length != 42)
    {
        errorMessage = "Contract address must be exactly 42 characters long.";
        return false;
    }

    for (int i = 2; i < address.Length; i++)
    {
        char c = address[i];
        bool isHex =
            (c >= '0' && c <= '9') ||
            (c >= 'a' && c <= 'f') ||
            (c >= 'A' && c <= 'F');

        if (!isHex)
        {
            errorMessage = "Contract address contains invalid hexadecimal characters.";
            return false;
        }
    }

    return true;
}

app.MapPost("/api/evaluate", async (
    EvaluateRequest request,
    RiskScoringService scoringService,
    BlockchainValidationService blockchainValidationService,
    BlockchainLoggingService blockchainLoggingService,
    AppDbContext db) =>
{
    // Validate address
    if (!IsValidEthereumAddress(request.ContractAddress, out var validationError))
    {
        return Results.BadRequest(new { error = validationError });
    }

    // Validate contract exists on blockchain
    var blockchainValidation = await blockchainValidationService
        .ValidateContractExistsAsync(request.ContractAddress);

    if (!blockchainValidation.IsSuccess)
    {
        return Results.Json(new
        {
            error = blockchainValidation.ErrorMessage ?? "Blockchain validation service is unavailable."
        }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    if (!blockchainValidation.ContractExists)
    {
        return Results.BadRequest(new
        {
            error = blockchainValidation.ErrorMessage
        });
    }

    // Run scoring
    var result = scoringService.Evaluate(request.ContractAddress);

    // Create audit report
    var auditReport = new AuditReport
    {
        ContractAddress = request.ContractAddress,
        FinalScore = result.FinalScore,
        RiskTier = result.RiskTier,
        OwnershipRisk = result.Breakdown.OwnershipRisk,
        LiquidityRisk = result.Breakdown.LiquidityRisk,
        DistributionRisk = result.Breakdown.DistributionRisk,
        CodeTransparencyRisk = result.Breakdown.CodeTransparencyRisk,
        ActivityRisk = result.Breakdown.ActivityRisk,
        ReportHash = result.ReportHash,
        CreatedAt = DateTime.UtcNow,
        BlockchainLogged = false,
        TransactionHash = null
    };

    // Save to database FIRST
    db.AuditReports.Add(auditReport);
    await db.SaveChangesAsync();

    // 🔗 Blockchain logging
    try
    {
        Console.WriteLine("Logging to blockchain...");

        var txnHash = await blockchainLoggingService.LogAuditAsync(
    auditReport.ContractAddress,
    (int)Math.Round(auditReport.FinalScore),
    auditReport.RiskTier,
    auditReport.ReportHash
);

        Console.WriteLine($"Transaction sent: {txnHash}");

        auditReport.TransactionHash = txnHash;
        auditReport.BlockchainLogged = true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Blockchain error: {ex.Message}");
        auditReport.BlockchainLogged = false;
    }

    // Save updated blockchain result
    await db.SaveChangesAsync();

    // RETURN RESPONSE (THIS WAS YOUR MISSING PIECE)
    return Results.Ok(new
    {
        auditId = auditReport.Id,
        contractAddress = request.ContractAddress,
        finalScore = result.FinalScore,
        riskTier = result.RiskTier,
        breakdown = result.Breakdown,
        reportHash = result.ReportHash,
        createdAt = auditReport.CreatedAt,
        blockchainLogged = auditReport.BlockchainLogged,
        transactionHash = auditReport.TransactionHash
    });
});

// GET all reports
app.MapGet("/api/reports", async (AppDbContext db) =>
{
    var reports = await db.AuditReports
        .OrderByDescending(r => r.CreatedAt)
        .ToListAsync();

    return Results.Ok(reports);
});

// GET report by ID
app.MapGet("/api/reports/{id:int}", async (int id, AppDbContext db) =>
{
    var report = await db.AuditReports.FindAsync(id);

    return report is not null
        ? Results.Ok(report)
        : Results.NotFound(new { error = "Audit report not found." });
});

app.Run();

public record EvaluateRequest(string ContractAddress);