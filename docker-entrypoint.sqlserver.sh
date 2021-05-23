# start the script to create/setup the DB with a delay and start SQL Server
 /bin/bash -c "sleep 20s; /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $SA_PASSWORD -d master -Q \"CREATE DATABASE [$DATABASE]; ALTER DATABASE [$DATABASE] SET AUTO_CLOSE OFF WITH NO_WAIT;\"" & /opt/mssql/bin/sqlservr



 