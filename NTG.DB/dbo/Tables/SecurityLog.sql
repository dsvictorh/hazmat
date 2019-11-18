﻿CREATE TABLE [dbo].[SecurityLog]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [Date] DATETIMEOFFSET NOT NULL, 
    [Action] NVARCHAR(MAX) NOT NULL, 
    [IPAddress] NVARCHAR(MAX) NOT NULL, 
    [Browser] NVARCHAR(MAX) NOT NULL, 
    [UserId] INT NULL,
	[UserEmail] NVARCHAR(MAX) NULL, 
    [UserType] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_SecurityLog] PRIMARY KEY CLUSTERED ([Id] ASC), 
)
