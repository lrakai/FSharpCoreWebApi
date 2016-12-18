#!/bin/bash

cd src/FSharpWebApi/
dotnet publish --output bin/Publish --configuration Release

cd bin/Publish
git init
git checkout -b "master"
git config user.name "CircleCI"
git config user.email "CircleCI@circle.ci"
git add -A
git commit -m "build from Circle CI"
git push origin master:release -f