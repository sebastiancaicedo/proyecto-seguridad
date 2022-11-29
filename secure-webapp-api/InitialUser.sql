--creds=default:12345
INSERT INTO [SecureApp].[dbo].UserCreds
VALUES (NEWID(), 'default', 'apFu/1Wle3aKvBPnw+yQs4qOyhmqvGUDaaSUnX6rRr0=', '959387141DEC24B5AB1B5D35F99FDF8B1533A90C');

INSERT INTO [SecureApp].[dbo].Users
VALUES (NEWID(), 'Default', 'User', 'default');
