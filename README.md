# temporal-postgres-hello-world

```shell
docker compose build --no-cache
docker compose up -d postgres temporal temporal-ui
docker compose logs -f temporal # check for any errors
docker compose up -d worker
docker compose up --no-deps starter
```

## refs

[Temporal .NET SDK](https://github.com/temporalio/sdk-dotnet)

