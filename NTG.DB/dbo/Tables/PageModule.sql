CREATE TABLE [dbo].[PageModule] (
    [Id]                    INT            IDENTITY (1, 1) NOT NULL,
    [ModuleTypeId]          INT            NOT NULL,
    [PageId]                INT            NOT NULL,
    [ModuleId]              INT            NOT NULL,
    [Position]              INT            NOT NULL,
    [Title]                 NVARCHAR (100) NOT NULL,
    [BlueTitle]             BIT            NOT NULL,
    [Theme]                 BIT            NOT NULL,
    [TransparentBackground] BIT            NOT NULL,
	[LootBox] BIT            NOT NULL DEFAULT 0,
	[LootBoxTop] INT            NOT NULL DEFAULT 0,
	[LootBoxLeft] INT            NOT NULL DEFAULT 0,
    [State]                 INT            NOT NULL,
    CONSTRAINT [PK_ModulePage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PageModule_ModuleType_ModuleTypeId] FOREIGN KEY ([ModuleTypeId]) REFERENCES [dbo].[ModuleType] ([Id]),
    CONSTRAINT [FK_PageModule_Page_PageId] FOREIGN KEY ([PageId]) REFERENCES [dbo].[Page] ([Id])
);

