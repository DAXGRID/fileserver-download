# File-server download

Easy way to download files from our file-server.

## Running

- `-h`, `--host-url`: specifies the URL of the fileserver host. This option is required and should be followed by the URL of the fileserver host. Example: `-h https://fileserver.mydomain.com`.

- `-r`, `--resource-path`: specifies the path of the resource on the fileserver. This option is required and should be followed by the path of the resource. Example: `-r /my-folder-name`.

- `-u`, `--username`: specifies the username to use when connecting to the fileserver. This option is required and should be followed by the username. Example: `-u myUsername`.

- `-p`, `--password`: specifies the password to use when connecting to the fileserver. This option is required and should be followed by the password. Example: `-p myPassword`.

- `-f`, `--filename-prefix`: specifies the prefix to use when naming the file on the fileserver. This option is required and should be followed by the filename prefix. Example: `-f my-file-name`.

- `-o`, `--output-directory`: specifies the path of the local directory where the file should be saved. This option is required and should be followed by the path of the output directory. Example: `-o C:\MyFolder`.

```sh
./FileServerDownloader \
    -h "https://files.mydomain.com" \
    -r "/externalFolderPath" \
    -u "myUsername" \
    -p "myPassword" \
    -f "my_file_prefix" \
    -o "/tmp/"
```

_Note: When using Windows, the binary will be named `FileServerDownloader.exe`._

## Build

### Build for Windows x64

```sh
dotnet publish -r win-x64 -p:PublishSingleFile=true --self-contained true -o ./out
```

### Build for Linux x64

```sh
dotnet publish -r linux-x64 -p:PublishSingleFile=true --self-contained true -o ./out
```
