-- D�clarer des variables pour stocker les informations temporaires des r�servations
DECLARE @i int = 1;
DECLARE @dateResa date;
DECLARE @timeResa time;
DECLARE @clientId int;
DECLARE @freeTable21 char(1) = 'N';  -- Supposons que la valeur par d�faut est 'N'

-- Commencer la transaction
BEGIN TRANSACTION;

-- Boucle pour g�n�rer les clients et les r�servations
WHILE @i <= 40
BEGIN
    -- Ins�rer un nouveau client
    INSERT INTO [Clients] (Name, Prenom, Telephone, Email, NumberOfReservation)
    VALUES (
        'Client' + CAST(@i AS nvarchar(50)),  -- Nom g�n�rique avec index
        'Prenom' + CAST(@i AS nvarchar(50)),  -- Pr�nom g�n�rique avec index
        '555-01' + RIGHT('00' + CAST(@i AS nvarchar(50)), 2),  -- Num�ro de t�l�phone g�n�rique
        'email' + CAST(@i AS nvarchar(50)) + '@example.com',  -- Email g�n�rique
        1  -- NumberOfReservations � 1 par d�faut
    );

    -- S�lectionner le dernier ID de client ins�r�
    SET @clientId = SCOPE_IDENTITY();

    -- G�n�rer une date al�atoire pour la r�servation dans les 30 prochains jours
    SET @dateResa = DATEADD(day, ABS(CHECKSUM(NEWID()) % 30), GETDATE());

    -- G�n�rer une heure al�atoire pour le midi ou le soir
    IF @i % 2 = 0
        SET @timeResa = DATEADD(minute, ABS(CHECKSUM(NEWID()) % 121), CAST('12:00' AS time))  -- entre 12:00 et 14:00
    ELSE
        SET @timeResa = DATEADD(minute, ABS(CHECKSUM(NEWID()) % 181), CAST('19:00' AS time));  -- entre 19:00 et 22:00

    -- Ins�rer la r�servation pour le client
    INSERT INTO [Reservations] (
        IdClient, DateResa, TimeResa, NumberOfGuest, CreaTimeStamp, CreatedBy, Placed, IsPowerUser, Status, OccupationStatusOnBook, FreeTable21
    ) VALUES (
        @clientId,  -- ID du client r�cemment cr��
        @dateResa,  -- Date de la r�servation
        @timeResa,  -- Heure de la r�servation
        ABS(CHECKSUM(NEWID()) % 5) + 1,  -- Nombre d'invit�s, entre 1 et 5
        GETDATE(),  -- Timestamp de cr�ation
        'admin',  -- Cr�� par 'admin'
        'N',  -- Placed
        'N',  -- IsPowerUser
        'P',  -- Status
        'RAS',  -- OccupationStatusOnBook
        @freeTable21  -- FreeTable21 bas� sur une valeur fixe ou une condition
    );

    -- Incrementer le compteur
    SET @i = @i + 1;
END;

-- Valider la transaction
COMMIT;
