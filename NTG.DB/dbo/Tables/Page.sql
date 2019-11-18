CREATE TABLE [dbo].[Page] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (20)  NOT NULL,
    [HeroImage]       NVARCHAR (MAX) NULL,
    [MobileHeroImage] NVARCHAR (MAX) NULL,
    [Position]        INT            NOT NULL,
    [InMenu]          BIT            NOT NULL,
    [InBottomMenu]    BIT            NOT NULL,
    [Visible]         BIT            NOT NULL,
	[AdminLocked]	  BIT            NOT NULL DEFAULT 0,
    [UploadFolder]    NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_Page] PRIMARY KEY CLUSTERED ([Id] ASC)
);

