namespace KiddyAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DBModel : DbContext
    {
        public DBModel()
            : base("name=DBModel")
        {
        }

        public virtual DbSet<tblAddress> tblAddresses { get; set; }
        public virtual DbSet<tblCustomer> tblCustomers { get; set; }
        public virtual DbSet<tblEmployee> tblEmployees { get; set; }
        public virtual DbSet<tblFeedback> tblFeedbacks { get; set; }
        public virtual DbSet<tblOrder> tblOrders { get; set; }
        public virtual DbSet<tblOrderDetail> tblOrderDetails { get; set; }
        public virtual DbSet<tblToy> tblToys { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tblAddress>()
                .Property(e => e.cusID)
                .IsUnicode(false);

            modelBuilder.Entity<tblCustomer>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<tblCustomer>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<tblCustomer>()
                .HasMany(e => e.tblAddresses)
                .WithOptional(e => e.tblCustomer)
                .HasForeignKey(e => e.cusID);

            modelBuilder.Entity<tblCustomer>()
                .HasMany(e => e.tblFeedbacks)
                .WithOptional(e => e.tblCustomer)
                .HasForeignKey(e => e.cusID);

            modelBuilder.Entity<tblCustomer>()
                .HasMany(e => e.tblOrders)
                .WithOptional(e => e.tblCustomer)
                .HasForeignKey(e => e.cusID);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.username)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.password)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.dob)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.gender)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.role)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.firstname)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .Property(e => e.lastname)
                .IsUnicode(false);

            modelBuilder.Entity<tblEmployee>()
                .HasMany(e => e.tblFeedbacks)
                .WithOptional(e => e.tblEmployee)
                .HasForeignKey(e => e.confirmedBy);

            modelBuilder.Entity<tblEmployee>()
                .HasMany(e => e.tblOrders)
                .WithOptional(e => e.tblEmployee)
                .HasForeignKey(e => e.emlID);

            modelBuilder.Entity<tblEmployee>()
                .HasMany(e => e.tblToys)
                .WithOptional(e => e.tblEmployee)
                .HasForeignKey(e => e.createdBy);

            modelBuilder.Entity<tblFeedback>()
                .Property(e => e.cusID)
                .IsUnicode(false);

            modelBuilder.Entity<tblFeedback>()
                .Property(e => e.confirmedBy)
                .IsUnicode(false);

            modelBuilder.Entity<tblOrder>()
                .Property(e => e.cusID)
                .IsUnicode(false);

            modelBuilder.Entity<tblOrder>()
                .Property(e => e.emlID)
                .IsUnicode(false);

            modelBuilder.Entity<tblOrder>()
                .Property(e => e.payment)
                .IsUnicode(false);

            modelBuilder.Entity<tblOrder>()
                .Property(e => e.address)
                .IsUnicode(false);

            modelBuilder.Entity<tblOrder>()
                .HasMany(e => e.tblOrderDetails)
                .WithOptional(e => e.tblOrder)
                .HasForeignKey(e => e.orderID);

            modelBuilder.Entity<tblToy>()
                .Property(e => e.createdBy)
                .IsUnicode(false);

            modelBuilder.Entity<tblToy>()
                .Property(e => e.category)
                .IsUnicode(false);

            modelBuilder.Entity<tblToy>()
                .HasMany(e => e.tblFeedbacks)
                .WithOptional(e => e.tblToy)
                .HasForeignKey(e => e.toyID);

            modelBuilder.Entity<tblToy>()
                .HasMany(e => e.tblOrderDetails)
                .WithOptional(e => e.tblToy)
                .HasForeignKey(e => e.toyID);
        }
    }
}
