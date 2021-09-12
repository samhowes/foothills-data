#!/usr/bin/env bash

swagger-codegen generate -i swagger.yaml -l csharp -Dmodels-Dapis -c swagger.config.json 

rm .gitignore
rm git_push.sh
rm Orbit.Api.sln
