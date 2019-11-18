CREATE TABLE [dbo].[ModuleProfileCardsCard] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [Image]                NVARCHAR (MAX) NOT NULL,
    [Name]                 NVARCHAR (25)  NOT NULL,
    [FacebookUrl]          NVARCHAR (MAX) NULL,
    [TwitterUrl]           NVARCHAR (MAX) NULL,
    [TwitchUrl]            NVARCHAR (MAX) NULL,
    [InstagramUrl]         NVARCHAR (MAX) NULL,
    [YouTubeUrl]           NVARCHAR (MAX) NULL,
    [Position]             INT            NOT NULL,
    [ModuleProfileCardsId] INT            NOT NULL,
    CONSTRAINT [PK_ModuleProfileCardsCard] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModuleProfileCardsCard_ModuleProfileCards_ModuleProfileCardsId] FOREIGN KEY ([ModuleProfileCardsId]) REFERENCES [dbo].[ModuleProfileCards] ([Id])
);

