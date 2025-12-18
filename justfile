run:
    docker compose up

run-build:
    docker compose up --build

run-db:
    docker compose up db

migrations-add migrationName:
    dotnet-ef migrations add {{migrationName}} \
    --project=src/Infrastructure/ \
    --startup-project=src/Api/ \
    --namespace=Database/Migrations \
    --no-build

migrations-rm:
    dotnet-ef migrations remove --project=src/Infrastructure/ --startup-project=src/Api/

migrate:
    just -E=./.env migrate-with-env

migrate-build: migrator-build
    just -E=./.env migrate-with-env

migrate-with-env defaultConnection=env("CONNECTIONSTRINGS__DEFAULTCONNECTION"):
    docker run --rm \
        --network "studytracker_StudyTracker.Postgres.Network" \
        -e CONNECTIONSTRINGS__DEFAULTCONNECTION="{{defaultConnection}}" \
        -it studytracker-migrator \
        dotnet-ef database update \
        --project src/Infrastructure/ \
        --startup-project src/Api/ \
        --no-build

migrator-build:
    docker build -f Migrator.Dockerfile . -t studytracker-migrator

set-secret secretKey secretValue:
    dotnet user-secrets set "{{secretKey}}" "{{secretValue}}" --project StudyTrackerApi.sln

secret-list:
    dotnet user-secrets list --project StudyTrackerApi.sln