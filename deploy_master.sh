#!/bin/bash

# Create publish artifacts
cd src/FSharpWebApi/
dotnet publish --output bin/Publish --configuration Release

# Push artifacts to release branch (Triggering docker hub build)
cd bin/Publish
git config user.name "CircleCI"
git config user.email "CircleCI@circle.ci"
git add -f .
git commit -m "build from Circle CI"
git push origin master:release -f