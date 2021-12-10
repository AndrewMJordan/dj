# DJ

[![badge](https://img.shields.io/github/v/tag/andtechstudios/dj?label=nuget)](https://gitlab.com/andtech/pkg/-/packages)

# Installation
1. Check out installation instructions [here](https://gitlab.com/andtech/pkg).
2. Create an environment variable `PLAYER`.

For example

```bash
$ export PLAYER='vlc --qt-start-minimized --playlist-enqueue'
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
[![Open in Gitpod](https://gitpod.io/button/open-in-gitpod.svg)](https://gitpod.io#https://github.com/AndrewMJordan/dj)

