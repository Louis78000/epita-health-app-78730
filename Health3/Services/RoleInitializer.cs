using Microsoft.AspNetCore.Identity;

namespace Health3.Services;

public static class RoleInitializer
{
    // Méthode pour initialiser les rôles "Admin", "Doctor" et "Patient"
    public static async Task SeedRoles(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Liste des rôles à créer
        var roles = new[] { "Admin", "Doctor", "Patient" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
                Console.WriteLine($"Created role {role}");
            }
        }
    }
    public static async Task SeedPatient(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Vérifier si le rôle "Patient" existe, sinon le créer
        if (!await roleManager.RoleExistsAsync("Patient"))
        {
            await roleManager.CreateAsync(new IdentityRole("Patient"));
        }
    }
    public static async Task SeedAdmin(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Vérifier si le rôle "Admin" existe, sinon le créer
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Créer un utilisateur Admin par défaut s'il n'existe pas
        string adminEmail = "admin@example.com";
        string adminPassword = "Admin@1234";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }

    public static async Task SeedDoctor(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Vérifier si le rôle "Doctor" existe, sinon le créer
        if (!await roleManager.RoleExistsAsync("Doctor"))
        {
            await roleManager.CreateAsync(new IdentityRole("Doctor"));
        }

        // Créer un utilisateur Doctor par défaut s'il n'existe pas
        string doctorEmail = "doctor@example.com";
        string doctorPassword = "Doctor@1234";
        var doctorUser = await userManager.FindByEmailAsync(doctorEmail);

        if (doctorUser == null)
        {
            doctorUser = new IdentityUser
            {
                UserName = doctorEmail,
                Email = doctorEmail
            };

            var result = await userManager.CreateAsync(doctorUser, doctorPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(doctorUser, "Doctor");
            }
        }
    }
}