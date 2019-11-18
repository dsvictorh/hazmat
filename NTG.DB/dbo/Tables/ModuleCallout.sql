CREATE TABLE [dbo].[ModuleCallout] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [GreenTitlePrefix] NVARCHAR (20)  NULL,
    [Subtitle]         NVARCHAR(50)     NULL,
    [ButtonText]       NVARCHAR (20)  NULL,
    [ButtonLink]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ModuleHeroCallout] PRIMARY KEY CLUSTERED ([Id] ASC)
);

