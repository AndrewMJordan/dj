# DJ
1. Add my package registry.
```bash
$ dotnet nuget add source --username <USERNAME> --password <PERSONAL_ACCESS_TOKEN> --store-password-in-clear-text --name github/AndrewMJordan "https://nuget.pkg.github.com/AndrewMJordan/index.json"
```

2. Install this dotnet tool.
```bash
$ dotnet tool install --global Andtech.DJ
```

2. Create an environment variable `PLAYER`.
```bash
$ export PLAYER='vlc.exe'
```
