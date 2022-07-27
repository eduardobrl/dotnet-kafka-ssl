#!/bin/bash

cd app
dotnet restore
dotnet publish -c Release -o out

docker build -t processadorpedidos .

docker run --name processadorpedidos --network=kafka-docker-ssl_kafka processadorpedidos