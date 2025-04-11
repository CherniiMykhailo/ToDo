using Microsoft.EntityFrameworkCore;

namespace TodoListApp.WebApi.Models
{
    public class ToDoListContext : DbContext
    {
        public ToDoListContext(DbContextOptions<ToDoListContext> options) : base(options)
        {
        }

        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<ToDo>()
            .HasOne(t => t.ToDoList)
            .WithMany(tl => tl.ToDos)
            .HasForeignKey(t => t.ToDoListId)
            .OnDelete(DeleteBehavior.SetNull);

            _ = modelBuilder.Entity<Category>().HasData(
                        new Category { CategoryId = "work", Name = "Work" },
                        new Category { CategoryId = "home", Name = "Home" },
                        new Category { CategoryId = "exercise", Name = "Exercise" },
                        new Category { CategoryId = "shop", Name = "Shopping" },
                        new Category { CategoryId = "call", Name = "Contact" }
                );

            _ = modelBuilder.Entity<Status>().HasData(
                        new Status { StatusId = "open", Name = "Open" },
                        new Status { StatusId = "closed", Name = "Completed" }
                );
        }
    }

}
