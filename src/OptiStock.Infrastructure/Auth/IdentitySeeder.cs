using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OptiStock.Infrastructure.Auth;

namespace OptiStock.Infrastructure.Auth;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
    {
        using var scope = services.CreateScope();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        foreach (var roleName in new[] { "Administrador", "Funcionário" })
            if (!await roles.RoleExistsAsync(roleName))
                await roles.CreateAsync(new IdentityRole<Guid>(roleName));

        var email = configuration["BootstrapAdmin:Email"];
        var password = configuration["BootstrapAdmin:Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;
        var admin = await users.FindByEmailAsync(email);
        if (admin is null)
        {
            admin = new AppUser { UserName = email, Email = email, DisplayName = configuration["BootstrapAdmin:Name"] ?? "Administrador OptiStock", EmailConfirmed = true };
            var created = await users.CreateAsync(admin, password);
            if (!created.Succeeded) throw new InvalidOperationException("Não foi possível criar o usuário administrador inicial: " + string.Join("; ", created.Errors.Select(e => e.Code)));
        }
        if (!await users.IsInRoleAsync(admin, "Administrador"))
            await users.AddToRoleAsync(admin, "Administrador");
    }
}
