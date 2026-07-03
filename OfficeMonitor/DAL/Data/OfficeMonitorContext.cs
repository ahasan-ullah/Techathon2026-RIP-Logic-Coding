using DAL.Entities;
using DAL.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class OfficeMonitorContext : DbContext
    {
        public OfficeMonitorContext(DbContextOptions<OfficeMonitorContext> options) : base(options)
        {

        }
        public DbSet<Room> Rooms { get; set; } = null!;
        public DbSet<Device> Devices { get; set; } = null!;
        public DbSet<DeviceState> DeviceStates { get; set; } = null!;
        public DbSet<DeviceStateHistory> DeviceStateHistories { get; set; } = null!;
        public DbSet<Alert> Alerts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // room
            modelBuilder.Entity<Room>(e =>
            {
                e.HasKey(r => r.Id);
                e.Property(r => r.Name).HasMaxLength(100).IsRequired();
                e.Property(r => r.Code).HasMaxLength(20).IsRequired();
                e.HasIndex(r => r.Code).IsUnique();
            });

            //device
            modelBuilder.Entity<Device>(e =>
            {
                e.HasKey(d => d.Id);
                e.Property(d => d.Name).HasMaxLength(50).IsRequired();
                e.HasOne(d => d.Room)
                 .WithMany(r => r.Devices)
                 .HasForeignKey(d => d.RoomId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            //device state 1:1 to device
            modelBuilder.Entity<DeviceState>(e =>
            {
                e.HasKey(s => s.DeviceId);
                e.HasOne(s => s.Device)
                 .WithOne(d => d.DeviceState)
                 .HasForeignKey<DeviceState>(s => s.DeviceId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            //device state history
            modelBuilder.Entity<DeviceStateHistory>(e =>
            {
                e.HasKey(h => h.Id);
                e.HasOne(h => h.Device)
                 .WithMany(d => d.History)
                 .HasForeignKey(h => h.DeviceId)
                 .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(h => new { h.DeviceId, h.ChangedAt });
            });

            //alert
            modelBuilder.Entity<Alert>(e =>
            {
                e.HasKey(a => a.Id);
                e.Property(a => a.Message).HasMaxLength(300).IsRequired();
                e.HasOne(a => a.Room).WithMany().HasForeignKey(a => a.RoomId).OnDelete(DeleteBehavior.SetNull);
                e.HasOne(a => a.Device).WithMany().HasForeignKey(a => a.DeviceId).OnDelete(DeleteBehavior.SetNull);
                e.HasIndex(a => new { a.ResolvedAt, a.TriggeredAt });
            });

            //Seed(modelBuilder);
        }
        private static void Seed(ModelBuilder modelBuilder)
        {
            var rooms = new[]
            {
            new Room { Id = 1, Name = "Drawing Room", Code = "drawing", Type = RoomType.WaitingArea },
            new Room { Id = 2, Name = "Work Room 1",  Code = "work1",   Type = RoomType.WorkArea },
            new Room { Id = 3, Name = "Work Room 2",  Code = "work2",   Type = RoomType.WorkArea },
        };
            modelBuilder.Entity<Room>().HasData(rooms);

            var devices = new List<Device>();
            var states = new List<DeviceState>();
            int deviceId = 1;
            var seedTime = new DateTime(2026, 1, 1, 9, 0, 0, DateTimeKind.Utc);

            foreach (var room in rooms)
            {
                for (int f = 1; f <= 2; f++)
                {
                    devices.Add(new Device
                    {
                        Id = deviceId,
                        RoomId = room.Id,
                        Type = DeviceType.Fan,
                        Name = $"Fan {f}",
                        RatedPowerWatts = 60
                    });
                    states.Add(new DeviceState { DeviceId = deviceId, IsOn = false, LastChangedAt = seedTime });
                    deviceId++;
                }
                for (int l = 1; l <= 3; l++)
                {
                    devices.Add(new Device
                    {
                        Id = deviceId,
                        RoomId = room.Id,
                        Type = DeviceType.Light,
                        Name = $"Light {l}",
                        RatedPowerWatts = 15
                    });
                    states.Add(new DeviceState { DeviceId = deviceId, IsOn = false, LastChangedAt = seedTime });
                    deviceId++;
                }
            }

            modelBuilder.Entity<Device>().HasData(devices);
            modelBuilder.Entity<DeviceState>().HasData(states);
        }
    }
}
