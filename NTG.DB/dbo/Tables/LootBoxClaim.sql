CREATE TABLE [dbo].[LootBoxClaim]
(
	[Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Email]            NVARCHAR (100)  NOT NULL,
    [FirstName]       NVARCHAR (50) NOT NULL,
    [LastName] NVARCHAR (50) NOT NULL,
    [Date] DATETIMEOFFSET NOT NULL, 
    [Prize] NVARCHAR(MAX) NULL, 
    [Redeemed] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_LootBoxClaim] PRIMARY KEY CLUSTERED ([Id] ASC)
)
