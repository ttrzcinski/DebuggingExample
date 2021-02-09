#!/bin/bash
printf "/nInstall .net SDK 5.0/n"
# Download dotnet 5.0
sudo snap install dotnet-sdk --classic --channel=5.0
printf "/nAdd alias/n"
# Add alias to call it: dotnet
sudo snap alias dotnet-sdk.dotnet dotnet
printf "/nDONE./n"
