#!/bin/bash
until nc -z -v -w30 db 5432
do
  echo "Waiting for PostgreSQL database connection..."
  sleep 5
done
echo "PostgreSQL database is up!"
exec "$@"
