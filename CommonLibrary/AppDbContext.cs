using CommonLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace CommonLibrary
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region EFConfiguration
            #region TaskItem
            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.User)
                .WithMany(x => x.TaskItems)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
            #endregion

            #region SeedData
            #region TaskItem
            modelBuilder.Entity<TaskItem>().HasData(
                new()
                {
                    Id = 1,
                    Identifier = "86c53f64-6f9a-40c9-acce-766c8a88ae35",
                    UserId = 1,
                    Name = "Task 1.1",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Pending,
                },
                new()
                {
                    Id = 2,
                    Identifier = "e1d82661-3c50-4ba2-9c77-5521df64a6f8",
                    UserId = 1,
                    Name = "Task 1.2",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.InProgress,
                },
                new()
                {
                    Id = 3,
                    Identifier = "3311ff50-45f5-43a3-8a8b-0e9b6cbaf45f",
                    UserId = 1,
                    Name = "Task 1.3",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Completed,
                },
                new()
                {
                    Id = 4,
                    Identifier = "554fa5ed-5c60-4c01-985b-1292fcfd9cdd",
                    UserId = 1,
                    Name = "Task 1.4",
                    Description = "Task 1.4 Cancelled..",
                    TaskItemStatus = TaskItemStatusType.Cancelled,
                },
                new()
                {
                    Id = 5,
                    Identifier = "7e09726a-6037-432e-941e-80cf3fa93137",
                    UserId = 2,
                    Name = "Task A (001)",
                    Description = "Completed Before Time",
                    TaskItemStatus = TaskItemStatusType.Completed,
                },
                new()
                {
                    Id = 6,
                    Identifier = "687f5642-57db-4bdf-a8cf-12c7d441c7b7",
                    UserId = 2,
                    Name = "Task B (001)",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Cancelled,
                },
                new()
                {
                    Id = 7,
                    Identifier = "19ae216c-fbb7-4c56-b6da-aab9bbb58830",
                    UserId = 2,
                    Name = "Task B (002)",
                    Description = string.Empty,
                    TaskItemStatus = TaskItemStatusType.Cancelled,
                }
            );
            #endregion
            #region User
            modelBuilder.Entity<User>().HasData(
                new()
                {
                    Id = 1,
                    Identifier = "f47ac10b-58cc-4372-a567-0e02b2c3d479",
                    Email = "alice.borderland@testmail.com",
                    TaskItems = [],
                },
                new()
                {
                    Id = 2,
                    Identifier = "c9bf9e57-1685-4c89-bafb-ff5af830be8a",
                    Email = "frodo.smith@testmail.com",
                    TaskItems = [],
                },
                new()
                {
                    Id = 3,
                    Identifier = "7c9e6679-7425-40de-944b-e07fc1f90ae7",
                    Email = "charlie.shane@testmail.com",
                    TaskItems = [],
                }
            );
            #endregion
            #endregion
        }
    }
}
