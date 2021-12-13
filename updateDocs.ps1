# Update schemas
Remove-Item -Path ./docs/**/schemas -Recurse -Force
dotnet run -p ./TehPers.SchemaGen/TehPers.SchemaGen.csproj -- ./docs
