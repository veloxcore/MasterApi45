IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = 'MasterApi45Logs' OR name = 'MasterApi45Logs'))
BEGIN
	CREATE DATABASE MasterApi45Logs
END

GO

USE MasterApi45Logs

GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE Table_Name = 'Logs')
BEGIN
	CREATE TABLE [dbo].[Logs](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Message] [nvarchar](max) NOT NULL,
		[Timestamp] [datetime] NOT NULL,
		[Type] [nvarchar](20) NULL,
		[Context] [nvarchar](50) NULL,
		[Username] [nvarchar](100) NULL,
		[Path] [nvarchar](250) NULL,
		[Browser] [varchar](1000) NULL,
	 CONSTRAINT [PK_Log] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	ALTER TABLE [dbo].[Logs] ADD  CONSTRAINT [DF_Log_Timestamp]  DEFAULT (getdate()) FOR [Timestamp]

	CREATE FULLTEXT CATALOG SearchCatalog AS DEFAULT
	CREATE FULLTEXT INDEX ON Logs
	(
		Message
	)
	KEY INDEX PK_Log
END


GO




USE [master]
IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE ('[' + name + ']' = 'MasterApi45' OR name = 'MasterApi45'))
BEGIN
	CREATE DATABASE [MasterApi45]
END
GO

USE MasterApi45

GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE Table_Name = 'Tests')
BEGIN
CREATE TABLE [dbo].[Tests](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[CreatedDate] [datetime] NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_Tests] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
