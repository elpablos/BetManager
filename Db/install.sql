-- Database: BetManagerDevel
-- Date: 28.10.2016 10:48:04

print 'CurrentTime: START - ' + convert(varchar, getdate(), 120)

GO

-- Delete all constraints

declare @table varchar(250), @constraint varchar(250)
declare @cmd varchar(max)

declare tbl cursor local forward_only static for
select tables.name, constraints.name
from sysobjects as tables
inner join sysobjects as constraints on constraints.parent_obj = object_id(tables.name)
where tables.type = 'u'
	and constraints.xtype in ('C', 'F', 'UQ', 'D')
order by constraints.xtype asc

open tbl
fetch next from tbl into @table, @constraint
while (@@fetch_status=0)
  begin
	set @cmd = 'ALTER TABLE [' + @table + '] DROP CONSTRAINT [' + @constraint + ']'
	exec (@cmd)
    fetch next from tbl into @table, @constraint
  end
close tbl
deallocate tbl

GO

print 'CurrentTime: ConstraintsDel - ' + convert(varchar, getdate(), 120)

GO

-- Delete all indices
declare @sql varchar(max)
set @sql=''

declare @drop varchar(max)
declare tbl_Indexy cursor local forward_only static for
select 'DROP INDEX [' + object_name(sysindexes.id) + '].[' + sysindexes.name + ']
'
from
  sysindexes
  left join sysobjects on sysindexes.id = sysobjects.id
where sysindexes.indid <> 0
and (sysindexes.status & 64 = 0)
and (sysindexes.status & 2048 = 0) -- not primary key
and object_name(sysobjects.id) in ('BM_Category', 'BM_Event', 'BM_ImportData', 'BM_ImportDataOld', 'BM_OddsRegular', 'BM_Score', 'BM_Season', 'BM_Sport', 'BM_Status', 'BM_Team', 'BM_Tip', 'BM_Tournament', 'CR_User')

open tbl_Indexy
fetch next from tbl_Indexy into @drop
while (@@fetch_status=0)
  begin
	set @sql=@sql+@drop
    fetch next from tbl_Indexy into @drop
  end
close tbl_Indexy
deallocate tbl_Indexy

exec (@sql)

GO

print 'CurrentTime: IndecesDel - ' + convert(varchar, getdate(), 120)

GO

-- Delete all computed columns

declare  @table varchar(250), @column varchar(250), @sql varchar(max)

declare tbl cursor local forward_only static for
select sys.tables.name, col.name
from sys.computed_columns as col
inner join sys.tables on col.object_id=sys.tables.object_id

open tbl
fetch next from tbl into @table, @column
while (@@fetch_status=0)
  begin

	set @sql='ALTER TABLE [dbo].[' + @table + '] DROP COLUMN [' + @column + ']'
	exec (@sql)

    fetch next from tbl into @table, @column
  end
close tbl
deallocate tbl

GO

print 'CurrentTime: ColumnsComputedDel - ' + convert(varchar, getdate(), 120)

GO

-- Delete all triggers

declare TblTriggers cursor local forward_only static for
select name from sys.triggers
declare @name varchar(250)
open TblTriggers
fetch next from TblTriggers into @name
while (@@FETCH_STATUS=0)
begin
	exec ('DROP TRIGGER [' + @name + ']')
	fetch next from TblTriggers into @name
end
close TblTriggers
deallocate TblTriggers

GO

print 'CurrentTime: TriggersDel - ' + convert(varchar, getdate(), 120)

GO

-- Create user defined types

print 'CurrentTime: UserTypes - ' + convert(varchar, getdate(), 120)

GO

-- Create tables (with PK)

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Category')
begin
	CREATE TABLE [BM_Category]
	(
		[ID] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Event')
begin
	CREATE TABLE [BM_Event]
	(
		[ID] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_ImportData')
begin
	CREATE TABLE [BM_ImportData]
	(
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_ImportDataOld')
begin
	CREATE TABLE [BM_ImportDataOld]
	(
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_OddsRegular')
begin
	CREATE TABLE [BM_OddsRegular]
	(
		[ID_Event] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Score')
begin
	CREATE TABLE [BM_Score]
	(
		[ID_Event] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Season')
begin
	CREATE TABLE [BM_Season]
	(
		[ID] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Sport')
begin
	CREATE TABLE [BM_Sport]
	(
		[ID] int  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Status')
begin
	CREATE TABLE [BM_Status]
	(
		[ID] int  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Team')
begin
	CREATE TABLE [BM_Team]
	(
		[ID] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Tip')
begin
	CREATE TABLE [BM_Tip]
	(
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='BM_Tournament')
begin
	CREATE TABLE [BM_Tournament]
	(
		[ID] bigint  NOT NULL
	)
end

GO

if not exists(select * from INFORMATION_SCHEMA.TABLES where TABLE_NAME='CR_User')
begin
	CREATE TABLE [CR_User]
	(
		[ID] int IDENTITY NOT NULL
	)
end

GO

print 'CurrentTime: CreateTables - ' + convert(varchar, getdate(), 120)

GO

-- Create columns

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DisplayName')
	alter table [BM_Category] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Category] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Category] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='Slug')
	alter table [BM_Category] add [Slug] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
	alter table [BM_Category] alter column [Slug] nvarchar(255) NULL
else
	alter table [BM_Category] alter column [Slug] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='IsActive')
	alter table [BM_Category] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Category] alter column [IsActive] bit NULL
else
	alter table [BM_Category] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='ID_Sport')
	alter table [BM_Category] add [ID_Sport] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='ID_Sport' and IS_NULLABLE='YES')
	alter table [BM_Category] alter column [ID_Sport] int NULL
else
	alter table [BM_Category] alter column [ID_Sport] int NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DateCreated')
	alter table [BM_Category] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Category] alter column [DateCreated] datetime NULL
else
	alter table [BM_Category] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DateUpdated')
	alter table [BM_Category] add [DateUpdated] datetime NULL
else
	alter table [BM_Category] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DisplayName')
	alter table [BM_Event] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Event] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='Slug')
	alter table [BM_Event] add [Slug] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [Slug] nvarchar(255) NULL
else
	alter table [BM_Event] alter column [Slug] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='IsActive')
	alter table [BM_Event] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [IsActive] bit NULL
else
	alter table [BM_Event] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='CustomId')
	alter table [BM_Event] add [CustomId] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='CustomId' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [CustomId] nvarchar(255) NULL
else
	alter table [BM_Event] alter column [CustomId] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='FirstToServe')
	alter table [BM_Event] add [FirstToServe] bit NULL
else
	alter table [BM_Event] alter column [FirstToServe] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='HasDraw')
	alter table [BM_Event] add [HasDraw] bit NULL
else
	alter table [BM_Event] alter column [HasDraw] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='WinnerCode')
	alter table [BM_Event] add [WinnerCode] int NULL
else
	alter table [BM_Event] alter column [WinnerCode] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateStart')
	alter table [BM_Event] add [DateStart] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateStart' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [DateStart] datetime NULL
else
	alter table [BM_Event] alter column [DateStart] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='Changes')
	alter table [BM_Event] add [Changes] datetime NULL
else
	alter table [BM_Event] alter column [Changes] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_HomeTeam')
	alter table [BM_Event] add [ID_HomeTeam] bigint NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_HomeTeam' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [ID_HomeTeam] bigint NULL
else
	alter table [BM_Event] alter column [ID_HomeTeam] bigint NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_AwayTeam')
	alter table [BM_Event] add [ID_AwayTeam] bigint NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_AwayTeam' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [ID_AwayTeam] bigint NULL
else
	alter table [BM_Event] alter column [ID_AwayTeam] bigint NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Tournament')
	alter table [BM_Event] add [ID_Tournament] bigint NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Tournament' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [ID_Tournament] bigint NULL
else
	alter table [BM_Event] alter column [ID_Tournament] bigint NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Season')
	alter table [BM_Event] add [ID_Season] bigint NULL
else
	alter table [BM_Event] alter column [ID_Season] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Category')
	alter table [BM_Event] add [ID_Category] bigint NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Category' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [ID_Category] bigint NULL
else
	alter table [BM_Event] alter column [ID_Category] bigint NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Status')
	alter table [BM_Event] add [ID_Status] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Status' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [ID_Status] int NULL
else
	alter table [BM_Event] alter column [ID_Status] int NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='StatusDescription')
	alter table [BM_Event] add [StatusDescription] nvarchar(255) NULL
else
	alter table [BM_Event] alter column [StatusDescription] nvarchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='HomeScoreCurrent')
	alter table [BM_Event] add [HomeScoreCurrent] int NULL
else
	alter table [BM_Event] alter column [HomeScoreCurrent] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='AwayScoreCurrent')
	alter table [BM_Event] add [AwayScoreCurrent] int NULL
else
	alter table [BM_Event] alter column [AwayScoreCurrent] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateCreated')
	alter table [BM_Event] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Event] alter column [DateCreated] datetime NULL
else
	alter table [BM_Event] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateUpdated')
	alter table [BM_Event] add [DateUpdated] datetime NULL
else
	alter table [BM_Event] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='Date')
	alter table [BM_ImportData] add [Date] datetime NULL
else
	alter table [BM_ImportData] alter column [Date] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SportName')
	alter table [BM_ImportData] add [SportName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [SportName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SportSlug')
	alter table [BM_ImportData] add [SportSlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [SportSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SportId')
	alter table [BM_ImportData] add [SportId] int NULL
else
	alter table [BM_ImportData] alter column [SportId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='TournamentName')
	alter table [BM_ImportData] add [TournamentName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [TournamentName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='TournamentSlug')
	alter table [BM_ImportData] add [TournamentSlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [TournamentSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='TournamentId')
	alter table [BM_ImportData] add [TournamentId] int NULL
else
	alter table [BM_ImportData] alter column [TournamentId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='TournamentUniqueId')
	alter table [BM_ImportData] add [TournamentUniqueId] int NULL
else
	alter table [BM_ImportData] alter column [TournamentUniqueId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='CategoryName')
	alter table [BM_ImportData] add [CategoryName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [CategoryName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='CategorySlug')
	alter table [BM_ImportData] add [CategorySlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [CategorySlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='CategoryId')
	alter table [BM_ImportData] add [CategoryId] int NULL
else
	alter table [BM_ImportData] alter column [CategoryId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SeasonName')
	alter table [BM_ImportData] add [SeasonName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [SeasonName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SeasonSlug')
	alter table [BM_ImportData] add [SeasonSlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [SeasonSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SeasonId')
	alter table [BM_ImportData] add [SeasonId] bigint NULL
else
	alter table [BM_ImportData] alter column [SeasonId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='SeasonYear')
	alter table [BM_ImportData] add [SeasonYear] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [SeasonYear] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventId')
	alter table [BM_ImportData] add [EventId] bigint NULL
else
	alter table [BM_ImportData] alter column [EventId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventCustomId')
	alter table [BM_ImportData] add [EventCustomId] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [EventCustomId] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventFirstToServe')
	alter table [BM_ImportData] add [EventFirstToServe] int NULL
else
	alter table [BM_ImportData] alter column [EventFirstToServe] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventHasDraw')
	alter table [BM_ImportData] add [EventHasDraw] bit NULL
else
	alter table [BM_ImportData] alter column [EventHasDraw] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventWinnerCode')
	alter table [BM_ImportData] add [EventWinnerCode] int NULL
else
	alter table [BM_ImportData] alter column [EventWinnerCode] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventName')
	alter table [BM_ImportData] add [EventName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [EventName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventSlug')
	alter table [BM_ImportData] add [EventSlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [EventSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventStartDate')
	alter table [BM_ImportData] add [EventStartDate] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [EventStartDate] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventStartTime')
	alter table [BM_ImportData] add [EventStartTime] time NULL
else
	alter table [BM_ImportData] alter column [EventStartTime] time NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='EventChanges')
	alter table [BM_ImportData] add [EventChanges] varchar(50) NULL
else
	alter table [BM_ImportData] alter column [EventChanges] varchar(50) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='StatusCode')
	alter table [BM_ImportData] add [StatusCode] int NULL
else
	alter table [BM_ImportData] alter column [StatusCode] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='StatusType')
	alter table [BM_ImportData] add [StatusType] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [StatusType] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='StatusDescription')
	alter table [BM_ImportData] add [StatusDescription] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [StatusDescription] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeTeamId')
	alter table [BM_ImportData] add [HomeTeamId] bigint NULL
else
	alter table [BM_ImportData] alter column [HomeTeamId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeTeamName')
	alter table [BM_ImportData] add [HomeTeamName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [HomeTeamName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeTeamSlug')
	alter table [BM_ImportData] add [HomeTeamSlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [HomeTeamSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeTeamGender')
	alter table [BM_ImportData] add [HomeTeamGender] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [HomeTeamGender] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScoreCurrent')
	alter table [BM_ImportData] add [HomeScoreCurrent] int NULL
else
	alter table [BM_ImportData] alter column [HomeScoreCurrent] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScorePeriod1')
	alter table [BM_ImportData] add [HomeScorePeriod1] int NULL
else
	alter table [BM_ImportData] alter column [HomeScorePeriod1] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScorePeriod2')
	alter table [BM_ImportData] add [HomeScorePeriod2] int NULL
else
	alter table [BM_ImportData] alter column [HomeScorePeriod2] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScorePeriod3')
	alter table [BM_ImportData] add [HomeScorePeriod3] int NULL
else
	alter table [BM_ImportData] alter column [HomeScorePeriod3] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScoreNormaltime')
	alter table [BM_ImportData] add [HomeScoreNormaltime] int NULL
else
	alter table [BM_ImportData] alter column [HomeScoreNormaltime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScoreOvertime')
	alter table [BM_ImportData] add [HomeScoreOvertime] int NULL
else
	alter table [BM_ImportData] alter column [HomeScoreOvertime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='HomeScorePenalties')
	alter table [BM_ImportData] add [HomeScorePenalties] int NULL
else
	alter table [BM_ImportData] alter column [HomeScorePenalties] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayTeamId')
	alter table [BM_ImportData] add [AwayTeamId] bigint NULL
else
	alter table [BM_ImportData] alter column [AwayTeamId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayTeamName')
	alter table [BM_ImportData] add [AwayTeamName] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [AwayTeamName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayTeamSlug')
	alter table [BM_ImportData] add [AwayTeamSlug] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [AwayTeamSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayTeamGender')
	alter table [BM_ImportData] add [AwayTeamGender] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [AwayTeamGender] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScoreCurrent')
	alter table [BM_ImportData] add [AwayScoreCurrent] int NULL
else
	alter table [BM_ImportData] alter column [AwayScoreCurrent] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScorePeriod1')
	alter table [BM_ImportData] add [AwayScorePeriod1] int NULL
else
	alter table [BM_ImportData] alter column [AwayScorePeriod1] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScorePeriod2')
	alter table [BM_ImportData] add [AwayScorePeriod2] int NULL
else
	alter table [BM_ImportData] alter column [AwayScorePeriod2] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScorePeriod3')
	alter table [BM_ImportData] add [AwayScorePeriod3] int NULL
else
	alter table [BM_ImportData] alter column [AwayScorePeriod3] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScoreNormaltime')
	alter table [BM_ImportData] add [AwayScoreNormaltime] int NULL
else
	alter table [BM_ImportData] alter column [AwayScoreNormaltime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScoreOvertime')
	alter table [BM_ImportData] add [AwayScoreOvertime] int NULL
else
	alter table [BM_ImportData] alter column [AwayScoreOvertime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='AwayScorePenalties')
	alter table [BM_ImportData] add [AwayScorePenalties] int NULL
else
	alter table [BM_ImportData] alter column [AwayScorePenalties] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularFirstSourceId')
	alter table [BM_ImportData] add [OddsRegularFirstSourceId] bigint NULL
else
	alter table [BM_ImportData] alter column [OddsRegularFirstSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularFirstValue')
	alter table [BM_ImportData] add [OddsRegularFirstValue] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [OddsRegularFirstValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularFirstWining')
	alter table [BM_ImportData] add [OddsRegularFirstWining] bit NULL
else
	alter table [BM_ImportData] alter column [OddsRegularFirstWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularXSourceId')
	alter table [BM_ImportData] add [OddsRegularXSourceId] bigint NULL
else
	alter table [BM_ImportData] alter column [OddsRegularXSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularXValue')
	alter table [BM_ImportData] add [OddsRegularXValue] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [OddsRegularXValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularXWining')
	alter table [BM_ImportData] add [OddsRegularXWining] bit NULL
else
	alter table [BM_ImportData] alter column [OddsRegularXWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularSecondSourceId')
	alter table [BM_ImportData] add [OddsRegularSecondSourceId] bigint NULL
else
	alter table [BM_ImportData] alter column [OddsRegularSecondSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularSecondValue')
	alter table [BM_ImportData] add [OddsRegularSecondValue] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [OddsRegularSecondValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsRegularSecondWining')
	alter table [BM_ImportData] add [OddsRegularSecondWining] bit NULL
else
	alter table [BM_ImportData] alter column [OddsRegularSecondWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeFirstXSourceId')
	alter table [BM_ImportData] add [OddsDoubleChangeFirstXSourceId] bigint NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeFirstXSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeFirstXValue')
	alter table [BM_ImportData] add [OddsDoubleChangeFirstXValue] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeFirstXValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeFirstXWining')
	alter table [BM_ImportData] add [OddsDoubleChangeFirstXWining] bit NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeFirstXWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeXSecondSourceId')
	alter table [BM_ImportData] add [OddsDoubleChangeXSecondSourceId] bigint NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeXSecondSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeXSecondValue')
	alter table [BM_ImportData] add [OddsDoubleChangeXSecondValue] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeXSecondValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeXSecondWining')
	alter table [BM_ImportData] add [OddsDoubleChangeXSecondWining] bit NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeXSecondWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeFirstSecondSourceId')
	alter table [BM_ImportData] add [OddsDoubleChangeFirstSecondSourceId] bigint NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeFirstSecondSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeFirstSecondValue')
	alter table [BM_ImportData] add [OddsDoubleChangeFirstSecondValue] varchar(255) NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeFirstSecondValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='OddsDoubleChangeFirstSecondWining')
	alter table [BM_ImportData] add [OddsDoubleChangeFirstSecondWining] bit NULL
else
	alter table [BM_ImportData] alter column [OddsDoubleChangeFirstSecondWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='ID')
	alter table [BM_ImportData] add [ID] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
	alter table [BM_ImportData] alter column [ID] int NULL
else
	alter table [BM_ImportData] alter column [ID] int NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='IsProcessed')
	alter table [BM_ImportData] add [IsProcessed] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='IsProcessed' and IS_NULLABLE='YES')
	alter table [BM_ImportData] alter column [IsProcessed] bit NULL
else
	alter table [BM_ImportData] alter column [IsProcessed] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='Date')
	alter table [BM_ImportDataOld] add [Date] datetime NULL
else
	alter table [BM_ImportDataOld] alter column [Date] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SportName')
	alter table [BM_ImportDataOld] add [SportName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [SportName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SportSlug')
	alter table [BM_ImportDataOld] add [SportSlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [SportSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SportId')
	alter table [BM_ImportDataOld] add [SportId] int NULL
else
	alter table [BM_ImportDataOld] alter column [SportId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='TournamentName')
	alter table [BM_ImportDataOld] add [TournamentName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [TournamentName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='TournamentSlug')
	alter table [BM_ImportDataOld] add [TournamentSlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [TournamentSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='TournamentId')
	alter table [BM_ImportDataOld] add [TournamentId] int NULL
else
	alter table [BM_ImportDataOld] alter column [TournamentId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='TournamentUniqueId')
	alter table [BM_ImportDataOld] add [TournamentUniqueId] int NULL
else
	alter table [BM_ImportDataOld] alter column [TournamentUniqueId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='CategoryName')
	alter table [BM_ImportDataOld] add [CategoryName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [CategoryName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='CategorySlug')
	alter table [BM_ImportDataOld] add [CategorySlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [CategorySlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='CategoryId')
	alter table [BM_ImportDataOld] add [CategoryId] int NULL
else
	alter table [BM_ImportDataOld] alter column [CategoryId] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SeasonName')
	alter table [BM_ImportDataOld] add [SeasonName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [SeasonName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SeasonSlug')
	alter table [BM_ImportDataOld] add [SeasonSlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [SeasonSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SeasonId')
	alter table [BM_ImportDataOld] add [SeasonId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [SeasonId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='SeasonYear')
	alter table [BM_ImportDataOld] add [SeasonYear] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [SeasonYear] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventId')
	alter table [BM_ImportDataOld] add [EventId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [EventId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventCustomId')
	alter table [BM_ImportDataOld] add [EventCustomId] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [EventCustomId] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventFirstToServe')
	alter table [BM_ImportDataOld] add [EventFirstToServe] int NULL
else
	alter table [BM_ImportDataOld] alter column [EventFirstToServe] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventHasDraw')
	alter table [BM_ImportDataOld] add [EventHasDraw] bit NULL
else
	alter table [BM_ImportDataOld] alter column [EventHasDraw] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventWinnerCode')
	alter table [BM_ImportDataOld] add [EventWinnerCode] int NULL
else
	alter table [BM_ImportDataOld] alter column [EventWinnerCode] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventName')
	alter table [BM_ImportDataOld] add [EventName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [EventName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventSlug')
	alter table [BM_ImportDataOld] add [EventSlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [EventSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventStartDate')
	alter table [BM_ImportDataOld] add [EventStartDate] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [EventStartDate] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventStartTime')
	alter table [BM_ImportDataOld] add [EventStartTime] time NULL
else
	alter table [BM_ImportDataOld] alter column [EventStartTime] time NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='EventChanges')
	alter table [BM_ImportDataOld] add [EventChanges] varchar(50) NULL
else
	alter table [BM_ImportDataOld] alter column [EventChanges] varchar(50) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='StatusCode')
	alter table [BM_ImportDataOld] add [StatusCode] int NULL
else
	alter table [BM_ImportDataOld] alter column [StatusCode] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='StatusType')
	alter table [BM_ImportDataOld] add [StatusType] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [StatusType] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='StatusDescription')
	alter table [BM_ImportDataOld] add [StatusDescription] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [StatusDescription] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeTeamId')
	alter table [BM_ImportDataOld] add [HomeTeamId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [HomeTeamId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeTeamName')
	alter table [BM_ImportDataOld] add [HomeTeamName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [HomeTeamName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeTeamSlug')
	alter table [BM_ImportDataOld] add [HomeTeamSlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [HomeTeamSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeTeamGender')
	alter table [BM_ImportDataOld] add [HomeTeamGender] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [HomeTeamGender] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScoreCurrent')
	alter table [BM_ImportDataOld] add [HomeScoreCurrent] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScoreCurrent] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScorePeriod1')
	alter table [BM_ImportDataOld] add [HomeScorePeriod1] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScorePeriod1] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScorePeriod2')
	alter table [BM_ImportDataOld] add [HomeScorePeriod2] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScorePeriod2] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScorePeriod3')
	alter table [BM_ImportDataOld] add [HomeScorePeriod3] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScorePeriod3] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScoreNormaltime')
	alter table [BM_ImportDataOld] add [HomeScoreNormaltime] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScoreNormaltime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScoreOvertime')
	alter table [BM_ImportDataOld] add [HomeScoreOvertime] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScoreOvertime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='HomeScorePenalties')
	alter table [BM_ImportDataOld] add [HomeScorePenalties] int NULL
else
	alter table [BM_ImportDataOld] alter column [HomeScorePenalties] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayTeamId')
	alter table [BM_ImportDataOld] add [AwayTeamId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [AwayTeamId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayTeamName')
	alter table [BM_ImportDataOld] add [AwayTeamName] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [AwayTeamName] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayTeamSlug')
	alter table [BM_ImportDataOld] add [AwayTeamSlug] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [AwayTeamSlug] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayTeamGender')
	alter table [BM_ImportDataOld] add [AwayTeamGender] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [AwayTeamGender] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScoreCurrent')
	alter table [BM_ImportDataOld] add [AwayScoreCurrent] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScoreCurrent] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScorePeriod1')
	alter table [BM_ImportDataOld] add [AwayScorePeriod1] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScorePeriod1] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScorePeriod2')
	alter table [BM_ImportDataOld] add [AwayScorePeriod2] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScorePeriod2] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScorePeriod3')
	alter table [BM_ImportDataOld] add [AwayScorePeriod3] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScorePeriod3] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScoreNormaltime')
	alter table [BM_ImportDataOld] add [AwayScoreNormaltime] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScoreNormaltime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScoreOvertime')
	alter table [BM_ImportDataOld] add [AwayScoreOvertime] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScoreOvertime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='AwayScorePenalties')
	alter table [BM_ImportDataOld] add [AwayScorePenalties] int NULL
else
	alter table [BM_ImportDataOld] alter column [AwayScorePenalties] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularFirstSourceId')
	alter table [BM_ImportDataOld] add [OddsRegularFirstSourceId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularFirstSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularFirstValue')
	alter table [BM_ImportDataOld] add [OddsRegularFirstValue] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularFirstValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularFirstWining')
	alter table [BM_ImportDataOld] add [OddsRegularFirstWining] bit NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularFirstWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularXSourceId')
	alter table [BM_ImportDataOld] add [OddsRegularXSourceId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularXSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularXValue')
	alter table [BM_ImportDataOld] add [OddsRegularXValue] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularXValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularXWining')
	alter table [BM_ImportDataOld] add [OddsRegularXWining] bit NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularXWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularSecondSourceId')
	alter table [BM_ImportDataOld] add [OddsRegularSecondSourceId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularSecondSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularSecondValue')
	alter table [BM_ImportDataOld] add [OddsRegularSecondValue] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularSecondValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsRegularSecondWining')
	alter table [BM_ImportDataOld] add [OddsRegularSecondWining] bit NULL
else
	alter table [BM_ImportDataOld] alter column [OddsRegularSecondWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeFirstXSourceId')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeFirstXSourceId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeFirstXSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeFirstXValue')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeFirstXValue] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeFirstXValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeFirstXWining')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeFirstXWining] bit NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeFirstXWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeXSecondSourceId')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeXSecondSourceId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeXSecondSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeXSecondValue')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeXSecondValue] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeXSecondValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeXSecondWining')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeXSecondWining] bit NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeXSecondWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeFirstSecondSourceId')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeFirstSecondSourceId] bigint NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeFirstSecondSourceId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeFirstSecondValue')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeFirstSecondValue] varchar(255) NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeFirstSecondValue] varchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='OddsDoubleChangeFirstSecondWining')
	alter table [BM_ImportDataOld] add [OddsDoubleChangeFirstSecondWining] bit NULL
else
	alter table [BM_ImportDataOld] alter column [OddsDoubleChangeFirstSecondWining] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='ID')
	alter table [BM_ImportDataOld] add [ID] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
	alter table [BM_ImportDataOld] alter column [ID] int NULL
else
	alter table [BM_ImportDataOld] alter column [ID] int NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='IsProcessed')
	alter table [BM_ImportDataOld] add [IsProcessed] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='IsProcessed' and IS_NULLABLE='YES')
	alter table [BM_ImportDataOld] alter column [IsProcessed] bit NULL
else
	alter table [BM_ImportDataOld] alter column [IsProcessed] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='Type')
	alter table [BM_OddsRegular] add [Type] nvarchar(50) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='Type' and IS_NULLABLE='YES')
	alter table [BM_OddsRegular] alter column [Type] nvarchar(50) NULL
else
	alter table [BM_OddsRegular] alter column [Type] nvarchar(50) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='FirstId')
	alter table [BM_OddsRegular] add [FirstId] bigint NULL
else
	alter table [BM_OddsRegular] alter column [FirstId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='FirstValue')
	alter table [BM_OddsRegular] add [FirstValue] decimal (8, 2) NULL
else
	alter table [BM_OddsRegular] alter column [FirstValue] decimal (8, 2) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='HasFirstWin')
	alter table [BM_OddsRegular] add [HasFirstWin] bit NULL
else
	alter table [BM_OddsRegular] alter column [HasFirstWin] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='XId')
	alter table [BM_OddsRegular] add [XId] bigint NULL
else
	alter table [BM_OddsRegular] alter column [XId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='XValue')
	alter table [BM_OddsRegular] add [XValue] decimal (8, 2) NULL
else
	alter table [BM_OddsRegular] alter column [XValue] decimal (8, 2) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='HasXWin')
	alter table [BM_OddsRegular] add [HasXWin] bit NULL
else
	alter table [BM_OddsRegular] alter column [HasXWin] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='SecondId')
	alter table [BM_OddsRegular] add [SecondId] bigint NULL
else
	alter table [BM_OddsRegular] alter column [SecondId] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='SecondValue')
	alter table [BM_OddsRegular] add [SecondValue] decimal (8, 2) NULL
else
	alter table [BM_OddsRegular] alter column [SecondValue] decimal (8, 2) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='HasSecondWin')
	alter table [BM_OddsRegular] add [HasSecondWin] bit NULL
else
	alter table [BM_OddsRegular] alter column [HasSecondWin] bit NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='DateCreated')
	alter table [BM_OddsRegular] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_OddsRegular] alter column [DateCreated] datetime NULL
else
	alter table [BM_OddsRegular] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='DateUpdated')
	alter table [BM_OddsRegular] add [DateUpdated] datetime NULL
else
	alter table [BM_OddsRegular] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScoreCurrent')
	alter table [BM_Score] add [HomeScoreCurrent] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScoreCurrent' and IS_NULLABLE='YES')
	alter table [BM_Score] alter column [HomeScoreCurrent] int NULL
else
	alter table [BM_Score] alter column [HomeScoreCurrent] int NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScorePeriod1')
	alter table [BM_Score] add [HomeScorePeriod1] int NULL
else
	alter table [BM_Score] alter column [HomeScorePeriod1] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScorePeriod2')
	alter table [BM_Score] add [HomeScorePeriod2] int NULL
else
	alter table [BM_Score] alter column [HomeScorePeriod2] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScorePeriod3')
	alter table [BM_Score] add [HomeScorePeriod3] int NULL
else
	alter table [BM_Score] alter column [HomeScorePeriod3] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScoreNormaltime')
	alter table [BM_Score] add [HomeScoreNormaltime] int NULL
else
	alter table [BM_Score] alter column [HomeScoreNormaltime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScoreOvertime')
	alter table [BM_Score] add [HomeScoreOvertime] int NULL
else
	alter table [BM_Score] alter column [HomeScoreOvertime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScorePenalties')
	alter table [BM_Score] add [HomeScorePenalties] int NULL
else
	alter table [BM_Score] alter column [HomeScorePenalties] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScoreCurrent')
	alter table [BM_Score] add [AwayScoreCurrent] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScoreCurrent' and IS_NULLABLE='YES')
	alter table [BM_Score] alter column [AwayScoreCurrent] int NULL
else
	alter table [BM_Score] alter column [AwayScoreCurrent] int NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScorePeriod1')
	alter table [BM_Score] add [AwayScorePeriod1] int NULL
else
	alter table [BM_Score] alter column [AwayScorePeriod1] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScorePeriod2')
	alter table [BM_Score] add [AwayScorePeriod2] int NULL
else
	alter table [BM_Score] alter column [AwayScorePeriod2] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScorePeriod3')
	alter table [BM_Score] add [AwayScorePeriod3] int NULL
else
	alter table [BM_Score] alter column [AwayScorePeriod3] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScoreNormaltime')
	alter table [BM_Score] add [AwayScoreNormaltime] int NULL
else
	alter table [BM_Score] alter column [AwayScoreNormaltime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScoreOvertime')
	alter table [BM_Score] add [AwayScoreOvertime] int NULL
else
	alter table [BM_Score] alter column [AwayScoreOvertime] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScorePenalties')
	alter table [BM_Score] add [AwayScorePenalties] int NULL
else
	alter table [BM_Score] alter column [AwayScorePenalties] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='DateCreated')
	alter table [BM_Score] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Score] alter column [DateCreated] datetime NULL
else
	alter table [BM_Score] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='DateUpdated')
	alter table [BM_Score] add [DateUpdated] datetime NULL
else
	alter table [BM_Score] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DisplayName')
	alter table [BM_Season] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Season] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Season] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='Slug')
	alter table [BM_Season] add [Slug] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
	alter table [BM_Season] alter column [Slug] nvarchar(255) NULL
else
	alter table [BM_Season] alter column [Slug] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='IsActive')
	alter table [BM_Season] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Season] alter column [IsActive] bit NULL
else
	alter table [BM_Season] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='Year')
	alter table [BM_Season] add [Year] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='Year' and IS_NULLABLE='YES')
	alter table [BM_Season] alter column [Year] nvarchar(255) NULL
else
	alter table [BM_Season] alter column [Year] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DateCreated')
	alter table [BM_Season] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Season] alter column [DateCreated] datetime NULL
else
	alter table [BM_Season] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DateUpdated')
	alter table [BM_Season] add [DateUpdated] datetime NULL
else
	alter table [BM_Season] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DisplayName')
	alter table [BM_Sport] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Sport] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Sport] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='Slug')
	alter table [BM_Sport] add [Slug] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
	alter table [BM_Sport] alter column [Slug] nvarchar(255) NULL
else
	alter table [BM_Sport] alter column [Slug] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='IsActive')
	alter table [BM_Sport] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Sport] alter column [IsActive] bit NULL
else
	alter table [BM_Sport] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DateCreated')
	alter table [BM_Sport] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Sport] alter column [DateCreated] datetime NULL
else
	alter table [BM_Sport] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DateUpdated')
	alter table [BM_Sport] add [DateUpdated] datetime NULL
else
	alter table [BM_Sport] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DisplayName')
	alter table [BM_Status] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Status] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Status] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='Description')
	alter table [BM_Status] add [Description] nvarchar(255) NULL
else
	alter table [BM_Status] alter column [Description] nvarchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='IsActive')
	alter table [BM_Status] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Status] alter column [IsActive] bit NULL
else
	alter table [BM_Status] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DateCreated')
	alter table [BM_Status] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Status] alter column [DateCreated] datetime NULL
else
	alter table [BM_Status] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DateUpdated')
	alter table [BM_Status] add [DateUpdated] datetime NULL
else
	alter table [BM_Status] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DisplayName')
	alter table [BM_Team] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Team] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Team] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='Slug')
	alter table [BM_Team] add [Slug] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
	alter table [BM_Team] alter column [Slug] nvarchar(255) NULL
else
	alter table [BM_Team] alter column [Slug] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='IsActive')
	alter table [BM_Team] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Team] alter column [IsActive] bit NULL
else
	alter table [BM_Team] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='Gender')
	alter table [BM_Team] add [Gender] nvarchar(255) NULL
else
	alter table [BM_Team] alter column [Gender] nvarchar(255) NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DateCreated')
	alter table [BM_Team] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Team] alter column [DateCreated] datetime NULL
else
	alter table [BM_Team] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DateUpdated')
	alter table [BM_Team] add [DateUpdated] datetime NULL
else
	alter table [BM_Team] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='ID')
	alter table [BM_Tip] add [ID] int NULL
else
	alter table [BM_Tip] alter column [ID] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeLastForm')
	alter table [BM_Tip] add [HomeLastForm] int NULL
else
	alter table [BM_Tip] alter column [HomeLastForm] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeLastGiven')
	alter table [BM_Tip] add [HomeLastGiven] int NULL
else
	alter table [BM_Tip] alter column [HomeLastGiven] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeLastTaken')
	alter table [BM_Tip] add [HomeLastTaken] int NULL
else
	alter table [BM_Tip] alter column [HomeLastTaken] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeSeasonForm')
	alter table [BM_Tip] add [HomeSeasonForm] int NULL
else
	alter table [BM_Tip] alter column [HomeSeasonForm] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeSeasonGiven')
	alter table [BM_Tip] add [HomeSeasonGiven] int NULL
else
	alter table [BM_Tip] alter column [HomeSeasonGiven] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeSeasonTaken')
	alter table [BM_Tip] add [HomeSeasonTaken] int NULL
else
	alter table [BM_Tip] alter column [HomeSeasonTaken] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='HomeSeasonCount')
	alter table [BM_Tip] add [HomeSeasonCount] int NULL
else
	alter table [BM_Tip] alter column [HomeSeasonCount] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwayLastForm')
	alter table [BM_Tip] add [AwayLastForm] int NULL
else
	alter table [BM_Tip] alter column [AwayLastForm] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwayLastGiven')
	alter table [BM_Tip] add [AwayLastGiven] int NULL
else
	alter table [BM_Tip] alter column [AwayLastGiven] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwayLastTaken')
	alter table [BM_Tip] add [AwayLastTaken] int NULL
else
	alter table [BM_Tip] alter column [AwayLastTaken] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwaySeasonForm')
	alter table [BM_Tip] add [AwaySeasonForm] int NULL
else
	alter table [BM_Tip] alter column [AwaySeasonForm] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwaySeasonGiven')
	alter table [BM_Tip] add [AwaySeasonGiven] int NULL
else
	alter table [BM_Tip] alter column [AwaySeasonGiven] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwaySeasonTaken')
	alter table [BM_Tip] add [AwaySeasonTaken] int NULL
else
	alter table [BM_Tip] alter column [AwaySeasonTaken] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tip' and COLUMN_NAME='AwaySeasonCount')
	alter table [BM_Tip] add [AwaySeasonCount] int NULL
else
	alter table [BM_Tip] alter column [AwaySeasonCount] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DisplayName')
	alter table [BM_Tournament] add [DisplayName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
	alter table [BM_Tournament] alter column [DisplayName] nvarchar(255) NULL
else
	alter table [BM_Tournament] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='Slug')
	alter table [BM_Tournament] add [Slug] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
	alter table [BM_Tournament] alter column [Slug] nvarchar(255) NULL
else
	alter table [BM_Tournament] alter column [Slug] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='IsActive')
	alter table [BM_Tournament] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [BM_Tournament] alter column [IsActive] bit NULL
else
	alter table [BM_Tournament] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='UniqueID')
	alter table [BM_Tournament] add [UniqueID] int NULL
else
	alter table [BM_Tournament] alter column [UniqueID] int NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='ID_Category')
	alter table [BM_Tournament] add [ID_Category] bigint NULL
else
	alter table [BM_Tournament] alter column [ID_Category] bigint NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DateCreated')
	alter table [BM_Tournament] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [BM_Tournament] alter column [DateCreated] datetime NULL
else
	alter table [BM_Tournament] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DateUpdated')
	alter table [BM_Tournament] add [DateUpdated] datetime NULL
else
	alter table [BM_Tournament] alter column [DateUpdated] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='IsActive')
	alter table [CR_User] add [IsActive] bit NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [IsActive] bit NULL
else
	alter table [CR_User] alter column [IsActive] bit NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='UserName')
	alter table [CR_User] add [UserName] nvarchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='UserName' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [UserName] nvarchar(255) NULL
else
	alter table [CR_User] alter column [UserName] nvarchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Password')
	alter table [CR_User] add [Password] varchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Password' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [Password] varchar(255) NULL
else
	alter table [CR_User] alter column [Password] varchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Salt')
	alter table [CR_User] add [Salt] varchar(255) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Salt' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [Salt] varchar(255) NULL
else
	alter table [CR_User] alter column [Salt] varchar(255) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='LastLogin')
	alter table [CR_User] add [LastLogin] datetime NULL
else
	alter table [CR_User] alter column [LastLogin] datetime NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='DateCreated')
	alter table [CR_User] add [DateCreated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [DateCreated] datetime NULL
else
	alter table [CR_User] alter column [DateCreated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='DateUpdated')
	alter table [CR_User] add [DateUpdated] datetime NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='DateUpdated' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [DateUpdated] datetime NULL
else
	alter table [CR_User] alter column [DateUpdated] datetime NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Odd')
	alter table [CR_User] add [Odd] decimal (9, 2) NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Odd' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [Odd] decimal (9, 2) NULL
else
	alter table [CR_User] alter column [Odd] decimal (9, 2) NOT NULL

GO

if not exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Form')
	alter table [CR_User] add [Form] int NULL
else if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Form' and IS_NULLABLE='YES')
	alter table [CR_User] alter column [Form] int NULL
else
	alter table [CR_User] alter column [Form] int NOT NULL

GO

print 'CurrentTime: Columns - ' + convert(varchar, getdate(), 120)

GO

-- Create/update functions before views

print 'CurrentTime: Functions before views - ' + convert(varchar, getdate(), 120)

GO

-- Create/update views

if exists (select * from sys.views where name='BM_EventView')
begin
  drop view BM_EventView
end

GO

CREATE VIEW dbo.BM_EventView
AS
SELECT        ID, DisplayName, Slug, IsActive, CustomId, FirstToServe, HasDraw, WinnerCode, DateStart, Changes, ID_HomeTeam, ID_AwayTeam, ID_Tournament, ID_Season, ID_Category, ID_Status, StatusDescription, 
                         HomeScoreCurrent, AwayScoreCurrent, DateCreated, DateUpdated, (CASE WHEN WinnerCode = 1 THEN 3 ELSE CASE WHEN WinnerCode = 3 THEN 1 ELSE 0 END END) AS HomePoints, 
                         (CASE WHEN WinnerCode = 2 THEN 3 ELSE CASE WHEN WinnerCode = 3 THEN 1 ELSE 0 END END) AS AwayPoints, 
                         (CASE WHEN WinnerCode = 1 THEN 1 ELSE CASE WHEN WinnerCode = 3 THEN 0.5 ELSE 0 END END) AS HomeScore, 
                         (CASE WHEN WinnerCode = 2 THEN 1 ELSE CASE WHEN WinnerCode = 3 THEN 0.5 ELSE 0 END END) AS AwayScore
FROM            dbo.BM_Event


GO

if exists (select * from sys.views where name='BM_ImportDataView')
begin
  drop view BM_ImportDataView
end

GO

CREATE VIEW dbo.BM_ImportDataView
AS
SELECT        Date, SportName, SportSlug, SportId, TournamentName, TournamentSlug, TournamentId, TournamentUniqueId, CategoryName, CategorySlug, CategoryId, SeasonName, SeasonSlug, SeasonId, SeasonYear, 
                         EventId, EventCustomId, EventFirstToServe, EventHasDraw, EventWinnerCode, EventName, EventSlug, EventStartDate, EventStartTime, EventChanges, StatusCode, StatusType, StatusDescription, HomeTeamId, 
                         HomeTeamName, HomeTeamSlug, HomeTeamGender, HomeScoreCurrent, HomeScorePeriod1, HomeScorePeriod2, HomeScorePeriod3, HomeScoreNormaltime, HomeScoreOvertime, HomeScorePenalties, 
                         AwayTeamId, AwayTeamName, AwayTeamSlug, AwayTeamGender, AwayScoreCurrent, AwayScorePeriod1, AwayScorePeriod2, AwayScorePeriod3, AwayScoreNormaltime, AwayScoreOvertime, 
                         AwayScorePenalties, OddsRegularFirstSourceId, OddsRegularFirstValue, OddsRegularFirstWining, OddsRegularXSourceId, OddsRegularXValue, OddsRegularXWining, OddsRegularSecondSourceId, 
                         OddsRegularSecondValue, OddsRegularSecondWining, OddsDoubleChangeFirstXSourceId, OddsDoubleChangeFirstXValue, OddsDoubleChangeFirstXWining, OddsDoubleChangeXSecondSourceId, 
                         OddsDoubleChangeXSecondValue, OddsDoubleChangeXSecondWining, OddsDoubleChangeFirstSecondSourceId, OddsDoubleChangeFirstSecondValue, OddsDoubleChangeFirstSecondWining
FROM            dbo.BM_ImportData


GO

print 'CurrentTime: Views - ' + convert(varchar, getdate(), 120)

GO

-- Create/update functions after views

if exists (select * from sysobjects where name='BM_Event_DETAIL_LastEvent')
begin
  drop function BM_Event_DETAIL_LastEvent
end

GO


-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 14.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky
-- =============================================
CREATE FUNCTION BM_Event_DETAIL_LastEvent
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull((100.0/7.0 * sum(LastEvent.Form)),0) as Form , IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken from 
	(
		select top 7 BM_EventOriginal.ID, 
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScore else BM_Event.HomeScore end) as Form, 
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end) as Given,
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end) as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID < BM_EventOriginal.ID 
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
		order by BM_Event.DateStart desc
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_TeamForm')
begin
  drop function BM_Event_DETAIL_TeamForm
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 13.10.2016
-- Description: Vrátí formu pro aktuální zápas vypočítanou 
-- z posledních 7 zápasů, přičemž win=1, draw=0.5, lose=0 
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_TeamForm] 
(
	@ID_Event int,
	@ID_Team int
)
RETURNS decimal(9,3)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Form decimal(9,3)

	select @Form=(100.0/7.0 * IsNull(sum(TeamScore),0))
	from 
	(
		select top 7 (case when BM_Event.ID_AwayTeam=@ID_Team then AwayScore else HomeScore end) as TeamScore
		from BM_EventView as BM_Event
		where 
			BM_Event.ID < @ID_Event
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
		order by BM_Event.DateStart desc
	) x

	-- Return the result of the function
	RETURN @Form

END


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_TeamScoreGiven')
begin
  drop function BM_Event_DETAIL_TeamScoreGiven
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 13.10.2016
-- Description: Vrátí počet střel za posledních 7 zápasů
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_TeamScoreGiven] 
(
	@ID_Event int,
	@ID_Team int
)
RETURNS decimal(9,3)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @goal int

	select @goal=IsNull(sum(TeamScore), 0)
	from 
	(
		select top 7 (case when BM_Event.ID_AwayTeam=@ID_Team then AwayScoreCurrent else HomeScoreCurrent end) as TeamScore
		from BM_EventView as BM_Event
		where 
			BM_Event.ID < @ID_Event
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
		order by BM_Event.DateStart desc
	) x

	-- Return the result of the function
	RETURN @goal

END


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_TeamScoreTaken')
begin
  drop function BM_Event_DETAIL_TeamScoreTaken
end

GO


-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 13.10.2016
-- Description: Vrátí počet golů, který tým dostal za 7 zápasů
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_TeamScoreTaken] 
(
	@ID_Event int,
	@ID_Team int
)
RETURNS decimal(9,3)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @goal int

	select @goal=IsNull(sum(TeamScore), 0)
	from 
	(
		select top 7 (case when BM_Event.ID_AwayTeam=@ID_Team then HomeScoreCurrent else AwayScoreCurrent end) as TeamScore
		from BM_EventView as BM_Event
		where 
			BM_Event.ID < @ID_Event
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
		order by BM_Event.DateStart desc
	) x

	-- Return the result of the function
	RETURN @goal

END


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_SeasonEvent')
begin
  drop function BM_Event_DETAIL_SeasonEvent
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 18.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_SeasonEvent]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, case when count(*)=1 then 0 else IsNull(((100.0/(count(*)-1)) * sum(LastEvent.Form)),0) end as Form, 
	IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken, (count(*)-1) as [Count] from 
	(
		select BM_EventOriginal.ID,
			case when BM_Event.ID=BM_EventOriginal.ID then 0 else (case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScore else BM_Event.HomeScore end) end as Form, 
			case when BM_Event.ID=BM_EventOriginal.ID then 0 else (case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end) end as Given,
			case when BM_Event.ID=BM_EventOriginal.ID then 0 else (case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end) end as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID <= BM_EventOriginal.ID 
			and BM_Event.ID_Season=BM_EventOriginal.ID_Season
			and BM_Event.ID_Tournament=BM_EventOriginal.ID_Tournament
			and BM_Event.ID_Status >= 90
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_LastEvent_AwayOnly')
begin
  drop function BM_Event_DETAIL_LastEvent_AwayOnly
end

GO


-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 26.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky POUZE VENKU 
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_LastEvent_AwayOnly]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull((100.0/7.0 * sum(LastEvent.Form)),0) as Form , IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken from 
	(
		select top 7 BM_EventOriginal.ID, 
			BM_Event.AwayScore as Form, 
			BM_Event.AwayScoreCurrent as Given,
			BM_Event.HomeScoreCurrent as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID < BM_EventOriginal.ID 
			and BM_Event.ID_AwayTeam=@ID_Team
		order by BM_Event.DateStart desc
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_LastEvent_HomeOnly')
begin
  drop function BM_Event_DETAIL_LastEvent_HomeOnly
end

GO


-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 26.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky POUZE DOMACI 
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_LastEvent_HomeOnly]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull((100.0/7.0 * sum(LastEvent.Form)),0) as Form , IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken from 
	(
		select top 7 BM_EventOriginal.ID, 
			BM_Event.HomeScore as Form, 
			BM_Event.HomeScoreCurrent as Given,
			BM_Event.AwayScoreCurrent as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID < BM_EventOriginal.ID 
			and BM_Event.ID_HomeTeam=@ID_Team
		order by BM_Event.DateStart desc
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_SeasonEvent_AwayOnly')
begin
  drop function BM_Event_DETAIL_SeasonEvent_AwayOnly
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 26.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky POUZE VENKU
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_SeasonEvent_AwayOnly]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull(((100.0/count(*)) * sum(LastEvent.Form)),0) as Form, 
	IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken, count(*) as [Count] from 
	(
		select BM_EventOriginal.ID, 
			BM_Event.AwayScore as Form, 
			BM_Event.AwayScoreCurrent as Given,
			BM_Event.AwayScoreCurrent as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID < BM_EventOriginal.ID 
			and BM_Event.ID_Season=BM_EventOriginal.ID_Season
			and BM_Event.ID_Tournament=BM_EventOriginal.ID_Tournament
			and BM_Event.ID_Status >= 90
			and BM_Event.ID_AwayTeam=@ID_Team
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_SeasonEvent_HomeOnly')
begin
  drop function BM_Event_DETAIL_SeasonEvent_HomeOnly
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 26.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky POUZE DOMA
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_SeasonEvent_HomeOnly]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull(((100.0/count(*)) * sum(LastEvent.Form)),0) as Form, 
	IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken, count(*) as [Count] from 
	(
		select BM_EventOriginal.ID, 
			BM_Event.HomeScore as Form, 
			BM_Event.HomeScoreCurrent as Given,
			BM_Event.HomeScoreCurrent as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID < BM_EventOriginal.ID 
			and BM_Event.ID_Season=BM_EventOriginal.ID_Season
			and BM_Event.ID_Tournament=BM_EventOriginal.ID_Tournament
			and BM_Event.ID_Status >= 90
			and BM_Event.ID_HomeTeam=@ID_Team
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Tip_CalcForm')
begin
  drop function BM_Tip_CalcForm
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[BM_Tip_CalcForm]
(
	@LastForm decimal,
	@SeasonForm decimal,
	@LastGiven decimal,
	@SeasonGiven decimal,
	@LastTaken decimal,
	@SeasonTaken decimal,
	@SeasonCount decimal
)
RETURNS int -- dbo.BM_Tip_CalcForm(AwayLastForm, AwaySeasonForm, AwayLastGiven, AwaySeasonGiven, AwayLastTaken, AwaySeasonTaken, AwaySeasonCount)
AS
BEGIN
	return 
	(
		@LastForm
		+
		dbo.BM_Tip_Sigmoid((@LastGiven/7.0), 1.1, 3.0)*100
		+
		dbo.BM_Tip_Sigmoid((@LastTaken/7.0), 1.1, 3.0)*100

		+
		@SeasonForm
		+
		dbo.BM_Tip_Sigmoid((@SeasonGiven/@SeasonCount), 1.1, 3.0)*100
		+
		dbo.BM_Tip_Sigmoid((@SeasonTaken/@SeasonCount), 1.1, 3.0)*100
	) / 3.0
	---- forma za posledních 7 zápasů
	--1.25/2.0 *(
	--1.0/3.0 * @LastForm
	--+
	---- utok
	--1.0/3.0 * (((@LastGiven/7.0)/1.54) * 50)
	--+
	---- obrana
	--1.0/3.0 * (154/(1.0+(@LastTaken/7.0))))
	--+
	--0.75/2.0 *(
	---- sezóny
	---- forma
	--1.0/3.0 * @SeasonForm
	--+
	---- utok
	--1.0/3.0 * (((@SeasonGiven/@SeasonCount)/1.54) * 50)
	--+
	---- obrana
	--1.0/3.0 * (154/(1.0+(@SeasonTaken/@SeasonCount))))

END


GO

if exists (select * from sysobjects where name='BM_Tip_Sigmoid')
begin
  drop function BM_Tip_Sigmoid
end

GO


-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 26.10.2016
-- Description:	Pseudosigmoid, center je posun středu, speed je rychlost 
-- =============================================
CREATE FUNCTION BM_Tip_Sigmoid
(
	@Value decimal(9,2),
	@Center decimal(9,2),
	@Speed decimal(9,2)
)
RETURNS decimal(9,2)
AS
BEGIN
	RETURN (1.0/(1.0 + exp(((@Value - @Center)* -@Speed))))
END


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_SeasonEvent_BackUp')
begin
  drop function BM_Event_DETAIL_SeasonEvent_BackUp
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 18.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_SeasonEvent_BackUp]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull(((100.0/count(*)) * sum(LastEvent.Form)),0) as Form, IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken, count(*) as [Count] from 
	(
		select BM_EventOriginal.ID, 
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScore else BM_Event.HomeScore end) as Form, 
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end) as Given,
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end) as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			and BM_Event.ID < BM_EventOriginal.ID 
			and BM_Event.ID_Season=BM_EventOriginal.ID_Season
			and BM_Event.ID_Tournament=BM_EventOriginal.ID_Tournament
			and BM_Event.ID_Status >= 90
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Event_DETAIL_SeasonEvent_Bad')
begin
  drop function BM_Event_DETAIL_SeasonEvent_Bad
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 27.10.2016
-- Description:	Dopočítání formy, střelený a obdržený branky
-- Původní chybný!
-- =============================================
CREATE FUNCTION [dbo].[BM_Event_DETAIL_SeasonEvent_Bad]
(	
	@ID_Event int,
	@ID_Team int
)
RETURNS TABLE 
AS
RETURN 
(
	-- Add the SELECT statement with parameter references here
	select LastEvent.ID, IsNull(((100.0/count(*)) * sum(LastEvent.Form)),0) as Form, IsNull(sum(LastEvent.Given),0) as Given, IsNull(sum(LastEvent.Taken),0) as Taken, count(*) as [Count] from 
	(
		select BM_EventOriginal.ID, 
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScore else BM_Event.HomeScore end) as Form, 
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end) as Given,
		(case when BM_Event.ID_AwayTeam=@ID_Team then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end) as Taken
		from BM_Event BM_EventOriginal
			cross join BM_EventView BM_Event
		where BM_EventOriginal.ID=@ID_Event 
			--and BM_Event.ID < BM_EventOriginal.ID 
			and BM_Event.ID_Season=BM_EventOriginal.ID_Season
			and BM_Event.ID_Tournament=BM_EventOriginal.ID_Tournament
			and BM_Event.ID_Status >= 90
			and (BM_Event.ID_AwayTeam=@ID_Team OR BM_Event.ID_HomeTeam=@ID_Team)
	) LastEvent
	group by LastEvent.ID
)


GO

if exists (select * from sysobjects where name='BM_Genetic_ALL_Score')
begin
  drop function BM_Genetic_ALL_Score
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[BM_Genetic_ALL_Score]
(	
	@Form int,
	@Odd decimal(9,2),
	@DateFrom date,
	@DateTo date,
	@limit int,
	@LastFormKoef decimal(9,2),
	@SeasonFormKoef decimal(9,2)
)
RETURNS TABLE 
AS
RETURN 
(
	select 
		BM_EventExtend.ID,
		BM_EventExtend.DisplayName,
		BM_EventExtend.DateStart,
		BM_EventExtend.Form,
		BM_EventExtend.Odd,
		BM_EventExtend.WinnerCode,
		(case when BM_EventExtend.Form > @limit then 1 else (case when BM_EventExtend.Form < @limit*-1 then 2 else 3 end) end) as Bet
	from 
	(
		select 
			BM_Event.ID,
			BM_Event.DisplayName,
			BM_Event.DateStart,
			(case when dbo.BM_Genetic_Score(HomeLastForm,HomeSeasonForm,@LastFormKoef,@SeasonFormKoef) > 
			dbo.BM_Genetic_Score(AwayLastForm,AwaySeasonForm,@LastFormKoef,@SeasonFormKoef) 
			then (dbo.BM_Genetic_Score(HomeLastForm,HomeSeasonForm,@LastFormKoef,@SeasonFormKoef) - 
			dbo.BM_Genetic_Score(AwayLastForm,AwaySeasonForm,@LastFormKoef,@SeasonFormKoef)) 
			else (dbo.BM_Genetic_Score(AwayLastForm,AwaySeasonForm,@LastFormKoef,@SeasonFormKoef) - 
			dbo.BM_Genetic_Score(HomeLastForm,HomeSeasonForm,@LastFormKoef,@SeasonFormKoef)) * -1 end) as Form,
			(case when dbo.BM_Genetic_Score(HomeLastForm,HomeSeasonForm,@LastFormKoef,@SeasonFormKoef) > 
			dbo.BM_Genetic_Score(AwayLastForm,AwaySeasonForm,@LastFormKoef,@SeasonFormKoef) then FirstValue else SecondValue end) as Odd,
			(case when BM_Event.WinnerCode=0 then 0 else 
			(case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 
			else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode
		from BM_Tip
		inner join BM_Event on BM_Event.ID=BM_Tip.ID
		inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
		inner join BM_Category on BM_Category.ID=BM_Event.ID_Category
		where BM_Category.ID_Sport=1
			and (BM_Event.DateStart >= @DateFrom)
			and (BM_Event.DateStart < @DateTo)
	) BM_EventExtend
	where abs(BM_EventExtend.Form) >= @Form
		and BM_EventExtend.Odd >= @Odd
)


GO

if exists (select * from sysobjects where name='BM_Genetic_ALL_Score_Extend')
begin
  drop function BM_Genetic_ALL_Score_Extend
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[BM_Genetic_ALL_Score_Extend]
(	
	@Form int,
	@Odd decimal(9,2),
	@DateFrom date,
	@DateTo date,
	@limit int,
	@LastFormKoef decimal(9,2),
	@SeasonFormKoef decimal(9,2),
	@LastGivenKoeficient decimal(9,2),
	@LastTakenKoeficient decimal(9,2),
	@SeasonGivenKoeficient decimal(9,2),
	@SeasonTakenKoeficient decimal(9,2)
)
RETURNS TABLE 
AS
RETURN 
(
	select 
		BM_EventExtend.ID,
		BM_EventExtend.DisplayName,
		BM_EventExtend.DateStart,
		BM_EventExtend.Form,
		BM_EventExtend.Odd,
		BM_EventExtend.WinnerCode,
		(case when BM_EventExtend.Form > @limit then 1 else (case when BM_EventExtend.Form < @limit*-1 then 2 else 3 end) end) as Bet
	from 
	(
		select 
			BM_Event.ID,
			BM_Event.DisplayName,
			BM_Event.DateStart,
			(case when dbo.BM_Genetic_Score_Extend(HomeLastForm,HomeSeasonForm,HomeLastGiven,HomeLastTaken,HomeSeasonGiven,HomeSeasonTaken,HomeSeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient) > 
			dbo.BM_Genetic_Score_Extend(AwayLastForm,AwaySeasonForm,AwayLastGiven,AwayLastTaken,AwaySeasonGiven,AwaySeasonTaken,AwaySeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient) 
			then (dbo.BM_Genetic_Score_Extend(HomeLastForm,HomeSeasonForm,HomeLastGiven,HomeLastTaken,HomeSeasonGiven,HomeSeasonTaken,HomeSeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient) - 
			dbo.BM_Genetic_Score_Extend(AwayLastForm,AwaySeasonForm,AwayLastGiven,AwayLastTaken,AwaySeasonGiven,AwaySeasonTaken,AwaySeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient)) 
			else (dbo.BM_Genetic_Score_Extend(AwayLastForm,AwaySeasonForm,AwayLastGiven,AwayLastTaken,AwaySeasonGiven,AwaySeasonTaken,AwaySeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient) - 
			dbo.BM_Genetic_Score_Extend(HomeLastForm,HomeSeasonForm,HomeLastGiven,HomeLastTaken,HomeSeasonGiven,HomeSeasonTaken,HomeSeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient)) * -1 end) as Form,
			(case when dbo.BM_Genetic_Score_Extend(HomeLastForm,HomeSeasonForm,HomeLastGiven,HomeLastTaken,HomeSeasonGiven,HomeSeasonTaken,HomeSeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient) > 
			dbo.BM_Genetic_Score_Extend(AwayLastForm,AwaySeasonForm,AwayLastGiven,AwayLastTaken,AwaySeasonGiven,AwaySeasonTaken,AwaySeasonCount,@LastFormKoef,@SeasonFormKoef,@LastGivenKoeficient,@LastTakenKoeficient,@SeasonGivenKoeficient,@SeasonTakenKoeficient) then FirstValue else SecondValue end) as Odd,
			(case when BM_Event.WinnerCode=0 then 0 else 
			(case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 
			else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode
		from BM_Tip
		inner join BM_Event on BM_Event.ID=BM_Tip.ID
		inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
		inner join BM_Category on BM_Category.ID=BM_Event.ID_Category
		where BM_Category.ID_Sport=1
			and (BM_Event.DateStart >= @DateFrom)
			and (BM_Event.DateStart < @DateTo)
	) BM_EventExtend
	where abs(BM_EventExtend.Form) >= @Form
		and BM_EventExtend.Odd >= @Odd
)


GO

if exists (select * from sysobjects where name='BM_Genetic_DETAIL_Price')
begin
  drop function BM_Genetic_DETAIL_Price
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[BM_Genetic_DETAIL_Price]
(
	@Form int,
	@Odd decimal(9,2),
	@DateFrom date,
	@DateTo date,
	@Limit int,
	@Price int,
	@LastFormKoef decimal(9,2),
	@SeasonFormKoef decimal(9,2)
)
RETURNS decimal(9,2)
AS
BEGIN
	return (select sum(((case when WinnerCode=Bet then (Odd * @price) else 0 end) - @price)) as Price from dbo.BM_Genetic_ALL_Score(@Form, @Odd, @DateFrom,@DateTo, @limit, @LastFormKoef, @SeasonFormKoef) x)

END


GO

if exists (select * from sysobjects where name='BM_Genetic_DETAIL_Price_Extend')
begin
  drop function BM_Genetic_DETAIL_Price_Extend
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[BM_Genetic_DETAIL_Price_Extend]
(
	@Form int,
	@Odd decimal(9,2),
	@DateFrom date,
	@DateTo date,
	@Limit int,
	@Price int,
	@LastFormKoef decimal(9,2),
	@SeasonFormKoef decimal(9,2),
	@LastGivenKoef decimal(9,2),
	@LastTakenKoef decimal(9,2),
	@SeasonGivenKoef decimal(9,2),
	@SeasonTakenKoef decimal(9,2)
)
RETURNS decimal(9,2)
AS
BEGIN
	return (select sum(((case when WinnerCode=Bet then (Odd * @price) else 0 end) - @price)) as Price 
	from dbo.BM_Genetic_ALL_Score_Extend(@Form, @Odd, @DateFrom,@DateTo, @limit, @LastFormKoef, @SeasonFormKoef,@LastGivenKoef,@LastTakenKoef,@SeasonGivenKoef,@SeasonTakenKoef) x)

END


GO

if exists (select * from sysobjects where name='BM_Genetic_Score')
begin
  drop function BM_Genetic_Score
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION BM_Genetic_Score
(
	@LastForm decimal(9,2),
	@SeasonForm decimal(9,2),
	@LastKoeficient decimal(9,2),
	@SeasonKoeficient decimal(9,2)
)
RETURNS decimal(9,2)
AS
BEGIN

	-- Return the result of the function
	RETURN 
	(
		(@LastForm * @LastKoeficient) + (@SeasonForm * @SeasonKoeficient)
	)

END


GO

if exists (select * from sysobjects where name='BM_Genetic_Score_Extend')
begin
  drop function BM_Genetic_Score_Extend
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION BM_Genetic_Score_Extend
(
	@LastForm decimal(9,2),
	@SeasonForm decimal(9,2),
	@LastGiven decimal(9,2),
	@LastTaken decimal(9,2),
	@SeasonGiven decimal(9,2),
	@SeasonTaken decimal(9,2),
	@SeasonCount decimal(9,2),
	-- koeficienty
	@LastKoeficient decimal(9,2),
	@SeasonKoeficient decimal(9,2),
	@LastGivenKoeficient decimal(9,2),
	@LastTakenKoeficient decimal(9,2),
	@SeasonGivenKoeficient decimal(9,2),
	@SeasonTakenKoeficient decimal(9,2)
)
RETURNS decimal(9,2)
AS
BEGIN

	-- Return the result of the function
	RETURN 
	(
		(@LastForm * @LastKoeficient) + 
		(@SeasonForm * @SeasonKoeficient) + 
		(dbo.BM_Tip_Sigmoid((@LastGiven/7.0), 1.1, 3.0)*100 * @LastGivenKoeficient) +
		(dbo.BM_Tip_Sigmoid((@LastTaken/7.0), 1.1, 3.0)*100 * @LastTakenKoeficient) + 
		(@SeasonForm) +
		(dbo.BM_Tip_Sigmoid((@SeasonGiven/@SeasonCount), 1.1, 3.0)*100 * @SeasonGivenKoeficient) + 
		(dbo.BM_Tip_Sigmoid((@SeasonTaken/@SeasonCount), 1.1, 3.0)*100 * @SeasonTakenKoeficient)
	)

END


GO

print 'CurrentTime: Functions after views - ' + convert(varchar, getdate(), 120)

GO

print 'CurrentTime: Data - ' + convert(varchar, getdate(), 120)

GO

print 'CurrentTime: Delete data - ' + convert(varchar, getdate(), 120)

GO

-- Create constraints

print 'CurrentTime: Constraints: Check - ' + convert(varchar, getdate(), 120)

GO

alter table [BM_Category] ADD CONSTRAINT [DF_BM_Category_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Category] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Category] ADD CONSTRAINT [DF_BM_Category_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Category] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [BM_Event] ADD CONSTRAINT [DF_BM_Event_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Event] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Event] ADD CONSTRAINT [DF_BM_Event_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Event] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [BM_ImportData] ADD CONSTRAINT [DF_BM_ImportData_IsProcessed] DEFAULT (((0))) FOR [IsProcessed]

GO

update [BM_ImportData] set [IsProcessed]=((0)) where [IsProcessed] is null

GO

alter table [BM_OddsRegular] ADD CONSTRAINT [DF_OddsRegular_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_OddsRegular] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Score] ADD CONSTRAINT [DF_BM_Score_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Score] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Season] ADD CONSTRAINT [DF_BM_Season_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Season] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Season] ADD CONSTRAINT [DF_BM_Season_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Season] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [BM_Sport] ADD CONSTRAINT [DF_BM_Sport_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Sport] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Sport] ADD CONSTRAINT [DF_BM_Sport_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Sport] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [BM_Status] ADD CONSTRAINT [DF_BM_Status_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Status] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Status] ADD CONSTRAINT [DF_BM_Status_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Status] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [BM_Team] ADD CONSTRAINT [DF_BM_Team_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Team] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Team] ADD CONSTRAINT [DF_BM_Team_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Team] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [BM_Tournament] ADD CONSTRAINT [DF_BM_Tournament_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [BM_Tournament] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [BM_Tournament] ADD CONSTRAINT [DF_BM_Tournament_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [BM_Tournament] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [CR_User] ADD CONSTRAINT [DF_CR_User_DateCreated] DEFAULT ((getdate())) FOR [DateCreated]

GO

update [CR_User] set [DateCreated]=(getdate()) where [DateCreated] is null

GO

alter table [CR_User] ADD CONSTRAINT [DF_CR_User_DateUpdated] DEFAULT ((getdate())) FOR [DateUpdated]

GO

update [CR_User] set [DateUpdated]=(getdate()) where [DateUpdated] is null

GO

alter table [CR_User] ADD CONSTRAINT [DF_CR_User_Form] DEFAULT (((30))) FOR [Form]

GO

update [CR_User] set [Form]=((30)) where [Form] is null

GO

alter table [CR_User] ADD CONSTRAINT [DF_CR_User_IsActive] DEFAULT (((1))) FOR [IsActive]

GO

update [CR_User] set [IsActive]=((1)) where [IsActive] is null

GO

alter table [CR_User] ADD CONSTRAINT [DF_CR_User_Odd] DEFAULT (((2.0))) FOR [Odd]

GO

update [CR_User] set [Odd]=((2.0)) where [Odd] is null

GO

print 'CurrentTime: Constraints: Default - ' + convert(varchar, getdate(), 120)

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Category] alter column [ID] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Category] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
    alter table [BM_Category] alter column [Slug] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Category] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='ID_Sport' and IS_NULLABLE='YES')
    alter table [BM_Category] alter column [ID_Sport] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Category' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Category] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [ID] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [Slug] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='CustomId' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [CustomId] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateStart' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [DateStart] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_HomeTeam' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [ID_HomeTeam] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_AwayTeam' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [ID_AwayTeam] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Tournament' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [ID_Tournament] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Category' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [ID_Category] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='ID_Status' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [ID_Status] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Event' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Event] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_ImportData] alter column [ID] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportData' and COLUMN_NAME='IsProcessed' and IS_NULLABLE='YES')
    alter table [BM_ImportData] alter column [IsProcessed] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_ImportDataOld] alter column [ID] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_ImportDataOld' and COLUMN_NAME='IsProcessed' and IS_NULLABLE='YES')
    alter table [BM_ImportDataOld] alter column [IsProcessed] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='ID_Event' and IS_NULLABLE='YES')
    alter table [BM_OddsRegular] alter column [ID_Event] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='Type' and IS_NULLABLE='YES')
    alter table [BM_OddsRegular] alter column [Type] nvarchar(50) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_OddsRegular' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_OddsRegular] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='ID_Event' and IS_NULLABLE='YES')
    alter table [BM_Score] alter column [ID_Event] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='HomeScoreCurrent' and IS_NULLABLE='YES')
    alter table [BM_Score] alter column [HomeScoreCurrent] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='AwayScoreCurrent' and IS_NULLABLE='YES')
    alter table [BM_Score] alter column [AwayScoreCurrent] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Score' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Score] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Season] alter column [ID] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Season] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
    alter table [BM_Season] alter column [Slug] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Season] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='Year' and IS_NULLABLE='YES')
    alter table [BM_Season] alter column [Year] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Season' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Season] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Sport] alter column [ID] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Sport] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
    alter table [BM_Sport] alter column [Slug] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Sport] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Sport' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Sport] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Status] alter column [ID] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Status] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Status] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Status' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Status] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Team] alter column [ID] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Team] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
    alter table [BM_Team] alter column [Slug] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Team] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Team' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Team] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [BM_Tournament] alter column [ID] bigint NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DisplayName' and IS_NULLABLE='YES')
    alter table [BM_Tournament] alter column [DisplayName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='Slug' and IS_NULLABLE='YES')
    alter table [BM_Tournament] alter column [Slug] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [BM_Tournament] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='BM_Tournament' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [BM_Tournament] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='ID' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [ID] int NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='IsActive' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [IsActive] bit NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='UserName' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [UserName] nvarchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Password' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [Password] varchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Salt' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [Salt] varchar(255) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='DateCreated' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [DateCreated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='DateUpdated' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [DateUpdated] datetime NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Odd' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [Odd] decimal (9, 2) NOT NULL

GO

if exists(select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME='CR_User' and COLUMN_NAME='Form' and IS_NULLABLE='YES')
    alter table [CR_User] alter column [Form] int NOT NULL

GO

print 'CurrentTime: Constraints: NotNull - ' + convert(varchar, getdate(), 120)

GO

if not exists(select * from sys.indexes where name='PK_BM_Team' and is_primary_key=1)
  alter table [BM_Team] ADD CONSTRAINT [PK_BM_Team] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_BM_Event' and is_primary_key=1)
  alter table [BM_Event] ADD CONSTRAINT [PK_BM_Event] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_CR_User' and is_primary_key=1)
  alter table [CR_User] ADD CONSTRAINT [PK_CR_User] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_OddsRegular' and is_primary_key=1)
  alter table [BM_OddsRegular] ADD CONSTRAINT [PK_OddsRegular] PRIMARY KEY ([ID_Event])

GO

if not exists(select * from sys.indexes where name='PK_BM_Category' and is_primary_key=1)
  alter table [BM_Category] ADD CONSTRAINT [PK_BM_Category] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_BM_Sport' and is_primary_key=1)
  alter table [BM_Sport] ADD CONSTRAINT [PK_BM_Sport] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_BM_Score' and is_primary_key=1)
  alter table [BM_Score] ADD CONSTRAINT [PK_BM_Score] PRIMARY KEY ([ID_Event])

GO

if not exists(select * from sys.indexes where name='PK_BM_Tournament' and is_primary_key=1)
  alter table [BM_Tournament] ADD CONSTRAINT [PK_BM_Tournament] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_BM_Status' and is_primary_key=1)
  alter table [BM_Status] ADD CONSTRAINT [PK_BM_Status] PRIMARY KEY ([ID])

GO

if not exists(select * from sys.indexes where name='PK_BM_Season' and is_primary_key=1)
  alter table [BM_Season] ADD CONSTRAINT [PK_BM_Season] PRIMARY KEY ([ID])

GO

print 'CurrentTime: Constraints: Primary Keys - ' + convert(varchar, getdate(), 120)

GO

print 'CurrentTime: Constraints - ' + convert(varchar, getdate(), 120)

GO

-- Create computed columns

print 'CurrentTime: ColumnsComputed - ' + convert(varchar, getdate(), 120)

GO

-- Create Description

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID kategorie' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID kategorie' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'Slug')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'Slug'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'Slug'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'ID_Sport')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Sport' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'ID_Sport'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sport' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'ID_Sport'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Category' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Category', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'Slug')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'Slug'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'Slug'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'CustomId')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jiné ID' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'CustomId'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jiné ID' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'CustomId'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'FirstToServe')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda první podává (tenis)' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'FirstToServe'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda první podává (tenis)' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'FirstToServe'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'HasDraw')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda byla plichta' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'HasDraw'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda byla plichta' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'HasDraw'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'WinnerCode')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Kód vítěze' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'WinnerCode'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kód vítěze' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'WinnerCode'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'DateStart')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Začátek zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DateStart'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Začátek zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DateStart'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'Changes')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum poslední změny' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'Changes'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum poslední změny' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'Changes'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID_HomeTeam')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Domácí' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_HomeTeam'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Domácí' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_HomeTeam'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID_AwayTeam')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Hosté' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_AwayTeam'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hosté' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_AwayTeam'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID_Tournament')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Turnaj' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Tournament'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Turnaj' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Tournament'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID_Season')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Sezóna' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Season'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sezóna' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Season'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID_Category')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Kategorie' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Category'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kategorie' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Category'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'ID_Status')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Status' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Status'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Status' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'ID_Status'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'StatusDescription')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Popis statusu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'StatusDescription'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Popis statusu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'StatusDescription'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'HomeScoreCurrent')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Domácí - skóre' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'HomeScoreCurrent'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Domácí - skóre' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'HomeScoreCurrent'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'AwayScoreCurrent')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Hosté - skóre' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'AwayScoreCurrent'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Hosté - skóre' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'AwayScoreCurrent'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Event' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Event', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'ID_Event')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'ID_Event'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'ID_Event'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'Type')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Typ' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'Type'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Typ' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'Type'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'FirstId')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'ID kurzu - domácí' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'FirstId'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID kurzu - domácí' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'FirstId'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'FirstValue')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Kurz - domácí' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'FirstValue'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kurz - domácí' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'FirstValue'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'HasFirstWin')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda domácí vyhráli' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'HasFirstWin'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda domácí vyhráli' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'HasFirstWin'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'XId')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'ID kurzu - remíza' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'XId'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID kurzu - remíza' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'XId'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'XValue')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Kurz - remíza' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'XValue'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kurz - remíza' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'XValue'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'HasXWin')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda byla remíza' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'HasXWin'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda byla remíza' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'HasXWin'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'SecondId')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'ID kurzu - hosté' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'SecondId'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ID kurzu - hosté' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'SecondId'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'SecondValue')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Kurz - hosté' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'SecondValue'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kurz - hosté' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'SecondValue'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'HasSecondWin')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda hosté vyhráli' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'HasSecondWin'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda hosté vyhráli' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'HasSecondWin'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_OddsRegular' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_OddsRegular', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Score' and c.name = 'ID_Event')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Score', @level2type=N'COLUMN',@level2name=N'ID_Event'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID zápasu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Score', @level2type=N'COLUMN',@level2name=N'ID_Event'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Score' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Score', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Score', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Score' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Score', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Score', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID sezóny' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID sezóny' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'Slug')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'Slug'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'Slug'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'Year')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Rok' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'Year'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Rok' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'Year'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Season' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Season', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Sport' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID sportu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID sportu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Sport' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Sport' and c.name = 'Slug')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'Slug'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'Slug'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Sport' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Sport' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Sport' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Sport', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Status' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID statusu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID statusu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Status' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název/Typ' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název/Typ' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Status' and c.name = 'Description')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Popis' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'Description'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Popis' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'Description'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Status' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Status' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Status' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Status', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID týmu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID týmu' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'Slug')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'Slug'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'Slug'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'Gender')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Pohlaví' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'Gender'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Pohlaví' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'Gender'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Team' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Team', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID turnaje' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID turnaje' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'DisplayName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'DisplayName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Název' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'DisplayName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'Slug')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'Slug'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zkratka' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'Slug'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'UniqueID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Unikátní ID' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'UniqueID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Unikátní ID' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'UniqueID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'ID_Category')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Kategorie' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'ID_Category'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Kategorie' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'ID_Category'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'BM_Tournament' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'BM_Tournament', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'ID')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID uživatele' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'ID'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Jedinečné ID uživatele' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'ID'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'IsActive')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'IsActive'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Zda je aktivní' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'IsActive'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'UserName')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Uživatelské jméno' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'UserName'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Uživatelské jméno' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'UserName'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'Password')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Heslo' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Password'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Heslo' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Password'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'Salt')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Sůl' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Salt'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sůl' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Salt'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'LastLogin')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum posledního přihlášení' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'LastLogin'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum posledního přihlášení' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'LastLogin'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'DateCreated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'DateCreated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum vytvoření' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'DateCreated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'DateUpdated')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'DateUpdated'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Datum úpravy' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'DateUpdated'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'Odd')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Výchozí kurz' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Odd'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Výchozí kurz' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Odd'

GO

if exists(select *
        from sys.columns c inner join sys.extended_properties ex ON  ex.major_id = c.object_id and c.column_id=ex.minor_id
        where OBJECT_NAME(c.object_id) = 'CR_User' and c.name = 'Form')
    EXEC sys.sp_updateextendedproperty @name=N'MS_Description', @value=N'Výchozí forma' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Form'
else
    EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Výchozí forma' , @level0type=N'SCHEMA', @level0name=N'dbo', @level1type=N'TABLE', @level1name=N'CR_User', @level2type=N'COLUMN',@level2name=N'Form'

GO

print 'CurrentTime: Description - ' + convert(varchar, getdate(), 120)

GO

-- Create triggers

print 'CurrentTime: Triggers - ' + convert(varchar, getdate(), 120)

GO

-- Create foreign keys

alter table [BM_Event] ADD CONSTRAINT [FK_BM_Event_BM_TeamHome] FOREIGN KEY ([ID_HomeTeam]) REFERENCES [BM_Team]([ID])

GO

alter table [BM_Event] ADD CONSTRAINT [FK_BM_Event_BM_TeamAway] FOREIGN KEY ([ID_AwayTeam]) REFERENCES [BM_Team]([ID])

GO

alter table [BM_OddsRegular] ADD CONSTRAINT [FK_OddsRegular_BM_Event] FOREIGN KEY ([ID_Event]) REFERENCES [BM_Event]([ID])

GO

alter table [BM_Score] ADD CONSTRAINT [FK_BM_Score_BM_Event] FOREIGN KEY ([ID_Event]) REFERENCES [BM_Event]([ID])

GO

alter table [BM_Event] ADD CONSTRAINT [FK_BM_Event_BM_Category] FOREIGN KEY ([ID_Category]) REFERENCES [BM_Category]([ID])

GO

alter table [BM_Tournament] ADD CONSTRAINT [FK_BM_Tournament_BM_Category] FOREIGN KEY ([ID_Category]) REFERENCES [BM_Category]([ID])

GO

alter table [BM_Category] ADD CONSTRAINT [FK_BM_Category_BM_Sport] FOREIGN KEY ([ID_Sport]) REFERENCES [BM_Sport]([ID])

GO

alter table [BM_Event] ADD CONSTRAINT [FK_BM_Event_BM_Tournament] FOREIGN KEY ([ID_Tournament]) REFERENCES [BM_Tournament]([ID])

GO

alter table [BM_Event] ADD CONSTRAINT [FK_BM_Event_BM_Status] FOREIGN KEY ([ID_Status]) REFERENCES [BM_Status]([ID])

GO

alter table [BM_Event] ADD CONSTRAINT [FK_BM_Event_BM_Season] FOREIGN KEY ([ID_Season]) REFERENCES [BM_Season]([ID])

GO

print 'CurrentTime: ContraintsForeignKeys - ' + convert(varchar, getdate(), 120)

GO

-- Create/update procedures

if exists (select * from sysobjects where name='BM_Event_ALL_Tournament')
begin
  drop procedure BM_Event_ALL_Tournament
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 23.09.2016
-- Description: Prepared data for FANN..
-- =============================================
CREATE PROCEDURE [dbo].[BM_Event_ALL_Tournament]
	@ID_Tournament int,
	@ID_Season int = null
AS
BEGIN
	declare @true int = 1, @false int = 0
	select
		[BM_Event].ID,
		[BM_Event].DisplayName,
		[BM_OddsRegular].FirstValue,
		[BM_OddsRegular].XValue,
		[BM_OddsRegular].SecondValue,
		[BM_Event].ID_Category,
		[BM_Event].ID_Season,
		[BM_Event].[ID_HomeTeam],
		[BM_Event].[ID_AwayTeam],
		[BM_Event].[WinnerCode],

		'HomePoint'= IsNull((select sum (p.Points) from 
		(select top 6 (case when ((Points.WinnerCode=1 AND Points.ID_HomeTeam=[BM_Event].[ID_HomeTeam]) 
		OR (Points.WinnerCode=2 AND Points.ID_AwayTeam=[BM_Event].[ID_HomeTeam])) then 3 else 
		(case when (Points.WinnerCode=3) then 1 else 0 end) end) as Points
		from [BM_Event] Points where (Points.ID_HomeTeam=[BM_Event].[ID_HomeTeam] OR Points.ID_AwayTeam=[BM_Event].[ID_HomeTeam])
		AND Points.ID <[BM_Event].ID
		order by Points.DateStart desc) p), 0),

		'AwayPoint'= IsNull((select sum (p.Points) from 
		(select top 6 (case when ((Points.WinnerCode=1 AND Points.ID_HomeTeam=[BM_Event].[ID_AwayTeam]) 
		OR (Points.WinnerCode=2 AND Points.ID_AwayTeam=[BM_Event].[ID_AwayTeam])) then 3 else 
		(case when (Points.WinnerCode=3) then 1 else 0 end) end) as Points
		from [BM_Event] Points where (Points.ID_HomeTeam=[BM_Event].[ID_AwayTeam] OR Points.ID_AwayTeam=[BM_Event].[ID_AwayTeam])
		AND Points.ID <[BM_Event].ID
		order by Points.DateStart desc) p), 0),

		-- results
		'Home'=case when [BM_Event].[WinnerCode]=1 then @true else @false end,
		'Draw'=case when [BM_Event].[WinnerCode]=3 then @true else @false end,
		'Away'=case when [BM_Event].[WinnerCode]=2 then @true else @false end
	from [dbo].[BM_Event] 
	inner join [BM_Season] on [BM_Season].ID=[BM_Event].ID_Season
	left join [BM_OddsRegular] on [BM_OddsRegular].ID_Event=[BM_Event].ID
	where [BM_Event].ID_Tournament=@ID_Tournament 
	and (@ID_Season is null or BM_Event.ID_Season=@ID_Season)
	order by [BM_Event].[DateStart]
END


GO

if exists (select * from sysobjects where name='BM_Event_ALL_Tournament_Extend')
begin
  drop procedure BM_Event_ALL_Tournament_Extend
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 25.09.2016
-- Description: Prepared data for FANN..
-- =============================================
CREATE PROCEDURE [dbo].[BM_Event_ALL_Tournament_Extend]
	@ID_Tournament int,
	@ID_Season int,
	@Inverse bit = 0
AS
BEGIN
	declare @true int = 1, @false int = 0, @ID int

	declare @FootballTournament table (ID int, DateStart datetime, ID_Season int, ID_Tournament int,  
	HomePoints int, HomeSeasonPoints int, HomeScoreGiven int, HomeScoreTaken int, HomeForm int, HomeLastGiven int, HomeLastTaken int,
	AwayPoints int, AwaySeasonPoints int, AwayScoreGiven int, AwayScoreTaken int, AwayForm int, AwayLastGiven int, AwayLastTaken int,
	[HomeRound] int, [AwayRound] int, WinnerCode int)

	insert into @FootballTournament (ID, DateStart, ID_Tournament, ID_Season, HomePoints, AwayPoints, WinnerCode)
	select 
		BM_Event.ID,
		BM_Event.DateStart,
		BM_Event.ID_Tournament,
		BM_Event.ID_Season,
		BM_Event.HomePoints,
		BM_Event.AwayPoints,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode
	from BM_EventView BM_Event
	where BM_Event.ID_Tournament=@ID_Tournament AND (BM_Event.ID_Status=0 or BM_Event.ID_Status>=90)
	order by BM_Event.DateStart

	DECLARE x_cursor CURSOR FOR   
	select  ID from @FootballTournament BM_Event
	order by BM_Event.DateStart
  
	OPEN x_cursor  
  
	-- next
	FETCH NEXT FROM x_cursor INTO @ID
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		-- 
		declare @HomeScoreGiven int, @HomeSeasonPoints int, @HomeRound int, @HomeScoreTaken int, @HomeForm int, @HomeLastGiven int, @HomeLastTaken int,
		@AwayScoreGiven int, @AwaySeasonPoints int, @AwayRound int, @AwayScoreTaken int, @AwayForm int, @AwayLastGiven int, @AwayLastTaken int,
		@ID_HomeTeam int, @ID_AwayTeam int


		-- home
		select @HomeScoreGiven=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeScoreTaken=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeSeasonPoints=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then HomePoints else AwayPoints end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeRound=count(*) from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart<= BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select
			@ID_HomeTeam=ID_HomeTeam,
			@ID_AwayTeam=ID_AwayTeam,
			@HomeForm=0,
			@HomeLastGiven=0,
			@HomeLastTaken=0,
			@AwayForm=0,
			@AwayLastGiven=0,
			@AwayLastTaken=0
		from BM_Event
		where BM_Event.ID=@ID

		select 
			@HomeForm=LastHomeEvent.Form,
			@HomeLastGiven=LastHomeEvent.Given,
			@HomeLastTaken=LastHomeEvent.Taken
		from dbo.BM_Event_DETAIL_LastEvent(@ID, @ID_HomeTeam) LastHomeEvent

		select 
			@AwayForm=LastAwayEvent.Form,
			@AwayLastGiven=LastAwayEvent.Given,
			@AwayLastTaken=LastAwayEvent.Taken
		from dbo.BM_Event_DETAIL_LastEvent(@ID, @ID_AwayTeam) LastAwayEvent

		-- away
		select @AwayScoreGiven=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwayScoreTaken=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwaySeasonPoints=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then HomePoints else AwayPoints end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwayRound=count(*) from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart<= BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		update @FootballTournament 
		set [HomeScoreGiven]=@HomeScoreGiven, 
			[HomeScoreTaken]=@HomeScoreTaken,
			[HomeRound]=@HomeRound, 
			[HomeSeasonPoints]=@HomeSeasonPoints,
			[HomeLastGiven]=@HomeLastGiven,
			[HomeLastTaken]=@HomeLastTaken,
			[HomeForm]=@HomeForm,
			[AwayScoreGiven]=@AwayScoreGiven, 
			[AwayScoreTaken]=@AwayScoreTaken,
			[AwayRound]=@AwayRound, 
			[AwaySeasonPoints]=@AwaySeasonPoints,
			[AwayLastGiven]=@AwayLastGiven,
			[AwayLastTaken]=@AwayLastTaken,
			[AwayForm]=@AwayForm
		where ID=@ID
  
		-- next
		FETCH NEXT FROM x_cursor INTO @ID
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor; 


	--select BM_Event.DisplayName, BM_Season.DisplayName, x.*,
	--	-- results
	--	'Home'=case when [BM_Event].[WinnerCode]=1 then @true else @false end,
	--	'Draw'=case when [BM_Event].[WinnerCode]=3 then @true else @false end,
	--	'Away'=case when [BM_Event].[WinnerCode]=2 then @true else @false end
	--from @FootballTournament x
	--	inner join BM_Event on BM_Event.ID=x.ID
	--	inner join BM_Season on BM_Season.ID=x.ID_Season
	--where x.ID_Season=@ID_Season
	--order by x.ID_Season, x.DateStart

	select BM_Event.DisplayName, BM_Season.DisplayName, 
		BM_Event.ID_HomeTeam, BM_Event.ID_AwayTeam,
		x.*,
		-- odds
		[BM_OddsRegular].FirstValue,
		[BM_OddsRegular].XValue,
		[BM_OddsRegular].SecondValue,
		-- results
		'Home'=case when x.[WinnerCode]=1 then @true else @false end,
		'Draw'=case when x.[WinnerCode]=3 then @true else @false end,
		'Away'=case when x.[WinnerCode]=2 then @true else @false end
	from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		inner join BM_Season on BM_Season.ID=x.ID_Season
		left join [BM_OddsRegular] on [BM_OddsRegular].ID_Event=[BM_Event].ID
	where (x.ID_Season=@ID_Season and @Inverse=0) or (x.ID_Season<>@ID_Season and @Inverse=1)
	order by x.ID_Season, x.DateStart
END


GO

if exists (select * from sysobjects where name='BM_Event_ALL_Tournament_ExtendUnique')
begin
  drop procedure BM_Event_ALL_Tournament_ExtendUnique
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 25.09.2016
-- Description: Prepared data for FANN..
-- =============================================
CREATE PROCEDURE [dbo].[BM_Event_ALL_Tournament_ExtendUnique]
	@ID_Tournament int,
	@ID_Season int = null
AS
BEGIN
	declare @true int = 1, @false int = 0, @ID int

	declare @FootballTournament table (ID int, DateStart datetime, ID_Season int, ID_Tournament int,  
	HomePoints int, HomeSeasonPoints int, HomeScoreGiven int, HomeScoreTaken int, AwayPoints int, AwaySeasonPoints int, AwayScoreGiven int, AwayScoreTaken int,
	[HomeRound] int, [AwayRound] int, WinnerCode int)

	insert into @FootballTournament (ID, DateStart, ID_Tournament, ID_Season, HomePoints, AwayPoints, WinnerCode)
	select 
		BM_Event.ID,
		BM_Event.DateStart,
		BM_Event.ID_Tournament,
		BM_Event.ID_Season,
		BM_Event.HomePoints,
		BM_Event.AwayPoints,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode
	from BM_EventView BM_Event
	inner join BM_Tournament on BM_Tournament.ID=BM_Event.ID_Tournament
	where BM_Tournament.UniqueID=@ID_Tournament AND BM_Event.ID_Status in (0,100)
	order by BM_Event.DateStart

	DECLARE x_cursor CURSOR FOR   
	select  ID from @FootballTournament BM_Event
	order by BM_Event.DateStart
  
	OPEN x_cursor  
  
	-- next
	FETCH NEXT FROM x_cursor INTO @ID
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		-- 
		declare @HomeScoreGiven int, @HomeSeasonPoints int, @HomeRound int, @AwayScoreGiven int, @AwaySeasonPoints int, @AwayRound int, @AwayScoreTaken int, @HomeScoreTaken int

		-- home
		select @HomeScoreGiven=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeScoreTaken=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeSeasonPoints=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then HomePoints else AwayPoints end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeRound=count(*) from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart<= BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		-- away
		select @AwayScoreGiven=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		-- away
		select @AwayScoreTaken=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwaySeasonPoints=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then HomePoints else AwayPoints end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwayRound=count(*) from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart<= BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)
		
		update @FootballTournament 
		set [HomeScoreGiven]=@HomeScoreGiven, 
			[HomeScoreTaken]=@HomeScoreTaken,
			[HomeRound]=@HomeRound, 
			[HomeSeasonPoints]=@HomeSeasonPoints,
			[AwayScoreGiven]=@AwayScoreGiven, 
			[AwayScoreTaken]=@AwayScoreTaken,
			[AwayRound]=@AwayRound, 
			[AwaySeasonPoints]=@AwaySeasonPoints
		where ID=@ID
  
		-- next
		FETCH NEXT FROM x_cursor INTO @ID
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor; 


	--select BM_Event.DisplayName, BM_Season.DisplayName, x.*,
	--	-- results
	--	'Home'=case when [BM_Event].[WinnerCode]=1 then @true else @false end,
	--	'Draw'=case when [BM_Event].[WinnerCode]=3 then @true else @false end,
	--	'Away'=case when [BM_Event].[WinnerCode]=2 then @true else @false end
	--from @FootballTournament x
	--	inner join BM_Event on BM_Event.ID=x.ID
	--	inner join BM_Season on BM_Season.ID=x.ID_Season
	--where x.ID_Season=@ID_Season
	--order by x.ID_Season, x.DateStart

	select BM_Event.DisplayName, BM_Season.DisplayName, 
		BM_Event.ID_HomeTeam, BM_Event.ID_AwayTeam,
		x.*,
		-- odds
		[BM_OddsRegular].FirstValue,
		[BM_OddsRegular].XValue,
		[BM_OddsRegular].SecondValue,
		-- results
		'Home'=case when x.[WinnerCode]=1 then @true else @false end,
		'Draw'=case when x.[WinnerCode]=3 then @true else @false end,
		'Away'=case when x.[WinnerCode]=2 then @true else @false end
	from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		inner join BM_Season on BM_Season.ID=x.ID_Season
		left join [BM_OddsRegular] on [BM_OddsRegular].ID_Event=[BM_Event].ID
	where x.ID_Season=@ID_Season or @ID_Season is null -- in (10406, 8404)
	order by x.ID_Season, x.DateStart
END


GO

if exists (select * from sysobjects where name='BM_Event_ALL_Tournament_Score')
begin
  drop procedure BM_Event_ALL_Tournament_Score
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 03.10.2016
-- Description: Prepared data for FANN..
-- =============================================
CREATE PROCEDURE [dbo].[BM_Event_ALL_Tournament_Score]
	@ID_Tournament int,
	@ID_Season int = null
AS
BEGIN
	declare @true int = 1, @false int = 0, @ID int

	declare @FootballTournament table (ID int, DateStart datetime, ID_Season int, ID_Tournament int,  
	HomePoints int, HomeSeasonPoints int, HomeScoreGiven int, HomeScoreTaken int, AwayPoints int, AwaySeasonPoints int, AwayScoreGiven int, AwayScoreTaken int,
	[HomeRound] int, [AwayRound] int)

	insert into @FootballTournament (ID, DateStart, ID_Tournament, ID_Season, HomePoints, AwayPoints)
	select 
		BM_Event.ID,
		BM_Event.DateStart,
		BM_Event.ID_Tournament,
		BM_Event.ID_Season,
		BM_Event.HomePoints,
		BM_Event.AwayPoints
	from BM_EventView BM_Event
	where BM_Event.ID_Tournament=@ID_Tournament AND BM_Event.ID_Status=100
	order by BM_Event.DateStart

	DECLARE x_cursor CURSOR FOR   
	select  ID from @FootballTournament BM_Event
	order by BM_Event.DateStart
  
	OPEN x_cursor  
  
	-- next
	FETCH NEXT FROM x_cursor INTO @ID
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		-- 
		declare @HomeScoreGiven int, @HomeSeasonPoints int, @HomeRound int, @AwayScoreGiven int, @AwaySeasonPoints int, @AwayRound int, @AwayScoreTaken int, @HomeScoreTaken int

		-- home
		select @HomeScoreGiven=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeScoreTaken=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeSeasonPoints=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam then HomePoints else AwayPoints end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		select @HomeRound=count(*) from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart<= BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_HomeTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_HomeTeam)

		-- away
		select @AwayScoreGiven=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then BM_Event.HomeScoreCurrent else BM_Event.AwayScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		-- away
		select @AwayScoreTaken=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then BM_Event.AwayScoreCurrent else BM_Event.HomeScoreCurrent end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwaySeasonPoints=IsNull(sum(case when BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam then HomePoints else AwayPoints end), 0)
		from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart< BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)

		select @AwayRound=count(*) from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		cross join BM_Event BM_EventOriginal
		where BM_EventOriginal.ID=@ID AND x.ID_Season=BM_EventOriginal.ID_Season AND x.DateStart<= BM_EventOriginal.DateStart
		AND (BM_Event.ID_HomeTeam=BM_EventOriginal.ID_AwayTeam or BM_Event.ID_AwayTeam=BM_EventOriginal.ID_AwayTeam)
		
		update @FootballTournament 
		set [HomeScoreGiven]=@HomeScoreGiven, 
			[HomeScoreTaken]=@HomeScoreTaken,
			[HomeRound]=@HomeRound, 
			[HomeSeasonPoints]=@HomeSeasonPoints,
			[AwayScoreGiven]=@AwayScoreGiven, 
			[AwayScoreTaken]=@AwayScoreTaken,
			[AwayRound]=@AwayRound, 
			[AwaySeasonPoints]=@AwaySeasonPoints
		where ID=@ID
  
		-- next
		FETCH NEXT FROM x_cursor INTO @ID
	END   
	CLOSE x_cursor;  
	DEALLOCATE x_cursor; 


	--select BM_Event.DisplayName, BM_Season.DisplayName, x.*,
	--	-- results
	--	'Home'=case when [BM_Event].[WinnerCode]=1 then @true else @false end,
	--	'Draw'=case when [BM_Event].[WinnerCode]=3 then @true else @false end,
	--	'Away'=case when [BM_Event].[WinnerCode]=2 then @true else @false end
	--from @FootballTournament x
	--	inner join BM_Event on BM_Event.ID=x.ID
	--	inner join BM_Season on BM_Season.ID=x.ID_Season
	--where x.ID_Season=@ID_Season
	--order by x.ID_Season, x.DateStart

	select BM_Event.DisplayName, BM_Season.DisplayName, 
		BM_Event.ID_HomeTeam, BM_Event.ID_AwayTeam,
		x.*,
		-- odds
		[BM_OddsRegular].FirstValue,
		[BM_OddsRegular].XValue,
		[BM_OddsRegular].SecondValue,
		-- results
		'HomeScore'=[BM_Event].HomeScoreCurrent * 0.1,
		'AwayScore'=[BM_Event].AwayScoreCurrent * 0.1
	from @FootballTournament x
		inner join BM_Event on BM_Event.ID=x.ID
		inner join BM_Season on BM_Season.ID=x.ID_Season
		left join [BM_OddsRegular] on [BM_OddsRegular].ID_Event=[BM_Event].ID
	where x.ID_Season=@ID_Season or @ID_Season is null 
	order by x.ID_Season, x.DateStart
END


GO

if exists (select * from sysobjects where name='BM_ImportData_IMPORT')
begin
  drop procedure BM_ImportData_IMPORT
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 13.09.2016
-- Update date: 16.09.2016
-- Description:	Import dat z tabulky ImportData do ostatních tabulek
-- =============================================
CREATE PROCEDURE [dbo].[BM_ImportData_IMPORT]
AS
BEGIN
	-- sporty
	print 'sports'
	MERGE [dbo].[BM_Sport] AS [target]
	USING 
	(
		select distinct SportId, SportName, SportSlug from BM_ImportData
	) AS [source] (SportId, SportName, SportSlug)
	ON ([target].ID = [source].SportId)
	WHEN matched THEN
		UPDATE SET 
			[target].[DisplayName]=[source].SportName,
			[target].[Slug]=[source].SportSlug,
			[target].DateUpdated=getdate()
	when not matched then
		INSERT (ID, DisplayName, [Slug])
		VALUES ([source].SportId, [source].[SportName], [source].[SportSlug]);

	-- status
	print 'status'
	MERGE [dbo].[BM_Status] AS [target]
	USING 
	(
		select distinct StatusCode, StatusType from BM_ImportData
	) AS [source] (StatusCode, StatusType)
	ON ([target].ID = [source].StatusCode)
	WHEN matched THEN
		UPDATE SET 
			[target].[DisplayName]=[source].StatusType,
			[target].DateUpdated=getdate()
	when not matched then
		INSERT (ID, DisplayName)
		VALUES ([source].StatusCode, [source].[StatusType]);

	-- Category
	print 'Category'
	MERGE [dbo].[BM_Category] AS [target]
	USING 
	(
		select distinct CategoryId, CategoryName, CategorySlug, SportId from BM_ImportData
	) AS [source] (CategoryId, CategoryName, CategorySlug, SportId)
	ON ([target].ID = [source].CategoryId)
	WHEN matched THEN
		UPDATE SET 
			[target].[DisplayName]=[source].CategoryName,
			[target].[Slug]=[source].CategorySlug,
			[target].[ID_Sport]=[source].SportId,
			[target].DateUpdated=getdate()
	when not matched then
		INSERT (ID, DisplayName, [Slug], ID_Sport)
		VALUES ([source].CategoryId, [source].CategoryName, [source].CategorySlug, [source].SportId);

	-- Season
	print 'Season'
	MERGE [dbo].[BM_Season] AS [target]
	USING 
	(
		select distinct SeasonId, max(SeasonName), max(SeasonSlug), max(SeasonYear) 
		from BM_ImportData where SeasonId is not null group by SeasonId
	) AS [source] (SeasonId, SeasonName, SeasonSlug, SeasonYear)
	ON ([target].ID = [source].SeasonId)
	WHEN matched THEN
		UPDATE SET 
			[target].[DisplayName]=[source].SeasonName,
			[target].[Slug]=[source].SeasonSlug,
			[target].[Year]=[source].SeasonYear,
			[target].DateUpdated=getdate()
	when not matched then
		INSERT (ID, DisplayName, [Slug], [Year])
		VALUES ([source].SeasonId, [source].SeasonName, [source].SeasonSlug, [source].SeasonYear);

	-- Tournament
	print 'Tournament'
	MERGE [dbo].[BM_Tournament] AS [target]
	USING 
	(
		select TournamentId, max(TournamentName),
		TournamentSlug, TournamentUniqueId, CategoryId
		from BM_ImportData
		group by TournamentId, TournamentSlug, TournamentUniqueId, CategoryId
	) AS [source] (TournamentId, TournamentName, TournamentSlug, TournamentUniqueId, CategoryId)
	ON ([target].ID = [source].TournamentId)
	WHEN matched THEN
		UPDATE SET 
			[target].[DisplayName]=[source].TournamentName,
			[target].[Slug]=[source].TournamentSlug,
			[target].[UniqueId]=[source].TournamentUniqueId,
			[target].[ID_Category]=[source].CategoryId,
			[target].DateUpdated=getdate()
	when not matched then
		INSERT (ID, DisplayName, [Slug], [ID_Category], [UniqueId])
		VALUES ([source].TournamentId, [source].TournamentName, [source].TournamentSlug, [source].CategoryId, [source].TournamentUniqueId);

	-- Event
	print 'Event'
	DECLARE @db_cursor CURSOR,
		@Date datetime

	DECLARE db_cursor CURSOR FAST_FORWARD FOR  
	select distinct [Date] from BM_ImportData where IsProcessed=0 order by Date

	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @Date   

	WHILE @@FETCH_STATUS = 0   
	BEGIN		
		  print @Date

		  -- Team
			MERGE [dbo].[BM_Team] AS [target]
			USING 
			(
				select TeamId, max(Name) as Name, Slug, Gender
				from
				(
				select distinct HomeTeamId as TeamId, HomeTeamName as Name, HomeTeamSlug as Slug, HomeTeamGender as Gender  from BM_ImportData where [Date]=@Date
				union
				select distinct AwayTeamId as TeamId, AwayTeamName as Name, AwayTeamSlug as Slug, AwayTeamGender as Gender from BM_ImportData where [Date]=@Date
				) x
				group by TeamId, Slug, Gender
			) AS [source] (TeamId, Name, Slug, Gender)
			ON ([target].ID = [source].TeamId)
			WHEN matched THEN
				UPDATE SET 
					[target].[DisplayName]=[source].Name,
					[target].[Slug]=[source].Slug,
					[target].[Gender]=[source].Gender,
					[target].DateUpdated=getdate()
			when not matched then
				INSERT (ID, DisplayName, [Slug], [Gender])
				VALUES ([source].TeamId, [source].Name, [source].Slug, [source].Gender);
		
			print 'event'
		   -- Event
		   MERGE [dbo].[BM_Event] AS [target]
		   USING
			(
				--select distinct EventId, EventName, EventSlug, EventCustomId, EventFirstToServe, EventHasDraw, EventWinnerCode, 
				--convert(datetime, convert(nvarchar(10),EventStartDate) +' '+ convert(nvarchar(8), EventStartTime), 104) as EventDateStart,
				--convert(datetime, convert(nvarchar(19), EventChanges), 126) as [Changes],
				--HomeTeamId, AwayTeamId, TournamentId, SeasonId, CategoryId, StatusCode, StatusDescription, HomeScoreCurrent, AwayScoreCurrent
				--from BM_ImportData where [Date]=@Date
				select EventId, max(EventName) as EventName, EventSlug, EventCustomId, EventFirstToServe, EventHasDraw, max(EventWinnerCode) as EventWinnerCode, 
				max(convert(datetime, convert(nvarchar(10),EventStartDate) +' '+ convert(nvarchar(8), EventStartTime), 104)) as EventDateStart,
				max(convert(datetime, convert(nvarchar(19), EventChanges), 126)) as [Changes],
				HomeTeamId, AwayTeamId, TournamentId, SeasonId, CategoryId, max(StatusCode) as StatusCode, max(StatusDescription) as StatusDescription, 
				max(HomeScoreCurrent) as HomeScoreCurrent, max(AwayScoreCurrent) as AwayScoreCurrent
				from BM_ImportData where [Date]=@Date
				group by EventId, EventSlug, EventCustomId, EventFirstToServe, EventHasDraw, HomeTeamId, 
				AwayTeamId, TournamentId, SeasonId, CategoryId
			) AS [source]
			ON ([target].ID = [source].EventId)
			WHEN matched THEN
				UPDATE SET 
					[target].[DisplayName]=[source].EventName,
					[target].[Slug]=[source].EventSlug,
					[target].[CustomId]=[source].EventCustomId,
					[target].[FirstToServe]=[source].EventFirstToServe,
					[target].[HasDraw]=[source].EventHasDraw,
					[target].[WinnerCode]=[source].EventWinnerCode,
					[target].[DateStart]=[source].EventDateStart,
					[target].[ID_HomeTeam]=[source].HomeTeamId,
					[target].[ID_AwayTeam]=[source].AwayTeamId,
					[target].[ID_Tournament]=[source].TournamentId,
					[target].[ID_Season]=[source].SeasonId,
					[target].[ID_Status]=[source].StatusCode,
					[target].[ID_Category]=[source].CategoryId,
					[target].[StatusDescription]=[source].StatusDescription,
					[target].[HomeScoreCurrent]=[source].HomeScoreCurrent,
					[target].[AwayScoreCurrent]=[source].AwayScoreCurrent,
					[target].DateUpdated=getdate()
			when not matched then
				INSERT (ID, DisplayName, [Slug], [CustomId], [FirstToServe], [HasDraw], [WinnerCode], [DateStart], [ID_HomeTeam],[ID_AwayTeam],
				[ID_Tournament], [ID_Season], [ID_Status], [ID_Category], [StatusDescription], [HomeScoreCurrent], [AwayScoreCurrent])
				VALUES ([source].EventId, [source].EventName, [source].EventSlug, [source].EventCustomId, [source].EventFirstToServe, [source].EventHasDraw,
				[source].EventWinnerCode, [source].EventDateStart,[source].HomeTeamId,[source].AwayTeamId,[source].TournamentId,[source].SeasonId,
				[source].StatusCode,[source].CategoryId,[source].StatusDescription,[source].HomeScoreCurrent,[source].AwayScoreCurrent);
	   
		print 'regularOdds'
		   -- Regular Odds
		   MERGE [dbo].[BM_OddsRegular] AS [target]
		   USING
			(
				select EventId, case when max(OddsDoubleChangeXSecondValue) is null then '12' else '1X2' end as [Type], 
				max(OddsRegularFirstSourceId) as OddsRegularFirstSourceId, max(OddsRegularFirstValue) as OddsRegularFirstValue, MAX(CONVERT(int,OddsRegularFirstWining)) as OddsRegularFirstWining,
				max(OddsRegularXSourceId) as OddsRegularXSourceId, max(OddsRegularXValue) as OddsRegularXValue,  MAX(CONVERT(int,OddsRegularXWining)) as OddsRegularXWining,
				max(OddsRegularSecondSourceId) as OddsRegularSecondSourceId, max(OddsRegularSecondValue) as OddsRegularSecondValue, MAX(CONVERT(int,OddsRegularSecondWining)) as OddsRegularSecondWining
				from BM_ImportData where [Date]=@date and OddsRegularFirstSourceId is not null
				group by EventId
			) AS [source]
			ON ([target].ID_Event = [source].EventId)
			WHEN matched THEN
				UPDATE SET 
					[target].[Type]=[source].[Type],
					[target].[FirstId]=[source].OddsRegularFirstSourceId,
					[target].[FirstValue]=[source].OddsRegularFirstValue,
					[target].[HasFirstWin]=[source].OddsRegularFirstWining,
					[target].[XId]=[source].OddsRegularXSourceId,
					[target].[XValue]=[source].OddsRegularXValue,
					[target].[HasXWin]=[source].OddsRegularXWining,
					[target].[SecondId]=[source].OddsRegularSecondSourceId,
					[target].[SecondValue]=[source].OddsRegularSecondValue,
					[target].[HasSecondWin]=[source].OddsRegularSecondWining,
					[target].DateUpdated=getdate()
			when not matched then
				INSERT ([ID_Event], [Type], [FirstId], [FirstValue], [HasFirstWin], [XId], [XValue], [HasXWin], [SecondId], [SecondValue], [HasSecondWin])
				VALUES ([source].[EventId], [source].[Type], [source].OddsRegularFirstSourceId, [source].OddsRegularFirstValue, [source].OddsRegularFirstWining,
				[source].OddsRegularXSourceId,[source].OddsRegularXValue,[source].OddsRegularXWining,
				[source].OddsRegularSecondSourceId,[source].OddsRegularSecondValue,[source].OddsRegularSecondWining);

		   -- Score
		   print 'score'
		   MERGE [dbo].[BM_Score] AS [target]
		   USING
			(
				select EventId, max([HomeScoreCurrent]) as HomeScoreCurrent, max([HomeScorePeriod1]) as HomeScorePeriod1, 
				max([HomeScorePeriod2]) as HomeScorePeriod2, max([HomeScorePeriod3]) as HomeScorePeriod3, 
				max([HomeScoreNormaltime]) as HomeScoreNormaltime, max([HomeScoreOvertime]) as HomeScoreOvertime, max( [HomeScorePenalties]) as HomeScorePenalties,
				max([AwayScoreCurrent]) as AwayScoreCurrent, max ([AwayScorePeriod1]) AwayScorePeriod1, max( [AwayScorePeriod2]) as AwayScorePeriod2, max ([AwayScorePeriod3]) as AwayScorePeriod3, 
				max([AwayScoreNormaltime]) as AwayScoreNormaltime, max ([AwayScoreOvertime])  as AwayScoreOvertime, max([AwayScorePenalties]) as AwayScorePenalties
				from BM_ImportData where [Date]=@date and [HomeScoreCurrent] is not null
				group by EventId
			) AS [source]
			ON ([target].ID_Event = [source].EventId)
			WHEN matched THEN
				UPDATE SET 
					[target].[HomeScoreCurrent]=[source].[HomeScoreCurrent],
					[target].[HomeScorePeriod1]=[source].[HomeScorePeriod1],
					[target].[HomeScorePeriod2]=[source].[HomeScorePeriod2],
					[target].[HomeScorePeriod3]=[source].[HomeScorePeriod3],
					[target].[HomeScoreNormaltime]=[source].[HomeScoreNormaltime],
					[target].[HomeScoreOvertime]=[source].[HomeScoreOvertime],
					[target].[HomeScorePenalties]=[source].[HomeScorePenalties],
					[target].[AwayScoreCurrent]=[source].[AwayScoreCurrent],
					[target].[AwayScorePeriod1]=[source].[AwayScorePeriod1],
					[target].[AwayScorePeriod2]=[source].[AwayScorePeriod2],
					[target].[AwayScorePeriod3]=[source].[AwayScorePeriod3],
					[target].[AwayScoreNormaltime]=[source].[AwayScoreNormaltime],
					[target].[AwayScoreOvertime]=[source].[AwayScoreOvertime],
					[target].[AwayScorePenalties]=[source].[AwayScorePenalties],
					[target].DateUpdated=getdate()
			when not matched then
				INSERT ([ID_Event], [HomeScoreCurrent], [HomeScorePeriod1], [HomeScorePeriod2], [HomeScorePeriod3], [HomeScoreNormaltime], [HomeScoreOvertime], [HomeScorePenalties], 
				[AwayScoreCurrent], [AwayScorePeriod1], [AwayScorePeriod2], [AwayScorePeriod3], [AwayScoreNormaltime], [AwayScoreOvertime], [AwayScorePenalties])
				VALUES ([source].[EventId], [source].[HomeScoreCurrent], [source].[HomeScorePeriod1], [source].[HomeScorePeriod2], [source].[HomeScorePeriod3],
				[source].[HomeScoreNormaltime],[source].[HomeScoreOvertime],[source].[HomeScorePenalties],
				[source].[AwayScoreCurrent], [source].[AwayScorePeriod1], [source].[AwayScorePeriod2], [source].[AwayScorePeriod3],
				[source].[AwayScoreNormaltime],[source].[AwayScoreOvertime],[source].[AwayScorePenalties]);

		   -- edit processingu
		   update BM_ImportData set IsProcessed=1 where [Date]=@Date AND IsProcessed=0

		   FETCH NEXT FROM db_cursor INTO @Date  
	END   

	CLOSE db_cursor   
	DEALLOCATE db_cursor


END


GO

if exists (select * from sysobjects where name='BM_ImportData_OLD')
begin
  drop procedure BM_ImportData_OLD
end

GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE BM_ImportData_OLD
AS
BEGIN
	-- prenos
	insert into [dbo].[BM_ImportDataOld] 
(
		[Date]
      ,[SportName]
      ,[SportSlug]
      ,[SportId]
      ,[TournamentName]
      ,[TournamentSlug]
      ,[TournamentId]
      ,[TournamentUniqueId]
      ,[CategoryName]
      ,[CategorySlug]
      ,[CategoryId]
      ,[SeasonName]
      ,[SeasonSlug]
      ,[SeasonId]
      ,[SeasonYear]
      ,[EventId]
      ,[EventCustomId]
      ,[EventFirstToServe]
      ,[EventHasDraw]
      ,[EventWinnerCode]
      ,[EventName]
      ,[EventSlug]
      ,[EventStartDate]
      ,[EventStartTime]
      ,[EventChanges]
      ,[StatusCode]
      ,[StatusType]
      ,[StatusDescription]
      ,[HomeTeamId]
      ,[HomeTeamName]
      ,[HomeTeamSlug]
      ,[HomeTeamGender]
      ,[HomeScoreCurrent]
      ,[HomeScorePeriod1]
      ,[HomeScorePeriod2]
      ,[HomeScorePeriod3]
      ,[HomeScoreNormaltime]
      ,[HomeScoreOvertime]
      ,[HomeScorePenalties]
      ,[AwayTeamId]
      ,[AwayTeamName]
      ,[AwayTeamSlug]
      ,[AwayTeamGender]
      ,[AwayScoreCurrent]
      ,[AwayScorePeriod1]
      ,[AwayScorePeriod2]
      ,[AwayScorePeriod3]
      ,[AwayScoreNormaltime]
      ,[AwayScoreOvertime]
      ,[AwayScorePenalties]
      ,[OddsRegularFirstSourceId]
      ,[OddsRegularFirstValue]
      ,[OddsRegularFirstWining]
      ,[OddsRegularXSourceId]
      ,[OddsRegularXValue]
      ,[OddsRegularXWining]
      ,[OddsRegularSecondSourceId]
      ,[OddsRegularSecondValue]
      ,[OddsRegularSecondWining]
      ,[OddsDoubleChangeFirstXSourceId]
      ,[OddsDoubleChangeFirstXValue]
      ,[OddsDoubleChangeFirstXWining]
      ,[OddsDoubleChangeXSecondSourceId]
      ,[OddsDoubleChangeXSecondValue]
      ,[OddsDoubleChangeXSecondWining]
      ,[OddsDoubleChangeFirstSecondSourceId]
      ,[OddsDoubleChangeFirstSecondValue]
      ,[OddsDoubleChangeFirstSecondWining]
      ,[IsProcessed]
)
select [Date]
      ,[SportName]
      ,[SportSlug]
      ,[SportId]
      ,[TournamentName]
      ,[TournamentSlug]
      ,[TournamentId]
      ,[TournamentUniqueId]
      ,[CategoryName]
      ,[CategorySlug]
      ,[CategoryId]
      ,[SeasonName]
      ,[SeasonSlug]
      ,[SeasonId]
      ,[SeasonYear]
      ,[EventId]
      ,[EventCustomId]
      ,[EventFirstToServe]
      ,[EventHasDraw]
      ,[EventWinnerCode]
      ,[EventName]
      ,[EventSlug]
      ,[EventStartDate]
      ,[EventStartTime]
      ,[EventChanges]
      ,[StatusCode]
      ,[StatusType]
      ,[StatusDescription]
      ,[HomeTeamId]
      ,[HomeTeamName]
      ,[HomeTeamSlug]
      ,[HomeTeamGender]
      ,[HomeScoreCurrent]
      ,[HomeScorePeriod1]
      ,[HomeScorePeriod2]
      ,[HomeScorePeriod3]
      ,[HomeScoreNormaltime]
      ,[HomeScoreOvertime]
      ,[HomeScorePenalties]
      ,[AwayTeamId]
      ,[AwayTeamName]
      ,[AwayTeamSlug]
      ,[AwayTeamGender]
      ,[AwayScoreCurrent]
      ,[AwayScorePeriod1]
      ,[AwayScorePeriod2]
      ,[AwayScorePeriod3]
      ,[AwayScoreNormaltime]
      ,[AwayScoreOvertime]
      ,[AwayScorePenalties]
      ,[OddsRegularFirstSourceId]
      ,[OddsRegularFirstValue]
      ,[OddsRegularFirstWining]
      ,[OddsRegularXSourceId]
      ,[OddsRegularXValue]
      ,[OddsRegularXWining]
      ,[OddsRegularSecondSourceId]
      ,[OddsRegularSecondValue]
      ,[OddsRegularSecondWining]
      ,[OddsDoubleChangeFirstXSourceId]
      ,[OddsDoubleChangeFirstXValue]
      ,[OddsDoubleChangeFirstXWining]
      ,[OddsDoubleChangeXSecondSourceId]
      ,[OddsDoubleChangeXSecondValue]
      ,[OddsDoubleChangeXSecondWining]
      ,[OddsDoubleChangeFirstSecondSourceId]
      ,[OddsDoubleChangeFirstSecondValue]
      ,[OddsDoubleChangeFirstSecondWining]
      ,[IsProcessed] from [dbo].[BM_ImportData]
	-- clear
	truncate table [BM_ImportData]
END


GO

if exists (select * from sysobjects where name='BM_Tip_ALL')
begin
  drop procedure BM_Tip_ALL
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 15.10.2016
-- Description:	Přehled tipů
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_ALL]
	@Form int = 30,
	@Odd decimal(9,2) = 2.0,
	@DateFrom date = null,
	@DateTo date = null,
	@WithoutTournament bit = 0
AS
BEGIN
	select 
		BM_Event.ID,
		'Url'='http://www.sofascore.com/'+BM_Event.Slug+'/'+BM_Event.CustomId,
		BM_Event.DisplayName,
		BM_Category.Slug,
		BM_Event.DateStart,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) * -1 end) as Form,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) as Odd,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode,
		BM_Event.HomeScoreCurrent,
		BM_Event.AwayScoreCurrent,
		BM_Event.ID_Status,
        BM_Tip.[HomeLastForm],
        BM_Tip.[HomeLastGiven],
        BM_Tip.[HomeLastTaken],
        BM_Tip.[HomeSeasonForm],
        BM_Tip.[HomeSeasonGiven],
        BM_Tip.[HomeSeasonTaken],
		BM_Tip.[HomeSeasonCount],
        BM_Tip.[AwayLastForm],
        BM_Tip.[AwayLastGiven],
        BM_Tip.[AwayLastTaken],
        BM_Tip.[AwaySeasonForm],
        BM_Tip.[AwaySeasonGiven],
        BM_Tip.[AwaySeasonTaken],
		BM_Tip.[AwaySeasonCount],
		'ID_Category'=BM_Category.ID,
		'Category'=BM_Category.DisplayName,
		'ID_Season'=BM_Season.ID,
		'Season'=BM_Season.DisplayName,
		BM_OddsRegular.FirstValue,
		BM_OddsRegular.XValue,
		BM_OddsRegular.SecondValue
	from BM_Tip
		inner join BM_Event on BM_Event.ID=BM_Tip.ID
		inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
		inner join BM_Category on BM_Category.ID=BM_Event.ID_Category
		left join BM_Season on BM_Season.ID=BM_Event.ID_Season
	where (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) end) >= @Form
		and (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) >= @Odd
		and (@DateFrom is null or BM_Event.DateStart >= @DateFrom)
		and (@DateTo is null or BM_Event.DateStart < dateadd(day, 1, @DateTo))
		and ((@WithoutTournament=1 and BM_Category.Slug not like 'international%') or @WithoutTournament=0)
	order by BM_Event.DateStart
END


GO

if exists (select * from sysobjects where name='BM_Tip_DETAIL')
begin
  drop procedure BM_Tip_DETAIL
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 15.10.2016
-- Description:	Detail tipu
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_DETAIL]
	@ID int
AS
BEGIN
	select 
		BM_Event.ID,
		'Url'='http://www.sofascore.com/'+BM_Event.Slug+'/'+BM_Event.CustomId,
		BM_Event.DisplayName,
		BM_Event.DateStart,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) * -1 end) as Form,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then FirstValue else SecondValue end) as Odd,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode,
		BM_Event.ID_Status,
		'ID_Category'=BM_Category.ID,
		'Category'=BM_Category.DisplayName,
		'ID_Season'=BM_Season.ID,
		'Season'=BM_Season.DisplayName,
		'HomeTeam'=BM_HomeTeam.DisplayName,
		'AwayTeam'=BM_AwayTeam.DisplayName,
		BM_Event.HomeScoreCurrent,
		BM_Event.AwayScoreCurrent,
        BM_Tip.[HomeLastForm],
        BM_Tip.[HomeLastGiven],
        BM_Tip.[HomeLastTaken],
        BM_Tip.[HomeSeasonForm],
        BM_Tip.[HomeSeasonGiven],
        BM_Tip.[HomeSeasonTaken],
		BM_Tip.[HomeSeasonCount],
        BM_Tip.[AwayLastForm],
        BM_Tip.[AwayLastGiven],
        BM_Tip.[AwayLastTaken],
        BM_Tip.[AwaySeasonForm],
        BM_Tip.[AwaySeasonGiven],
        BM_Tip.[AwaySeasonTaken],
		BM_Tip.[AwaySeasonCount],
		BM_OddsRegular.FirstId,
		BM_OddsRegular.FirstValue,
		BM_OddsRegular.XId,
		BM_OddsRegular.XValue,
		BM_OddsRegular.SecondId,
		BM_OddsRegular.SecondValue
	from BM_Tip
	inner join BM_Event on BM_Event.ID=BM_Tip.ID
	inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	inner join BM_Team BM_HomeTeam on BM_HomeTeam.ID=BM_Event.ID_HomeTeam
	inner join BM_Team BM_AwayTeam on BM_AwayTeam.ID=BM_Event.ID_AwayTeam
	left join BM_Category on BM_Category.ID=BM_Event.ID_Category
	left join BM_Season on BM_Season.ID=BM_Event.ID_Season
	where BM_Event.ID=@ID
END


GO

if exists (select * from sysobjects where name='BM_Tip_GENERATE')
begin
  drop procedure BM_Tip_GENERATE
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_GENERATE]
AS
BEGIN

	declare @FootballTournament table (ID int, HomeLastForm int, HomeLastGiven int, HomeLastTaken int, HomeSeasonForm int, HomeSeasonGiven int, HomeSeasonTaken int, HomeSeasonCount int,  
				AwayLastForm int, AwayLastGiven int, AwayLastTaken int, AwaySeasonForm int, AwaySeasonGiven int, AwaySeasonTaken int, AwaySeasonCount int)

	DECLARE @ID int, @ID_HomeTeam int, @ID_AwayTeam int

	DECLARE db_cursor CURSOR FORWARD_ONLY FOR  
	select BM_Event.ID, BM_Event.ID_HomeTeam, BM_Event.ID_AwayTeam
	from BM_Event
	inner join BM_Category on BM_Category.ID=BM_Event.ID_Category
	inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	where BM_Category.ID_Sport=1
		and DateStart >= CAST(GETDATE() AS DATE)
	order by BM_Event.DateStart

	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @ID, @ID_HomeTeam, @ID_AwayTeam

	WHILE @@FETCH_STATUS = 0   
	BEGIN   
		insert into @FootballTournament (ID, HomeLastForm, HomeLastGiven, HomeLastTaken, HomeSeasonForm, HomeSeasonGiven, HomeSeasonTaken, HomeSeasonCount,
			AwayLastForm, AwayLastGiven, AwayLastTaken, AwaySeasonForm, AwaySeasonGiven, AwaySeasonTaken, AwaySeasonCount)
		select 
			@ID,
			-- domaci
			HomeLastEvent.Form, 
			HomeLastEvent.Given,
			HomeLastEvent.Taken,
			HomeSeasonEvent.Form,
			HomeSeasonEvent.Given,
			HomeSeasonEvent.Taken,
			HomeSeasonEvent.[Count],
			-- hoste
			AwayLastEvent.Form, 
			AwayLastEvent.Given,
			AwayLastEvent.Taken,
			AwaySeasonEvent.Form,
			AwaySeasonEvent.Given,
			AwaySeasonEvent.Taken,
			AwaySeasonEvent.[Count]
		from [dbo].[BM_Event_DETAIL_LastEvent](@ID, @ID_HomeTeam) HomeLastEvent
		outer apply [dbo].[BM_Event_DETAIL_LastEvent](@ID, @ID_AwayTeam) AwayLastEvent
		outer apply [dbo].[BM_Event_DETAIL_SeasonEvent](@ID, @ID_HomeTeam) HomeSeasonEvent
		outer apply [dbo].[BM_Event_DETAIL_SeasonEvent](@ID, @ID_AwayTeam) AwaySeasonEvent

		FETCH NEXT FROM db_cursor INTO @ID, @ID_HomeTeam, @ID_AwayTeam
	END   

	CLOSE db_cursor   
	DEALLOCATE db_cursor

	--SELECT *
	--INTO BM_Tip
	--FROM @FootballTournament

	MERGE [dbo].[BM_Tip] AS [target]
	USING 
	(
		select ID, HomeLastForm, HomeLastGiven, HomeLastTaken, HomeSeasonForm, HomeSeasonGiven, HomeSeasonTaken, HomeSeasonCount,
			AwayLastForm, AwayLastGiven, AwayLastTaken, AwaySeasonForm, AwaySeasonGiven, AwaySeasonTaken, AwaySeasonCount from @FootballTournament
	) AS [source]
	ON ([target].ID = [source].ID)
	WHEN matched THEN
		UPDATE SET 
			[target].HomeLastForm=[source].HomeLastForm,
			[target].HomeLastGiven=[source].HomeLastGiven,
			[target].HomeLastTaken=[source].HomeLastTaken,
			[target].HomeSeasonForm=[source].HomeSeasonForm,
			[target].HomeSeasonGiven=[source].HomeSeasonGiven,
			[target].HomeSeasonTaken=[source].HomeSeasonTaken,
			[target].HomeSeasonCount=[source].HomeSeasonCount,

			[target].AwayLastForm=[source].AwayLastForm,
			[target].AwayLastGiven=[source].AwayLastGiven,
			[target].AwayLastTaken=[source].AwayLastTaken,
			[target].AwaySeasonForm=[source].AwaySeasonForm,
			[target].AwaySeasonGiven=[source].AwaySeasonGiven,
			[target].AwaySeasonTaken=[source].AwaySeasonTaken,
			[target].AwaySeasonCount=[source].AwaySeasonCount
	when not matched then
		INSERT (ID, HomeLastForm, HomeLastGiven, HomeLastTaken, HomeSeasonForm, HomeSeasonGiven, HomeSeasonTaken, HomeSeasonCount, 
				AwayLastForm, AwayLastGiven, AwayLastTaken, AwaySeasonForm, AwaySeasonGiven, AwaySeasonTaken, AwaySeasonCount)
		VALUES ([source].ID, [source].HomeLastForm, [source].HomeLastGiven, [source].HomeLastTaken, [source].HomeSeasonForm, 
				[source].HomeSeasonGiven, [source].HomeSeasonTaken, [source].HomeSeasonCount, [source].AwayLastForm, [source].AwayLastGiven, [source].AwayLastTaken, 
				[source].AwaySeasonForm, [source].AwaySeasonGiven, [source].AwaySeasonTaken, [source].AwaySeasonCount);
END


GO

if exists (select * from sysobjects where name='BM_Tip_GENERATE_Old')
begin
  drop procedure BM_Tip_GENERATE_Old
end

GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_GENERATE_Old]
	@DateStart datetime,
	@DateTo datetime = null
AS
BEGIN
	select @DateTo=getdate() where @DateTo is null
	declare @FootballTournament table (ID int, HomeLastForm int, HomeLastGiven int, HomeLastTaken int, HomeSeasonForm int, HomeSeasonGiven int, HomeSeasonTaken int, HomeSeasonCount int,  
				AwayLastForm int, AwayLastGiven int, AwayLastTaken int, AwaySeasonForm int, AwaySeasonGiven int, AwaySeasonTaken int, AwaySeasonCount int)

	DECLARE @ID int, @ID_HomeTeam int, @ID_AwayTeam int

	DECLARE db_cursor CURSOR FORWARD_ONLY FOR  
	select BM_Event.ID, BM_Event.ID_HomeTeam, BM_Event.ID_AwayTeam
	from BM_Event
	inner join BM_Category on BM_Category.ID=BM_Event.ID_Category
	left join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	where BM_Category.ID_Sport=1
		and DateStart >=@DateStart and DateStart <= @DateTo
	order by BM_Event.DateStart

	OPEN db_cursor   
	FETCH NEXT FROM db_cursor INTO @ID, @ID_HomeTeam, @ID_AwayTeam

	WHILE @@FETCH_STATUS = 0   
	BEGIN   
		insert into @FootballTournament (ID, HomeLastForm, HomeLastGiven, HomeLastTaken, HomeSeasonForm, HomeSeasonGiven, HomeSeasonTaken, HomeSeasonCount,
			AwayLastForm, AwayLastGiven, AwayLastTaken, AwaySeasonForm, AwaySeasonGiven, AwaySeasonTaken, AwaySeasonCount)
		select 
			@ID,
			-- domaci
			HomeLastEvent.Form, 
			HomeLastEvent.Given,
			HomeLastEvent.Taken,
			HomeSeasonEvent.Form,
			HomeSeasonEvent.Given,
			HomeSeasonEvent.Taken,
			HomeSeasonEvent.[Count],
			-- hoste
			AwayLastEvent.Form, 
			AwayLastEvent.Given,
			AwayLastEvent.Taken,
			AwaySeasonEvent.Form,
			AwaySeasonEvent.Given,
			AwaySeasonEvent.Taken,
			AwaySeasonEvent.[Count]
		from [dbo].[BM_Event_DETAIL_LastEvent](@ID, @ID_HomeTeam) HomeLastEvent
		outer apply [dbo].[BM_Event_DETAIL_LastEvent](@ID, @ID_AwayTeam) AwayLastEvent
		outer apply [dbo].[BM_Event_DETAIL_SeasonEvent](@ID, @ID_HomeTeam) HomeSeasonEvent
		outer apply [dbo].[BM_Event_DETAIL_SeasonEvent](@ID, @ID_AwayTeam) AwaySeasonEvent

		FETCH NEXT FROM db_cursor INTO @ID, @ID_HomeTeam, @ID_AwayTeam
	END   

	CLOSE db_cursor   
	DEALLOCATE db_cursor

	--SELECT *
	--INTO BM_Tip
	--FROM @FootballTournament

	MERGE [dbo].[BM_Tip] AS [target]
	USING 
	(
		select ID, HomeLastForm, HomeLastGiven, HomeLastTaken, HomeSeasonForm, HomeSeasonGiven, HomeSeasonTaken, HomeSeasonCount,
			AwayLastForm, AwayLastGiven, AwayLastTaken, AwaySeasonForm, AwaySeasonGiven, AwaySeasonTaken, AwaySeasonCount from @FootballTournament
	) AS [source]
	ON ([target].ID = [source].ID)
	WHEN matched THEN
		UPDATE SET 
			[target].HomeLastForm=[source].HomeLastForm,
			[target].HomeLastGiven=[source].HomeLastGiven,
			[target].HomeLastTaken=[source].HomeLastTaken,
			[target].HomeSeasonForm=[source].HomeSeasonForm,
			[target].HomeSeasonGiven=[source].HomeSeasonGiven,
			[target].HomeSeasonTaken=[source].HomeSeasonTaken,
			[target].HomeSeasonCount=[source].HomeSeasonCount,

			[target].AwayLastForm=[source].AwayLastForm,
			[target].AwayLastGiven=[source].AwayLastGiven,
			[target].AwayLastTaken=[source].AwayLastTaken,
			[target].AwaySeasonForm=[source].AwaySeasonForm,
			[target].AwaySeasonGiven=[source].AwaySeasonGiven,
			[target].AwaySeasonTaken=[source].AwaySeasonTaken,
			[target].AwaySeasonCount=[source].AwaySeasonCount
	when not matched then
		INSERT (ID, HomeLastForm, HomeLastGiven, HomeLastTaken, HomeSeasonForm, HomeSeasonGiven, HomeSeasonTaken, HomeSeasonCount, 
				AwayLastForm, AwayLastGiven, AwayLastTaken, AwaySeasonForm, AwaySeasonGiven, AwaySeasonTaken, AwaySeasonCount)
		VALUES ([source].ID, [source].HomeLastForm, [source].HomeLastGiven, [source].HomeLastTaken, [source].HomeSeasonForm, 
				[source].HomeSeasonGiven, [source].HomeSeasonTaken, [source].HomeSeasonCount, [source].AwayLastForm, [source].AwayLastGiven, [source].AwayLastTaken, 
				[source].AwaySeasonForm, [source].AwaySeasonGiven, [source].AwaySeasonTaken, [source].AwaySeasonCount);
END


GO

if exists (select * from sysobjects where name='BM_Tip_DETAIL_Graph')
begin
  drop procedure BM_Tip_DETAIL_Graph
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 23.10.2016
-- Description:	Graf ziskovosti
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_DETAIL_Graph]
	@Limit int = 30,
	@Odd decimal(9,2) = 2.0,
	@DateFrom date = '2016-10-01',
	@DateTo date = '2016-10-23',
	@Form int = 50,
	@price decimal = 100
AS
BEGIN
select DateStart, count(*) as Total, sum(GoodBet) as Correct, avg(Odd) as AverageOdd, sum(Price) as Price,
cast(cast(sum(GoodBet) as decimal(9,2))/cast(count(*) as decimal(9,2)) * 100 as decimal(9,2)) as Percentile from
(
select Form, DisplayName, GoodBet, Odd,
	(case when GoodBet=1 then @price*Odd else 0 end) - @price as Price, DateStart
from
(
select ID, DisplayName, Form, Odd, WinnerCode, 
	case when WinnerCode=
	(case when Form >= @Limit then 1 else case when Form <= -@Limit then 2 else 3 end end)
	then 1 else 0 end as GoodBet,
	--round(abs(Form), -1) as [Group]
	CAST(DateStart AS DATE) as DateStart
from 
(
	select 
		BM_Event.ID,
		'Url'='http://www.sofascore.com/'+BM_Event.Slug+'/'+BM_Event.CustomId,
		BM_Event.DisplayName,
		BM_Event.DateStart,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) * -1 end) as Form,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) as Odd,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode,
		BM_Event.ID_Status,
        BM_Tip.[HomeLastForm],
        BM_Tip.[HomeLastGiven],
        BM_Tip.[HomeLastTaken],
        BM_Tip.[HomeSeasonForm],
        BM_Tip.[HomeSeasonGiven],
        BM_Tip.[HomeSeasonTaken],
		BM_Tip.[HomeSeasonCount],
        BM_Tip.[AwayLastForm],
        BM_Tip.[AwayLastGiven],
        BM_Tip.[AwayLastTaken],
        BM_Tip.[AwaySeasonForm],
        BM_Tip.[AwaySeasonGiven],
        BM_Tip.[AwaySeasonTaken],
		BM_Tip.[AwaySeasonCount],
		'ID_Category'=BM_Category.ID,
		'Category'=BM_Category.DisplayName,
		'ID_Season'=BM_Season.ID,
		'Season'=BM_Season.DisplayName,
		BM_OddsRegular.FirstValue,
		BM_OddsRegular.XValue,
		BM_OddsRegular.SecondValue
	from BM_Tip
	inner join BM_Event on BM_Event.ID=BM_Tip.ID
	inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	left join BM_Category on BM_Category.ID=BM_Event.ID_Category
	left join BM_Season on BM_Season.ID=BM_Event.ID_Season
	where (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) end) >= @Limit
		AND (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) > @Odd
		and (@DateFrom is null or BM_Event.DateStart >= @DateFrom)
		and (@DateTo is null or BM_Event.DateStart < dateadd(day, 1, @DateTo))
) Tips
) Bet
where abs(Form) >= @Form
) x
group by DateStart
END


GO

if exists (select * from sysobjects where name='BM_Tip_DETAIL_Income')
begin
  drop procedure BM_Tip_DETAIL_Income
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 20.10.2016
-- Description:	Zisk z tipů při dané konfiguraci
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_DETAIL_Income]
@Form int = 0,
	@Odd decimal(9,2) = 2.0,
	@DateFrom date = '2016-10-01',
	@DateTo date = '2016-10-20',
	@limit int = 50,
	@price decimal = 100
AS
BEGIN

select count(*) as Total, sum(GoodBet) as Correct, avg(Odd) as AverageOdd, sum(Price) as Price,
cast(cast(sum(GoodBet) as decimal(9,2))/cast(count(*) as decimal(9,2)) * 100 as decimal(9,2)) as Percentile from
(
select [Group], DisplayName, GoodBet, Odd,
	(case when GoodBet=1 then @price*Odd else 0 end) - @price as Price
from
(
select ID, DisplayName, Form, Odd, WinnerCode, 
	case when WinnerCode=
	(case when Form > @limit then 1 else case when Form < -@limit then 2 else 3 end end)
	then 1 else 0 end as GoodBet,
	--round(abs(Form), -1) as [Group]
	abs(Form)/10*10 as [Group]
from 
(
	select 
		BM_Event.ID,
		'Url'='http://www.sofascore.com/'+BM_Event.Slug+'/'+BM_Event.CustomId,
		BM_Event.DisplayName,
		BM_Event.DateStart,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) * -1 end) as Form,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) as Odd,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode,
		BM_Event.ID_Status,
        BM_Tip.[HomeLastForm],
        BM_Tip.[HomeLastGiven],
        BM_Tip.[HomeLastTaken],
        BM_Tip.[HomeSeasonForm],
        BM_Tip.[HomeSeasonGiven],
        BM_Tip.[HomeSeasonTaken],
		BM_Tip.[HomeSeasonCount],
        BM_Tip.[AwayLastForm],
        BM_Tip.[AwayLastGiven],
        BM_Tip.[AwayLastTaken],
        BM_Tip.[AwaySeasonForm],
        BM_Tip.[AwaySeasonGiven],
        BM_Tip.[AwaySeasonTaken],
		BM_Tip.[AwaySeasonCount],
		'ID_Category'=BM_Category.ID,
		'Category'=BM_Category.DisplayName,
		'ID_Season'=BM_Season.ID,
		'Season'=BM_Season.DisplayName,
		BM_OddsRegular.FirstValue,
		BM_OddsRegular.XValue,
		BM_OddsRegular.SecondValue
	from BM_Tip
	inner join BM_Event on BM_Event.ID=BM_Tip.ID
	inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	left join BM_Category on BM_Category.ID=BM_Event.ID_Category
	left join BM_Season on BM_Season.ID=BM_Event.ID_Season
	where (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) end) >= @Form
		AND (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) > @Odd
		and (@DateFrom is null or BM_Event.DateStart >= @DateFrom)
		and (@DateTo is null or BM_Event.DateStart < @DateTo)
) Tips
) Bet
where [Group] > @limit
) x
END


GO

if exists (select * from sysobjects where name='BM_Tip_DETAIL_Rozlozeni')
begin
  drop procedure BM_Tip_DETAIL_Rozlozeni
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 20.10.2016
-- Description:	Rozlozeni uspesnosti dle limitu
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_DETAIL_Rozlozeni]
		@Form int = 0,
		@Odd decimal(9,2) = 2.0,
		@DateFrom date = '2016-01-01',
		@DateTo date = '2016-10-20',
		@limit int = 20
AS
BEGIN

	select [Group], count(*) as Total, sum(GoodBet) as Correct, 
	cast(cast(sum(GoodBet) as decimal(9,2))/cast(count(*) as decimal(9,2)) * 100 as decimal(9,2)) as Percentile from
	(
	select ID, DisplayName, Form, WinnerCode, 
		case when WinnerCode=
		(case when Form > @limit then 1 else case when Form < -@limit then 2 else 3 end end)
		then 1 else 0 end as GoodBet,
		--round(abs(Form), -1) as [Group]
		abs(Form)/10*10 as [Group]
	from 
	(
		select 
			BM_Event.ID,
			'Url'='http://www.sofascore.com/'+BM_Event.Slug+'/'+BM_Event.CustomId,
			BM_Event.DisplayName,
			BM_Event.DateStart,
			(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
				else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) * -1 end) as Form,
			(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) as Odd,
			(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode,
			BM_Event.ID_Status,
			BM_Tip.[HomeLastForm],
			BM_Tip.[HomeLastGiven],
			BM_Tip.[HomeLastTaken],
			BM_Tip.[HomeSeasonForm],
			BM_Tip.[HomeSeasonGiven],
			BM_Tip.[HomeSeasonTaken],
			BM_Tip.[HomeSeasonCount],
			BM_Tip.[AwayLastForm],
			BM_Tip.[AwayLastGiven],
			BM_Tip.[AwayLastTaken],
			BM_Tip.[AwaySeasonForm],
			BM_Tip.[AwaySeasonGiven],
			BM_Tip.[AwaySeasonTaken],
			BM_Tip.[AwaySeasonCount],
			'ID_Category'=BM_Category.ID,
			'Category'=BM_Category.DisplayName,
			'ID_Season'=BM_Season.ID,
			'Season'=BM_Season.DisplayName,
			BM_OddsRegular.FirstValue,
			BM_OddsRegular.XValue,
			BM_OddsRegular.SecondValue
		from BM_Tip
		inner join BM_Event on BM_Event.ID=BM_Tip.ID
		inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
		left join BM_Category on BM_Category.ID=BM_Event.ID_Category
		left join BM_Season on BM_Season.ID=BM_Event.ID_Season
		where (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
				else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) end) >= @Form
			AND (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) > @Odd
			and (@DateFrom is null or BM_Event.DateStart >= @DateFrom)
			and (@DateTo is null or BM_Event.DateStart < @DateTo)
	) Tips
	) Bet
	group by [Group]
END


GO

if exists (select * from sysobjects where name='BM_Event_ALL_Tournament_Form')
begin
  drop procedure BM_Event_ALL_Tournament_Form
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 26.10.2016
-- Description: Prepared data for FANN..
-- =============================================
CREATE PROCEDURE [dbo].[BM_Event_ALL_Tournament_Form]
	@ID_Tournament int,
	@ID_Season int,
	@Inverse bit = 0
AS
BEGIN
	declare @true int = 1, @false int = 0, @ID int

select 
	BM_Event.ID,
	BM_Event.DisplayName,
	BM_Event.DateStart,
	BM_Event.ID_Season,
	BM_Event.ID_HomeTeam,
	BM_Event.ID_AwayTeam,
	cast(HomeLastEvent.Form as int) as HomeLastForm,
	cast((dbo.BM_Tip_Sigmoid((HomeLastEvent.Given/7.0), 1.1, 3.0)*100) as int) as HomeLastGiven,
	cast((dbo.BM_Tip_Sigmoid((HomeLastEvent.Taken/7.0), 1.1, 3.0)*100) as int) as HomeLastTaken,
	cast(HomeLastEventOnly.Form as int) as HomeLastOnlyForm,
	cast((dbo.BM_Tip_Sigmoid((HomeLastEventOnly.Given/7.0), 1.1, 3.0)*100) as int) as HomeLastOnlyGiven,
	cast((dbo.BM_Tip_Sigmoid((HomeLastEventOnly.Taken/7.0), 1.1, 3.0)*100) as int) as HomeLastOnlyTaken,
	cast(AwayLastEvent.Form as int) as AwayLastForm,
	cast((dbo.BM_Tip_Sigmoid((AwayLastEvent.Given/7.0), 1.1, 3.0)*100) as int) as AwayLastGiven,
	cast((dbo.BM_Tip_Sigmoid((AwayLastEvent.Taken/7.0), 1.1, 3.0)*100) as int) as AwayLastTaken,
	cast(AwayLastEventOnly.Form as int) as AwayLastOnlyForm,
	cast((dbo.BM_Tip_Sigmoid((AwayLastEventOnly.Given/7.0), 1.1, 3.0)*100) as int) as AwayLastOnlyGiven,
	cast((dbo.BM_Tip_Sigmoid((AwayLastEventOnly.Taken/7.0), 1.1, 3.0)*100) as int) as AwayLastOnlyTaken,
	-- odds
	[BM_OddsRegular].FirstValue,
	[BM_OddsRegular].XValue,
	[BM_OddsRegular].SecondValue,
	-- results
	'Home'=case when BM_Event.[WinnerCode]=1 then @true else @false end,
	'Draw'=case when BM_Event.[WinnerCode]=3 then @true else @false end,
	'Away'=case when BM_Event.[WinnerCode]=2 then @true else @false end
from BM_Event
	--inner join BM_Tip on BM_Event.ID=BM_Tip.ID
	left join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	inner join BM_Season on BM_Season.ID=BM_Event.ID_Season
	outer apply [dbo].[BM_Event_DETAIL_LastEvent](BM_Event.ID, BM_Event.ID_HomeTeam) HomeLastEvent
	outer apply [dbo].[BM_Event_DETAIL_LastEvent](BM_Event.ID, BM_Event.ID_AwayTeam) AwayLastEvent
	outer apply [dbo].[BM_Event_DETAIL_LastEvent_HomeOnly](BM_Event.ID, BM_Event.ID_HomeTeam) HomeLastEventOnly
	outer apply [dbo].[BM_Event_DETAIL_LastEvent_AwayOnly](BM_Event.ID, BM_Event.ID_AwayTeam) AwayLastEventOnly
where ((BM_Event.ID_Season=@ID_Season and @Inverse=0) or (BM_Event.ID_Season<>@ID_Season and @Inverse=1))
	and BM_Event.ID_Tournament=@ID_Tournament
order by BM_Event.ID_Season, BM_Event.DateStart 
END


GO

if exists (select * from sysobjects where name='BM_Tip_ALL_BackUp')
begin
  drop procedure BM_Tip_ALL_BackUp
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 15.10.2016
-- Description:	Přehled tipů
-- =============================================
CREATE PROCEDURE [dbo].[BM_Tip_ALL_BackUp]
	@Form int = 30,
	@Odd decimal(9,2) = 2.0,
	@DateFrom date = null,
	@DateTo date = null,
	@WithoutTournament bit = 0
AS
BEGIN
	select 
		BM_Event.ID,
		'Url'='http://www.sofascore.com/'+BM_Event.Slug+'/'+BM_Event.CustomId,
		BM_Event.DisplayName,
		BM_Category.Slug,
		BM_Event.DateStart,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) * -1 end) as Form,
		(case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) as Odd,
		(case when BM_Event.WinnerCode=0 then 0 else (case when BM_Event.HomeScoreCurrent>BM_Event.AwayScoreCurrent then 1 else (case when BM_Event.HomeScoreCurrent<BM_Event.AwayScoreCurrent then 2 else 3 end) end) end) as WinnerCode,
		BM_Event.HomeScoreCurrent,
		BM_Event.AwayScoreCurrent,
		BM_Event.ID_Status,
        BM_Tip.[HomeLastForm],
        BM_Tip.[HomeLastGiven],
        BM_Tip.[HomeLastTaken],
        BM_Tip.[HomeSeasonForm],
        BM_Tip.[HomeSeasonGiven],
        BM_Tip.[HomeSeasonTaken],
		BM_Tip.[HomeSeasonCount],
        BM_Tip.[AwayLastForm],
        BM_Tip.[AwayLastGiven],
        BM_Tip.[AwayLastTaken],
        BM_Tip.[AwaySeasonForm],
        BM_Tip.[AwaySeasonGiven],
        BM_Tip.[AwaySeasonTaken],
		BM_Tip.[AwaySeasonCount],
		'ID_Category'=BM_Category.ID,
		'Category'=BM_Category.DisplayName,
		'ID_Season'=BM_Season.ID,
		'Season'=BM_Season.DisplayName,
		BM_OddsRegular.FirstValue,
		BM_OddsRegular.XValue,
		BM_OddsRegular.SecondValue
	from BM_Tip
		inner join BM_Event on BM_Event.ID=BM_Tip.ID
		inner join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
		inner join BM_Category on BM_Category.ID=BM_Event.ID_Category
		left join BM_Season on BM_Season.ID=BM_Event.ID_Season
	where (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwaySeasonForm) then ((HomeLastForm + HomeSeasonForm) - (AwayLastForm + AwaySeasonForm)) 
			else ((AwayLastForm + AwaySeasonForm) - (HomeLastForm + HomeSeasonForm)) end) >= @Form
		and (case when (HomeLastForm + HomeSeasonForm) > (AwayLastForm + AwayLastForm) then FirstValue else SecondValue end) >= @Odd
		and (@DateFrom is null or BM_Event.DateStart >= @DateFrom)
		and (@DateTo is null or BM_Event.DateStart < dateadd(day, 1, @DateTo))
		and ((@WithoutTournament=1 and BM_Category.Slug not like 'international%') or @WithoutTournament=0)
	order by BM_Event.DateStart
END


GO

if exists (select * from sysobjects where name='BM_Tip_DETAIL_Genetic')
begin
  drop procedure BM_Tip_DETAIL_Genetic
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 27.10.2016
-- Description:	Implementace genetického algoritmu
-- =============================================
CREATE PROCEDURE BM_Tip_DETAIL_Genetic
	@Form int = 30,
	@Odd decimal(9,2) = 2.0,
	@DateFrom date = '2016-10-01',
	@DateTo date = '2016-10-27',
	@limit int = 20,
	@price int = 100,
	-- promenne
	@PopulationSize int = 20, -- počáteční počet populace
	@generationLength int = 50, -- počet interací/generací
	@crossover decimal(9,2) = 0.8, -- ppdst křížení
	@mutation decimal(9,2) = 0.2 -- ppdst mutace
AS
BEGIN
	declare
	@i int = 0, -- iterátor nad generacemi
	@j int = 0 -- iterátor nad populací

	-- 1) deklarace počáteční populace a další generace
	create table #PopulationCurrent  
	(
		ID int,
		LastKoef decimal(9,2),
		SeasonKoef decimal(9,2),
		Fitness decimal(9,2)
	)

	-- budouci generace
	create table #PopulationNext  
	(
		ID int identity,
		LastKoef decimal(9,2),
		SeasonKoef decimal(9,2),
		Fitness decimal(9,2)
	)

	-- vygeneruju X náhodných mutací do začátku
	;WITH Nbrs (Number) AS (
		SELECT 1 UNION ALL
		SELECT 1 + Number FROM Nbrs WHERE Number < @PopulationSize
	)
	insert into #PopulationCurrent
	select Number, RAND(CHECKSUM(NEWID())), RAND(CHECKSUM(NEWID())), null from Nbrs

	-- výchozí výpočet vah
	update [Population]
	set Fitness=IsNull([dbo].[BM_Genetic_DETAIL_Price](@Form, @Odd, @DateFrom,@DateTo, @limit, @price, [Population].LastKoef, [Population].SeasonKoef), -9999999.99)
	from #PopulationCurrent [Population]
	where [Population].Fitness is null

	-- 2) iterujeme nad generacemi
	while(@i < @generationLength)
	begin



		set @j = 0
		while (@j < @PopulationSize)
		begin
			declare @leftOut int, @rightOut int, @leftIn int, @rightIn int
			select @leftOut=ROUND(((@PopulationSize-2) * RAND() + 1), 0), @rightOut=ROUND(((@PopulationSize-2) * RAND() + 1), 0),
				@leftIn=ROUND(((@PopulationSize-2) * RAND() + 1), 0), @rightIn=ROUND(((@PopulationSize-2) * RAND() + 1), 0)
			-- 3) křížení nebo výběr
			if (RAND(CHECKSUM(NEWID())) < @crossover)
			begin
				-- křížení
				insert into #PopulationNext
				select 
					 (case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.LastKoef else PRight.LastKoef end) as LastKoef,
					 (case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.SeasonKoef else PRight.SeasonKoef end) as SeasonKoef,
					 null as Crossing
				from
				(select
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastKoef else [RightPopulation].LastKoef end) as LastKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonKoef else [RightPopulation].SeasonKoef end) as SeasonKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].Fitness else [RightPopulation].Fitness end) as Fitness
				from #PopulationCurrent [LeftPopulation] 
				cross join #PopulationCurrent [RightPopulation]
				where [LeftPopulation].ID=@leftIn
					and [RightPopulation].ID=@rightIn) as PLeft
				cross join (select
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastKoef else [RightPopulation].LastKoef end) as LastKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonKoef else [RightPopulation].SeasonKoef end) as SeasonKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].Fitness else [RightPopulation].Fitness end) as Fitness
				from #PopulationCurrent [LeftPopulation] 
				cross join #PopulationCurrent [RightPopulation]
				where [LeftPopulation].ID=@leftOut
					and [RightPopulation].ID=@rightOut) as PRight
			end
			else
			begin 
				-- výběr
				insert into #PopulationNext
				select
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastKoef else [RightPopulation].LastKoef end) as LastKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonKoef else [RightPopulation].SeasonKoef end) as SeasonKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].Fitness else [RightPopulation].Fitness end) as Fitness
				from #PopulationCurrent [LeftPopulation] 
				cross join #PopulationCurrent [RightPopulation]
				where [LeftPopulation].ID=@leftOut
					and [RightPopulation].ID=@rightOut
			end

			-- 4) mutace
			if (RAND(CHECKSUM(NEWID())) < @mutation)
			begin
				declare @rand decimal(9,2)
				set @rand=RAND(CHECKSUM(NEWID()))

				update PopulationNext set 
					PopulationNext.LastKoef=case when @rand<0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.LastKoef end,
					PopulationNext.SeasonKoef=case when @rand>0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.SeasonKoef end
				from #PopulationNext PopulationNext
				where ID=@@IDENTITY
			end

			select @j += 1
		end

		truncate table #PopulationCurrent

		insert into #PopulationCurrent
		select * from #PopulationNext

		truncate table #PopulationNext

		-- výchozí výpočet vah
		update [Population]
		set Fitness=IsNull([dbo].[BM_Genetic_DETAIL_Price](@Form, @Odd, @DateFrom,@DateTo, @limit, @price, [Population].LastKoef, [Population].SeasonKoef), -9999999.99)
		from #PopulationCurrent [Population]
		where [Population].Fitness is null

		-- inc
		select @i += 1
	end

	select TOP(1) * from #PopulationCurrent order by Fitness desc

	drop table #PopulationCurrent
	drop table #PopulationNext

END


GO

if exists (select * from sysobjects where name='BM_Tip_DETAIL_Genetic_Extend')
begin
  drop procedure BM_Tip_DETAIL_Genetic_Extend
end

GO

-- =============================================
-- Author:		Pavel Lorenz
-- Create date: 27.10.2016
-- Description:	Implementace genetického algoritmu
-- Rozšířeno o další parametry
-- =============================================
CREATE PROCEDURE BM_Tip_DETAIL_Genetic_Extend
	@Form int = 30,
	@Odd decimal(9,2) = 2.0,
	@DateFrom date = '2016-10-01',
	@DateTo date = '2016-10-27',
	@limit int = 20,
	@price int = 100,
	-- promenne
	@PopulationSize int = 20, -- počáteční počet populace
	@generationLength int = 10, -- počet interací/generací
	@crossover decimal(9,2) = 0.8, -- ppdst křížení
	@mutation decimal(9,2) = 0.2 -- ppdst mutace
AS
BEGIN
	declare
	@i int = 0, -- iterátor nad generacemi
	@j int = 0 -- iterátor nad populací

	-- 1) deklarace počáteční populace a další generace
	create table #PopulationCurrent  
	(
		ID int,
		LastKoef decimal(9,2),
		--SeasonKoef decimal(9,2),
		LastGivenKoef decimal(9,2),
		LastTakenKoef decimal(9,2),
		--SeasonGivenKoef decimal(9,2),
		--SeasonTakenKoef decimal(9,2),
		Fitness decimal(9,2)
	)

	-- budouci generace
	create table #PopulationNext  
	(
		ID int identity,
		LastKoef decimal(9,2),
		--SeasonKoef decimal(9,2),
		LastGivenKoef decimal(9,2),
		LastTakenKoef decimal(9,2),
		--SeasonGivenKoef decimal(9,2),
		--SeasonTakenKoef decimal(9,2),
		Fitness decimal(9,2)
	)

	-- vygeneruju X náhodných mutací do začátku
	;WITH Nbrs (Number) AS (
		SELECT 1 UNION ALL
		SELECT 1 + Number FROM Nbrs WHERE Number < @PopulationSize
	)
	insert into #PopulationCurrent
	select Number, RAND(CHECKSUM(NEWID())), RAND(CHECKSUM(NEWID())),RAND(CHECKSUM(NEWID())), null from Nbrs
	-- RAND(CHECKSUM(NEWID())),RAND(CHECKSUM(NEWID())), RAND(CHECKSUM(NEWID()))

	-- výchozí výpočet vah
	update [Population]
	set Fitness=IsNull([dbo].[BM_Genetic_DETAIL_Price_Extend](@Form, @Odd, @DateFrom,@DateTo, @limit, @price, 
	[Population].LastKoef, 0, [Population].LastGivenKoef, [Population].LastTakenKoef, 0, 0), -9999999.99)
	from #PopulationCurrent [Population]
	where [Population].Fitness is null

	-- 2) iterujeme nad generacemi
	while(@i < @generationLength)
	begin
		set @j = 0
		while (@j < @PopulationSize)
		begin
			declare @leftOut int, @rightOut int, @leftIn int, @rightIn int
			select @leftOut=ROUND(((@PopulationSize-2) * RAND() + 1), 0), @rightOut=ROUND(((@PopulationSize-2) * RAND() + 1), 0),
				@leftIn=ROUND(((@PopulationSize-2) * RAND() + 1), 0), @rightIn=ROUND(((@PopulationSize-2) * RAND() + 1), 0)
			-- 3) křížení nebo výběr
			if (RAND(CHECKSUM(NEWID())) < @crossover)
			begin
				-- křížení (vždy 0.5)
				insert into #PopulationNext
				select 
					 (case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.LastKoef else PRight.LastKoef end) as LastKoef,
					 --(case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.SeasonKoef else PRight.SeasonKoef end) as SeasonKoef,
					 (case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.LastGivenKoef else PRight.LastGivenKoef end) as LastGivenKoef,
					 (case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.LastTakenKoef else PRight.LastTakenKoef end) as LastTakenKoef,
					 --(case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.SeasonGivenKoef else PRight.SeasonGivenKoef end) as SeasonGivenKoef,
					 --(case when RAND(CHECKSUM(NEWID()))< 0.5 then PLeft.SeasonTakenKoef else PRight.SeasonTakenKoef end) as SeasonTakenKoef,
					 null as Crossing
				from
				(select
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastKoef else [RightPopulation].LastKoef end) as LastKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonKoef else [RightPopulation].SeasonKoef end) as SeasonKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastGivenKoef else [RightPopulation].LastGivenKoef end) as LastGivenKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastTakenKoef else [RightPopulation].LastTakenKoef end) as LastTakenKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonGivenKoef else [RightPopulation].SeasonGivenKoef end) as SeasonGivenKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonTakenKoef else [RightPopulation].SeasonTakenKoef end) as SeasonTakenKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].Fitness else [RightPopulation].Fitness end) as Fitness
				from #PopulationCurrent [LeftPopulation] 
				cross join #PopulationCurrent [RightPopulation]
				where [LeftPopulation].ID=@leftIn
					and [RightPopulation].ID=@rightIn) as PLeft
				cross join (select
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastKoef else [RightPopulation].LastKoef end) as LastKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonKoef else [RightPopulation].SeasonKoef end) as SeasonKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastGivenKoef else [RightPopulation].LastGivenKoef end) as LastGivenKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastTakenKoef else [RightPopulation].LastTakenKoef end) as LastTakenKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonGivenKoef else [RightPopulation].SeasonGivenKoef end) as SeasonGivenKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonTakenKoef else [RightPopulation].SeasonTakenKoef end) as SeasonTakenKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].Fitness else [RightPopulation].Fitness end) as Fitness
				from #PopulationCurrent [LeftPopulation] 
				cross join #PopulationCurrent [RightPopulation]
				where [LeftPopulation].ID=@leftOut
					and [RightPopulation].ID=@rightOut) as PRight
			end
			else
			begin 
				-- výběr
				insert into #PopulationNext
				select
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastKoef else [RightPopulation].LastKoef end) as LastKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonKoef else [RightPopulation].SeasonKoef end) as SeasonKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastGivenKoef else [RightPopulation].LastGivenKoef end) as LastGivenKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].LastTakenKoef else [RightPopulation].LastTakenKoef end) as LastTakenKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonGivenKoef else [RightPopulation].SeasonGivenKoef end) as SeasonGivenKoef,
					 --(case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].SeasonTakenKoef else [RightPopulation].SeasonTakenKoef end) as SeasonTakenKoef,
					 (case when [LeftPopulation].Fitness > [RightPopulation].Fitness then [LeftPopulation].Fitness else [RightPopulation].Fitness end) as Fitness
				from #PopulationCurrent [LeftPopulation] 
				cross join #PopulationCurrent [RightPopulation]
				where [LeftPopulation].ID=@leftOut
					and [RightPopulation].ID=@rightOut
			end

			-- 4) mutace - pouze jeden parametr
			if (RAND(CHECKSUM(NEWID())) < @mutation)
			begin
				declare @rand decimal(9,2)
				set @rand=RAND(CHECKSUM(NEWID()))

				update PopulationNext set 
					PopulationNext.LastKoef=case when @rand<0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.LastKoef end,
					--PopulationNext.SeasonKoef=case when @rand>0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.SeasonKoef end,
					PopulationNext.LastGivenKoef=case when @rand<0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.LastGivenKoef end,
					PopulationNext.LastTakenKoef=case when @rand>0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.LastTakenKoef end --,
					--PopulationNext.SeasonGivenKoef=case when @rand<0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.SeasonGivenKoef end,
					--PopulationNext.SeasonTakenKoef=case when @rand>0.5 then RAND(CHECKSUM(NEWID())) else PopulationNext.SeasonTakenKoef end
				from #PopulationNext PopulationNext
				where ID=@@IDENTITY
			end

			select @j += 1
		end

		truncate table #PopulationCurrent

		insert into #PopulationCurrent
		select * from #PopulationNext

		truncate table #PopulationNext

		-- výchozí výpočet vah
		update [Population]
		set Fitness=IsNull([dbo].[BM_Genetic_DETAIL_Price_Extend](@Form, @Odd, @DateFrom,@DateTo, @limit, @price, 
		[Population].LastKoef, 0, [Population].LastGivenKoef, [Population].LastTakenKoef, 0, 0), -9999999.99)
		--set Fitness=IsNull([dbo].[BM_Genetic_DETAIL_Price_Extend](@Form, @Odd, @DateFrom,@DateTo, @limit, @price, 
		--[Population].LastKoef, [Population].SeasonKoef, [Population].LastGivenKoef, [Population].LastTakenKoef, [Population].SeasonGivenKoef, [Population].SeasonTakenKoef), -9999999.99)
		from #PopulationCurrent [Population]
		where [Population].Fitness is null

		-- inc
		select @i += 1
	end

	select TOP(1) * from #PopulationCurrent order by Fitness desc

	drop table #PopulationCurrent
	drop table #PopulationNext

END


GO

print 'CurrentTime: Procedures - ' + convert(varchar, getdate(), 120)

GO

-- Create indices

if not exists(select * from sys.indexes where name='IX_BM_Event_DateStart' and is_primary_key=0)
begin
    create  nonclustered index IX_BM_Event_DateStart on [BM_Event] ([DateStart] ASC)
end

GO

if not exists(select * from sys.indexes where name='IX_BM_Event_Status' and is_primary_key=0)
begin
    create  nonclustered index IX_BM_Event_Status on [BM_Event] ([ID_Status] ASC)
  INCLUDE ([WinnerCode], [ID_HomeTeam], [ID_AwayTeam], [ID_Tournament], [ID_Season], [HomeScoreCurrent], [AwayScoreCurrent])
end

GO

if not exists(select * from sys.indexes where name='IX_BM_Event_Whole' and is_primary_key=0)
begin
    create  nonclustered index IX_BM_Event_Whole on [BM_Event] ([ID_Tournament] ASC, [ID_Season] ASC, [ID_Status] ASC)
  INCLUDE ([WinnerCode], [ID_HomeTeam], [ID_AwayTeam], [HomeScoreCurrent], [AwayScoreCurrent])
end

GO

print 'CurrentTime: Indices - ' + convert(varchar, getdate(), 120)

GO

-- Check columns

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Category' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Slug', 'IsActive', 'ID_Sport', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Category obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Event' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Slug', 'IsActive', 'CustomId', 'FirstToServe', 'HasDraw', 'WinnerCode', 'DateStart', 'Changes', 'ID_HomeTeam', 'ID_AwayTeam', 'ID_Tournament', 'ID_Season', 'ID_Category', 'ID_Status', 'StatusDescription', 'HomeScoreCurrent', 'AwayScoreCurrent', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Event obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_ImportData' 
    and sys.columns.name not in ('Date', 'SportName', 'SportSlug', 'SportId', 'TournamentName', 'TournamentSlug', 'TournamentId', 'TournamentUniqueId', 'CategoryName', 'CategorySlug', 'CategoryId', 'SeasonName', 'SeasonSlug', 'SeasonId', 'SeasonYear', 'EventId', 'EventCustomId', 'EventFirstToServe', 'EventHasDraw', 'EventWinnerCode', 'EventName', 'EventSlug', 'EventStartDate', 'EventStartTime', 'EventChanges', 'StatusCode', 'StatusType', 'StatusDescription', 'HomeTeamId', 'HomeTeamName', 'HomeTeamSlug', 'HomeTeamGender', 'HomeScoreCurrent', 'HomeScorePeriod1', 'HomeScorePeriod2', 'HomeScorePeriod3', 'HomeScoreNormaltime', 'HomeScoreOvertime', 'HomeScorePenalties', 'AwayTeamId', 'AwayTeamName', 'AwayTeamSlug', 'AwayTeamGender', 'AwayScoreCurrent', 'AwayScorePeriod1', 'AwayScorePeriod2', 'AwayScorePeriod3', 'AwayScoreNormaltime', 'AwayScoreOvertime', 'AwayScorePenalties', 'OddsRegularFirstSourceId', 'OddsRegularFirstValue', 'OddsRegularFirstWining', 'OddsRegularXSourceId', 'OddsRegularXValue', 'OddsRegularXWining', 'OddsRegularSecondSourceId', 'OddsRegularSecondValue', 'OddsRegularSecondWining', 'OddsDoubleChangeFirstXSourceId', 'OddsDoubleChangeFirstXValue', 'OddsDoubleChangeFirstXWining', 'OddsDoubleChangeXSecondSourceId', 'OddsDoubleChangeXSecondValue', 'OddsDoubleChangeXSecondWining', 'OddsDoubleChangeFirstSecondSourceId', 'OddsDoubleChangeFirstSecondValue', 'OddsDoubleChangeFirstSecondWining', 'ID', 'IsProcessed') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_ImportData obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_ImportDataOld' 
    and sys.columns.name not in ('Date', 'SportName', 'SportSlug', 'SportId', 'TournamentName', 'TournamentSlug', 'TournamentId', 'TournamentUniqueId', 'CategoryName', 'CategorySlug', 'CategoryId', 'SeasonName', 'SeasonSlug', 'SeasonId', 'SeasonYear', 'EventId', 'EventCustomId', 'EventFirstToServe', 'EventHasDraw', 'EventWinnerCode', 'EventName', 'EventSlug', 'EventStartDate', 'EventStartTime', 'EventChanges', 'StatusCode', 'StatusType', 'StatusDescription', 'HomeTeamId', 'HomeTeamName', 'HomeTeamSlug', 'HomeTeamGender', 'HomeScoreCurrent', 'HomeScorePeriod1', 'HomeScorePeriod2', 'HomeScorePeriod3', 'HomeScoreNormaltime', 'HomeScoreOvertime', 'HomeScorePenalties', 'AwayTeamId', 'AwayTeamName', 'AwayTeamSlug', 'AwayTeamGender', 'AwayScoreCurrent', 'AwayScorePeriod1', 'AwayScorePeriod2', 'AwayScorePeriod3', 'AwayScoreNormaltime', 'AwayScoreOvertime', 'AwayScorePenalties', 'OddsRegularFirstSourceId', 'OddsRegularFirstValue', 'OddsRegularFirstWining', 'OddsRegularXSourceId', 'OddsRegularXValue', 'OddsRegularXWining', 'OddsRegularSecondSourceId', 'OddsRegularSecondValue', 'OddsRegularSecondWining', 'OddsDoubleChangeFirstXSourceId', 'OddsDoubleChangeFirstXValue', 'OddsDoubleChangeFirstXWining', 'OddsDoubleChangeXSecondSourceId', 'OddsDoubleChangeXSecondValue', 'OddsDoubleChangeXSecondWining', 'OddsDoubleChangeFirstSecondSourceId', 'OddsDoubleChangeFirstSecondValue', 'OddsDoubleChangeFirstSecondWining', 'ID', 'IsProcessed') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_ImportDataOld obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_OddsRegular' 
    and sys.columns.name not in ('ID_Event', 'Type', 'FirstId', 'FirstValue', 'HasFirstWin', 'XId', 'XValue', 'HasXWin', 'SecondId', 'SecondValue', 'HasSecondWin', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_OddsRegular obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Score' 
    and sys.columns.name not in ('ID_Event', 'HomeScoreCurrent', 'HomeScorePeriod1', 'HomeScorePeriod2', 'HomeScorePeriod3', 'HomeScoreNormaltime', 'HomeScoreOvertime', 'HomeScorePenalties', 'AwayScoreCurrent', 'AwayScorePeriod1', 'AwayScorePeriod2', 'AwayScorePeriod3', 'AwayScoreNormaltime', 'AwayScoreOvertime', 'AwayScorePenalties', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Score obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Season' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Slug', 'IsActive', 'Year', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Season obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Sport' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Slug', 'IsActive', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Sport obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Status' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Description', 'IsActive', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Status obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Team' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Slug', 'IsActive', 'Gender', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Team obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Tip' 
    and sys.columns.name not in ('ID', 'HomeLastForm', 'HomeLastGiven', 'HomeLastTaken', 'HomeSeasonForm', 'HomeSeasonGiven', 'HomeSeasonTaken', 'HomeSeasonCount', 'AwayLastForm', 'AwayLastGiven', 'AwayLastTaken', 'AwaySeasonForm', 'AwaySeasonGiven', 'AwaySeasonTaken', 'AwaySeasonCount') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Tip obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='BM_Tournament' 
    and sys.columns.name not in ('ID', 'DisplayName', 'Slug', 'IsActive', 'UniqueID', 'ID_Category', 'DateCreated', 'DateUpdated') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka BM_Tournament obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

declare @Message nvarchar(500) = '' 
select @Message = @Message + sys.columns.name + ', ' 
from 
    sys.columns 
    inner join sys.tables on sys.tables.object_id=sys.columns.object_id 
where 
    sys.tables.name='CR_User' 
    and sys.columns.name not in ('ID', 'IsActive', 'UserName', 'Password', 'Salt', 'LastLogin', 'DateCreated', 'DateUpdated', 'Odd', 'Form') 

if @Message<>'' 
begin 
    set @Message = 'Tabulka CR_User obsahuje navíc sloupce: ' + substring(@Message, 1, len(@Message)-1) 
    raiserror (@Message, 16, 1) 
end

GO

print 'CurrentTime: CheckColumns - ' + convert(varchar, getdate(), 120)

GO

