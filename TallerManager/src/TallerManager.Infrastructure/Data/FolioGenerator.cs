using Microsoft.EntityFrameworkCore;

namespace TallerManager.Infrastructure.Data;

public static class FolioGenerator
{
    public static async Task<string> SiguienteFolioAsync(TallerManagerDbContext db, int tallerId)
    {
        var ultimoFolio = await db.Ordenes
            .Where(o => o.TallerId == tallerId)
            .OrderByDescending(o => o.Id)
            .Select(o => o.Folio)
            .FirstOrDefaultAsync();

        var siguienteNumero = 1;

        if (!string.IsNullOrWhiteSpace(ultimoFolio) && ultimoFolio.StartsWith("OS-"))
        {
            var parteNumerica = ultimoFolio["OS-".Length..];
            if (int.TryParse(parteNumerica, out var numeroActual))
            {
                siguienteNumero = numeroActual + 1;
            }
        }

        return $"OS-{siguienteNumero:D6}";
    }
}