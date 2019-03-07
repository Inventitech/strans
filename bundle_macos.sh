#!/bin/bash

APP=bin/Release/netcoreapp2.2/osx-x64/publish/strans.app
DMG_NAME=strans

dotnet bundle -r osx-x64 -c Release
hdiutil create -volname ${DMG_NAME} -srcfolder ${APP} -ov -format UDZO ${DMG_NAME}-macos.dmg
