using FishStoreSystem.DAL.Entities;
using FishStoreSystem_DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FishStoreSystem_DAL
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Weekly> Weeklys { get; set; }
        public DbSet<WeeklyItem> WeeklyItems { get; set; }
        public DbSet<DailySale> DailySales { get; set; }
        public DbSet<DailyExpense> DailyExpenses { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- إعدادات العلاقات (الكود الأصلي الخاص بك) ---
            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Payments)
                .WithOne(p => p.Invoice)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
                .HasMany(i => i.Items)
                .WithOne(it => it.Invoice)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Invoices)
                .WithOne(i => i.Customer)
                .OnDelete(DeleteBehavior.Cascade);


            // --- الإعدادات الجديدة لإصلاح تحذيرات الـ decimal ---

            // إعداد خاصية حد الائتمان للعميل (CreditLimit)
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.CreditLimit)
                      .HasPrecision(18, 2); // إجمالي 18 رقم، منها رقمان بعد الفاصلة العشرية
            });

            // إعداد خصائص الفواتير (PaidAmount و TotalAmount)
            modelBuilder.Entity<Invoice>(entity =>
            {
                entity.Property(e => e.PaidAmount)
                      .HasPrecision(18, 2);

                entity.Property(e => e.TotalAmount)
                      .HasPrecision(18, 2);
            });

            // إعداد خصائص بنود الفواتير (Quantity و UnitPrice)
            modelBuilder.Entity<InvoiceItem>(entity =>
            {
                entity.Property(e => e.Quantity)
                      .HasPrecision(10, 4); // الكمية قد تحتاج لأرقام عشرية أكثر دقة

                entity.Property(e => e.UnitPrice)
                      .HasPrecision(18, 4); // سعر الوحدة قد يحتاج لدقة عالية في الحسابات
            });

            // إعداد خاصية مبلغ الدفعة (Amount)
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.Property(e => e.Amount)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Weekly>(entity =>
            {
                entity.Property(e => e.TotalSales).HasPrecision(18, 2);
                entity.Property(e => e.TotalExpenses).HasPrecision(18, 2);
                entity.Property(e => e.TotalPaymentsReceived).HasPrecision(18, 2);
                entity.Property(e => e.TotalReceivables).HasPrecision(18, 2);
            });

            modelBuilder.Entity<WeeklyItem>(entity =>
            {
                entity.Property(e => e.Sales).HasPrecision(18, 2);
                entity.Property(e => e.Expenses).HasPrecision(18, 2);
                entity.Property(e => e.PaymentsReceived).HasPrecision(18, 2);
                entity.Property(e => e.Receivables).HasPrecision(18, 2);
            });

            modelBuilder.Entity<DailySale>().Property(s => s.Amount).HasPrecision(18, 2);
            modelBuilder.Entity<DailyExpense>().Property(e => e.Amount).HasPrecision(18, 2);

        }
    }
}