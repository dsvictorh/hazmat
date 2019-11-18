CREATE TABLE [dbo].[ModuleGalleryImage] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [Image]                NVARCHAR (MAX) NOT NULL,
	[Position]             INT            NOT NULL,
    [ModuleGalleryId] INT            NOT NULL,
    CONSTRAINT [PK_ModuleGalleryImage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ModuleGalleryImage_ModuleGallery_ModuleGalleryId] FOREIGN KEY ([ModuleGalleryId]) REFERENCES [dbo].[ModuleGallery] ([Id])
);
