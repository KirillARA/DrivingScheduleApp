using Microsoft.EntityFrameworkCore;

namespace DrivingSchoolApp.Models;

public partial class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public virtual DbSet<view_driving_schedule> view_driving_schedules { get; set; }
    public virtual DbSet<view_exam_result> view_exam_results { get; set; }
    public virtual DbSet<view_groups_summary> view_groups_summaries { get; set; }
    public virtual DbSet<view_students_info> view_students_infos { get; set; }

    public virtual DbSet<Сотрудник> Сотрудники { get; set; }
    public virtual DbSet<Категория_прав> Категории_прав { get; set; }
    public virtual DbSet<Группа> Группы { get; set; }
    public virtual DbSet<Тариф> Тарифы { get; set; }
    public virtual DbSet<Ученик> Ученики { get; set; }
    public virtual DbSet<Транспорт> Транспорт { get; set; }
    public virtual DbSet<Теоретическое_занятие> Теоретические_занятия { get; set; }
    public virtual DbSet<Практическое_занятие> Практические_занятия { get; set; }
    public virtual DbSet<Экзамен> Экзамены { get; set; }
    public virtual DbSet<РезультатыЭкзамена> Результаты_экзаменов { get; set; }
    public virtual DbSet<Скидка> Скидки { get; set; }
    public virtual DbSet<СкидкаТариф> Скидки_тарифы { get; set; }
    public virtual DbSet<ПринадлежностьСотрудника> Принадлежности_сотрудников { get; set; }
    public virtual DbSet<ЗакреплениеУченика> Закрепления_учеников { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Настройки представлений (оставьте как есть, они уже были в проекте)
        modelBuilder.Entity<view_driving_schedule>(entity =>
        {
            entity.HasNoKey().ToView("view_driving_schedule");
            entity.Property(e => e.Инструктор).HasMaxLength(255);
            entity.Property(e => e.Ученик).HasMaxLength(255);
        });

        modelBuilder.Entity<view_exam_result>(entity =>
        {
            entity.HasNoKey().ToView("view_exam_results");
            entity.Property(e => e.Категория).HasMaxLength(10);
            entity.Property(e => e.Ученик).HasMaxLength(255);
        });

        modelBuilder.Entity<view_groups_summary>(entity =>
        {
            entity.HasNoKey().ToView("view_groups_summary");
            entity.Property(e => e.Группа).HasMaxLength(100);
            entity.Property(e => e.Категория).HasMaxLength(10);
            entity.Property(e => e.Текущее_кол_во_учеников).HasColumnName("Текущее кол-во учеников");
        });

        modelBuilder.Entity<view_students_info>(entity =>
        {
            entity.HasNoKey().ToView("view_students_info");
            entity.Property(e => e.Группа).HasMaxLength(100);
            entity.Property(e => e.Инструктор).HasMaxLength(255);
            entity.Property(e => e.Категория_прав).HasMaxLength(10).HasColumnName("Категория прав");
            entity.Property(e => e.ФИО_ученика).HasMaxLength(255).HasColumnName("ФИО ученика");
            entity.Property(e => e.телефон).HasMaxLength(20);
        });

        // Сотрудник
        modelBuilder.Entity<Сотрудник>(entity =>
        {
            entity.HasKey(e => e.id_сотрудника);
            entity.ToTable("Сотрудник");
            entity.HasIndex(e => e.телефон).IsUnique();
            entity.HasIndex(e => e.email).IsUnique();
            entity.HasIndex(e => new { e.паспорт_серия, e.паспорт_номер }).IsUnique();
            entity.Property(e => e.дата_приема).HasDefaultValueSql("CURRENT_DATE");
        });

        // Категория прав
        modelBuilder.Entity<Категория_прав>(entity =>
        {
            entity.HasKey(e => e.id_категории);
            entity.ToTable("Категория прав");
            entity.HasIndex(e => e.название).IsUnique();
            entity.Property(e => e.название).HasMaxLength(10);
        });

        // Группа
        modelBuilder.Entity<Группа>(entity =>
        {
            entity.HasKey(e => e.id_группы);
            entity.ToTable("Группа");
            entity.Property(e => e.статус).HasConversion<string>();
            entity.Property(e => e.текущ_учеников).HasDefaultValue(0);
            entity.HasOne(d => d.id_категорииNavigation)
                .WithMany(p => p.Группаs)
                .HasForeignKey(d => d.id_категории)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Тариф
        modelBuilder.Entity<Тариф>(entity =>
        {
            entity.HasKey(e => e.id_тарифа);
            entity.ToTable("Тариф");
            entity.HasOne(d => d.id_категорииNavigation)
                .WithMany(p => p.Тарифы)
                .HasForeignKey(d => d.id_категории)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Ученик
        modelBuilder.Entity<Ученик>(entity =>
        {
            entity.HasKey(e => e.id_ученика);
            entity.ToTable("Ученик");
            entity.HasIndex(e => e.телефон).IsUnique();
            entity.HasIndex(e => new { e.паспорт_серия, e.паспорт_номер }).IsUnique();
            entity.HasOne(d => d.id_группыNavigation)
                .WithMany(p => p.Ученикs)
                .HasForeignKey(d => d.id_группы)
                .OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.id_тарифаNavigation)
                .WithMany(p => p.Ученики)
                .HasForeignKey(d => d.id_тарифа)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Транспорт
        modelBuilder.Entity<Транспорт>(entity =>
        {
            entity.HasKey(e => e.id_транспорта);
            entity.ToTable("Транспорт");
            entity.HasIndex(e => e.госномер).IsUnique();
            entity.HasOne(d => d.id_категорииNavigation)
                .WithMany(p => p.Транспортs)
                .HasForeignKey(d => d.id_категории)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Теоретическое занятие
        modelBuilder.Entity<Теоретическое_занятие>(entity =>
        {
            entity.HasKey(e => e.id_теорзан);
            entity.ToTable("Теоретическое занятие");
            entity.HasOne(d => d.id_группыNavigation)
                .WithMany(p => p.Теоретическое_занятиеs)
                .HasForeignKey(d => d.id_группы)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.id_преподавателяNavigation)
                .WithMany(p => p.Теоретические_занятия)
                .HasForeignKey(d => d.id_преподавателя)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Практическое занятие
        modelBuilder.Entity<Практическое_занятие>(entity =>
        {
            entity.HasKey(e => e.id_практзан);
            entity.ToTable("Практическое занятие");
            entity.HasOne(d => d.id_ученикаNavigation)
                .WithMany(p => p.Практическое_занятиеs)
                .HasForeignKey(d => d.id_ученика)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.id_инструктораNavigation)
                .WithMany(p => p.Практические_занятия)
                .HasForeignKey(d => d.id_инструктора)
                .OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.id_транспортаNavigation)
                .WithMany(p => p.Практическое_занятиеs)
                .HasForeignKey(d => d.id_транспорта)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Экзамен
        modelBuilder.Entity<Экзамен>(entity =>
        {
            entity.HasKey(e => e.id_экзамена);
            entity.ToTable("Экзамен");
            entity.HasOne(d => d.id_категорииNavigation)
                .WithMany(p => p.Экзаменs)
                .HasForeignKey(d => d.id_категории)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        // Результаты экзамена
        modelBuilder.Entity<РезультатыЭкзамена>(entity =>
        {
            entity.HasKey(e => new { e.id_ученика, e.id_экзамена, e.дата_попытки });
            entity.ToTable("РезультатыЭкзамена");
            entity.Property(e => e.дата_попытки).HasDefaultValueSql("CURRENT_DATE");
            entity.HasOne(d => d.id_ученикаNavigation)
                .WithMany(p => p.РезультатыЭкзаменаs)
                .HasForeignKey(d => d.id_ученика)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.id_экзаменаNavigation)
                .WithMany(p => p.РезультатыЭкзаменаs)
                .HasForeignKey(d => d.id_экзамена)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Скидка
        modelBuilder.Entity<Скидка>(entity =>
        {
            entity.HasKey(e => e.id_скидки);
            entity.ToTable("Скидка");
        });

        // СкидкаТариф
        modelBuilder.Entity<СкидкаТариф>(entity =>
        {
            entity.HasKey(e => new { e.id_скидки, e.id_тарифа });
            entity.ToTable("СкидкаТариф");
            entity.Property(e => e.дата_назначения).HasDefaultValueSql("CURRENT_DATE");
            entity.HasOne(d => d.id_скидкиNavigation)
                .WithMany(p => p.СкидкаТарифы)
                .HasForeignKey(d => d.id_скидки)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.id_тарифаNavigation)
                .WithMany(p => p.СкидкаТарифы)
                .HasForeignKey(d => d.id_тарифа)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ПринадлежностьСотрудника
        modelBuilder.Entity<ПринадлежностьСотрудника>(entity =>
        {
            entity.HasKey(e => new { e.id_сотрудника, e.id_категории });
            entity.ToTable("ПринадлежностьСотрудника");
            entity.HasOne(d => d.id_сотрудникаNavigation)
                .WithMany(p => p.ПринадлежностиСотрудника)
                .HasForeignKey(d => d.id_сотрудника)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.id_категорииNavigation)
                .WithMany(p => p.ПринадлежностиСотрудника)
                .HasForeignKey(d => d.id_категории)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ЗакреплениеУченика
        modelBuilder.Entity<ЗакреплениеУченика>(entity =>
        {
            entity.HasKey(e => new { e.id_ученика, e.id_сотрудника, e.дата_закрепления });
            entity.ToTable("ЗакреплениеУченика");
            entity.Property(e => e.дата_закрепления).HasDefaultValueSql("CURRENT_DATE");
            entity.HasOne(d => d.id_ученикаNavigation)
                .WithMany(p => p.ЗакрепленияУченика)
                .HasForeignKey(d => d.id_ученика)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(d => d.id_сотрудникаNavigation)
                .WithMany(p => p.ЗакрепленияУченика)
                .HasForeignKey(d => d.id_сотрудника)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}