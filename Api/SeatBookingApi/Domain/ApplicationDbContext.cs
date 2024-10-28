using Microsoft.EntityFrameworkCore;

namespace SeatBookingApi.Domain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<VenueSection> VenueSections { get; set; }
        public DbSet<Canvas> Canvases { get; set; }
    }
}
