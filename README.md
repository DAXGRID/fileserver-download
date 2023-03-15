# File-server download

Easy way to download files from our file-server.

## Build

### Build for Windows x64

```sh
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true -o ./out
```

### Build for Linux x64

```sh
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true -o ./out
```
