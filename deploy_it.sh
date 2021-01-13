#!/bin/bash
printf "\nPackaging..\n"
sam package \
  --template-file template.yaml \
  --output-template-file debugging-example.yaml \
  --s3-bucket the-debugging-example-deploy
printf "\nDeploying..\n"
sam deploy \
   --template-file debugging-example.yaml \
   --stack-name DebuggingExample \
   --capabilities CAPABILITY_IAM \
   --region eu-west-1
printf "\nTesting remotely..\n"
dotnet lambda invoke-function DebuggingExample -–region eu-west-1
printf "\nDONE\n"

