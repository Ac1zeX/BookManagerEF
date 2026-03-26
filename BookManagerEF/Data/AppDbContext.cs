using Microsoft.EntityFrameworkCore;
using BookManagerEF.Models;

namespace BookManagerEF.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Простая строка подключения
            optionsBuilder.UseSqlServer(@"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=BooksDB;Integrated Security=True;Connect Timeout=30");

            // Альтернативный вариант с файлом .mdf в папке проекта
            // string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BooksDB.mdf");
            // optionsBuilder.UseSqlServer($@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;Connect Timeout=30");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Добавляем тестового пользователя
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "admin", Password = "123" }
            );
        }
    }
}