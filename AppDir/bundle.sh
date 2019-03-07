#!/bin/bash

rm -Rf AppDir/usr/bin
mkdir -p AppDir/usr/bin
dotnet restore -r linux-x64
dotnet publish -c Release -r linux-x64
cp bin/Release/netcoreapp2.2/linux-x64/publish/* AppDir/usr/bin/
wget "https://github.com/AppImage/AppImageKit/releases/download/continuous/appimagetool-x86_64.AppImage"
chmod +x appimagetool-x86_64.AppImage
./appimagetool-x86_64.AppImage ./AppDir
mv ./strans-x86_64.AppImage strans-linux.AppImage
