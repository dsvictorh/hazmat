CREATE TABLE [dbo].[ModuleSimpleCardsCard] (
    [Id]                  INT            IDENTITY (1, 1) NOT NULL,
    [Image]               NVARCHAR (MAX) NOT NULL,
    [Title]               NVARCHAR (30)  NOT NULL,
    [Subtitle]            NVARCHAR (30)  NOT NULL,
    [Position]            INT            NOT NULL,
    [ModuleSimpleCardsId] INT            NOT NULL,
    CONSTRAINT [PK_ModuleSimpleCardsCard] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModuleSimpleCardsCard_ModuleSimpleCards_ModuleSimpleCardsId] FOREIGN KEY ([ModuleSimpleCardsId]) REFERENCES [dbo].[ModuleSimpleCards] ([Id])
);

