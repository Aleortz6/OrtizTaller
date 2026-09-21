using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TallerManager.Domain.Entities;

namespace TallerManager.Infrastructure.Data;

public static class DbInitializer
{
    public static readonly string[] RolesDelSistema = { "Administrador", "Secretaria", "Mecanico" };

    public static async Task InitializeAsync(
        TallerManagerDbContext db,
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        await db.Database.MigrateAsync();

        Taller? taller = null;

        if (!db.Talleres.Any())
        {
            taller = new Taller
            {
                Nombre = "ORTIZ Servicio Automotriz",
                FechaCreacion = DateTime.UtcNow,
                Activo = true
            };

            db.Talleres.Add(taller);
            await db.SaveChangesAsync();
        }
        else
        {
            taller = await db.Talleres.OrderBy(t => t.FechaCreacion).FirstAsync();
        }

        foreach (var rol in RolesDelSistema)
        {
            if (!await roleManager.RoleExistsAsync(rol))
            {
                await roleManager.CreateAsync(new IdentityRole(rol));
            }
        }

        const string adminEmail = "admin@tallermanager.local";

        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                NombreCompleto = "Administrador",
                TallerId = taller.Id,
                Activo = true
            };

            var resultado = await userManager.CreateAsync(admin, "Admin123!");

            if (resultado.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Administrador");
            }
        }
    }
}