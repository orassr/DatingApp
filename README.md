# DatingApp
.NET Core + Angular


dotnet ef migrations add ExtendedUserClass :::::::::::::

:::::::::::::::::::: SOMETHING ::::::::::::::::::::
dotnet ef migrations 

Usage: dotnet ef migrations [options] [command]

Options:
  -h|--help        Show help information
  -v|--verbose     Show verbose output.
  --no-color       Don't colorize output.
  --prefix-output  Prefix output with level.

Commands:
  add     Adds a new migration.
  list    Lists available migrations.
  remove  Removes the last migration.
  script  Generates a SQL script from migrations.

Use "migrations [command] --help" for more information about a command.

EF Cases : 

> dotnet ef migrations remove -> will remove the latest migration(if not done 'dotnet ef database-update').
> dotnet ef database update AddedUserModel  <-- will revert to that migration

docs.microsoft.com/en-us/ef/core/providers/sqlite/limitations

** in case of using SQLite there is no option to drop column, will have to drop the whole db and re create it.

>dotnet ef database drop
>dotnet ef migrations remove
>dotnet ef database update


Seeding the DB:



Using AutoMapper ->
after installing AutoMapper.Extensions.Microsoft.Dependenceinjection 
-> Startup.cs -> services.AddAutoMapper(); At the ConfigureServices section.

**Inject to the controller ->
public UserController(IDatingRepository repo, IMapper mapper)

and initialize the mapper -> private readonly IMapper _mapper .
+ creating a helper.