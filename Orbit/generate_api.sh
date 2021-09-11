#!/usr/bin/env bash

swagger-codegen generate -i swagger.yaml -l csharp -Dmodels -Dapis -c swagger.config.json
