/* DEPLOY SCRIPT */
/* Any entry in here needs to ensure the data won't be duplicated since this is a deployment script, constantly being ran whenever there are changes */
/* ModuleTypes */
INSERT INTO ModuleType (Name) SELECT 'Boxes' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'Boxes')
GO
INSERT INTO ModuleType (Name) SELECT 'Callout' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'Callout')
GO
INSERT INTO ModuleType (Name) SELECT 'FreeText' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'FreeText')
GO
INSERT INTO ModuleType (Name) SELECT 'SimpleCards' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'SimpleCards')
GO
INSERT INTO ModuleType (Name) SELECT 'ProfileCards' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'ProfileCards')
GO
INSERT INTO ModuleType (Name) SELECT 'Gallery' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'Gallery')
GO
INSERT INTO ModuleType (Name) SELECT 'Promo' WHERE NOT EXISTS (SELECT 1 FROM ModuleType WHERE Name = 'Promo')
GO

/* Roles */
INSERT INTO Role (Name) SELECT 'Admin' WHERE NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'Admin')
GO
INSERT INTO Role (Name) SELECT 'Content Manager' WHERE NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'Content Manager')
GO

/* Default User: Password 123 */
INSERT INTO Admin (Email, Password, FirstName, LastName, RoleId, Active) SELECT 'admin@nontoxicgaming.com', 'A665A45920422F9D417E4867EFDC4FB8A04A1F3FFF1FA07E998E86F7F7A27AE3', 'Admin', 'NTG', 1, 1 WHERE NOT EXISTS (SELECT 1 FROM Admin)
GO

/*SiteSettings*/
IF NOT EXISTS(SELECT 1 FROM SiteSettings)
BEGIN
INSERT INTO SiteSettings DEFAULT VALUES
END
GO 