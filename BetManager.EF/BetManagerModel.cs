namespace BetManager.EF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BetManagerModel : DbContext
    {
        public BetManagerModel()
            : base("name=BetManagerModel")
        {
        }

        public virtual DbSet<BM_Category> BM_Category { get; set; }
        public virtual DbSet<BM_Event> BM_Event { get; set; }
        public virtual DbSet<BM_OddsRegular> BM_OddsRegular { get; set; }
        public virtual DbSet<BM_Score> BM_Score { get; set; }
        public virtual DbSet<BM_Season> BM_Season { get; set; }
        public virtual DbSet<BM_Sport> BM_Sport { get; set; }
        public virtual DbSet<BM_Status> BM_Status { get; set; }
        public virtual DbSet<BM_Team> BM_Team { get; set; }
        public virtual DbSet<BM_Tournament> BM_Tournament { get; set; }
        public virtual DbSet<BM_ImportData> BM_ImportData { get; set; }
        public virtual DbSet<BM_ImportDataOld> BM_ImportDataOld { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BM_Category>()
                .HasMany(e => e.BM_Event)
                .WithRequired(e => e.BM_Category)
                .HasForeignKey(e => e.ID_Category)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BM_Category>()
                .HasMany(e => e.BM_Tournament)
                .WithOptional(e => e.BM_Category)
                .HasForeignKey(e => e.ID_Category);

            modelBuilder.Entity<BM_Event>()
                .HasOptional(e => e.BM_Score)
                .WithRequired(e => e.BM_Event);

            modelBuilder.Entity<BM_Event>()
                .HasOptional(e => e.BM_OddsRegular)
                .WithRequired(e => e.BM_Event);

            modelBuilder.Entity<BM_OddsRegular>()
                .Property(e => e.FirstValue)
                .HasPrecision(8, 2);

            modelBuilder.Entity<BM_OddsRegular>()
                .Property(e => e.XValue)
                .HasPrecision(8, 2);

            modelBuilder.Entity<BM_OddsRegular>()
                .Property(e => e.SecondValue)
                .HasPrecision(8, 2);

            modelBuilder.Entity<BM_Season>()
                .HasMany(e => e.BM_Event)
                .WithOptional(e => e.BM_Season)
                .HasForeignKey(e => e.ID_Season);

            modelBuilder.Entity<BM_Sport>()
                .HasMany(e => e.BM_Category)
                .WithRequired(e => e.BM_Sport)
                .HasForeignKey(e => e.ID_Sport)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BM_Status>()
                .HasMany(e => e.BM_Event)
                .WithRequired(e => e.BM_Status)
                .HasForeignKey(e => e.ID_Status)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BM_Team>()
                .HasMany(e => e.BM_Event)
                .WithRequired(e => e.BM_Team)
                .HasForeignKey(e => e.ID_AwayTeam)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BM_Team>()
                .HasMany(e => e.BM_Event1)
                .WithRequired(e => e.BM_Team1)
                .HasForeignKey(e => e.ID_HomeTeam)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BM_Tournament>()
                .HasMany(e => e.BM_Event)
                .WithRequired(e => e.BM_Tournament)
                .HasForeignKey(e => e.ID_Tournament)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.SportName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.SportSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.TournamentName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.TournamentSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.CategoryName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.CategorySlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.SeasonName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.SeasonSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.SeasonYear)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.EventCustomId)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.EventName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.EventSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.EventStartDate)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.EventStartTime)
                .HasPrecision(0);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.EventChanges)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.StatusType)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.StatusDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.HomeTeamName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.HomeTeamSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.HomeTeamGender)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.AwayTeamName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.AwayTeamSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.AwayTeamGender)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.OddsRegularFirstValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.OddsRegularXValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.OddsRegularSecondValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.OddsDoubleChangeFirstXValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.OddsDoubleChangeXSecondValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportData>()
                .Property(e => e.OddsDoubleChangeFirstSecondValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.SportName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.SportSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.TournamentName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.TournamentSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.CategoryName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.CategorySlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.SeasonName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.SeasonSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.SeasonYear)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.EventCustomId)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.EventName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.EventSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.EventStartDate)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.EventStartTime)
                .HasPrecision(0);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.EventChanges)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.StatusType)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.StatusDescription)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.HomeTeamName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.HomeTeamSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.HomeTeamGender)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.AwayTeamName)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.AwayTeamSlug)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.AwayTeamGender)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.OddsRegularFirstValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.OddsRegularXValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.OddsRegularSecondValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.OddsDoubleChangeFirstXValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.OddsDoubleChangeXSecondValue)
                .IsUnicode(false);

            modelBuilder.Entity<BM_ImportDataOld>()
                .Property(e => e.OddsDoubleChangeFirstSecondValue)
                .IsUnicode(false);
        }
    }
}
