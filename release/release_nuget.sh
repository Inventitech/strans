#!/bin/bash

rm -Rf release/nupkg

## Insert PackAsTool parameter. Without it, the generated NuGet is
## invalid, even though no error is raised. See
## https://github.com/Inventitech/strans/issues/9
sed -i "8i \    <PackAsTool>True</PackAsTool>" strans.csproj

dotnet pack -c Release -o release/nupkg

# Restore file to its previous state
sed -i "8d" strans.csproj
