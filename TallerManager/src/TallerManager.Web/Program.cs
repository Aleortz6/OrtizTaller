using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TallerManager.Domain.Entities;
using TallerManager.Infrastructure.Data;
using TallerManager.Web.Components;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Infrastructure;
using TallerManager.Web.PdfDocuments;
using QuestPDF.Fluent;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? "Data Source=storage/tallermanager.db";

builder.Services.AddDbContextFactory<TallerManagerDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<TallerManagerDbContext>(sp =>
    sp.GetRequiredService<IDbContextFactory<TallerManagerDbContext>>().CreateDbContext());

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<TallerManagerDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login";
    options.AccessDeniedPath = "/login";
});

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

Directory.CreateDirectory(
    Path.Combine(app.Environment.ContentRootPath, "storage"));

using (var scope = app.Services.CreateScope())
{
    var factory = scope.ServiceProvider
        .GetRequiredService<IDbContextFactory<TallerManagerDbContext>>();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await using var db = await factory.CreateDbContextAsync();

    await DbInitializer.InitializeAsync(db, roleManager, userManager);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets().AllowAnonymous();

app.MapPost("/account/login", async (
    HttpContext httpContext,
    SignInManager<ApplicationUser> signInManager,
    UserManager<ApplicationUser> userManager,
    [FromForm] string email,
    [FromForm] string password) =>
{
    var usuario = await userManager.FindByEmailAsync(email);

    if (usuario is not null && !usuario.Activo)
    {
        return Results.LocalRedirect("/login?error=Esta cuenta est%C3%A1 desactivada");
    }

    var resultado = await signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);

    if (resultado.Succeeded)
    {
        return Results.LocalRedirect("/");
    }

    return Results.LocalRedirect("/login?error=Correo o contrase%C3%B1a incorrectos");
}).AllowAnonymous().DisableAntiforgery();

app.MapPost("/account/logout", async (SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.LocalRedirect("/login");
});

app.MapGet("/ordenes/{id:int}/cotizacion.pdf", async (
    int id,
    IDbContextFactory<TallerManagerDbContext> dbFactory) =>
{
    await using var db = await dbFactory.CreateDbContextAsync();

    var orden = await db.Ordenes
        .Include(o => o.Cliente)
        .Include(o => o.Vehiculo)
        .Include(o => o.Detalles)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (orden is null) return Results.NotFound();

    var taller = await db.Talleres.FirstOrDefaultAsync(t => t.Id == orden.TallerId);
    if (taller is null) return Results.NotFound();

    var documento = new CotizacionDocument(orden, taller, app.Environment.WebRootPath);
    var bytes = documento.GeneratePdf();

    return Results.File(bytes, "application/pdf", $"Cotizacion-{orden.Folio}.pdf");
})
.RequireAuthorization(policy => policy.RequireRole("Administrador", "Secretaria"));

app.MapGet("/ordenes/{id:int}/recepcion.pdf", async (
    int id,
    IDbContextFactory<TallerManagerDbContext> dbFactory) =>
{
    await using var db = await dbFactory.CreateDbContextAsync();

    var orden = await db.Ordenes
        .Include(o => o.Cliente)
        .Include(o => o.Vehiculo)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (orden is null) return Results.NotFound();

    var taller = await db.Talleres.FirstOrDefaultAsync(t => t.Id == orden.TallerId);
    if (taller is null) return Results.NotFound();

    var fotos = await db.FotografiasOrden
        .Where(f => f.OrdenId == id)
        .OrderBy(f => f.Fecha)
        .ToListAsync();

    var documento = new ReciboRecepcionDocument(orden, taller, fotos, app.Environment.WebRootPath);
    var bytes = documento.GeneratePdf();

    return Results.File(bytes, "application/pdf", $"Recepcion-{orden.Folio}.pdf");
})
.RequireAuthorization(policy => policy.RequireRole("Administrador", "Secretaria"));

app.MapGet("/ordenes/{id:int}/entrega.pdf", async (
    int id,
    IDbContextFactory<TallerManagerDbContext> dbFactory) =>
{
    await using var db = await dbFactory.CreateDbContextAsync();

    var orden = await db.Ordenes
        .Include(o => o.Cliente)
        .Include(o => o.Vehiculo)
        .Include(o => o.Detalles)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (orden is null) return Results.NotFound();

    var taller = await db.Talleres.FirstOrDefaultAsync(t => t.Id == orden.TallerId);
    if (taller is null) return Results.NotFound();

    var fotos = await db.FotografiasOrden.Where(f => f.OrdenId == id).ToListAsync();
    var pagos = await db.Pagos.Where(p => p.OrdenId == id).ToListAsync();

    var documento = new ComprobanteEntregaDocument(orden, taller, fotos, pagos, app.Environment.WebRootPath);
    var bytes = documento.GeneratePdf();

    return Results.File(bytes, "application/pdf", $"Entrega-{orden.Folio}.pdf");
})
.RequireAuthorization(policy => policy.RequireRole("Administrador", "Secretaria"));

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();