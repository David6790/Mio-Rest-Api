-- Déclarer des variables pour stocker les informations temporaires des clients et des réservations
DECLARE @i int = 1;
DECLARE @dateResa date;
DECLARE @timeResa time;
DECLARE @clientId uniqueidentifier;

-- Commencer la transaction
BEGIN TRANSACTION;

-- Boucle pour générer les clients et les réservations
WHILE @i <= 40
BEGIN
    -- Générer un nouveau GUID pour le client
    SET @clientId = NEWID();

    -- Insérer un nouveau client avec le GUID
    INSERT INTO [Clients] (Id, Name, Prenom, Telephone, Email, FreeTable21)
    VALUES (
        @clientId, -- GUID du client
        'Client' + CAST(@i AS nvarchar(50)), -- Nom générique avec index
        'Prenom' + CAST(@i AS nvarchar(50)), -- Prénom générique avec index
        '555-01' + RIGHT('00' + CAST(@i AS nvarchar(50)), 2), -- Numéro de téléphone générique
        'email' + CAST(@i AS nvarchar(50)) + '@example.com', -- Email générique
        'N' -- FreeTable21 toujours à 'N'
    );

    -- Générer une date aléatoire pour la réservation dans les 30 prochains jours
    SET @dateResa = DATEADD(day, ABS(CHECKSUM(NEWID()) % 30), GETDATE());

    -- Générer une heure aléatoire pour le midi ou le soir
    IF @i % 2 = 0
        SET @timeResa = DATEADD(minute, ABS(CHECKSUM(NEWID()) % 121), CAST('12:00' AS time)) -- entre 12:00 et 14:00
    ELSE
        SET @timeResa = DATEADD(minute, ABS(CHECKSUM(NEWID()) % 181), CAST('19:00' AS time)); -- entre 19:00 et 22:00

    -- Insérer la réservation pour le client
    INSERT INTO [Reservations] (
        IdClient, DateResa, TimeResa, NumberOfGuest, CreaTimeStamp, CreatedBy, Placed, IsPowerUser, Status, OccupationStatusOnBook
    ) VALUES (
        @clientId, -- ID du client récemment créé
        @dateResa, -- Date de la réservation
        @timeResa, -- Heure de la réservation
        ABS(CHECKSUM(NEWID()) % 5) + 1, -- Nombre d'invités, entre 1 et 5
        GETDATE(), -- Timestamp de création
        'admin', -- Créé par 'admin'
        'N', -- Placed
        'N', -- IsPowerUser
        'P', -- Status
        'RAS' -- OccupationStatusOnBook
    );

    -- Incrementer le compteur
    SET @i = @i + 1;
END;

-- Valider la transaction
COMMIT;
