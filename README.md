# DJ

# Installation
0. Create a GitHub personal access token ([here](https://github.com/settings/tokens/new)) with at least `read:packages` scope.

2. Add my package registry.
```bash
$ dotnet nuget add source --username <USERNAME> --password <PERSONAL_ACCESS_TOKEN> --store-password-in-clear-text --name github/AndrewMJordan "https://nuget.pkg.github.com/AndrewMJordan/index.json"
```

2. Install this dotnet tool.
```bash
$ dotnet tool install --global Andtech.DJ
```

3. Create an environment variable `PLAYER`.
```bash
$ export PLAYER='vlc.exe'
```

# Usage
* Use `dj` to enqueue audio files. The search is performed from your current directory (and all child directories).

```
$ dj master of puppets
$ dj puppets
$ dj master
$ dj m o p
$ dj m
```

> The longer your queries are, the more likely `dj` will find the correct file.

* You can also preview the results with the `list` option.

```
$ dj --list mas
100 Master of Puppets
92  The Master
44  Monster Mash
22  The Mask Theme Song
```

# Tips
This works best with [VLC Media Player](https://www.videolan.org/) and the following preferences:
* **Interface > Allow only one instance:** `ON`
* **Interface > Enqueue items into playlist in one instance mode:** `ON`

# Links
[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io#https://github.com/AndrewMJordan/dj/tree/gitpod)

