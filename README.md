# File-server download

Easy way to download files from our file-server.

## Running

When using Windows, the binary will be named `FileServerDownloader.exe`.

```sh
./FileServerDownloader \
    -h "https://files.mydomain.com" \
    -r "/externalFolderPath" \
    -u "myUsername" \
    -p "myPassword" \
    -f "my_file_prefix" \
    -o "/tmp/"
```

## Build

### Build for Windows x64

```sh
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true -o ./out
```

### Build for Linux x64

```sh
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true -o ./out
```
