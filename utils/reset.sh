#!/usr/bin/env bash
CONTAINER_ALIAS="oracle"
SYS_PASSWORD="password"
DEV_USERNAME="USERNAME"
DEV_PASSWORD="password"
PDB_NAME="FREEPDB1"

docker exec -i $CONTAINER_ALIAS sqlplus sys/"$SYS_PASSWORD"@localhost:1521/"$PDB_NAME" as sysdba <<EOF
-- Drop the development user if it exists
DROP USER "$DEV_USERNAME" CASCADE;

-- Create a development user
CREATE USER "$DEV_USERNAME" IDENTIFIED BY "$DEV_PASSWORD";

-- Grant necessary permissions.
GRANT CONNECT, RESOURCE TO "$DEV_USERNAME";
GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO "$DEV_USERNAME";
GRANT DROP ANY TABLE, ALTER ANY TABLE TO "$DEV_USERNAME";

-- Set unlimited quota on USERS tablespace.
ALTER USER "$DEV_USERNAME" QUOTA UNLIMITED ON users;

-- Verify the user creation.
SELECT username FROM dba_users WHERE username = '$DEV_USERNAME';
EOF

echo "Database user '$DEV_USERNAME' has been successfully configured."
