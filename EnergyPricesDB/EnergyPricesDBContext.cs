using EnergyPricesDB.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyPricesDB;
public class EnergyPricesDBContext : DbContext
{
    public EnergyPricesDBContext(DbContextOptions<EnergyPricesDBContext> options)
    : base(options)
    { }

    public DbSet<ProductModel> NordicElectricityPrices { get; set; }
}